using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OBSMidiRemote.Lib
{
    public enum InputDeviceType
    {
        UNKNOWN,
        SERIAL,
        MIDI
    }

    public enum EMidiEvent
    {
        Unknown,
        Error,
        WrongPassword,
        Connecting,
        Connected,
        Disconnected,
        DeviceError,
        DeviceReady,
        DeviceDisconnected
    }

    public enum EMidiOBSItemType
    {
        None,
        Modifier,
        Scene,
        SceneItem,
        PScene,
        PSceneItem,
        AudioItem,
        AudioVolume,
        Transition,
        ConnectionStatus,
        ReloadOBSData,
        Stream,
        Record,
        Mode
    }

    public enum EMidiOBSOutputType
    {
        Unknown,
        Any,
        Off,
        On,
        Starting,
        Stopping,
        Active,
        Value,
        Muted
    }

    public enum EMidiOBSInputType
    {
        Unknown,
        Any,
        Off,
        On,
        Toggle,
        Value
    }

    public enum EMidiActionType
    {
        Invalid = 0x00,
        NoteOff = 0x80,
        NoteOn = 0x90,
        ControlChange = 0xB0
    }

}
