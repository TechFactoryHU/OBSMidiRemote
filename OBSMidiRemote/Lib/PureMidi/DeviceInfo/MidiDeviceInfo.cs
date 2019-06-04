using System;

namespace OBSMidiRemote.Lib.PureMidi.DeviceInfo
{
    public abstract class MidiDeviceInfo
    {
        public ushort DeviceIndex { get; protected set; }

        public string ProductName { get; protected set; }

        public ushort ManufacturerId { get; protected set; }

        public ushort ProductId { get; protected set; }

        public ushort DriverVerssion { get; protected set; }

        public uint Support { get; protected set; }

        public override string ToString()
        {
            return ProductName;
        }

    }
}