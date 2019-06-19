#region license
/*
    The MIT License (MIT)
    Copyright (c) 2019 Techfactory.hu
*/
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OBSMidiRemote.Lib.OBSWebsocketdotnet;
using System.Security.Cryptography;

namespace OBSMidiRemote.Lib
{
    class COBSConnector : IOBSConnector
    {
        #region EventHandlers
        public event OBSConnectionStatus ConnectionStatus;
        public event OBSSceneListChanged SceneListChanged;
        public event OBSSceneChanged SceneChanged;
        public event OBSSceneItemChanged SceneItemChanged;
        public event OBSStreamingStatusChanged StreamingStatusChanged;
        public event OBSSourceChanged SourceChanged;
        public event OBSAudioSourceChanged AudioSourceChanged;
        public event OBSTransitionChanged TransitionChanged;
        public event OBSStudioModeChanged ModeChanged;
        #endregion

        private OBSWebsocket ws;
        public SOBSScene ActiveScene { get; private set; }
        public SOBSScene ActivePreviewScene { get; private set; }
        public SOBSTransition ActiveTransition { get; private set; }

        public SOBSStreamingState State
        {
            get { return _state; }
            private set { _state = value; }
        }

        public bool Connected { get; private set; }

        public List<SOBSScene> Scenes { get; private set; }
        public List<SOBSScene> PreviewScenes { get; private set; }
        public List<SOBSTransition> Transitions { get; private set; }
        public List<SOBSSource> Sources { get; private set; }
        public List<SOBSAudioSource> AudioSources { get; private set; }
        public EOBSMode Mode { get; private set; }

        private string _auth;
        private SOBSStreamingState _state;
        private List<OBSSourceTypes> ObsTypes = new List<OBSSourceTypes>();
        private OBSVersion Version;
        private Dictionary<string, string> SpecialSources;

        public COBSConnector()
        {
            Mode = EOBSMode.Normal;

            Scenes = new List<SOBSScene>();
            PreviewScenes = new List<SOBSScene>();
            Transitions = new List<SOBSTransition>();
            Sources = new List<SOBSSource>();
            AudioSources = new List<SOBSAudioSource>();

            ActiveScene = new SOBSScene();
            ActivePreviewScene = new SOBSScene();
            ActiveTransition = new SOBSTransition();

            _state = new SOBSStreamingState();
            _state.Stream = EOBSStreamingState.Unkown;
            _state.Record = EOBSStreamingState.Unkown;
            _state.Replay = EOBSStreamingState.Unkown;

            SpecialSources = new Dictionary<string, string>();

            ws = new OBSWebsocket();
            ws.Connected += Ws_Connected;
            ws.Disconnected += Ws_Disconnected;
            ws.OnError += Ws_OnError;
            ws.SceneChanged += Ws_SceneChanged;
            ws.PreviewSceneChanged += Ws_PreviewSceneChanged;
            ws.SceneCollectionChanged += Ws_SceneCollectionChanged;
            ws.TransitionChanged += Ws_TransitionChanged;
            ws.TransitionListChanged += Ws_TransitionListChanged;
            ws.SceneItemVisibilityChanged += Ws_SceneItemVisibilityChanged; 
            ws.SceneItemAdded += Ws_SceneItemAdded;
            ws.SceneItemRemoved += Ws_SceneItemRemoved;
            ws.SourceOrderChanged += Ws_SourceOrderChanged;
            ws.StudioModeSwitched += Ws_StudioModeSwitched; 
            ws.StreamingStateChanged += Ws_StreamingStateChanged;
            ws.RecordingStateChanged += Ws_RecordingStateChanged; 
        }

        public void Connect(string url, int port, string pwd = null)
        {
            _auth = pwd;
            ws.ConnectAsync("ws://" + url + ":" + port);
        }

        public void Disconnect()
        {
            ws.Disconnect();
        }

        public string GetVersionInfo()
        {
            if (!String.IsNullOrEmpty(Version.OBSStudioVersion))
            {
                return "OBS Connector, OBS Version: " + Version.OBSStudioVersion + "\r\n Obs-websocket-version: " + Version.PluginVersion;
            }
            return "OBS Connector, No version info available...";
        }

