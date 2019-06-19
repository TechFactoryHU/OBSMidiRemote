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
using System.Xml;
using System.Xml.Serialization;
using System.Xml.Linq;
using System.Xml.XPath;
using System.IO;

namespace OBSMidiRemote.Lib
{
    public class CMidiXMLSchema
    {
        private string midimapFile;
        public bool Loaded { get; private set; }
        public int ParsedRules { get; private set; }
        public List<SMidiOBSAction> MidiActions { get; private set; }
        public SMidiXMLData MidiMapData { get; private set; }
        public SMidiXMLInfo MidiMapInfo { get; private set; }

        private readonly XmlWriterSettings _xmlSettings = new XmlWriterSettings {
            Indent = true,
            Encoding = Encoding.UTF8,
            ConformanceLevel = ConformanceLevel.Document
        };

        public CMidiXMLSchema() {
            MidiActions = new List<SMidiOBSAction>();
            MidiMapData = new SMidiXMLData();
            MidiMapInfo = new SMidiXMLInfo();
            Loaded = false;
            ParsedRules = 0;
        }

        public SMidiXMLInfo? LoadHeaders(string filename, string preferred_lang = null)
        {
            XDocument schemaXML;
            var filepath = System.IO.Path.Combine(Program.GetMapsDir(), filename);
            if (File.Exists(filepath))
            {
                schemaXML = XDocument.Load(filepath);
                SMidiXMLInfo info = new SMidiXMLInfo();
                if (schemaXML != null)
                {
                   if (_parseHeaders(schemaXML, ref info, preferred_lang))
                   {
                        info.Filename = filename;
                        info.FileMTime = File.GetLastWriteTime(filepath);

                        return info;
                   }
                }
            }
            return null;
        }

        public bool LoadSchema(string filename, string preferred_lang = null)
        {
            MidiActions = new List<SMidiOBSAction>();
            var mapInfo = new SMidiXMLInfo();
            var mapData = new SMidiXMLData();

            midimapFile = filename;
            XDocument schemaXML;

            Loaded = false;
            ParsedRules = 0;

            try
            {
                schemaXML = XDocument.Load(System.IO.Path.Combine(Program.GetMapsDir(), filename));
              
                mapInfo.Filename = filename;
                if (!_parseHeaders(schemaXML, ref mapInfo, preferred_lang))
                {
                    return false;
                }
                MidiMapInfo = mapInfo;

                var maproot = schemaXML.XPathSelectElement("OBSDeviceMap/Map");
                if (maproot != null)
                {
                    if (maproot.Attribute("max-packet") != null)
                    {
                        mapData.PacketCount = (int)maproot.Attribute("max-packet");
                    }

                    if (maproot.Attribute("packet-interval") != null)
                    {
                        var iv = (int)maproot.Attribute("packet-interval");
                        if (iv > 0)
                        {
                            mapData.PacketInterval = iv;
                        }
                        else { mapData.PacketInterval = 0; }
                    }

                    if (maproot.Attribute("baudrate") != null)
                    {
                        mapData.BaudRate = (int)maproot.Attribute("baudrate");
                    }

                    MidiMapData = mapData;

                    //check modifiers
                    var mapmods = maproot.XPathSelectElement("Modifiers");
                    if (mapmods != null)
                    {
                        int _mi = 1;
                        foreach (XElement item in mapmods.Descendants("Modifier"))
                        {
                            SMidiOBSAction action = new SMidiOBSAction();
                            action.Modifier = _mi;
                            if (item.Attribute("index") != null)
                            {
                                action.Modifier = (int)item.Attribute("index");
                                if (action.Modifier < 1 || action.Modifier > 20)
                                {
                                    action.Modifier = _mi;
                                }
                            }
                            action.Index = _mi - 1;
                            action.Type = EMidiOBSItemType.Modifier;
                            if (item.Attribute("obsmode") != null)
                            {
                                action.ObsMode = (int)item.Attribute("obsmode");
                            }
                            action.InActions = new List<SMidiInput>();
                            action.OutActions = new List<SMidiOutput>();
                            _parseXMLInAndOutput(ref action, item, 0);
                            MidiActions.Add(action);
                            _mi++;
                        }
                    }
                    
                    var items = maproot.XPathSelectElement("Items");
                    if (items != null)
                    {
                        foreach (XElement item in items.Descendants("Item"))
                        {
                            _parseItemfromXElement(item);
                        }

                        ParsedRules = MidiActions.Count();
                        Loaded = true;
                        return true;
                    }
                }
            }
            catch (System.Xml.XmlException e)
            {
                Console.WriteLine(e.Message);
            }
            
            return false;
        }

        public void SetMidiActions(List<SMidiOBSAction> actions)
        {
            MidiActions = actions;
        }

