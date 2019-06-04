using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OBSMidiRemote.Lib.OBSWebsocketdotnet;
using OBSMidiRemote.Lib.Device;
using System.Xml.Linq;
using System.Xml.XPath;
using System.Timers;
using System.Windows.Forms;
using Newtonsoft.Json.Linq;
using System.Threading;

namespace OBSMidiRemote.Lib
{
    public class CDeviceObsGw
    {
        private OBSWebsocket websocket;
        private string OBSUrl;
        private string OBSPwd;
        private IOBSDevice Device;
        private CMidiObserver MidiObserver;

        public List<OBSSourceTypes> ObsTypes;
        public List<OBSScene> Scenes;
        public List<string> Transitions;
        public List<OBSSourceItem> SourceItems;
        public TransitionSettings CurrentTransition;
        public int _audiosources;
        public int Mode; //ObsMode (Studio/Normal)

        public int ActiveScene;
        public int ActivePScene;
        public string ActiveSceneName;
        public string ActivePSceneName;

        public OBSVersion Version;
        public OutputStatus OutputStatus;
        private bool connection_start;

        private System.Timers.Timer connectionTimer;
        public event StatusChanged StatusChanged;

        public CDeviceObsGw() {
            MidiObserver = new CMidiObserver(this);
            MidiObserver.OnMidiOutput += MidiObserver_OnMidiOutput;

            websocket = new OBSWebsocket();
            websocket.Connected += onConnect;
            websocket.Disconnected += onDisconnect;
            websocket.SceneChanged += onSceneChange;
            websocket.PreviewSceneChanged += onPSceneChange;
            websocket.SceneCollectionChanged += onSceneColChange;
            websocket.ProfileChanged += onProfileChange;
            websocket.TransitionChanged += onTransitionChange;
            websocket.TransitionDurationChanged += onTransitionDurationChange;

            websocket.StreamingStateChanged += onStreamingStateChange;
            websocket.RecordingStateChanged += onRecordingStateChange;

            websocket.SceneItemVisibilityChanged += onSceneItemVisibilityChange;
            websocket.SceneItemAdded += onSceneItemChange;
            websocket.SceneItemRemoved += onSceneItemChange;
            websocket.StreamStatus += onStreamData;
            websocket.SourceOrderChanged += onSourceOrderChange;
            websocket.StudioModeSwitched += onModeChange;
            websocket.OnError += onWebsocketError;
            websocket.AsyncQueueEmpty += onAsyncQueue;

            connection_start = false;
            connectionTimer = new System.Timers.Timer(2000);
            connectionTimer.Elapsed += onConnectionTimerEvent;
            connectionTimer.AutoReset = true;
            connectionTimer.Enabled = false;
        }

        public void SetDevice(InputDeviceItem input, InputDeviceItem output = null)
        {
            if (Device != null) { Device.Disconnect();  }
            if (input.Type == InputDeviceType.MIDI)
            {
                Device = new CObsDeviceMidi();
                Device.Input = input;
                if (output != null)
                {
                    Device.Output = output;
                }
            }else if (input.Type == InputDeviceType.SERIAL)
            {
                Device = new CObsDeviceSerial();
                Device.BaudRate = MidiObserver.serialBaudRate;
                Device.Input    = input;
            }

            Console.WriteLine("Device set");

            Device.OnData += Device_OnData;
            Device.OnStatusChanged += Device_OnStatusChanged;
        }

        public bool loadSchema(string filename)
        {
            if (MidiObserver.loadSchema(filename))
            {
                if (Device != null) { Device.BaudRate = MidiObserver.serialBaudRate; }
                return true;
            }
            return false;
        }

        public void Start(String url, String pwd = null)
        {
            OBSUrl = url;
            OBSPwd = pwd;
            if (Device != null) {
                if (StatusChanged != null) StatusChanged(this, EMidiEvent.Connecting);
                connection_start = true;
                Device.Connect();
            }
        }

        public void Stop()
        {
           
            if (websocket.IsConnected)
            {
                websocket.Disconnect();
            }

            MidiObserver.ResetSurface();
            if (Device != null) { Device.Disconnect(); }

            connectionTimer.Enabled = false;
            if (StatusChanged != null) StatusChanged(this, EMidiEvent.Disconnected);
        }