        public async Task<bool> Reload()
        {
            if (ObsTypes.Count == 0)
            {
                ObsTypes = await ws.GetSourcesTypesList();
            }

            Version = await ws.GetVersion();
            var studiomode = await ws.StudioModeEnabled();
            Mode = studiomode ? EOBSMode.Studio : EOBSMode.Normal;

            var OutputStatus = await ws.GetStreamingStatus();
            _state = _translateStreamingStatus(OutputStatus);

            await _loadTransitions();
            await _loadActiveTransition();
            SpecialSources = await ws.GetSpecialSources();

            await _loadSources();
            await _loadScenes();
            await _loadActiveScene();
        
            if (Mode == EOBSMode.Studio)
            {
                _loadActivePScene();
            }

            return true;
        }

        public void CheckTimeouts()
        {
            ws.CheckTimeouts();
        }

        public void SwitchScene(int index)
        {
            if (index >= 0 && index < Scenes.Count)
            {
                ws.SetCurrentScene(Scenes[index].Name);
            }
        }

        public void SwitchScene(string name)
        {
            var scene = Scenes.Find(x => x.Name == name);
            if (scene.Id != null)
            {
                ws.SetCurrentScene(scene.Name);
            }
        }

        public void SwitchPreviewScene(int index) {

            if (index >= 0 && index < Scenes.Count)
            {
                ws.SetPreviewScene(Scenes[index].Name);
            }
        }

        public void SwitchPreviewScene(string name) {
            var scene = Scenes.Find(x => x.Name == name);
            if (scene.Id != null)
            {
                ws.SetPreviewScene(scene.Name);
            }
        }

        public void SetSceneItemVisibility(string scene_id, string item_id, bool visible)
        {
            try
            {
                var scene_index = Scenes.FindIndex(x => x.Id == scene_id);
                var item_index = Scenes[scene_index].Items.FindIndex(x => x.Id == item_id);
                SetSceneItemVisibility(scene_index, item_index, visible);
            }
            catch (ArgumentNullException e)
            {

            }
            catch (Exception e)
            {

            }

        }

        public void SetSceneItemVisibility(int scene_index, int item_index, bool visible)
        {
            if (scene_index >= 0 && scene_index < Scenes.Count)
            {
                if (item_index >= 0 && item_index < Scenes[scene_index].Items.Count)
                {
                    var item = Scenes[scene_index].Items[item_index];
                   /* if (item.IsFolder)
                    {
                       
                    }
                    else
                    {*/
                        ws.SetSceneItemVisibility(Scenes[scene_index].Items[item_index].Name, visible, Scenes[scene_index].Name);
                    /*}*/
                }
            }
        }

        public void ToggleSceneItemVisibility(int scene_id, int item_id)
        {
            if (scene_id >= 0 && scene_id < Scenes.Count)
            {
                if (item_id >= 0 && item_id < Scenes[scene_id].Items.Count)
                {
                    SetSceneItemVisibility(scene_id, item_id, !Scenes[scene_id].Items[item_id].Visible);
                }
            }
        }

        public void SetSourceAudioMute(int source_id, bool muted)
        {
            if (source_id >= 0 && source_id < AudioSources.Count)
            {
                if (AudioSources[source_id].Muted != muted)
                {
                    ws.SetMute(AudioSources[source_id].Name, muted);
                    if (IsPluginVersion("4.5.1", "lte"))
                    {
                        var item = AudioSources[source_id];
                        item.Muted = muted;
                        AudioSources[source_id] = item;
                        if (AudioSourceChanged != null)
                        {
                            AudioSourceChanged(this, EOBSEvent.AudioVolumeChanged);
                        }
                    }
                }
            }
        }

        public void ToggleSourceAudioMute(int source_id)
        {
            if (source_id >= 0 && source_id < AudioSources.Count)
            {
                if (IsPluginVersion("4.5.1", "lte"))
                {
                    var item = AudioSources[source_id];
                    ws.SetMute(AudioSources[source_id].Name, !item.Muted);
                    item.Muted = !item.Muted;
                    AudioSources[source_id] = item;

                    if (AudioSourceChanged != null)
                    {
                        AudioSourceChanged(this, EOBSEvent.AudioVolumeChanged);
                    }
                }
                else
                {
                    ws.ToggleMute(AudioSources[source_id].Name);
                }
            }
        }

        public void SetSourceAudioVolume(int source_id, float volume)
        {
            if (source_id >= 0 && source_id < AudioSources.Count)
            {
                ws.SetVolume(AudioSources[source_id].Name, volume);
            }
        }

        public void StartStreaming()
        {
           ws.StartStreaming();
        }