        public void SetMidiInfo(SMidiXMLInfo info)
        {
            MidiMapInfo = info;
        }

        public void SetMidiData(SMidiXMLData data)
        {
            MidiMapData = data;
        }

        #region Deserialization
        private void _parseItemfromXElement(XElement item)
        {
            string type = (string)item.Attribute("type");
            string bind = (string)item.Attribute("bind");
            if (type != null && bind != null)
            {
                SMidiOBSAction action = new SMidiOBSAction();
                action.InActions = new List<SMidiInput>();
                action.OutActions = new List<SMidiOutput>();
                if (item.Attribute("modifier") != null) { action.Modifier = (int)item.Attribute("modifier"); }
                else { action.Modifier = 0;  }

                if (item.Attribute("obsmode") != null)
                {
                    action.ObsMode = (int)item.Attribute("obsmode");
                }

                action.Type = _midiActionTypeFromString(bind);
                /*switch (bind)
                {
                    case "scene":
                        action.Type = EMidiOBSItemType.Scene;
                        break;
                    case "scene-item":
                        action.Type = EMidiOBSItemType.SceneItem;
                        break;
                    case "audio-item":
                        action.Type = EMidiOBSItemType.AudioItem;
                        break;
                    case "audio-volume":
                        action.Type = EMidiOBSItemType.AudioVolume;
                        break;
                    case "status":
                        action.Type = EMidiOBSItemType.ConnectionStatus;
                        break;
                    case "transition":
                        action.Type = EMidiOBSItemType.Transition;
                        break;
                    case "reload-obs-data":
                        action.Type = EMidiOBSItemType.ReloadObsData;
                        break;
                    case "stream":
                        action.Type = EMidiOBSItemType.Stream;
                        break;
                    case "record":
                        action.Type = EMidiOBSItemType.Record;
                        break;
                    case "mode":
                        action.Type = EMidiOBSItemType.Mode;
                        break;
                    case "pscene":
                        action.Type = EMidiOBSItemType.Pscene;
                        break;
                    case "pscene-item":
                        action.Type = EMidiOBSItemType.PsceneItem;
                        break;
                    default:
                        action.Type = EMidiOBSItemType.None;
                        break;
                }*/

                if (type == "single")
                {
                    action.Index = (int)item.Attribute("index");
                    _parseXMLInAndOutput(ref action, item, 0);
                    MidiActions.Add(action);
                }
                else if (type == "range")
                {
                    int from = (int)item.Attribute("from-index");
                    int to = (int)item.Attribute("to-index");

                    int from_addr = (int)item.Attribute("from-address");
                    int to_addr = (int)item.Attribute("to-address");
                    int step = (int)item.Attribute("step");

                    if (from_addr >= 0 && to_addr >= 0 && step > 0)
                    {
                        if (from_addr > to_addr)
                        {
                            for (int i = from_addr; i >= to_addr; i -= step)
                            {
                                action.Index = from;
                                action.InActions = new List<SMidiInput>();
                                action.OutActions = new List<SMidiOutput>();
                                _parseXMLInAndOutput(ref action, item, i);
                                MidiActions.Add(action);
                                from++;
                            }
                        }
                        else
                        {
                            for (int i = from_addr; i <= to_addr; i += step)
                            {
                                action.Index = from;
                                action.InActions = new List<SMidiInput>();
                                action.OutActions = new List<SMidiOutput>();
                                _parseXMLInAndOutput(ref action, item, i);
                                MidiActions.Add(action);
                                from++;
                            }
                        }
                    }
                }
            }
        }

        private bool _parseHeaders(XDocument doc, ref SMidiXMLInfo mapInfo, string preferred_lang = null)
        {
            var info = doc.XPathSelectElement("OBSDeviceMap/MapInfo");
            if (info != null)
            {
                foreach (XElement item in info.Nodes())
                {
                    switch (item.Name.LocalName)
                    {
                        case "Name":
                            mapInfo.Name = item.FirstNode != null ? item.FirstNode.ToString() : null;
                            break;
                        case "DeviceName":
                            mapInfo.DeviceName = item.FirstNode != null ? item.FirstNode.ToString() : null;
                            break;
                        case "Author":
                            mapInfo.Author = item.FirstNode != null ? item.FirstNode.ToString() : null;
                            break;
                        case "Version":
                            if (item.FirstNode != null) { mapInfo.Version = Version.Parse(item.FirstNode.ToString()); }
                            break;
                        case "Website":
                            mapInfo.Website = item.FirstNode != null ? item.FirstNode.ToString() : null;
                            break;
                    }
                }

                if (mapInfo.Name == null || String.IsNullOrEmpty(mapInfo.Name)) { return false; }

                if (info.Descendants("Description").Count() > 0)
                {
                    var desc = new List<SMidiXMLInfoDescription>();
                    foreach (XElement item in info.Descendants("Description"))
                    {
                        var descitem = new SMidiXMLInfoDescription();
                        descitem.Lang = (string)item.Attribute("lang");
                        if (item.FirstNode.NodeType == XmlNodeType.CDATA) { 
                           XCData xcdata = (XCData)item.FirstNode;
                           descitem.Text = xcdata.Value;
                        }else if(item.FirstNode.NodeType == XmlNodeType.Text)
                        {
                            descitem.Text = item.Value;
                        }
                        desc.Add(descitem);
                    }
                    mapInfo.Description = desc;
                }
                if (preferred_lang == null) { preferred_lang = Program.lang;  }

                return true;
            }
            return false;
        }