        public bool Connected()
        {
            if (websocket.IsConnected) return true;
            return false;
        }

        public bool Connecting()
        {
            if (!websocket.IsConnected && connection_start) return true;
            return false;
        }

        public void Refresh()
        {
            if (websocket.IsConnected)
            {
                MidiObserver.Display(EMidiOBSItemType.ReloadOBSData, EMidiOBSOutputType.Off, -1, -1, true);

                var studiomode = websocket.StudioModeEnabled();
                Mode = studiomode ? 2 : 1;
                OutputStatus = websocket.GetStreamingStatus();
                CurrentTransition = websocket.GetCurrentTransition();

                LoadSources();
                LoadScenesAndSources();
                LoadTransitions();

                if (studiomode)
                {
                    ActivePScene = -1;
                    var pscene = websocket.GetPreviewScene();
                    ActivePSceneName = pscene.Name;
                    for (int i = 0; i < Scenes.Count(); i++)
                    {
                        if (Scenes[i].Name == ActivePSceneName)
                        {
                            ActivePScene = i;
                        }
                    }
                }

                MidiObserver.Display(EMidiOBSItemType.ReloadOBSData, EMidiOBSOutputType.On, -1, -1, true);
                MidiObserver.RenderSurface();
          
            }
        }


        #region EventHandlers
        private void onWebsocketError(object sender, EventArgs e)
        {
            //throw new NotImplementedException();
            if (StatusChanged != null) StatusChanged(this, EMidiEvent.Error);
        }

        private void onConnectionTimerEvent(Object source, ElapsedEventArgs e)
        {
            websocket.CheckTimeouts();
        }

        private void Device_OnStatusChanged(IOBSDevice sender, EMidiEvent eventType)
        {
            if (StatusChanged != null) { StatusChanged(this, eventType); }
            if (eventType == EMidiEvent.DeviceReady)
            {
                MidiObserver.ResetSurface();
               
                connection_start = true;
                websocket.ConnectAsync(OBSUrl);
                connectionTimer.Enabled = true;
            }
        }

        private void Device_OnData(IOBSDevice sender, SMidiAction data)
        {
           /* Console.Write(data.Cmd.ToString("X2")+" ");
            Console.Write(data.Channel.ToString("X2") + " ");
            Console.Write(data.Data1.ToString("X2") + " ");
            Console.WriteLine(data.Data2.ToString("X2") + " ");
            */
            MidiObserver.ParseInputData(data);
        }

        private void MidiObserver_OnMidiOutput(CMidiObserver sender, SMidiAction data)
        {
            if (Device != null)
            {
                //Console.WriteLine(data.Cmd.ToString("x2") + ", " + data.Data1.ToString("x2") + ", " + data.Data2.ToString("x2"));
                Device.Send(data);
            }
        }
        #endregion

        #region OBS functions (through websocket)

        public void StartRecording()
        {
            websocket.StartRecording();
        }

        public void StopRecording()
        {
            websocket.StopRecording();
        }

        public void SetPreviewScene(String name) {
            try
            {
                websocket.SetPreviewScene(name);
            }
            catch(ErrorResponseException e)
            {
                //
            }
        }

        public void SetCurrentScene(String name)
        {
            websocket.SetCurrentScene(name);
        }

        public void LoadActiveScene()
        {
            var scene = websocket.GetCurrentScene();
            for (int x = 0; x < scene.Items.Count(); x++)
            {
                var item = scene.Items[x];
                var prop = websocket.GetSceneItemProperties(scene.Name, scene.Items[x].SourceName);
                item.Visible = prop.Visible;
                scene.Items[x] = item;
            }

            bool found = false;
            for (int i = 0; i < Scenes.Count(); i++)
            {
                if (Scenes[i].Name == scene.Name)
                {
                    Scenes[i] = scene;
                    found = true;
                    break;
                }
            }

            if (!found)
            {
                Scenes.Add(scene);
            }
            ActiveSceneName = scene.Name;
        }

