using System.Text;

namespace OBSMidiRemote.Lib.PureMidi.Exceptions
{
    internal class MidiDeviceException : DeviceException
    {
        protected readonly StringBuilder ErrMsg;

        public MidiDeviceException(int errCode) : base(errCode)
        {
            ErrMsg = new StringBuilder(0x80);
        }

        public override string Message
        {
            get
            {
                return ErrMsg.ToString();
            }
        }
    }
}