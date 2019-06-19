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
using OBSMidiRemote.Lib;
using OBSMidiRemote.Lib.Device;
using System.Timers;

namespace OBSMidiRemote.Lib
{
    public class COBSMidiRemote
    {
        private IOBSConnector Obs;
        private IOBSDevice Device;
        private CMidiObserver Midi;

        private string _url;
        private int _port;
        private string _auth;

        private System.Timers.Timer connectionTimer;
        public event OBaseStatusChanged StatusChanged;
        private bool connection_start = false;

        public bool Connected { get; private set; }
        public bool Connecting { get; private set; }
        public EOBSConnectorType ConnectorType { get; private set; }

        public COBSMidiRemote() {
            _auth = null;

            connection_start = false;
            connectionTimer = new System.Timers.Timer(2000);
            connectionTimer.Elapsed += onConnectionTimerEvent;
            connectionTimer.AutoReset = true;
            connectionTimer.Enabled = false;
            Connected = false;
            Connecting = false;
        }

        public void SetEndoint(string url, int port, string auth = null)
        {
            _url = url;
            _port = port;
            _auth = auth;
        }

        public void SetDevice(InputDeviceItem input, InputDeviceItem output = null)
        {
            if (Device != null) { Device.Disconnect(); }
            if (input.Type == InputDeviceType.MIDI)
            {
                Device = new CObsDeviceMidi();
                Device.Input = input;
                if (output != null)
                {
                    Device.Output = output;
                }
            }
            else if (input.Type == InputDeviceType.SERIAL)
            {
                Device = new CObsDeviceSerial();
                Device.BaudRate = Midi.serialBaudRate;
                Device.Input = input;
            }
            Device.OnData += Device_OnData;
            Device.OnStatusChanged += Device_OnStatusChanged;
        }

        public void SetConnector(EOBSConnectorType type)
        {
            if (type == EOBSConnectorType.SLOBS)
            {
                Obs = new CSLOBSConnector();
            }else if(type == EOBSConnectorType.OBS)
            {
                Obs = new COBSConnector();
            }

            Obs.ConnectionStatus += Obs_ConnectionStatus;
            Obs.AudioSourceChanged += Obs_AudioSourceChanged;
            Obs.SceneChanged += Obs_SceneChanged;
            Obs.SceneItemChanged += Obs_SceneItemChanged;
            Obs.SceneListChanged += Obs_SceneListChanged;
            Obs.SourceChanged += Obs_SourceChanged;
            Obs.StreamingStatusChanged += Obs_StreamingStatusChanged;
            Obs.TransitionChanged += Obs_TransitionChanged;
            Obs.ModeChanged += Obs_ModeChanged;

            Midi = new CMidiObserver(Obs);
            Midi.OnMidiOutput += Midi_OnMidiOutput;
            ConnectorType = type;
        }

        public bool LoadSchema(string filename)
        {
            if (Midi != null && Midi.LoadSchema(filename))
            {
                if (Device != null) { Device.BaudRate = Midi.serialBaudRate; }
                return true;
            }
            return false;
        }

        public void Connect()
        {
            if (Obs != null && Device != null && _url != null && _port != 0)
            {
                if (StatusChanged != null) StatusChanged(this, EOBSCStatus.Connecting);
                connection_start = true;
                Connecting = true;
                Device.Connect();
            }
        }

        public void Disconnect()
        {
            Obs.Disconnect();
            Midi.ResetSurface();
            if (Device != null) { Device.Disconnect(); }
            connectionTimer.Enabled = false;
            if (StatusChanged != null) StatusChanged(this, EOBSCStatus.Disconnected);
            Connected = false;
            Connecting = false;
        }

        public string GetVersionInfo()
        {
            return Obs.GetVersionInfo();
        }

        private void _renderScenes()
        {
            for (int i = 0; i < Obs.Scenes.Count(); i++)
            {
                if (Obs.Scenes[i].Id == Obs.ActiveScene.Id)
                {
                    Midi.Display(EMidiOBSItemType.Scene, EMidiOBSOutputType.Active, i);
                }
                else
                {
                    Midi.Display(EMidiOBSItemType.Scene, EMidiOBSOutputType.On, i);
                }
            }
        }