        private void _parseXMLInAndOutput(ref SMidiOBSAction action, XElement item, int rp_addr)
        {
            foreach (XElement input in item.Descendants("Input"))
            {
                SMidiInput _in = new SMidiInput();
                string itype = (string)input.Attribute("type");
                switch (itype)
                {
                    case "on":
                        _in.Type = EMidiOBSInputType.On;
                        break;
                    case "off":
                        _in.Type = EMidiOBSInputType.Off;
                        break;
                    case "toggle":
                        _in.Type = EMidiOBSInputType.Toggle;
                        break;
                    case "value":
                        _in.Type = EMidiOBSInputType.Value;
                        break;
                    default:
                        _in.Type = EMidiOBSInputType.Unknown;
                        break;
                }
                _in.Action = new SMidiAction();
                if (input.Attribute("channel") != null) {
                    _in.Action.Channel = _parseXMLDataFields((string)input.Attribute("channel"), rp_addr);
                }
                else { _in.Action.Channel = 0; }
                _in.Action.Cmd = _parseXMLDataFields((string)input.Attribute("cmd"), rp_addr);
                _in.Action.Data1 = _parseXMLDataFields((string)input.Attribute("data1"), rp_addr);
                _in.Action.Data2 = _parseXMLDataFields((string)input.Attribute("data2"), rp_addr);
                action.InActions.Add(_in);
            }

            foreach (XElement output in item.Descendants("Output"))
            {
                SMidiOutput _out = new SMidiOutput();
                string itype = (string)output.Attribute("type");
                switch (itype)
                {
                    case "on":
                        _out.Type = EMidiOBSOutputType.On;
                        break;
                    case "off":
                        _out.Type = EMidiOBSOutputType.Off;
                        break;
                    case "active":
                        _out.Type = EMidiOBSOutputType.Active;
                        break;
                    case "starting":
                        _out.Type = EMidiOBSOutputType.Starting;
                        break;
                    case "stopping":
                        _out.Type = EMidiOBSOutputType.Stopping;
                        break;
                    case "value":
                        _out.Type = EMidiOBSOutputType.Value;
                        break;
                    case "muted":
                        _out.Type = EMidiOBSOutputType.Muted;
                        break;
                    default:
                        _out.Type = EMidiOBSOutputType.Unknown;
                        break;
                }

                _out.Action = new SMidiAction();
                if (output.Attribute("channel") != null)
                {
                    _out.Action.Channel = _parseXMLDataFields((string)output.Attribute("channel"), rp_addr);
                }
                else
                {
                    _out.Action.Channel = 0;
                }
                _out.Action.Cmd = _parseXMLDataFields((string)output.Attribute("cmd"), rp_addr);
                _out.Action.Data1 = _parseXMLDataFields((string)output.Attribute("data1"), rp_addr);
                _out.Action.Data2 = _parseXMLDataFields((string)output.Attribute("data2"), rp_addr);
                action.OutActions.Add(_out);
            }
        }

        private int _parseXMLDataFields(string data, int address)
        {
            int r = 0;
            data = data.Replace("%ADDRESS", address.ToString("X2")).Trim();

            try
            {
                r = Convert.ToInt32(data, 16);
            }
            catch (FormatException e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine(data);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine(data);
            }
            return r;
        }
        #endregion

        #region Serialization

