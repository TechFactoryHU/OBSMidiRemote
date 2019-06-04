using System.Text;

namespace OBSMidiRemote.Lib.PureMidi.Exceptions
{
    internal sealed class InputDeviceException : MidiDeviceException
    {
       
        public InputDeviceException(int errCode) : base(errCode)
        {
            WindowsMultimediaDevice.midiInGetErrorText(errCode, ErrMsg, ErrMsg.Capacity);
        }       
        
    }
}