        private void _renderSceneItems()
        {
            Midi.Display(EMidiOBSItemType.SceneItem, EMidiOBSOutputType.Off, -1);
            if ((int)Obs.Mode == 2)
            {
                Midi.Display(EMidiOBSItemType.PsceneItem, EMidiOBSOutputType.Off, -1);
            }
            int x = 0;

            foreach (var scene in Obs.Scenes)
            {
                if (scene.Id == Obs.ActiveScene.Id)
                {
                    x = 0;
                    foreach (var scitem in scene.Items)
                    {
                        Midi.Display(EMidiOBSItemType.SceneItem, scitem.Visible ? EMidiOBSOutputType.Active : EMidiOBSOutputType.On, x);
                        x++;
                    }
                }

                if ((int)Obs.Mode == 2 && scene.Id == Obs.ActivePreviewScene.Id)
                {
                    x = 0;
                    foreach (var scitem in scene.Items)
                    {
                        Midi.Display(EMidiOBSItemType.PsceneItem, scitem.Visible ? EMidiOBSOutputType.Active : EMidiOBSOutputType.On, x);
                        x++;
                    }
                }
            }
        }

        #region OBS Events
        private void Obs_StreamingStatusChanged(IOBSConnector sender, SOBSStreamingState state)
        {
            //streaming
            if (state.Stream == EOBSStreamingState.Starting || state.Stream == EOBSStreamingState.Reconnecting)
            {
                Midi.Display(EMidiOBSItemType.Stream, EMidiOBSOutputType.Starting, -1);
            }
            else if(state.Stream == EOBSStreamingState.Stopping)
            {
                Midi.Display(EMidiOBSItemType.Stream, EMidiOBSOutputType.Stopping, -1);
            }
            else if (state.Stream == EOBSStreamingState.Started)
            {
                Midi.Display(EMidiOBSItemType.Stream, EMidiOBSOutputType.Active, -1);
            }
            else if (state.Stream == EOBSStreamingState.Stopped)
            {
                Midi.Display(EMidiOBSItemType.Stream, EMidiOBSOutputType.On, -1);
            }

            //recording
            if (state.Record == EOBSStreamingState.Starting)
            {
                Midi.Display(EMidiOBSItemType.Record, EMidiOBSOutputType.Starting, -1);
            }
            else if (state.Record == EOBSStreamingState.Stopping)
            {
                Midi.Display(EMidiOBSItemType.Record, EMidiOBSOutputType.Stopping, -1);
            }
            else if (state.Record == EOBSStreamingState.Started)
            {
                Midi.Display(EMidiOBSItemType.Record, EMidiOBSOutputType.Active, -1);
            }
            else if (state.Record == EOBSStreamingState.Stopped)
            {
                Midi.Display(EMidiOBSItemType.Record, EMidiOBSOutputType.On, -1);
            }

            //replaybuffer
            if (state.Replay == EOBSStreamingState.Starting || state.Replay == EOBSStreamingState.Saving)
            {
                Midi.Display(EMidiOBSItemType.ReplayBuffer, EMidiOBSOutputType.Starting, -1);
            }
            else if (state.Replay == EOBSStreamingState.Stopping)
            {
                Midi.Display(EMidiOBSItemType.ReplayBuffer, EMidiOBSOutputType.Stopping, -1);
            }
            else if (state.Replay == EOBSStreamingState.Started)
            {
                Midi.Display(EMidiOBSItemType.ReplayBuffer, EMidiOBSOutputType.Active, -1);
            }
            else if (state.Replay == EOBSStreamingState.Stopped)
            {
                Midi.Display(EMidiOBSItemType.ReplayBuffer, EMidiOBSOutputType.On, -1);
            }

            if (state.Replay == EOBSStreamingState.Saving)
            {
                Midi.Display(EMidiOBSItemType.ReplayBufferSave, EMidiOBSOutputType.Active, -1);
            }
            else {
                Midi.Display(EMidiOBSItemType.ReplayBufferSave, EMidiOBSOutputType.On, -1);
            }
        }

        private void Obs_SourceChanged(IOBSConnector sender, EOBSEvent eventtype, SOBSSource eventdata)
        {
           
        }

        private void Obs_SceneListChanged(IOBSConnector sender, EOBSEvent eventtype, List<SOBSScene> scenes)
        {
            _renderScenes();
            _renderSceneItems();
            Midi.FlushQueue();
        }

        private void Obs_SceneItemChanged(IOBSConnector sender, EOBSEvent eventtype, SOBSSceneItem eventdata)
        {
            _renderSceneItems();
        }

