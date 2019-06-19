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
using System.IO.Ports;
using OBSMidiRemote.Lib.PureMidi.Data;

namespace OBSMidiRemote.Lib.Device
{
    class CObsDeviceSerial : IOBSDevice
    {
        private InputDeviceItem _input;
        private InputDeviceItem _output;
        private SerialPort _serialPort;

        private int _timeout = 2; //in seconds
        private int _baudRate = 115200;
        private int _dataBits = 8;
        private Handshake _handshake = Handshake.None;
        private Parity _parity = Parity.None;
        private StopBits _stopBits = StopBits.One;
        private byte _terminator = 0x4;

        private byte[] _cmdbuffer = new byte[4];
        private int _cmdbufferindex = 0;

        private bool ready = false;

        //handshake
        private byte _hsCmd = 0x10;
        private byte _hsChannel = 0x01;
        private byte _hsData1 = 0x1A;
        private byte _hsData2 = 0x1B;

        public event IOBSDeviceData OnData;
        public event IOBSDeviceStatus OnStatusChanged;

        private System.Timers.Timer handshakeTimer;

        public CObsDeviceSerial() {
            handshakeTimer = new System.Timers.Timer();
            handshakeTimer.Enabled = false;
            handshakeTimer.Interval = _timeout * 1000;
            handshakeTimer.Elapsed += HandshakeTimer_Elapsed;
        }

        private void HandshakeTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (!ready)
            {
                Disconnect();
                OnStatusChanged(this, EMidiEvent.DeviceError);
            }
            handshakeTimer.Enabled = false;
        }

        public int BaudRate
        {
            get
            {
                return _baudRate;
            }
            set
            {
                _baudRate = value;
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
                    _serialPort = new SerialPort((string)_input.Value, 115200);
                    _serialPort.ReadTimeout = _timeout * 1000;
                    _serialPort.DataBits = _dataBits;
                    _serialPort.Handshake = _handshake;
                    _serialPort.Parity = _parity;
                    _serialPort.StopBits = _stopBits;
                    _serialPort.RtsEnable = true;
                    _serialPort.Encoding = Encoding.ASCII;
                    _serialPort.DataReceived += new SerialDataReceivedEventHandler(_readPort);
                    _serialPort.Open();

                    //check serial device with a "status" command
                    //serial device MUST reply with same command and with Data1=_hsData2 Data2=_hsData1 
                    SMidiAction d = new SMidiAction();
                    d.Cmd     = _hsCmd;
                    d.Channel = _hsChannel;
                    d.Data1   = _hsData1;
                    d.Data2   = _hsData2;

                    Reset();
                    Send(d);

                    handshakeTimer.Enabled = true;
                    OnStatusChanged(this, EMidiEvent.Connecting);
                    return true;
                }
                catch (Exception e)
                {
                    if (OnStatusChanged!=null)
                    {
                        OnStatusChanged(this, EMidiEvent.DeviceError);
                    }
                }
            }
            return false;
        }

        public void Disconnect()
        {
            if (_serialPort.IsOpen) {
                _serialPort.Close();
            }
            handshakeTimer.Enabled = false;
            OnStatusChanged(this, EMidiEvent.DeviceDisconnected);
        }

        public bool IsReady()
        {
            return ready;
        }

        private void Reset()
        {
            _serialPort.Write(new byte[1] { _terminator }, 0, 1);
        }

        public void Send(SMidiAction data)
        {
            if (_serialPort.IsOpen)
            {
                var msg = new byte[5];
                msg[0] = (byte)(data.Cmd >> 4);
                msg[1] = (byte)(data.Cmd | data.Channel);
                msg[2] = (byte)(data.Data1);
                msg[3] = (byte)(data.Data2);
                msg[4] = (byte)_terminator;
                _serialPort.Write(msg, 0, 5);
            }
        }

        private void _readPort(object sender, SerialDataReceivedEventArgs e)
        {
            int canreadbytes = _serialPort.BytesToRead;
            byte[] readbuffer = new byte[canreadbytes];
            int bytesRead = _serialPort.Read(readbuffer, 0, readbuffer.Length);

            for (int i=0; i<bytesRead; i++)
            {
                if (readbuffer[i] == _terminator)
                {
                    if (_cmdbufferindex == 4)
                    {
                        _parseBuffer();
                    }
                    _resetBuffer();
                }else
                {
                    if (_cmdbufferindex > 3) { _resetBuffer(); }
                    _cmdbuffer[_cmdbufferindex] = readbuffer[i];
                    _cmdbufferindex++;
                }
            }
        }

        private void _resetBuffer()
        {
            _cmdbuffer[0] = 0;
            _cmdbuffer[1] = 0;
            _cmdbuffer[2] = 0;
            _cmdbuffer[3] = 0;
            _cmdbufferindex = 0;
        }

        private void _parseBuffer()
        {
            SMidiAction action = new SMidiAction();
            action.Cmd = _cmdbuffer[0] << 4;
            action.Channel = _cmdbuffer[1] & 0x0f;
            action.Data1 = _cmdbuffer[2];
            action.Data2 = _cmdbuffer[3];
            //handshake
            if (action.Cmd == _hsCmd )
            {
                if (action.Data1 == _hsData2 && action.Data2 == _hsData1 && action.Channel == _hsChannel) { 
                    ready = true;
                    if (OnStatusChanged != null) { OnStatusChanged(this, EMidiEvent.DeviceReady);  }
                    return;
                }
            }
            
            if (OnData != null)
            {
                OnData(this, action);
            }
        }
    }
}
