using System.Collections.Generic;
using System.Runtime.InteropServices;
using OBSMidiRemote.Lib.PureMidi.Exceptions;

namespace OBSMidiRemote.Lib.PureMidi.DeviceInfo
{
    public sealed class MidiInInfo : MidiDeviceInfo
    {
        private MidiInInfo(ushort deviceIndex, string productName, ushort manufacturerId, ushort productId,
                           ushort driverVerssion, uint support)
        {
            DeviceIndex = deviceIndex;
            ProductName = productName;
            ManufacturerId = manufacturerId;
            ProductId = productId;
            DriverVerssion = driverVerssion;
            Support = support;
        }


        public static IEnumerable<MidiInInfo> Informations
        {
            get
            {
                var retVal = new List<MidiInInfo>();
                for (ushort i = 0; i < WindowsMultimediaDevice.midiInGetNumDevs(); i++)
                {
                    var caps = new MidiInCaps();
                    int error = WindowsMultimediaDevice.midiInGetDevCaps(i, ref caps, Marshal.SizeOf(caps));
                    if (error != (int)EDeviceException.MmsyserrNoerror) throw new MidiDeviceException(error);
                    retVal.Add(
                        new MidiInInfo(i, caps.name, (ushort) caps.mid, (ushort) caps.pid, (ushort) caps.driverVersion,
                                       (uint) caps.support)
                        );
                }
                return retVal;
            }
        }
    }
}