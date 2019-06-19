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
using Newtonsoft.Json.Linq;

namespace OBSMidiRemote.Lib.SLOBS
{
    public delegate void SLOBSConnectionStatus (SLOBSWebsocket sender, SSLOBSConnectionEvent eventdata);
    public delegate void SLOBSSceneListChanged (SLOBSWebsocket sender, List<SLOBSScene> scenes);
    public delegate void SLOBSSceneChanged (SLOBSWebsocket sender, SLOBSSceneEvent eventdata);
    public delegate void SLOBSSceneItemChanged (SLOBSWebsocket sender, SLOBSSceneItemEvent eventdata);
    public delegate void SLOBSStreamingStatusChanged (SLOBSWebsocket sender, ESLOBSStreamingState state);
    public delegate void SLOBSSourceChanged (SLOBSWebsocket sender, SLOBSSourceEvent eventdata);

    public struct SLOBSEvent
    {
        public ESLOBSEventType type;
        public string resourceId;
        public JObject data;
    }

    public struct SLOBSSceneEvent
    {
        public ESLOBSEventType Type;
        public string ResourceId;
        public SLOBSScene Scene;
    }

    public struct SLOBSSourceEvent
    {
        public ESLOBSEventType Type;
        public string ResourceId;
        public SLOBSSource Source;
    }

    public struct SLOBSSceneItemEvent
    {
        public ESLOBSEventType Type;
        public string ResourceId;
        public SLOBSSceneItem SceneItem;
    }

    public struct SSLOBSConnectionEvent
    {
        public ESLOBSConnectionState state;
        public string message;
    }

    public struct SLOBSStreamingState
    {
        public ESLOBSStreamingState StreamingStatus;
        public ESLOBSSRecordingState RecordingStatus;
        public ESLOBSSReplayBufferState ReplayBufferStatus;
    }

    public enum ESLOBSStreamingState
    {
        Ending,
        Live,
        Offline,
        Reconnecting,
        Starting
    }

    public enum ESLOBSSRecordingState
    {
        Offline,
        Recording,
        Starting,
        Stopping
    }

    public enum ESLOBSSReplayBufferState
    {
        Offline,
        Running,
        Saving,
        Stopping
    }

    public enum ESLOBSEventType
    {
        SceneAdded,
        SceneRemoved,
        SceneSwitched,
        SceneItemAdded,
        SceneItemRemoved,
        SceneItemUpdated,
        SceneCollectionChanged,
        StreamingStatusChange,
        SourceAdded,
        SourceRemoved,
        SourceUpdated
    }

    public enum ESLOBSConnectionState
    {
        Unknown,
        Disconnected,
        Connected,
        Connecting,
        Error
    }

    public struct SLOBSAsyncCallback
    {
        public readonly DateTime timestamp;
        public Action<JObject> callable;

        public SLOBSAsyncCallback(Action<JObject> callback)
        {
            callable = callback;
            timestamp = DateTime.Now;
        }
    }

    public struct SLOBSCrop
    {
        public float Top;
        public float Bottom;
        public float Left;
        public float Right;

        public SLOBSCrop(JObject data)
        {
            Top = (float)data["top"];
            Bottom = (float)data["bottom"];
            Left = (float)data["left"];
            Right = (float)data["right"];
        }
    }

    public struct SLOBSFader
    {
        public int Db;
        public int Deflection;
        public int Mul;

        public SLOBSFader(JObject data)
        {
            Db = (int)data["db"];
            Deflection = (int)data["deflection"];
            Mul = (int)data["mul"];
        }
    }

    public struct SLOBSSource
    {
        public string Id;
        public string SourceId;
        public string ResourceId;
        public string Name;
        public string Type;
        public string PropertiesManagerType;
        public bool Audio;
        public bool Video;
        public bool Async;
        public bool DoNotDuplicate;
        public int Height;
        public int Width;
        public int Channel;
        public bool Muted;
       