        public void StopStreaming()
        {
            ws.StopStreaming();
        }

        public void ToggleStreaming()
        {
           ws.ToggleStreaming();
        }

        public void StartRecording()
        {
            ws.StartRecording();
        }

        public void StopRecording()
        {
            ws.StopRecording();
            
        }

        public void ToggleRecording()
        {
            ws.ToggleRecording();
        }

        public void StartReplayBuffer()
        {
            ws.StartReplayBuffer();
        }

        public void StopReplayBuffer()
        {
            ws.StopReplayBuffer();
        }

        public void SaveReplayBuffer()
        {
            ws.SaveReplayBuffer();
        }

        public void ToggleReplayBuffer()
        {
            if (_state.Replay == EOBSStreamingState.Started)
            {
                StopReplayBuffer();
            }
            else if (_state.Replay == EOBSStreamingState.Stopped)
            {
                StartReplayBuffer();
            }
        }

        public void SetStudioMode(bool mode) {
            ws.SetStudioMode(mode);
        }

        public void ToggleStudioMode()
        {
            ws.ToggleStudioMode();
        }

        public void SetTransition(int tr_id) {
            if (tr_id >= 0 && tr_id < Transitions.Count)
            {
                ws.SetCurrentTransition(Transitions[tr_id].Name);
            }
        }

        #region Data helpers

        private async Task<bool> _loadActiveTransition(bool trigger_event = false)
        {
            var CurrentTransition = await ws.GetCurrentTransition();
            ActiveTransition = _translateActiveTransition(CurrentTransition);

            if (trigger_event)
            {
                if (TransitionChanged != null)
                {
                    TransitionChanged(this, EOBSEvent.TransitionChanged);
                }
            }
            return true;
        }

        private async Task<bool> _loadTransitions(bool trigger_event = false)
        {
            var _transitions = await ws.ListTransitions();
            Transitions = _translateTransitions(_transitions);

            if (trigger_event)
            {
                if (TransitionChanged != null)
                {
                    TransitionChanged(this, EOBSEvent.TransitionListChanged);
                }
            }
            return true;
        }

        private async void _loadActivePScene(bool trigger_event = false)
        {
            var scene = await ws.GetPreviewScene();
            for (int i = 0; i < Scenes.Count; i++)
            {
                if (Scenes[i].Name == scene.Name)
                {
                    SOBSScene item = _translateScene(scene, i);
                    ActivePreviewScene = item;

                    if (trigger_event)
                    {
                        SceneChanged(this, EOBSEvent.SceneUpdated, item);
                    }
                }
            }
        }

        private async void _loadActiveScene(bool trigger_event, EOBSEvent eventType = EOBSEvent.SceneUpdated)
        {
            var scene = await ws.GetCurrentScene();
            for (int i = 0; i < Scenes.Count; i++)
            {
                if (Scenes[i].Name == scene.Name)
                {
                    Scenes[i] = _translateScene(scene, i);
                    ActiveScene = _translateScene(scene, i);
                    if (trigger_event)
                    {
                        SceneChanged(this, eventType, Scenes[i]);
                    }

                }
            }
        }

        private async Task<bool> _loadActiveScene()
        {
            var scene = await ws.GetCurrentScene();
            for (int i = 0; i < Scenes.Count; i++)
            {
                if (Scenes[i].Name == scene.Name)
                {
                    SOBSScene item = _translateScene(scene, i);
                    ActiveScene = item;
                }
            }
            return true;
        }

        private async Task<bool> _loadSources()
        {
            var SourceItems = await ws.GetSourcesList();
            for (int i = 0; i < SourceItems.Count(); i++)
            {
                if (_sourceHasAudio(SourceItems[i].TypeId))
                {
                    var item = SourceItems[i];
                    var volinfo = await ws.GetVolume(SourceItems[i].Name);
                    item.Muted = volinfo.Muted;
                    item.Volume = volinfo.Volume;
                    SourceItems[i] = item;
                }
            }
            Sources = _translateSources(SourceItems);
            _audioSources();
            return true;
        }

        private async Task<bool> _loadScenes()
        {
            var _scenes = await ws.ListScenes();
            for (int i = 0; i < _scenes.Count(); i++)
            {
                for (int x = 0; x < _scenes[i].Items.Count(); x++)
                {
                    var prop = await ws.GetSceneItemProperties(_scenes[i].Name, _scenes[i].Items[x].SourceName);
                    if (prop != null)
                    {
                        var item = _scenes[i].Items[x];
                        item.Visible = ((SceneItemProperty)prop).Visible;
                        _scenes[i].Items[x] = item;
                    }
                }
            }
            Scenes = _translateScenes(_scenes);
            return true;
        }

