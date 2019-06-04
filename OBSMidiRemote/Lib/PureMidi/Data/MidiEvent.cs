using System.Text;
using OBSMidiRemote.Lib.PureMidi.Definitions;

namespace OBSMidiRemote.Lib.PureMidi.Data
{
    public class MidiEvent
    {
        public MidiEvent(byte[] data)
        {
            AllData = data;
        }

        public readonly byte[] AllData;

        public string Hex
        {
            get
            {
                var sb = new StringBuilder();
                for (int i = 0; i < AllData.Length; i++)
                {
                    sb.Append(AllData[i].ToString("X2").ToUpper());
                }
                return sb.ToString();
            }
        }

        public byte Status { get { return AllData[0]; } }

        public EMidiEventType MidiEventType
        {
            get
            {
                switch (AllData[0])
                {
                    case 0xFF:
                        return EMidiEventType.Meta;
                    case 0xF0:
                        return EMidiEventType.Sysex;
                    case 0xF7:
                        return EMidiEventType.Sysex;
                    case 0:
                        return EMidiEventType.Empty;
                    default:
                        return EMidiEventType.Short;
                }
            }
        }

    }
}