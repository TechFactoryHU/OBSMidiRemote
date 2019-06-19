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
using OBSMidiRemote.Lib.SLOBS;

namespace OBSMidiRemote.Lib
{
    class CSLOBSConnector : IOBSConnector
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

        private SLOBSWebsocket ws;
        public SOBSScene ActiveScene { get; private set; }
        public SOBSScene ActivePreviewScene { get; private set; }
        public SOBSTransition ActiveTransition { get; private set; }

        public SOBSStreamingState State {
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
        private System.Timers.Timer statusTimer;

        public CSLOBSConnector()
        {
            Mode = EOBSMode.Normal;

            Scenes          = new List<SOBSScene>();
            PreviewScenes   = new List<SOBSScene>();
            Transitions     = new List<SOBSTransition>();
            Sources         = new List<SOBSSource>();
            AudioSources    = new List<SOBSAudioSource>();

            ActiveScene = new SOBSScene();
            ActivePreviewScene = new SOBSScene();
            ActiveTransition = new SOBSTransition();
           

            _state = new SOBSStreamingState();
            _state.Stream = EOBSStreamingState.Unkown;
            _state.Record = EOBSStreamingState.Unkown;
            _state.Replay = EOBSStreamingState.Unkown;

            ws = new SLOBSWebsocket();
            ws.ConnectionStatus += Ws_ConnectionStatus;
            ws.SceneChanged += Ws_SceneChanged;
            ws.SceneListChanged += Ws_SceneListChanged;
            ws.SceneItemChanged += Ws_SceneItemChanged;
            ws.SourceChanged += Ws_SourceChanged;
            ws.StreamingStatusChanged += Ws_StreamingStatusChanged;

            //timer for querying Recording & Replay buffer status
            //SLOBS api has no events for this types yet.
            statusTimer = new System.Timers.Timer(5000);
            statusTimer.Elapsed += StatusTimer_Elapsed; ;
            statusTimer.AutoReset = true;
            statusTimer.Enabled = false;
        }

        public void Connect(string url, int port, string token = null)
        {
            _auth = token;
            ws.Connect("ws://"+url+":"+port+"/api/websocket");
        }

        public void Disconnect()
        {
            ws.Disconnect();
        }

        public string GetVersionInfo()
        {
            return "SLOBS Connector, no version info available.";
        }

        public async Task<bool> Reload()
        {
            var status = await ws.GetStreamingStatus();
            if (status != null) { 
                _state = _translateStreamingStatus((SLOBSStreamingState)status);
            }

            var remote_scenes = await ws.ListScenes();
            if (remote_scenes.Count > 0)
            {
                Scenes = _translateScenes(remote_scenes);
            }

            var remote_sources = await ws.ListSources();
            if (remote_sources.Count > 0)
            {
                Sources = _translateSources(remote_sources);
            }

            var remote_audio_sources = await ws.ListAudioSources();
            if (remote_audio_sources.Count > 0)
            {
                AudioSources = _translateAudioSources(remote_audio_sources);
            }

            var activeSceneId = await ws.GetActiveSceneId();
            if (activeSceneId != null)
            {
                for (int i=0; i<Scenes.Count; i++)
                {
                    if (Scenes[i].Id == activeSceneId)
                    {
                        ActiveScene = Scenes[i];
                        break;
                    }
                }
            }

            return true;
        }

        public void CheckTimeouts()
        {
            ws.CheckTimeouts();
        }

        public void SwitchScene(int index)
        {
            if (index >=0 && index < Scenes.Count)
            {
                ws.SwitchScene(Scenes[index].ResourceId);
            }
        }

        public void SwitchScene(string name)
        {
            var scene = Scenes.Find(x => x.Name == name);
            if (scene.Id != null)
            {
                ws.SwitchScene(scene.ResourceId);
            }
        }

        public void SwitchPreviewScene(int index) {}

        public void SwitchPreviewScene(string name) {}

        public void SetSceneItemVisibility(string scene_id, string item_id, bool visible)
        {
            try
            {
                var scene_index = Scenes.FindIndex(x => x.Id == scene_id);
                var item_index = Scenes[scene_index].Items.FindIndex(x => x.Id == item_id);
                SetSceneItemVisibility(scene_index, item_index, visible);
            } catch(ArgumentNullException e)
            {

            } catch (Exception e)
            {

            }
            
        }

        public async void SetSceneItemVisibility(int scene_index, int item_index, bool visible)
        {
            if (scene_index >= 0 && scene_index < Scenes.Count)
            {
                if (item_index >= 0 && item_index < Scenes[scene_index].Items.Count )
                {
                    var item = Scenes[scene_index].Items[item_index];
                    if (item.IsFolder)
                    {
                        for (int i=0; i< Scenes[scene_index].Items.Count; i++)
                        {
                            if (Scenes[scene_index].Items[i].Id != item.Id) {
                               
                                if (Scenes[scene_index].Items[i].ParentId == item.Id)
                                {
                                    SetSceneItemVisibility(scene_index, i, !Scenes[scene_index].Items[i].Visible);
                                }
                            }
                        }
                    }else { 
                        if (item.ResourceId != null)
                        {
                            ws.SetSceneItemVisibility(item.ResourceId, visible);
                        }
                        else
                        {
                            var resId = await ws.GetSceneItemResourceId(Scenes[scene_index].ResourceId, item.Id);
                            if (resId != null)
                            {
                                ws.SetSceneItemVisibility(resId, visible);
                                item.ResourceId = resId;
                            }
                        }
                    }
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
                ws.SetAudioSourceMuted(AudioSources[source_id].ResourceId, muted);
            }
        }

        public void ToggleSourceAudioMute(int source_id)
        {
            if (source_id >= 0 && source_id < AudioSources.Count)
            {
                ws.SetAudioSourceMuted(AudioSources[source_id].ResourceId, !AudioSources[source_id].Muted);
            }
        }

        public void SetSourceAudioVolume(int source_id, float volume)
        {
            if (source_id >= 0 && source_id < AudioSources.Count)
            {
                ws.SetAudioSourceVolume(AudioSources[source_id].ResourceId, volume);
            }
        }

        public void StartStreaming()
        {
            if (_state.Stream == EOBSStreamingState.Stopped)
            {
                ws.ToggleStream();
            }
        }

        public void StopStreaming()
        {
            if (_state.Stream != EOBSStreamingState.Stopped && _state.Stream != EOBSStreamingState.Stopping)
            {
                ws.ToggleStream();
            }
        }

        public void ToggleStreaming()
        {
            ws.ToggleStream();
        }

        public void StartRecording()
        {
            if (_state.Record == EOBSStreamingState.Stopped)
            {
                ws.ToggleRecord();
            }
        }

        public void StopRecording()
        {
            if (_state.Record != EOBSStreamingState.Stopped && _state.Record != EOBSStreamingState.Stopping)
            {
                ws.ToggleRecord();
            }
        }

        public void ToggleRecording()
        {
            ws.ToggleRecord();
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
            ws.SaveReplay();
        }

        public void ToggleReplayBuffer()
        {
            if (_state.Replay == EOBSStreamingState.Started)
            {
                StopReplayBuffer();
            }else if(_state.Replay == EOBSStreamingState.Stopped)
            {
                StartReplayBuffer();
            }
        }

        //not implemented in slobs 
        public void SetStudioMode(bool mode) { }
        public void ToggleStudioMode() { }
        public void SetTransition(int tr_id) { }

        #region Data helpers

        private SOBSStreamingState _translateStreamingStatus(SLOBSStreamingState input)
        {
            var state = new SOBSStreamingState();
            switch(input.StreamingStatus)
            {
                case ESLOBSStreamingState.Live:
                    state.Stream = EOBSStreamingState.Started;
                    break;
                case ESLOBSStreamingState.Offline:
                    state.Stream = EOBSStreamingState.Stopped;
                    break;
                case ESLOBSStreamingState.Starting:
                    state.Stream = EOBSStreamingState.Starting;
                    break;
                case ESLOBSStreamingState.Ending:
                    state.Stream = EOBSStreamingState.Stopping;
                    break;
                case ESLOBSStreamingState.Reconnecting:
                    state.Stream = EOBSStreamingState.Reconnecting;
                    break;
                default:
                    state.Stream = EOBSStreamingState.Unkown;
                    break;
            }

            switch (input.RecordingStatus)
            {
                case ESLOBSSRecordingState.Recording:
                    state.Record = EOBSStreamingState.Started;
                    break;
                case ESLOBSSRecordingState.Offline:
                    state.Record = EOBSStreamingState.Stopped;
                    break;
                case ESLOBSSRecordingState.Starting:
                    state.Record = EOBSStreamingState.Starting;
                    break;
                case ESLOBSSRecordingState.Stopping:
                    state.Record = EOBSStreamingState.Stopping;
                    break;
                default:
                    state.Record = EOBSStreamingState.Unkown;
                    break;
            }

            switch (input.ReplayBufferStatus)
            {
                case ESLOBSSReplayBufferState.Running:
                    state.Replay = EOBSStreamingState.Started;
                    break;
                case ESLOBSSReplayBufferState.Offline:
                    state.Replay = EOBSStreamingState.Stopped;
                    break;
                case ESLOBSSReplayBufferState.Saving:
                    state.Replay = EOBSStreamingState.Stopping;
                    break;
                case ESLOBSSReplayBufferState.Stopping:
                    state.Replay = EOBSStreamingState.Stopping;
                    break;
                default:
                    state.Replay = EOBSStreamingState.Unkown;
                    break;
            }

            return state;
        }

        private List<SOBSScene> _translateScenes(List<SLOBSScene> input)
        {
            List<SOBSScene> output = new List<SOBSScene>();
            for (int i=0; i< input.Count; i++)
            {
                output.Add(_translateScene(input[i], i));
            }
            return output;
        }

        private SOBSScene _translateScene(SLOBSScene input, int index = -1)
        {
            SOBSScene output = new SOBSScene();
            output.Index = index == -1 ? Scenes.Count() : index;
            output.Id = input.Id;
            output.ResourceId = input.ResourceId;
            output.Name = input.Name;
            output.Items = new List<SOBSSceneItem>();
            if (input.Nodes != null && input.Nodes.Count > 0)
            {
                for (int i=0; i<input.Nodes.Count; i++)
                {
                    SOBSSceneItem item = _translateSceneItem(input.Nodes[i],i);
                    output.Items.Add(item);
                }
            }
            return output;
        }

        private SOBSSceneItem _translateSceneItem(SLOBSSceneItem input, int index = 0)
        {
            SOBSSceneItem item = new SOBSSceneItem();
            item.Id = input.Id;
            item.SourceId = input.SourceId;
            item.Index = index;
            item.Name = input.Name;
            item.Visible = input.Visible;
            item.IsFolder = input.SceneNodeType == "folder" ? true : false;
            item.ParentId = input.ParentId;
            return item;
        }

        private List<SOBSAudioSource> _translateAudioSources(List<SLOBSAudioSource> input)
        {
            List<SOBSAudioSource> sources = new List<SOBSAudioSource>();
            for (int i=0; i<input.Count; i++)
            {
                sources.Add(_translateAudioSource(input[i], i));
            }
            return sources;
        }

        private SOBSAudioSource _translateAudioSource(SLOBSAudioSource input, int index = 0)
        {
            SOBSAudioSource source = new SOBSAudioSource();
            source.SourceId = input.SourceId;
            source.ResourceId = input.ResourceId;
            source.Index = index;
            source.Name = input.Name;
            source.Muted = input.Muted;
            source.Hidden = input.MixerHidden;
            source.Volume = input.Fader.Db;
            return source;
        }

        private List<SOBSSource> _translateSources(List<SLOBSSource> input)
        {
            List<SOBSSource> sources = new List<SOBSSource>();
            for (int i=0; i<input.Count;i++)
            {
                sources.Add(_translateSource(input[i], i));
            }
            return sources;
        }

        private SOBSSource _translateSource(SLOBSSource input, int index = 0)
        {
            SOBSSource source = new SOBSSource();
            source.Id = input.Id;
            source.Index = index;
            source.ResourceId = input.ResourceId;
            source.Name = input.Name;
            source.Muted = input.Muted;
            source.Video = input.Video;
            source.Audio = input.Audio;
            source.Type = input.Type;
            source.Volume = 0;

            return source;
        }

        private bool _setActiveScene(SLOBSScene input)
        {
            for (int i = 0; i < Scenes.Count; i++)
            {
                if (Scenes[i].Id == input.Id)
                {
                    ActiveScene = _translateScene(input, i);
                    return true;
                }
            }
            return false;
        }
        #endregion

        #region Websocket Events
        private void Ws_SceneItemChanged(SLOBSWebsocket sender, SLOBSSceneItemEvent eventdata)
        {
            if (eventdata.Type == ESLOBSEventType.SceneItemAdded)
            {
                for (int i = 0; i < Scenes.Count; i++) {
                    if (Scenes[i].Id == eventdata.SceneItem.SceneId)
                    {
                        var sceneitem = _translateSceneItem(eventdata.SceneItem, Scenes[i].Items.Count);
                        Scenes[i].Items.Add(sceneitem);
                        if (ActiveScene.Id == Scenes[i].Id) {
                            if (SceneItemChanged != null)
                            {
                                SceneItemChanged(this, EOBSEvent.SceneItemAdded, sceneitem);
                            }
                        }
                    }
                }

            }
            else if(eventdata.Type == ESLOBSEventType.SceneItemRemoved)
            {
                int reindex = 0;
                for (int i = 0; i < Scenes.Count; i++)
                {
                    if (Scenes[i].Id == eventdata.SceneItem.SceneId)
                    {
                        var sceneitem = _translateSceneItem(eventdata.SceneItem, 0);
                        for (int si=0; si< Scenes[i].Items.Count; si++)
                        {
                            if (Scenes[i].Items[si].Id == eventdata.SceneItem.Id) {
                                Scenes[i].Items.RemoveAt(si);
                                sceneitem.Index = si;
                                if (ActiveScene.Id == Scenes[i].Id)
                                {
                                    if (SceneItemChanged != null)
                                    {
                                        SceneItemChanged(this, EOBSEvent.SceneItemRemoved, sceneitem);
                                    }
                                }
                            }
                            else
                            {
                                var item = Scenes[i].Items[si];
                                item.Index = reindex;
                                Scenes[i].Items[si] = item;
                                reindex++;
                            }
                        }
                    }
                }
            }
            else if (eventdata.Type == ESLOBSEventType.SceneItemUpdated)
            {
                for (int i = 0; i < Scenes.Count; i++)
                {
                    if (Scenes[i].Id == eventdata.SceneItem.SceneId)
                    {
                        var sceneitem = _translateSceneItem(eventdata.SceneItem, 0);
                        for (int si = 0; si < Scenes[i].Items.Count; si++)
                        {
                            if (Scenes[i].Items[si].Id == eventdata.SceneItem.Id)
                            {
                                sceneitem.Index = si;
                                Scenes[i].Items[si] = sceneitem;
                                
                                if (ActiveScene.Id == Scenes[i].Id)
                                {
                                    if (SceneItemChanged != null)
                                    {
                                        SceneItemChanged(this, EOBSEvent.SceneItemUpdated, sceneitem);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        private void Ws_SceneListChanged(SLOBSWebsocket sender, List<SLOBSScene> scenes)
        {
            throw new NotImplementedException();
        }

        private async void Ws_SceneChanged(SLOBSWebsocket sender, SLOBSSceneEvent eventdata)
        {
            if (eventdata.Type == ESLOBSEventType.SceneAdded)
            {
                SOBSScene item = _translateScene(eventdata.Scene);
                Scenes.Add(item);
                if (SceneChanged != null)
                {
                    SceneChanged(this, EOBSEvent.SceneAdded, item);
                }
            }
            else if (eventdata.Type == ESLOBSEventType.SceneRemoved)
            {
                int reindex = 0;
                SOBSScene item = _translateScene(eventdata.Scene);
                for (int i = 0; i < Scenes.Count; i++)
                {
                    if (Scenes[i].Id == eventdata.Scene.Id)
                    {
                        Scenes.RemoveAt(i);
                    }else
                    {
                        var scene = Scenes[i];
                        scene.Index = reindex;
                        Scenes[i] = scene;
                        reindex++;
                    }
                }
                if (SceneChanged != null)
                {
                    SceneChanged(this, EOBSEvent.SceneRemoved, item);
                }
            }
            else if (eventdata.Type == ESLOBSEventType.SceneSwitched)
            {
                if (!_setActiveScene(eventdata.Scene))
                {
                    var ascenes = await ws.ListScenes();
                    Scenes.Clear();
                    Scenes = _translateScenes(ascenes);
                    _setActiveScene(eventdata.Scene);
                }

                var audio = await ws.ListAudioSources();
                AudioSources.Clear();
                AudioSources = _translateAudioSources(audio);
                if (AudioSourceChanged != null)
                {
                    AudioSourceChanged(this, EOBSEvent.SourceAdded);
                }

                if (SceneChanged != null)
                {
                    SceneChanged(this, EOBSEvent.SceneSwitched, ActiveScene);
                }
            }
            else if (eventdata.Type == ESLOBSEventType.SceneCollectionChanged)
            {
                var ascenes = await ws.ListScenes();
                Scenes.Clear();
                Scenes = _translateScenes(ascenes);
                var active_scene = await ws.GetActiveScene();
                if (active_scene != null)
                {
                    for (int i = 0; i < Scenes.Count; i++)
                    {
                        if (Scenes[i].Id == ((SLOBSScene)active_scene).Id)
                        {
                            ActiveScene = Scenes[i];
                        }
                    }
                }

                if (SceneChanged != null)
                {
                    SceneChanged(this, EOBSEvent.SceneUpdated, ActiveScene);
                }
            }
        }

        private async void Ws_ConnectionStatus(SLOBSWebsocket sender, SSLOBSConnectionEvent eventdata)
        {
            if (ConnectionStatus != null)
            {
                EOBSCStatus translatedEventType = EOBSCStatus.Unknown;
                if (eventdata.state == ESLOBSConnectionState.Connected)
                {
                    if (_auth != null)
                    {
                        var auth = await ws.Auth(_auth);
                        if (auth)
                        {
                            translatedEventType = EOBSCStatus.Connected;
                            Connected = true;
                            statusTimer.Enabled = true;
                        }
                        else
                        {
                            translatedEventType = EOBSCStatus.WrongAuth;
                            Connected = false;
                            statusTimer.Enabled = false;
                        }
                    }else
                    {
                        translatedEventType = EOBSCStatus.Connected;
                        Connected = true;
                        statusTimer.Enabled = true;
                    }
                }
                else if (eventdata.state == ESLOBSConnectionState.Connecting)
                {
                    translatedEventType = EOBSCStatus.Connecting;
                    Connected = false;
                    statusTimer.Enabled = false;
                }
                else if (eventdata.state == ESLOBSConnectionState.Error)
                {
                    translatedEventType = EOBSCStatus.Error;
                    Connected = false;
                    statusTimer.Enabled = false;
                }
                else if (eventdata.state == ESLOBSConnectionState.Disconnected)
                {
                    translatedEventType = EOBSCStatus.Disconnected;
                    Connected = false;
                    statusTimer.Enabled = false;
                }

                if (Connected)
                {
                    //subscribing all of possible events
                    await ws.SubscribeToEvent("sceneSwitched", "ScenesService");
                    await ws.SubscribeToEvent("sceneAdded", "ScenesService");
                    await ws.SubscribeToEvent("sceneRemoved", "ScenesService");
                    await ws.SubscribeToEvent("itemAdded", "ScenesService");
                    await ws.SubscribeToEvent("itemRemoved", "ScenesService");
                    await ws.SubscribeToEvent("itemUpdated", "ScenesService");

                    await ws.SubscribeToEvent("streamingStatusChange", "StreamingService");
                    await ws.SubscribeToEvent("sourceAdded", "SourcesService");
                    await ws.SubscribeToEvent("sourceRemoved", "SourcesService");
                    await ws.SubscribeToEvent("sourceUpdated", "SourcesService");
                }

                ConnectionStatus(this, translatedEventType);
            }
        }

        private async void Ws_SourceChanged(SLOBSWebsocket sender, SLOBSSourceEvent eventdata)
        {
            if (eventdata.Type == ESLOBSEventType.SourceAdded) {
                var sourceItem = _translateSource(eventdata.Source, Sources.Count+1);
                Sources.Add(sourceItem);
                if (SourceChanged != null)
                {
                    SourceChanged(this, EOBSEvent.SourceAdded, sourceItem);
                }

                //Audio item 
                if (sourceItem.Audio)
                {
                    var audio = await ws.ListAudioSources();
                    AudioSources.Clear();
                    AudioSources = _translateAudioSources(audio);
                    if (AudioSourceChanged != null)
                    {
                        AudioSourceChanged(this, EOBSEvent.SourceAdded);
                    }
                }
            }
            else if(eventdata.Type == ESLOBSEventType.SourceRemoved)
            {
                var sourceItem = _translateSource(eventdata.Source, 0);
                int reindex = 0;
                for (int i=0; i<Sources.Count; i++)
                {
                    if (Sources[i].Id == eventdata.Source.Id)
                    {
                        Sources.RemoveAt(i);
                        sourceItem.Index = i;
                        if (SourceChanged != null)
                        {
                            SourceChanged(this, EOBSEvent.SourceRemoved, sourceItem);
                            //audio item
                            if (sourceItem.Audio)
                            {
                                for (int ia=0;ia<AudioSources.Count;ia++)
                                {
                                    if (AudioSources[i].SourceId == sourceItem.Id)
                                    {
                                        AudioSources.RemoveAt(ia);
                                        if (AudioSourceChanged != null)
                                        {
                                            AudioSourceChanged(this, EOBSEvent.SourceUpdated);
                                        }
                                    }
                                }
                            }
                        }
                    }else
                    {
                        var source = Sources[i];
                        source.Index = reindex;
                        Sources[i] = source;
                        reindex++;
                    }
                }
            }
            else if(eventdata.Type == ESLOBSEventType.SourceUpdated)
            {
                var sourceItem = _translateSource(eventdata.Source, 0);

                for (int i = 0; i < Sources.Count; i++)
                {
                    if (Sources[i].Id == eventdata.Source.Id)
                    {
                        sourceItem.Index = i;
                        Sources[i] = sourceItem;
                        if (SourceChanged != null)
                        {
                            SourceChanged(this, EOBSEvent.SourceUpdated, sourceItem);
                        }

                        //audio item
                        if (sourceItem.Audio)
                        {
                            var audio = await ws.ListAudioSources();
                            AudioSources.Clear();
                            AudioSources = _translateAudioSources(audio);

                            if (AudioSourceChanged != null)
                            {
                                AudioSourceChanged(this, EOBSEvent.SourceUpdated);
                            }
                        }
                    }
                }

            }
        }

        private void Ws_StreamingStatusChanged(SLOBSWebsocket sender, ESLOBSStreamingState state)
        {
            SLOBSStreamingState input = new SLOBSStreamingState();
            input.StreamingStatus = state;
            var output = _translateStreamingStatus(input);
            output.Record = _state.Record;
            output.Replay = _state.Replay;

            if (StreamingStatusChanged != null)
            {
                StreamingStatusChanged(this, output);
            }
        }

        private async void StatusTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            var state = await ws.GetStreamingStatus();
            if (state != null)
            {
                var converted_state = _translateStreamingStatus((SLOBSStreamingState)state);
                if (converted_state.Stream != _state.Stream || 
                    converted_state.Replay != _state.Replay || 
                    converted_state.Record == _state.Record)
                {
                    _state = converted_state;
                    if (StreamingStatusChanged != null)
                    {
                        StreamingStatusChanged(this, converted_state);
                    }
                }
            }
        }

        #endregion
    }
}