        public void LoadScenesAndSources()
        {
            var scene = websocket.GetCurrentScene();
            ActiveSceneName = scene.Name;
            Scenes = websocket.ListScenes();
            for (int i = 0; i < Scenes.Count(); i++)
            {
                if (Scenes[i].Name == ActiveSceneName)
                {
                    ActiveScene = i;
                }

                for (int x = 0; x < Scenes[i].Items.Count(); x++)
                {
                    var prop = websocket.GetSceneItemProperties(Scenes[i].Name, Scenes[i].Items[x].SourceName);
                    var item = Scenes[i].Items[x];
                    item.Visible = prop.Visible;
                    Scenes[i].Items[x] = item;
                }
            }
            _showSceneItems();
        }

        public void LoadTransitions()
        {
            Transitions = websocket.ListTransitions();
        }

        public void ToggleSceneItemVisibility(int scene_id, int item_id)
        {
            if (scene_id >= 0 && scene_id < Scenes.Count())
            {
                if (item_id >= 0 && item_id < Scenes[scene_id].Items.Count())
                {
                    websocket.SetSceneItemVisibility(Scenes[scene_id].Items[item_id].SourceName, !Scenes[scene_id].Items[item_id].Visible, Scenes[scene_id].Name);
                    if (Mode == 2)
                    {
                        //missing feature in obs-websocket
                        //In studiomode SceneItemVisibilityChange event not fired on preview scenes
                        LoadScenesAndSources();
                    }
                }
            }
        }

        public void SetSceneItemVisibility(int scene_id, int item_id, bool value)
        {
            if (scene_id >= 0 && scene_id < Scenes.Count())
            {
                if (item_id >= 0 && item_id < Scenes[scene_id].Items.Count())
                {
                    websocket.SetSceneItemVisibility(Scenes[scene_id].Items[item_id].SourceName, value, Scenes[scene_id].Name);
                }
            }
        }

        public void SetSourceAudioMute(int audio_source_id, bool mute)
        {
            int a = 0;
            for (int i = 0; i < SourceItems.Count(); i++)
            {
                if (SourceHasAudio(SourceItems[i].TypeId))
                {
                    if (a == audio_source_id)
                    {
                        var item = SourceItems[i];
                        item.Muted = mute;
                        websocket.SetMute(item.Name, mute);
                        SourceItems[i] = item;

                        if (mute)
                        {
                            MidiObserver.Display(EMidiOBSItemType.AudioItem, EMidiOBSOutputType.Muted, a, 0, true);
                        }
                        else
                        {
                            MidiObserver.Display(EMidiOBSItemType.AudioItem, EMidiOBSOutputType.On, a, 0, true);
                        }
                    }
                    a++;
                }
            }
        }

        public void ToggleSourceAudioMute(int audio_source_id)
        {
            int a = 0;
            for (int i = 0; i < SourceItems.Count(); i++)
            {
                if (SourceHasAudio(SourceItems[i].TypeId))
                {
                    if (a == audio_source_id)
                    {
                        var item = SourceItems[i];
                        websocket.ToggleMute(item.Name);
                        var volInfo = websocket.GetVolume(item.Name);
                        item.Muted = volInfo.Muted;
                        item.Volume = volInfo.Volume;
                        SourceItems[i] = item;
                        if (item.Muted)
                        {
                            MidiObserver.Display(EMidiOBSItemType.AudioItem, EMidiOBSOutputType.Muted, a, 0, true);
                        }
                        else
                        {
                            MidiObserver.Display(EMidiOBSItemType.AudioItem, EMidiOBSOutputType.On, a, 0, true);
                        }
                    }
                    a++;
                }
            }
        }

        public void SetSourceAudioVolume(int audio_source_id, float volume)
        {
            int a = 0;
            foreach (var item in SourceItems)
            {
                if (SourceHasAudio(item.TypeId))
                {
                    if (a == audio_source_id)
                    {
                        websocket.SetVolume(item.Name, volume);
                    }
                    a++;
                }
            }
        }

        public void SetTransition(int tr_id)
        {
            for (int i = 0; i < Transitions.Count(); i++)
            {
                if (i == tr_id)
                {
                    websocket.SetCurrentTransition(Transitions[tr_id]);
                }
            }
        }

