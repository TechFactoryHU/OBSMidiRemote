using System;

namespace OBSMidiRemote.Lib.PureMidi
{
    public class ErrorEventArgs : EventArgs
    {
        private readonly Exception _ex;

        public ErrorEventArgs(Exception ex)
        {
            _ex = ex;
        }

        public Exception Error
        {
            get
            {
                return _ex;
            }
        }
    }
}

