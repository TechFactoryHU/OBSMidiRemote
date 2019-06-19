#region license
/*
    The MIT License (MIT)
    Copyright (c) 2019 Techfactory.hu
*/
#endregion

using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OBSMidiRemote.Lib
{
    //public delegate void StatusChanged(CDeviceObsGw sender, EMidiEvent eventType);
    public delegate void OBaseStatusChanged(COBSMidiRemote sender, EOBSCStatus eventType);

    public delegate void IOBSDeviceStatus(IOBSDevice sender, EMidiEvent eventType);
    public delegate void IOBSDeviceData(IOBSDevice sender, SMidiAction data);
    public delegate void OnMidiOutput(CMidiObserver sender, SMidiAction data);
    public delegate void IOBSCStatus(IOBSConnector sender, EOBSCStatus eventType);


    public struct SMidiXMLInfoDescription
    {
        [XmlAttribute(AttributeName = "lang")] public string Lang;
        [XmlText] public string Text;
    }

    [XmlRoot("MapInfo")]
    public struct SMidiXMLInfo
    {
        [XmlIgnore] public string Filename;
        public string Name;
        public string Author;
        public string DeviceName;
        [XmlElement("Description")] public List<SMidiXMLInfoDescription> Description;
        public string Website;
        [XmlElement("Version")] public string VersionString;
        [XmlIgnore] public Version Version;
        [XmlIgnore] public DateTime FileMTime;
    }

    public struct SMidiXMLInputItem
    {
        [XmlAttribute] public string type;
        [XmlAttribute] public string cmd;
        [XmlAttribute] public string channel;
        [XmlAttribute] public string data1;
        [XmlAttribute] public string data2;
    }

    public struct SMidiXMLOutputItem
    {
        [XmlAttribute] public string type;
        [XmlAttribute] public string cmd;
        [XmlAttribute] public string channel;
        [XmlAttribute] public string data1;
        [XmlAttribute] public string data2;
    }

    [XmlRoot("Modifier")]
    public struct SMidiXMLModifier
    {
        [XmlAttribute] public int obsmode;
        [XmlAttribute] public int index;
        [XmlElement("Input")] public SMidiXMLInputItem[] Inputs;
        [XmlElement("Output")] public SMidiXMLOutputItem[] Outputs;
    }

    [XmlRoot("Item")]
    public struct SMidiXMLItem
    {
        [XmlAttribute] public string type;
        [XmlAttribute] public string bind;
        [XmlAttribute] public int obsmode;
        [XmlAttribute] public int index;
        [XmlAttribute] public int modifier;
        //[XmlAttribute(AttributeName = "from-index")]
        [XmlIgnore]
        public int fromIndex;
        //[XmlAttribute(AttributeName = "to-index")]
        [XmlIgnore]
        public int toIndex;
        //[XmlAttribute(AttributeName = "from-address")]
        [XmlIgnore]
        public int fromAddress;
        //[XmlAttribute(AttributeName = "to-address")]
        [XmlIgnore]
        public int toAddress;
        [XmlAttribute] public int step;
        [XmlElement("Input")] public SMidiXMLInputItem[] Inputs;
        [XmlElement("Output")] public SMidiXMLOutputItem[] Outputs;
    }

    [XmlRoot("Map")]
    public struct SMidiXMLData
    {
        [XmlAttribute(AttributeName = "packet-interval")] public int PacketInterval;
        [XmlAttribute(AttributeName = "max-packet")] public int PacketCount;
        [XmlAttribute(AttributeName = "baudrate")] public int BaudRate;
        [XmlArray("Modifiers"),XmlArrayItem("Modifier")] public SMidiXMLModifier[] Modifiers;
        [XmlArray("Items"), XmlArrayItem("Item")] public SMidiXMLItem[] Items;
    }

    public struct SMidiOutputStatus
    {
        int address; // data1
        EMidiOBSItemType item;
        EMidiOBSOutputType outputType;
    }

    public struct SMidiAction
    {
        public int Cmd;
        public int Channel;
        public int Data1;
        public int Data2;
    }


    public struct SMidiOutput
    {
        public EMidiOBSOutputType Type;
        public SMidiAction Action;
    }

    public struct SMidiInput
    {
        public EMidiOBSInputType Type;
        public SMidiAction Action;
    }

    public struct SMidiOBSAction
    {
        public EMidiOBSItemType Type;
        public int ObsMode;
        public bool IsModifier;
        public int Modifier;
        public int Index;
        public List<SMidiInput> InActions;
        public List<SMidiOutput> OutActions;
    }

    [XmlRoot("OBSDeviceMap")] // Namespace = "http://schemas.techfactory.hu/OBSMidiRemote/1.0/schema"
    public struct SXMLSchema
    {
        public SMidiXMLInfo MapInfo;
        public SMidiXMLData Map;
    }

}