        public void SetStudioMode(bool mode)
        {
            try
            {
                websocket.SetStudioMode(mode);
            }
            catch (Exception e)
            {
                /*if (mode == true)
                {
                    onModeChange(websocket, true);
                }
                else
                {
                    onModeChange(websocket, false);
                }*/
            }
        }

        #endregion

        #region OBS/Websocket helper functions

        private void _showSceneItems()
        {
            MidiObserver.Display(EMidiOBSItemType.SceneItem, EMidiOBSOutputType.Off, -1);
            if (Mode == 2)
            {
                MidiObserver.Display(EMidiOBSItemType.PSceneItem, EMidiOBSOutputType.Off, -1);
            }
            int x = 0;

            foreach (var scene in Scenes)
            {
                if (scene.Name == ActiveSceneName)
                {
                    x = 0;
                    foreach (var scitem in scene.Items)
                    {
                        MidiObserver.Display(EMidiOBSItemType.SceneItem, scitem.Visible ? EMidiOBSOutputType.Active : EMidiOBSOutputType.On, x);
                        x++;
                    }
                }

                if (Mode == 2 && scene.Name == ActivePSceneName)
                {
                    x = 0;
                    foreach (var scitem in scene.Items)
                    {
                        MidiObserver.Display(EMidiOBSItemType.PSceneItem, scitem.Visible ? EMidiOBSOutputType.Active : EMidiOBSOutputType.On, x);
                        x++;
                    }
                }
            }
            MidiObserver.FlushQueue();
        }

        public bool SourceHasAudio(string sourceTypeId)
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

        public void LoadSources()
        {
            SourceItems = websocket.GetSourcesList();
            for (int i = 0; i < SourceItems.Count(); i++)
            {
                if (SourceHasAudio(SourceItems[i].TypeId))
                {
                    var item = SourceItems[i];
                    var volinfo = websocket.GetVolume(SourceItems[i].Name);
                    item.Muted = volinfo.Muted;
                    item.Volume = volinfo.Volume;
                    SourceItems[i] = item;
                }
            }
            //reversing because sceneitems applied first and main audio devices last
            //we need main audio devices/and mics first
            SourceItems.Reverse();
        }

        public void StartStreaming()
        {
            websocket.StartStreaming();
        }

        public void StopStreaming()
        {
            websocket.StopStreaming();
        }

        public void onAsyncQueue(object sender, EventArgs e)
        {
            Refresh();
            if (StatusChanged != null) StatusChanged(this, EMidiEvent.Connected);
        }

        private void onConnect(object sender, EventArgs e)
        {
            connection_start = true;
            websocket.SendRequestAsync(delegate (JObject r)
            {
                OBSAuthInfo authInfo = new OBSAuthInfo(r);
                if (authInfo.AuthRequired)
                {
                    websocket.AuthenticateAsync(delegate (JObject authresponse)
                    {
                        Console.WriteLine(authresponse);
                        /*cstatus = false;
                        if (midiStatus != null) midiStatus(this, SMidiEvent.WrongPassword);
                        return;*/
                    }, OBSPwd, authInfo);
                }
            }, "GetAuthRequired");

            if (ObsTypes == null)
            {
                ObsTypes = new List<OBSSourceTypes>();
                websocket.SendRequestAsync(delegate (JObject response)
                {
                    Version = new OBSVersion(response);
                }, "GetVersion");

                websocket.SendRequestAsync(delegate (JObject response)
                {
                    if ((string)response["status"] == "ok")
                    {
                        JArray items = (JArray)response["types"];
                        ObsTypes.Clear();
                        foreach (JObject typeData in items)
                        {
                            OBSSourceTypes t = new OBSSourceTypes(typeData);
                            ObsTypes.Add(t);
                        }
                    }
                }, "GetSourceTypesList");
            }
        }

        private void onDisconnect(object sender, EventArgs e)
        {
            connection_start = false;
            var ce = (WebSocketSharp.CloseEventArgs)e;
            Console.WriteLine(ce.Code);

            //cant connect
            if (ce.Code == 1006)
            {
                if (StatusChanged != null) StatusChanged(this, EMidiEvent.Error);
            }
            else
            {
                if (StatusChanged != null) StatusChanged(this, EMidiEvent.Disconnected);
            }
        }

