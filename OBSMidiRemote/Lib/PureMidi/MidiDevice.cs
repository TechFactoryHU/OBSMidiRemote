using System;
using System.Runtime.InteropServices;
using System.Threading;
using OBSMidiRemote.Lib.PureMidi.Data;

namespace OBSMidiRemote.Lib.PureMidi
{
    public abstract class MidiDevice : IDisposable
    {        
        public int DeviceId
        {
            get { return _deviceId; }
        }

        private readonly int _deviceId;

        protected int Handle;

        public event EventHandler<ErrorEventArgs> Error;

        public abstract void Close();

        protected void OnError(ErrorEventArgs e)
        {
            if (Error != null)
            {
                Error(this, e);
            }
        }

        public abstract void Reset();

        protected static readonly int SizeOfMidiHeader = Marshal.SizeOf(typeof (MidiHeader));

        protected MidiDevice(int deviceId)
        {
            _deviceId = deviceId;
        }

        public bool IsDisposed { get; protected set; }

        public virtual void Dispose()
        {
            if (!IsDisposed)
            {
                Dispose(true);
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposing) return;
            IsDisposed = true;
            GC.SuppressFinalize(this);
        }

    }
}