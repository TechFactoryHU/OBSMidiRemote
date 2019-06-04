using System.Text;

namespace OBSMidiRemote.Lib.PureMidi.Exceptions
{
    internal sealed class OutputDeviceException : MidiDeviceException
    {

        public OutputDeviceException(int errCode) : base(errCode)
        {
            WindowsMultimediaDevice.midiOutGetErrorText(errCode, ErrMsg, ErrMsg.Capacity);
        }

    }
}