        private void onSceneChange(OBSWebsocket sender, string newSceneName)
        {
            ActiveSceneName = newSceneName;
            //MidiObserver.Display(SMidiOBSItemType.Scene, SMidiOBSOutputType.Off, -1);
            for (int i = 0; i < Scenes.Count(); i++)
            {
                if (Scenes[i].Name == newSceneName)
                {
                    ActiveScene = i;
                    MidiObserver.Display(EMidiOBSItemType.Scene, EMidiOBSOutputType.Active, i);
                }
                else
                {
                    MidiObserver.Display(EMidiOBSItemType.Scene, EMidiOBSOutputType.On, i);
                }
            }
            _showSceneItems();
        }

        private void onPSceneChange(OBSWebsocket sender, string newPSceneName)
        {
            ActivePSceneName = newPSceneName;
            MidiObserver.Display(EMidiOBSItemType.PScene, EMidiOBSOutputType.Off, -1);
            for (int i = 0; i < Scenes.Count(); i++)
            {
                if (Scenes[i].Name == newPSceneName)
                {
                    ActivePScene = i;
                    MidiObserver.Display(EMidiOBSItemType.PScene, EMidiOBSOutputType.Active, i);
                }
                else
                {
                    MidiObserver.Display(EMidiOBSItemType.PScene, EMidiOBSOutputType.On, i);
                }
            }
            _showSceneItems();
        }

        private void currentSceneLoaded(OBSScene scene)
        {
            for (int i = 0; i < Scenes.Count(); i++)
            {
                if (Scenes[i].Name == scene.Name)
                {
                    Scenes[i] = scene;
                }
            }
            MidiObserver.RenderSurface();
        }

        private void onSourceOrderChange(OBSWebsocket sender, string sceneName)
        {
            websocket.SendRequestAsync(delegate (JObject r)
            {
                JArray items = (JArray)r["sources"];
                SourceItems.Clear();
                foreach (JObject data in items)
                {
                    OBSSourceItem s = new OBSSourceItem(data);
                    SourceItems.Add(s);
                }

                websocket.SendRequestAsync(delegate (JObject x) {
                    currentSceneLoaded(new OBSScene(x));
                }, "GetCurrentScene");

            }, "GetSourcesList");
        }

        private void onSceneItemVisibilityChange(OBSWebsocket sender, string sceneName, string itemName, bool isVisible)
        {
            for (int i = 0; i < Scenes.Count(); i++)
            {
                if (Scenes[i].Name == sceneName)
                {
                    for (int x = 0; x < Scenes[i].Items.Count(); x++)
                    {
                        if (Scenes[i].Items[x].SourceName == itemName)
                        {
                            var chgSrc = Scenes[i].Items[x];
                            chgSrc.Visible = isVisible;
                            Scenes[i].Items[x] = chgSrc;

                            if (Scenes[i].Name == ActiveSceneName)
                            {
                                MidiObserver.Display(EMidiOBSItemType.SceneItem, isVisible ? EMidiOBSOutputType.Active : EMidiOBSOutputType.On, x, 0, true);
                            }

                            if (Scenes[i].Name == ActivePSceneName)
                            {
                                //missing feature in obs-websocket
                                //In studiomode itemVisibilityChange event not fired on preview scenes
                                MidiObserver.Display(EMidiOBSItemType.PSceneItem, isVisible ? EMidiOBSOutputType.Active : EMidiOBSOutputType.On, x, 0, true);
                            }
                        }
                    }
                }
            }
        }

        private void onSceneItemChange(OBSWebsocket sender, string sceneName, string itemName, bool isVisible)
        {
            onSourceOrderChange(sender, sceneName);
        }

        private void onSceneColChange(object sender, EventArgs e)
        {
            LoadScenesAndSources();
        }

        private void onProfileChange(object sender, EventArgs e)
        {
            LoadScenesAndSources();
        }