        private bool IsPluginVersion(string version, string action = "gt")
        {
            if (!String.IsNullOrEmpty(Version.PluginVersion))
            {
                var currentVersion = new Version(Version.PluginVersion);
                var checkVersion = new Version(version);

                switch (action)
                {
                    case "gt":
                        if (checkVersion > currentVersion) return true;
                        break;
                    case "lt":
                        if (checkVersion < currentVersion) return true;
                        break;
                    case "gte":
                        if (checkVersion >= currentVersion) return true;
                        break;
                    case "lte":
                        if (checkVersion <= currentVersion) return true;
                        break;
                }
            }
            return false;
        }


        private string _uidFromString(string input)
        {
            using (MD5 md5 = MD5.Create())
            {
                byte[] hash = md5.ComputeHash(Encoding.UTF8.GetBytes(input));
                Guid guid = new Guid(hash);
                return guid.ToString();
            }
        }

        private void _audioSources()
        {
            List<SOBSAudioSource> audios = new List<SOBSAudioSource>();
            //special sources first
            var keys = new string[] { "desktop-1", "desktop-2", "mic-1", "mic-2", "mic-3" };
            for (int s=0; s<keys.Length;s++)
            {
                if (SpecialSources.ContainsKey(keys[s]))
                {
                    var value = SpecialSources[keys[s]];
                    if (!String.IsNullOrEmpty(value))
                    {
                        try
                        {
                            var source = Sources.Find(x => x.Name == value);
                            SOBSAudioSource audio = new SOBSAudioSource();
                            audio.Name = source.Name;
                            audio.Id = source.Id;
                            audio.Index = source.Index;
                            audio.Volume = source.Volume;
                            audio.Muted = source.Muted;
                            audio.Hidden = false;
                            audios.Add(audio);  
                        }
                        catch (ArgumentNullException e)
                        {
                        }
                    }
                }
            }

            for (int s=0; s<Sources.Count; s++)
            {
               if (Sources[s].Audio)
               {
                    if (!audios.Exists(x => x.Name == Sources[s].Name)) {
                        SOBSAudioSource audio = new SOBSAudioSource();
                        audio.Name = Sources[s].Name;
                        audio.Id = Sources[s].Id;
                        audio.Index = Sources[s].Index;
                        audio.Volume = Sources[s].Volume;
                        audio.Muted = Sources[s].Muted;
                        audio.Hidden = false;
                        audios.Add(audio);
                    }
                }
            }
            AudioSources = audios;
        }

        private SOBSTransition _translateActiveTransition(TransitionSettings input)
        {
            SOBSTransition transition = new SOBSTransition();
            transition.Name = input.Name;
            transition.Id = _uidFromString(input.Name);
            for (int i=0; i<Transitions.Count;i++)
            {
                if (Transitions[i].Id == transition.Id)
                {
                    transition.Index = i;
                }
            }
            return transition;
        }

        private SOBSTransition _translateTransition(string input, int index = 0)
        {
            SOBSTransition transition = new SOBSTransition();
            transition.Name = input;
            transition.Id = _uidFromString(input);
            transition.Index = index;
            return transition;
        }

        private List<SOBSTransition> _translateTransitions(List<string> input)
        {
            int index = 0;
            List <SOBSTransition> transitions = new List<SOBSTransition>();
            foreach (var item in input)
            {
                transitions.Add(_translateTransition(item, index));
                index++;
            }

            return transitions;
        }

        private SOBSStreamingState _translateStreamingStatus(OutputStatus input)
        {
            var state = new SOBSStreamingState();
            if (input.IsRecording) { state.Record = EOBSStreamingState.Started; }
            else
            {
                state.Record = EOBSStreamingState.Stopped;
            }

            if (input.IsStreaming) { state.Stream = EOBSStreamingState.Started; }
            else
            {
                state.Stream = EOBSStreamingState.Stopped;
            }
            return state;
        }

        private List<SOBSScene> _translateScenes(List<OBSScene> input)
        {
            List<SOBSScene> output = new List<SOBSScene>();
            for (int i = 0; i < input.Count; i++)
            {
                output.Add(_translateScene(input[i], i));
            }
            return output;
        }

