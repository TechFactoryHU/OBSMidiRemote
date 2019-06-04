using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OBSMidiRemote.Lib.PureMidi;
using OBSMidiRemote.Lib.PureMidi.Data;
using OBSMidiRemote.Lib.PureMidi.Definitions;

namespace OBSMidiRemote.Lib.Device
{
    public class CObsDeviceMidi : IOBSDevice
    {

        private InputDeviceItem _input;
        private InputDeviceItem _output;

        private InputDevice _iDevice;
        private OutputDevice _oDevice;

        private bool ready = false;

        public event IOBSDeviceData OnData;
        public event IOBSDeviceStatus OnStatusChanged;

        public CObsDeviceMidi() {}

        public int BaudRate
        {
            get
            {
                return 0;
            }
            set
            {

            }
        }

        public InputDeviceItem Input
        {
            get
            {
                return _input;
            }

            set
            {
                _input = value;
            }
        }

        public InputDeviceItem Output
        {
            get
            {
                return _output;
            }

            set
            {
                _output = value;
            }
        }

        public bool Connect()
        {
            if (_input != null && _input.Index > -1)
            {
                try
                {
                    _iDevice = new InputDevice(_input.Index);
                    _iDevice.OnMidiEvent += OnMidiEvent;
                    _iDevice.Error += OnMidiError;
                    _iDevice.Start();
                    if (_output != null && _output.Index > -1)
                    {
                        _oDevice = new OutputDevice(_output.Index);
                        _oDevice.Error += OnMidiError;
                    }
                    ready = true;
                    if (OnStatusChanged != null) { OnStatusChanged(this, EMidiEvent.DeviceReady); }
                    return true;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
            return false;
        }

        public void Disconnect()
        {
            if (_iDevice != null && !_iDevice.IsDisposed) { _iDevice.Dispose(); }
            if (_oDevice != null && !_oDevice.IsDisposed) { _oDevice.Dispose(); }
        }

        public bool IsReady()
        {
            return ready;
        }

        public void Send(SMidiAction data)
        {
            if (_oDevice != null && !_oDevice.IsDisposed)
            {
                var msg = new byte[3];
                msg[0] = (byte)(data.Cmd | data.Channel); //SendMidiCommand(o.Action);
                msg[1] = (byte)(data.Data1);
                msg[2] = (byte)(data.Data2);
                _oDevice.Send(new MidiEvent(msg));
            }
        }

        private void OnMidiEvent(MidiEvent ev)
        {
            if (ev.MidiEventType == EMidiEventType.Short)
            {
                SMidiAction action = new SMidiAction();
                action.Cmd = ev.AllData[0];
                action.Channel = ev.AllData[0] & 0x0f;
                action.Data1 = ev.AllData[1];
                action.Data2 = ev.AllData[2];

                if (OnData != null)
                {
                    OnData(this, action);
                }
            }
        }

        private void OnMidiError(object sender, ErrorEventArgs e)
        {
            ready = false;
        }
    }
}
