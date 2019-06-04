using System.Runtime.InteropServices;

namespace OBSMidiRemote.Lib.PureMidi.DeviceInfo
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct MidiOutCaps
    {
        public short mid;
        public short pid;
        public int driverVersion;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 0x20)]
        public string name;
        public short technology;
        public short voices;
        public short notes;
        public short channelMask;
        public int support;
    }
}