        private SOBSScene _translateScene(OBSScene input, int index = -1)
        {
            SOBSScene output = new SOBSScene();
            output.Index = index == -1 ? Scenes.Count() : index;
            output.Id = _uidFromString(input.Name);
            output.Name = input.Name;
            output.Items = new List<SOBSSceneItem>();
            if (input.Items != null && input.Items.Count > 0)
            {
                for (int i = 0; i < input.Items.Count; i++)
                {
                    SOBSSceneItem item = _translateSceneItem(input.Items[i], i);
                    output.Items.Add(item);
                }
            }
            return output;
        }

        private SOBSSceneItem _translateSceneItem(SceneItem input, int index = 0)
        {
            SOBSSceneItem item = new SOBSSceneItem();
            item.Id = _uidFromString(input.SourceName+"_"+index);
            item.Index = index;
            item.Name = input.SourceName;
            item.Visible = input.Visible;
            item.IsFolder = input.InternalType == "group" ? true : false;
            item.ParentId = input.ParentGroupName;
            return item;
        }

        private List<SOBSSource> _translateSources(List<OBSSourceItem> input)
        {
            List<SOBSSource> sources = new List<SOBSSource>();
            for (int i = 0; i < input.Count; i++)
            {
                sources.Add(_translateSource(input[i], i));
            }
            return sources;
        }

        private SOBSSource _translateSource(OBSSourceItem input, int index = 0)
        {
            SOBSSource source = new SOBSSource();
            source.Id = _uidFromString(input.Name+"_"+ index.ToString());
            source.Index = index;
            source.Name = input.Name;
            source.Muted = input.Muted;
            source.Video = false;
            source.Audio = _sourceHasAudio(input.TypeId);
            source.Type = input.Type;
            source.Volume = 0;
            return source;
        }
        
        private bool _setActivePreviewScene(string scene_name)
        {
            var scene = Scenes.Find(x => x.Name == scene_name);
            if (scene.Id != null)
            {
                var item = new SOBSScene();
                item.Index = scene.Index;
                item.Name = scene.Name;
                item.ResourceId = scene.ResourceId;
                ActivePreviewScene = item;
                return true;
            }
            return false;
        }

        private bool _sourceHasAudio(string sourceTypeId)
        {
            foreach (var item in ObsTypes)
            {
                if (item.TypeId == sourceTypeId && item.HasAudio)
                {
                    return true;
                }
            }
            return false;
        }

        private bool _setActiveScene(string scene_name)
        {
            try
            {
                var scene = Scenes.Find(x => x.Name == scene_name);
                var item = new SOBSScene();
                item.Index = scene.Index;
                item.Id = scene.Id;
                item.Name = scene.Name;
                item.ResourceId = scene.ResourceId;
                ActiveScene = item;
                return true;
            }catch (ArgumentNullException e)
            {

            }
            return false;
        }

        #endregion

        #region Websocket Events
        private async void Ws_Connected(object sender, EventArgs e)
        {
            var authinfo = await ws.GetAuthInfo();
            if (authinfo.AuthRequired)
            {
                var auth = await ws.Authenticate(_auth, authinfo);
                if (auth)
                {
                    Connected = true;
                    ConnectionStatus(this, EOBSCStatus.Connected);
                }
                else
                {
                    ConnectionStatus(this, EOBSCStatus.WrongAuth);
                    Connected = false;
                }
            }
            else
            {
                Connected = true;
                ConnectionStatus(this, EOBSCStatus.Connected);
            }
        }

        private void Ws_OnError(object sender, EventArgs e)
        {
            Connected = false;
            ConnectionStatus(this, EOBSCStatus.Error);
        }

        private void Ws_Disconnected(object sender, EventArgs e)
        {
            Connected = false;
            ConnectionStatus(this, EOBSCStatus.Disconnected);
        }

        private void Ws_SceneChanged(OBSWebsocket sender, string newSceneName)
        {
            _setActiveScene(newSceneName);
            if (SceneChanged != null)
                SceneChanged(this, EOBSEvent.SceneSwitched, ActiveScene);
        }

        private void Ws_PreviewSceneChanged(OBSWebsocket sender, string newSceneName)
        {
            _setActivePreviewScene(newSceneName);
            if (SceneChanged != null)
                SceneChanged(this, EOBSEvent.PreviewSceneSwitched, ActivePreviewScene);
        }