        public SLOBSSource(JObject data)
        {
            Id = (string)data["id"];
            SourceId = (string)data["sourceId"];
            ResourceId = (string)data["resourceId"];
            Name = (string)data["name"];
            Type = (string)data["type"];
            PropertiesManagerType = (string)data["propertiesManagerType"];
            Audio = (bool)data["audio"];
            Video = (bool)data["video"];
            Async = (bool)data["async"];
            DoNotDuplicate = (bool)data["doNotDuplicate"];
            Muted = (bool)data["muted"];
            Height = (int)data["height"];
            Width = (int)data["width"];
            Channel = 0;
            if (data["channel"]!=null)
            {
                Channel = (int)data["channel"];
            }
        }
    }

    public struct SLOBSAudioSource
    {
        public string ResourceId;
        public string Name;
        public string SourceId;
        public int AudioMixers;
        public int MonitoringType;
        public bool ForceMono;
        public int SyncOffset;
        public bool Muted;
        public bool MixerHidden;
        public SLOBSFader Fader;

        public SLOBSAudioSource(JObject data)
        {
            ResourceId = (string)data["resourceId"];
            Name = (string)data["name"];
            SourceId = (string)data["sourceId"];
            AudioMixers = (int)data["audioMixers"];
            MonitoringType = (int)data["monitoringType"];
            ForceMono = (bool)data["forceMono"];
            SyncOffset = (int)data["syncOffset"];
            Muted = (bool)data["muted"];
            MixerHidden = (bool)data["mixerHidden"];

            Fader = new SLOBSFader((JObject)data["fader"]);
        }
    }

    public struct SLOBSSceneItem
    {
        public string Id;
        public string SceneId;
        public string SceneNodeType;
        public string ParentId;
        public string SourceId;
        public string SceneItemId;
        public string ResourceId;
        public string Name;
        public List<string> ChildrenIds;

        public float Xpos;
        public float Ypos;

        public float Yscale;
        public float Xscale;

        public SLOBSCrop Crop;

        public float Rotation;

        public bool Visible;
        public bool Locked;

        public SLOBSSceneItem(JObject data)
        {
            Id = (string)data["id"];
            Name = (string)data["name"];
            SceneId = (string)data["sceneId"];
            SceneNodeType = (string)data["sceneNodeType"];
            ParentId = null;
            ChildrenIds = null;
            ResourceId = null;

            Xpos = 0;
            Ypos = 0;
            Xscale = 0;
            Yscale = 0;
            Rotation = 0;
            Crop = new SLOBSCrop();
            SceneItemId = null;
            SourceId = null;
            Visible = false;
            Locked = false;

            if (data["parentId"] != null) { ParentId = (string)data["parentId"]; }

            if (SceneNodeType == "item")
            {
                SceneItemId = (string)data["sceneItemId"];
                SourceId = (string)data["sourceId"];
                 
                if (data["transform"] != null)
                {
                    if (data["transform"]["position"] != null)
                    {
                        Xpos = (float)data["transform"]["position"]["x"];
                        Ypos = (float)data["transform"]["position"]["y"];
                    }
                    if (data["transform"]["scale"] != null)
                    {
                        Xscale = (float)data["transform"]["scale"]["x"];
                        Yscale = (float)data["transform"]["scale"]["y"];
                    }

                    Rotation = (float)data["transform"]["rotation"];

                    if (data["transform"]["crop"] != null)
                    {
                        Crop = new SLOBSCrop((JObject)data["transform"]["crop"]);
                    }
                }

                Visible = (bool)data["visible"];
                Locked = (bool)data["locked"];
            }
            else if(SceneNodeType == "folder")
            {
                ChildrenIds = new List<string>();
                if (data["childrenIds"] != null)
                {
                    JArray childs = (JArray)data["childrenIds"];
                    foreach (string item in childs)
                    {
                        ChildrenIds.Add(item);
                    }
                }
            }
        }
    }

    public struct SLOBSScene
    {
        public string ResourceId;
        public string Id;
        public string Name;
        public List<SLOBSSceneItem> Nodes;

        public SLOBSScene(JObject data)
        {
           
            
            ResourceId = (string)data["resourceId"];
            Id = (string)data["id"];
            Name = (string)data["name"];
            Nodes = new List<SLOBSSceneItem>();

            if (data["nodes"] != null)
            {
                JArray sceneItems = (JArray)data["nodes"];
                foreach (JObject item in sceneItems)
                {
                    Nodes.Add(new SLOBSSceneItem(item));
                }
            }
        }
    }
}
