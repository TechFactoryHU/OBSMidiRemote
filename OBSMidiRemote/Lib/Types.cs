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

    public delegate void OBSConnectionStatus (IOBSConnector sender, EOBSCStatus eventdata);
    public delegate void OBSSceneListChanged(IOBSConnector sender, EOBSEvent eventtype, List<SOBSScene> scenes);
    public delegate void OBSSceneChanged (IOBSConnector sender, EOBSEvent eventtype, SOBSScene scene);

    public delegate void OBSSceneItemChanged (IOBSConnector sender, EOBSEvent eventtype, SOBSSceneItem eventdata);
    public delegate void OBSStreamingStatusChanged (IOBSConnector sender, SOBSStreamingState state);
    public delegate void OBSSourceChanged (IOBSConnector sender, EOBSEvent eventtype, SOBSSource eventdata);
    public delegate void OBSAudioSourceChanged(IOBSConnector sender, EOBSEvent eventtype);

    public delegate void OBSTransitionChanged(IOBSConnector sender, EOBSEvent eventtype);
    public delegate void OBSStudioModeChanged(IOBSConnector sender, EOBSMode mode);

    public enum EOBSEvent
    {
        SceneUpdated,
        SceneSwitched,
        SceneAdded,
        SceneRemoved,
        SceneItemAdded,
        SceneItemRemoved,
        SceneItemUpdated,
        SourceAdded,
        SourceUpdated,
        SourceRemoved,
        TransitionChanged,
        TransitionListChanged,
        AudioVolumeChanged,
        StreamingStatusChanged,
        PreviewSceneSwitched
    }

    public enum EOBSMode
    {
        Unknown = 0,
        Normal = 1 ,
        Studio = 2
    }

    public enum EOBSStreamingState
    {
        Unkown,
        Started,
        Stopped,
        Stopping,
        Starting,
        Reconnecting,
        Saving
    }


    public struct SOBSStreamingState
    {
        public EOBSStreamingState Stream;
        public EOBSStreamingState Record;
        public EOBSStreamingState Replay;
    }

    public struct SOBSScene
    {
        public int Index;
        public string Id;
        public string ResourceId;
        public string Name;
        public List<SOBSSceneItem> Items;
    }

    public struct SOBSSceneItem
    {
        public int Index;
        public string SourceId;
        public string ResourceId;
        public string Id;
        public string Name;
        public bool Visible;
        public bool IsFolder;
        public string ParentId;
    }

    public struct SOBSTransition {
        public int Index;
        public string Id;
        public string Name;
    }

    public struct SOBSSource
    {
        public int Index;
        public string Id;
        public string ResourceId;

        public string Name;
        public string Type;

        public bool Audio;
        public bool Video;

        public int Volume;
        public bool Muted;
    }

    public struct SOBSAudioSource
    {
        public int Index;
        public string Id;
        public string ResourceId;
        public string SourceId;
        public string Name;
        public int Volume;
        public bool Hidden;
        public bool Muted;
    }

}