        public bool WriteSchema(string filename)
        {
            SXMLSchema schema = new SXMLSchema();
            schema.MapInfo = MidiMapInfo;
            schema.Map = MidiMapData;

            int mc = 0;
            int ic = 0;
            int x = 0;
            for (int i=0; i< MidiActions.Count(); i++)
            {
                if (MidiActions[i].Type == EMidiOBSItemType.Modifier)
                {
                    mc++;
                }else { ic++; }
            }

            if (mc > 0) {
                x = 0;
                schema.Map.Modifiers = new SMidiXMLModifier[mc];
                for (int i = 0; i < MidiActions.Count(); i++)
                {
                    if (MidiActions[i].Type == EMidiOBSItemType.Modifier)
                    {
                        var item = new SMidiXMLModifier();
                        item.obsmode = MidiActions[i].ObsMode;
                        item.index = MidiActions[i].Modifier;

                        if (MidiActions[i].InActions != null)
                        {
                            item.Inputs = _toInputFormat(MidiActions[i]);
                        }

                        if (MidiActions[i].OutActions != null)
                        {
                            item.Outputs = _toOutputFormat(MidiActions[i]);
                        }
                        schema.Map.Modifiers[x] = item;
                        x++;
                    }
                }
            }

            x = 0;
            schema.Map.Items = new SMidiXMLItem[ic];
            for (int i = 0; i < MidiActions.Count(); i++)
            {
                if (MidiActions[i].Type != EMidiOBSItemType.Modifier)
                {
                    var item = new SMidiXMLItem();
                    item.obsmode  = MidiActions[i].ObsMode;
                    item.modifier = MidiActions[i].Modifier;
                    item.bind = _midiActionTypeToString(MidiActions[i].Type);
                    item.index = MidiActions[i].Index;
                    item.type = "single"; //force single mode; xmlwriter cant handle ranges for now

                    if (MidiActions[i].InActions != null)
                    {
                        item.Inputs = _toInputFormat(MidiActions[i]);
                    }

                    if (MidiActions[i].OutActions != null)
                    {
                        item.Outputs = _toOutputFormat(MidiActions[i]);
                    }
                    schema.Map.Items[x] = item;
                    x++;
                }
            }


            try
            {
                XmlSerializer w = new XmlSerializer(typeof(SXMLSchema));
                TextWriter writer = new StreamWriter(System.IO.Path.Combine(Program.GetMapsDir(), filename));
                w.Serialize(writer, schema);
                writer.Close();
                return true;
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
            }
           
            return false;
        }

        private EMidiOBSItemType _midiActionTypeFromString(string type)
        {
            string typestring = null;
            bool upper = true;
            foreach (char c in type)
            {
                if (upper)
                {
                    typestring += Char.ToUpper(c);
                    upper = false;
                }
                else
                {
                    if (c.ToString() == "-")
                    {
                        upper = true;
                    }
                    else
                    {
                        typestring += c;
                    }
                }
            }

            try
            {
                return (EMidiOBSItemType)Enum.Parse(typeof(EMidiOBSItemType), typestring);
            }catch (ArgumentException e) {
                return EMidiOBSItemType.None;
            }
        }

        private string _midiActionTypeToString(EMidiOBSItemType a)
        {
            var typestring = Enum.GetName(typeof(EMidiOBSItemType), a);
            int i = 0;
            string outstring = null;
            foreach (char c in typestring)
            {
                if (Char.IsUpper(c) && i != 0) {
                    outstring += "-";
                }
                outstring += Char.ToLower(c);
                i++;
            }
            return outstring;
        }

        private SMidiXMLInputItem[] _toInputFormat(SMidiOBSAction a)
        {
            SMidiXMLInputItem[] inputarray = new SMidiXMLInputItem[a.InActions.Count];
            for (int i=0; i< a.InActions.Count; i++)
            {
                var item = new SMidiXMLInputItem();
                item.type = Enum.GetName(typeof(EMidiOBSInputType), a.InActions[i].Type).ToLower();
                item.cmd = "0x"+a.InActions[i].Action.Cmd.ToString("X2");
                item.channel = "0x" + a.InActions[i].Action.Channel.ToString("X2");
                item.data1 = "0x" + a.InActions[i].Action.Data1.ToString("X2");
                item.data2 = "0x" + a.InActions[i].Action.Data2.ToString("X2");
                inputarray[i] = item;
             }
            return inputarray;
        }

        private SMidiXMLOutputItem[] _toOutputFormat(SMidiOBSAction a)
        {
            SMidiXMLOutputItem[] outputarray = new SMidiXMLOutputItem[a.OutActions.Count];
            for (int i = 0; i < a.OutActions.Count; i++)
            {
                var item = new SMidiXMLOutputItem();
                item.type = Enum.GetName(typeof(EMidiOBSOutputType), a.OutActions[i].Type).ToLower();
                item.cmd = "0x" + a.OutActions[i].Action.Cmd.ToString("X2");
                item.channel = "0x" + a.OutActions[i].Action.Channel.ToString("X2");
                item.data1 = "0x" + a.OutActions[i].Action.Data1.ToString("X2");
                item.data2 = "0x" + a.OutActions[i].Action.Data2.ToString("X2");
                outputarray[i] = item;
            }
            return outputarray;
        }

        #endregion
    }
}
