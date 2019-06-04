using System;
using System.Runtime.InteropServices;

namespace OBSMidiRemote.Lib.PureMidi.Data
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct MidiHeader
    {
        public IntPtr data;
        public int bufferLength;
        public int bytesRecorded;
        public int user;
        public int flags;
        public IntPtr next;
        public int reserved;
        public int offset;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst=4)]
        public int[] reservedArray;
    }
}

