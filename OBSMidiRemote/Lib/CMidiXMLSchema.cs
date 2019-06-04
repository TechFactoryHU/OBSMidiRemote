using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.XPath;

namespace OBSMidiRemote.Lib
{
    public class CMidiXMLSchema
    {
        private string midimapFile;

        public CMidiXMLSchema() { }

        public bool loadSchema(string filename, ref List<SMidiOBSAction> midiActions, ref int midiPacketCount, ref int midiPacketInterval, ref int baudRate)
        {
            midimapFile = filename;
            XDocument schemaXML;
            midiActions.Clear();
           
            try
            {
                schemaXML = XDocument.Load(System.IO.Path.Combine(Program.GetMapsDir(), filename));
                var maproot = schemaXML.XPathSelectElement("OBSDeviceMap/Map");
                if (maproot != null)
                {
                    if (maproot.Attribute("max-packet") != null)
                    {
                        midiPacketCount = (int)maproot.Attribute("max-packet");
                    }
                    if (maproot.Attribute("packet-interval") != null)
                    {
                        var iv = (int)maproot.Attribute("packet-interval");
                        if (iv > 0)
                        {
                            midiPacketInterval = iv;
                        }
                        else { midiPacketInterval = 0; }
                    }
                    if (maproot.Attribute("baudrate") != null)
                    {
                        baudRate = (int)maproot.Attribute("baudrate");
                    }
                    
                    //check modifiers
                    var mapmods = maproot.XPathSelectElement("Modifiers");
                    if (mapmods != null)
                    {
                        int _mi = 1;
                        foreach (XElement item in mapmods.Descendants("Modifier"))
                        {
                            SMidiOBSAction action = new SMidiOBSAction();
                            action.Modifier = _mi;
                            action.Index = _mi - 1;
                            action.Type = EMidiOBSItemType.Modifier;
                            if (item.Attribute("obsmode") != null)
                            {
                                action.ObsMode = (int)item.Attribute("obsmode");
                            }
                            action.InActions = new List<SMidiInput>();
                            action.OutActions = new List<SMidiOutput>();
                            _parseXMLInAndOutput(ref action, item, 0);
                            midiActions.Add(action);
                            _mi++;
                        }
                    }
                    
                    var items = maproot.XPathSelectElement("Items");
                    if (items != null)
                    {
                        foreach (XElement item in items.Descendants("Item"))
                        {
                            _parseItemfromXElement(item, ref midiActions);
                        }

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

        private void _parseItemfromXElement(XElement item, ref List<SMidiOBSAction> midiActions)
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
                switch (bind)
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
                        action.Type = EMidiOBSItemType.ReloadOBSData;
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
                        action.Type = EMidiOBSItemType.PScene;
                        break;
                    case "pscene-item":
                        action.Type = EMidiOBSItemType.PSceneItem;
                        break;
                    default:
                        action.Type = EMidiOBSItemType.None;
                        break;
                }

                if (type == "single")
                {
                    action.Index = (int)item.Attribute("index");
                    _parseXMLInAndOutput(ref action, item, 0);
                    midiActions.Add(action);
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
                                midiActions.Add(action);
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
                                midiActions.Add(action);
                                from++;
                            }
                        }
                    }
                }
            }
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
                if (input.Attribute("channel") != null) { _in.Action.Channel = (int)input.Attribute("channel"); }
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
                    _out.Action.Channel = (int)output.Attribute("channel");
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
    }
}