        private void Obs_SceneChanged(IOBSConnector sender, EOBSEvent eventtype, SOBSScene scene)
        {
            // throw new NotImplementedException();
            if (eventtype == EOBSEvent.SceneAdded || eventtype == EOBSEvent.SceneRemoved)
            {
                if (EOBSEvent.SceneRemoved == eventtype)
                {
                    Midi.Display(EMidiOBSItemType.Scene, EMidiOBSOutputType.Off, -1);
                }
                _renderScenes();
            }
            else if (eventtype == EOBSEvent.SceneSwitched || eventtype == EOBSEvent.SceneUpdated)
            {
                _renderScenes();
                _renderSceneItems();
            }
            Midi.FlushQueue();
        }

        private void Obs_AudioSourceChanged(IOBSConnector sender, EOBSEvent eventtype)
        {
            Midi.Display(EMidiOBSItemType.AudioItem, EMidiOBSOutputType.Off, -1, -1, true);
            for (int i=0; i<Obs.AudioSources.Count; i++)
            {
                if (!Obs.AudioSources[i].Hidden)
                {
                    if (Obs.AudioSources[i].Muted)
                    {
                        Midi.Display(EMidiOBSItemType.AudioItem, EMidiOBSOutputType.Muted, i, -1);
                    }
                    else
                    {
                        Midi.Display(EMidiOBSItemType.AudioItem, EMidiOBSOutputType.On, i, -1);
                    }
                }
            }
        }

        private async void Obs_ConnectionStatus(IOBSConnector sender, EOBSCStatus eventdata)
        {
            if (eventdata == EOBSCStatus.Connected)
            {
                Connected = true;
                Connecting = false;
                Midi.Display(EMidiOBSItemType.ReloadObsData, EMidiOBSOutputType.Off, -1, -1, true);

                await Obs.Reload();

                Midi.Display(EMidiOBSItemType.ReloadObsData, EMidiOBSOutputType.On, -1, -1, true);
                Midi.RenderSurface();
            }
            else if(eventdata == EOBSCStatus.Disconnected)
            {
                Midi.ResetSurface();
            }

            if (StatusChanged != null)
            {
                StatusChanged(this, eventdata);
            }
        }

        private void Obs_ModeChanged(IOBSConnector sender, EOBSMode mode)
        {
            if (mode == EOBSMode.Normal)
            {
                Midi.Display(EMidiOBSItemType.Pscene, EMidiOBSOutputType.Off, -1, -1);
                Midi.Display(EMidiOBSItemType.PsceneItem, EMidiOBSOutputType.Off, -1, -1);
                Midi.Display(EMidiOBSItemType.Scene, EMidiOBSOutputType.Off, -1, -1);
                Midi.Display(EMidiOBSItemType.SceneItem, EMidiOBSOutputType.Off, -1, -1);
            }
            Midi.Display(EMidiOBSItemType.Mode, (int)Obs.Mode == 2 ? EMidiOBSOutputType.Active : EMidiOBSOutputType.On, -1);
            _renderScenes();
            _renderSceneItems();
            Midi.FlushQueue();
        }

        private void Obs_TransitionChanged(IOBSConnector sender, EOBSEvent eventtype)
        {
            if (eventtype == EOBSEvent.TransitionListChanged)
            {
                Midi.Display(EMidiOBSItemType.Transition, EMidiOBSOutputType.Off, -1);
            }

            int tr_i = 0;
            if (Obs.Transitions.Count > 0)
            {
                foreach (var tr in Obs.Transitions)
                {
                    if (Obs.ActiveTransition.Id == tr.Id)
                    {
                        Midi.Display(EMidiOBSItemType.Transition, EMidiOBSOutputType.Active, tr_i);
                    }
                    else
                    {
                        Midi.Display(EMidiOBSItemType.Transition, EMidiOBSOutputType.On, tr_i);
                    }
                    tr_i++;
                }
            }
        }

        #endregion

        #region Midi device events
        private void Device_OnData(IOBSDevice sender, SMidiAction data) { 
            Midi.ParseInputData(data);
        }

        private void Device_OnStatusChanged(IOBSDevice sender, EMidiEvent eventType)
        {
           // if (StatusChanged != null) { StatusChanged(this, eventType); }

            if (eventType == EMidiEvent.DeviceReady)
            {
                Midi.ResetSurface();
                connection_start = true;
                Obs.Connect(_url, _port, _auth);
                connectionTimer.Enabled = true;
            }
        }

        private void Midi_OnMidiOutput(CMidiObserver sender, SMidiAction data)
        {
            if (Device != null)
            {
                Device.Send(data);
            }
        }
        #endregion

        private void onConnectionTimerEvent(Object source, ElapsedEventArgs e)
        {
            Obs.CheckTimeouts();
        }
    }
}