        private async void Ws_SceneCollectionChanged(object sender, EventArgs e)
        {
            await _loadScenes();
            await _loadActiveScene();
            if (Mode == EOBSMode.Studio)
            {
                _loadActivePScene();
                if (SceneChanged != null)
                    SceneChanged(this, EOBSEvent.PreviewSceneSwitched, ActivePreviewScene);
            }
            else
            {
                if (SceneChanged != null)
                    SceneChanged(this, EOBSEvent.SceneSwitched, ActiveScene);
            }
        }

        private void Ws_TransitionChanged(OBSWebsocket sender, string newTransitionName)
        {
            SOBSTransition newTransition = new SOBSTransition();
            newTransition.Name = newTransitionName;
            newTransition.Id = _uidFromString(newTransitionName);
            for (int i = 0; i < Transitions.Count; i++)
            {
                if (Transitions[i].Name == newTransitionName)
                {
                    newTransition.Index = i;
                }
            }
            ActiveTransition = newTransition;
            if (TransitionChanged != null)
            {
                TransitionChanged(this, EOBSEvent.TransitionChanged);
            }
        }

        private async void Ws_TransitionListChanged(object sender, EventArgs e)
        {
            await _loadTransitions(true);
            await _loadActiveTransition(true);
        }

        private void Ws_SceneItemVisibilityChanged(OBSWebsocket sender, string sceneName, string itemName, bool isVisible = false)
        {

            for (int i=0; i< Scenes.Count; i++)
            {
                if (Scenes[i].Name == ActiveScene.Name)
                {
                    for (int s = 0; s < Scenes[i].Items.Count; s++)
                    {
                        if (Scenes[i].Items[s].Name == itemName)
                        {
                            var item = Scenes[i].Items[s];
                            item.Visible = isVisible;
                            Scenes[i].Items[s] = item;
                            if (SceneItemChanged != null)
                            {
                                SceneItemChanged(this, EOBSEvent.SceneItemUpdated, item);
                            }
                        }

                    }
                }
            }
        }

        private void Ws_SceneItemRemoved(OBSWebsocket sender, string sceneName, string itemName, bool isVisible = false)
        {
            _loadActiveScene(true, EOBSEvent.SceneItemRemoved);
        }

        private void Ws_SceneItemAdded(OBSWebsocket sender, string sceneName, string itemName, bool isVisible = false)
        {
            _loadActiveScene(true, EOBSEvent.SceneItemAdded);
        }

        private void Ws_SourceOrderChanged(OBSWebsocket sender, string sceneName)
        {
            _loadActiveScene(true, EOBSEvent.SceneItemUpdated);
        }

        private void Ws_StudioModeSwitched(OBSWebsocket sender, bool enabled)
        {
            if (enabled) { Mode = EOBSMode.Studio; }
            else
            {
                Mode = EOBSMode.Normal;
            }
            if (ModeChanged!=null)
            {
                ModeChanged(this, Mode);
            }

            if (Mode == EOBSMode.Studio)
            {
                _loadActivePScene(true);
            }else
            {
                ActivePreviewScene = new SOBSScene();
            }

        }

        private void Ws_RecordingStateChanged(OBSWebsocket sender, OutputState type)
        {
            switch (type)
            {
                case OutputState.Started:
                    _state.Record = EOBSStreamingState.Started;
                    break;
                case OutputState.Starting:
                    _state.Record = EOBSStreamingState.Starting;
                    break;
                case OutputState.Stopping:
                    _state.Record = EOBSStreamingState.Stopping;
                    break;
                case OutputState.Stopped:
                    _state.Record = EOBSStreamingState.Stopped;
                    break;
            }

            if (StreamingStatusChanged != null)
            {
                StreamingStatusChanged(this, _state);
            }
        }

        private void Ws_StreamingStateChanged(OBSWebsocket sender, OutputState type)
        {
            switch (type)
            {
                case OutputState.Started:
                    _state.Stream = EOBSStreamingState.Started;
                    break;
                case OutputState.Starting:
                    _state.Stream = EOBSStreamingState.Starting;
                    break;
                case OutputState.Stopping:
                    _state.Stream = EOBSStreamingState.Stopping;
                    break;
                case OutputState.Stopped:
                    _state.Stream = EOBSStreamingState.Stopped;
                    break;
            }

            if (StreamingStatusChanged!=null)
            {
                StreamingStatusChanged(this, _state);
            }
        }

        
        #endregion
    }
}