        private void onTransitionChange(OBSWebsocket sender, string newTransitionName)
        {
            CurrentTransition.Name = newTransitionName;
            for (int i = 0; i < Transitions.Count(); i++)
            {
                if (Transitions[i] == CurrentTransition.Name)
                {
                    MidiObserver.Display(EMidiOBSItemType.Transition, EMidiOBSOutputType.Active, i);
                }
                else
                {
                    MidiObserver.Display(EMidiOBSItemType.Transition, EMidiOBSOutputType.On, i);
                }
            }

            MidiObserver.FlushQueue();
        }

        private void onTransitionDurationChange(OBSWebsocket sender, int newDuration)
        {
            // tbTransitionDuration.Value = newDuration;
        }

        private void onTransitionListChanged(OBSWebsocket sender)
        {
            websocket.SendRequestAsync(delegate (JObject r) {
                JArray items = (JArray)r["transitions"];
                Transitions.Clear();
                MidiObserver.Display(EMidiOBSItemType.Transition, EMidiOBSOutputType.Off, -1);
                int i = 0;
                foreach (JObject item in items)
                {
                    Transitions.Add((string)item["name"]);
                    if (CurrentTransition.Name == (string)item["name"])
                    {
                        MidiObserver.Display(EMidiOBSItemType.Transition, EMidiOBSOutputType.Active, i);
                    }
                    else
                    {
                        MidiObserver.Display(EMidiOBSItemType.Transition, EMidiOBSOutputType.On, i);
                    }
                    i++;
                }
                MidiObserver.FlushQueue();
            }, "GetTransitionList");
        }

        private void onStreamingStateChange(OBSWebsocket sender, OutputState newState)
        {
            if (newState == OutputState.Started)
            {
                MidiObserver.Display(EMidiOBSItemType.Stream, EMidiOBSOutputType.Active, -1);
                OutputStatus.IsStreaming = true;
            }
            else if (newState == OutputState.Starting)
            {
                MidiObserver.Display(EMidiOBSItemType.Stream, EMidiOBSOutputType.Starting, -1);
            }
            else if (newState == OutputState.Stopping)
            {
                MidiObserver.Display(EMidiOBSItemType.Stream, EMidiOBSOutputType.Stopping, -1);
            }
            else if (newState == OutputState.Stopped)
            {
                OutputStatus.IsStreaming = false;
                MidiObserver.Display(EMidiOBSItemType.Stream, EMidiOBSOutputType.On, -1);
            }
        }

        private void onRecordingStateChange(OBSWebsocket sender, OutputState newState)
        {
            if (newState == OutputState.Started)
            {
                MidiObserver.Display(EMidiOBSItemType.Record, EMidiOBSOutputType.Active, -1);
                OutputStatus.IsRecording = true;
            }
            else if (newState == OutputState.Starting)
            {
                MidiObserver.Display(EMidiOBSItemType.Record, EMidiOBSOutputType.Starting, -1);
            }
            else if (newState == OutputState.Stopping)
            {
                MidiObserver.Display(EMidiOBSItemType.Record, EMidiOBSOutputType.Stopping, -1);
            }
            else if (newState == OutputState.Stopped)
            {
                OutputStatus.IsRecording = false;
                MidiObserver.Display(EMidiOBSItemType.Record, EMidiOBSOutputType.On, -1);
            }
        }

        private void onStreamData(OBSWebsocket sender, StreamStatus data)
        {
        }

        private void onModeChange(OBSWebsocket sender, bool studioMode)
        {
            Console.WriteLine("Studio mode changed!");

            MidiObserver.Display(EMidiOBSItemType.PScene, EMidiOBSOutputType.Off, -1, -1);
            MidiObserver.Display(EMidiOBSItemType.PSceneItem, EMidiOBSOutputType.Off, -1, -1);
            if (studioMode)
            {
                Mode = 2;
                websocket.SendRequestAsync(delegate (JObject r) {
                    if (r["name"] != null)
                    {
                        ActivePSceneName = (string)r["name"];

                    }
                    MidiObserver.RenderSurface();
                }, "GetPreviewScene");
            }
            else
            {
                Mode = 1;
                ActivePSceneName = "";
                ActivePScene = -1;
                MidiObserver.RenderSurface();
            }
        }

        #endregion
    }
}
