#region license
/*
    The MIT License (MIT)
    Copyright (c) 2019 Techfactory.hu
*/
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OBSMidiRemote.Lib
{
    public enum EOBSConnectorType
    {
        Unknown,
        OBS,        //OBS
        SLOBS       //StreamLabs OBS
    }

    public enum InputDeviceType
    {
        UNKNOWN,
        SERIAL,
        MIDI
    }
    
    public enum EOBSCStatus
    {
        Unknown,
        Ready,
        Connecting,
        Connected,
        WrongAuth,
        Error,
        Disconnected,
        DeviceError,
        DeviceReady,
        DeviceDisconnected
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
        Pscene,
        PsceneItem,
        AudioItem,
        AudioVolume,
        Transition,
        ConnectionStatus,
        ReloadObsData,
        Stream,
        Record,
        Mode,
        ReplayBuffer,
        ReplayBufferSave,
        ReloadScheme
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

    public enum EMidiObsMode
    {
        Any = 0,
        Normal = 1,
        Studio = 2
    }

    public enum EMidiSchemaItemType
    {
        Item = 0,
        Modifier = 1
    }

    public static class CMidiFields
    {
        public readonly static EMidiOBSItemType[] Ranges = new EMidiOBSItemType[] {
            EMidiOBSItemType.Scene, EMidiOBSItemType.SceneItem, EMidiOBSItemType.Pscene, EMidiOBSItemType.PsceneItem,
            EMidiOBSItemType.AudioItem,  EMidiOBSItemType.AudioVolume,  EMidiOBSItemType.Transition

        };

        public readonly static EMidiOBSItemType[] Singles = new EMidiOBSItemType[] {
            EMidiOBSItemType.Record, EMidiOBSItemType.Stream, EMidiOBSItemType.Mode, EMidiOBSItemType.ConnectionStatus,
            EMidiOBSItemType.ReloadObsData
        };

        public readonly static int[] BaudRates = new int[] { 9600, 19200, 38400, 57600, 115200, 256000 };
        public readonly static int DefaultBaudRate = 115200;
    }


   

}
