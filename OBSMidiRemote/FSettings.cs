using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using OBSMidiRemote.Lib;
using OBSMidiRemote.Lib.PureMidi.DeviceInfo;
using System.IO;
using System.Xml.Linq;
using System.Xml.XPath;
using System.IO.Ports;

namespace OBSMidiRemote
{
    public struct DeviceMapFile {
        public string filename;
        public string deviceNames;
        public string description;
        public string author;
        public string website;
        public string version;
        public string name;
    }

    public partial class FSettings : Form
    {
        List<DeviceMapFile> deviceMaps;

        public FSettings()
        {
            Program.OBSgw.StatusChanged += OnGwStatusChanged;

            this.Icon = new Icon("icons/icon.ico");

            deviceMaps = new List<DeviceMapFile>();
            InitializeComponent();
            //load midi devices
            loadDevices();
            loadDeviceMaps();

            midi_in_label.Text = Program.res.GetString("midi_input_device");
            midi_out_label.Text = Program.res.GetString("midi_output_device");
            midi_scheme_label.Text = Program.res.GetString("midi_map");
            groupBox1.Text = Program.res.GetString("obs_settings");
            groupBox2.Text = Program.res.GetString("midi_settings");
            server_label.Text = Program.res.GetString("server_address");
            password_label.Text = Program.res.GetString("password");
            ws_info.Visible = false;

            comboBox_midi_srf.Enabled = false;
            comboBox_midi_out.Enabled = false;

            btn_refresh.Text = "\uD83D\uDDD8";
        }

        private delegate void gwStatusChangedInvoked(CDeviceObsGw sender, EMidiEvent type);

        private void OnGwStatusChanged(CDeviceObsGw sender, EMidiEvent type)
        {
            if (InvokeRequired)
            {
                var d = new gwStatusChangedInvoked(OnGwStatusChanged);
                Invoke(d, new object[] { sender,type });
                return;
            }

            Console.WriteLine(type);

            ws_info.Visible = false;
            if (type == EMidiEvent.Connected || type == EMidiEvent.Connecting || type == EMidiEvent.DeviceReady)
            {
                ws_info.Visible = true;
                LockFormElements();
                Program.ChangeTrayIcon(true);

                if (type == EMidiEvent.Connecting || type == EMidiEvent.DeviceReady)
                {
                    btn_start.Text = "...";
                    ws_info.Visible = true;
                    ws_info.Text = type == EMidiEvent.DeviceReady ? "HW Ready ..." : Program.res.GetString("connecting");
                }
                else if(type == EMidiEvent.Connected)
                {
                    if (sender.Version.OBSStudioVersion != null)
                    {
                        ws_info.Text = "OBS Version: " + sender.Version.OBSStudioVersion.ToString();
                        ws_info.Text += "\r\nOBS-Websocket Version: " + sender.Version.PluginVersion.ToString();
                    }
                    btn_start.Text = Program.res.GetString("disconnect");
                } 
            }
            else
            {
                if (type == EMidiEvent.WrongPassword)
                {
                    MessageBox.Show(
                        Program.res.GetString("error_password"),
                        Program.res.GetString("error"), MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
                else if (type == EMidiEvent.Error || type == EMidiEvent.Unknown)
                {
                    MessageBox.Show(
                        Program.res.GetString("error_connect"),
                        Program.res.GetString("error"), MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
                else if(type == EMidiEvent.DeviceError)
                {
                    MessageBox.Show(
                       Program.res.GetString("error_wrongdevice"),
                       Program.res.GetString("error"), MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }

                btn_start.Text = Program.res.GetString("connect");
                UnlockFormElements();
                Program.ChangeTrayIcon(false);
            } 
        }

        private void loadDevices()
        {
            comboBox_midi_in.Items.Clear();
            comboBox_midi_in.Items.Add(new InputDeviceItem(Program.res.GetString("select_in_device")));
            comboBox_midi_in.Items.Add(new InputDeviceItem("--" +Program.res.GetString("select_in_serial")+"--"));
            //get serial ports
            string[] ports = SerialPort.GetPortNames();
            int i = 0;
            foreach (string port in ports)
            {
                InputDeviceItem item = new InputDeviceItem();
                item.Text = port;
                item.Value = port;
                item.Type = InputDeviceType.SERIAL;
                item.Index = i;
                comboBox_midi_in.Items.Add(item);
                i++;
            }

            comboBox_midi_in.Items.Add(new InputDeviceItem("--" + Program.res.GetString("select_in_midi") + "--"));
            comboBox_midi_in.SelectedIndex = 0;
            i = 0;
            var in_devices = MidiInInfo.Informations;
            foreach (var midiIn in in_devices)
            {
                InputDeviceItem item = new InputDeviceItem();
                item.Text = midiIn.ProductName;
                item.Value = midiIn.DeviceIndex;
                item.Type = InputDeviceType.MIDI;
                item.Index = i;
                comboBox_midi_in.Items.Add(item);
                i++;
            }

            comboBox_midi_out.Items.Clear();
            comboBox_midi_out.Items.Add(new InputDeviceItem("--" + Program.res.GetString("select_out_device") + "--"));
            comboBox_midi_out.SelectedIndex = 0;
            var out_devices = MidiOutInfo.Informations;
            i = 0;
            foreach (var midiOut in out_devices)
            {
                InputDeviceItem item = new InputDeviceItem();
                item.Text = midiOut.ProductName;
                item.Value = midiOut.DeviceIndex;
                item.Type = InputDeviceType.MIDI;
                item.Index = i;
                comboBox_midi_out.Items.Add(item);
                i++;
            }
        }

        private void loadDeviceMaps()
        {
            deviceMaps.Clear();
            comboBox_midi_srf.Items.Clear();
            DirectoryInfo d = new DirectoryInfo(Program.GetMapsDir());
            if (d.Exists) { 
                FileInfo[] Files = d.GetFiles("*.xml");
                XDocument schemaXML;
                foreach (FileInfo file in Files)
                {
                    try
                    {
                        schemaXML = XDocument.Load(file.FullName);
                        var info = schemaXML.XPathSelectElement("OBSDeviceMap/MapInfo");
                        if (info != null)
                        {
                            var name = info.XPathSelectElement("Name");
                            var devname = info.XPathSelectElement("DeviceName");
                            var author = info.XPathSelectElement("Author");
                            var version = info.XPathSelectElement("Version");
                            var url = info.XPathSelectElement("Website");


                            if (name != null)
                            {
                                var midisurf = new DeviceMapFile();
                                midisurf.filename = file.Name;
                                midisurf.name = name.FirstNode.ToString();
                                midisurf.deviceNames = devname != null ? devname.FirstNode.ToString() : null;
                                midisurf.author = author != null ? author.FirstNode.ToString() : null;
                                midisurf.version = version != null ? version.FirstNode.ToString() : null;
                                midisurf.website = url != null ? url.FirstNode.ToString() : null;
                               
                                foreach (XElement item in info.Descendants("Description"))
                                {
                                    if (item.Attribute("lang") != null || (string)item.Attribute("lang") == Program.lang || (string)item.Attribute("lang") == "en" )
                                    {
                                        XCData xcdata = (XCData)item.FirstNode;
                                        midisurf.description = xcdata.Value;
                                        if ((string)item.Attribute("lang") == Program.lang) { break; }
                                    }
                                }
                                deviceMaps.Add(midisurf);
                            }  
                        }
                    }
                    catch (System.Xml.XmlException e)
                    {
                       
                    }
                }
            }

            comboBox_midi_srf.Items.Add(Program.res.GetString("select_midi_map"));
            comboBox_midi_srf.SelectedIndex = 0;
            foreach (var surf in deviceMaps)
            {
                comboBox_midi_srf.Items.Add(surf.name);
            }
        }

        private void LockFormElements()
        {
            comboBox_midi_srf.Enabled = false;
            comboBox_midi_in.Enabled = false;
            comboBox_midi_out.Enabled = false;
            text_obsurl.Enabled = false;
            text_obsport.Enabled = false;
            text_obspwd.Enabled = false;
            btn_refresh.Enabled = false;
        }

        private void UnlockFormElements()
        {
            comboBox_midi_srf.Enabled = true;
            comboBox_midi_in.Enabled = true;
            comboBox_midi_out.Enabled = true;
            text_obsurl.Enabled = true;
            text_obsport.Enabled = true;
            text_obspwd.Enabled = true;
            btn_refresh.Enabled = true;
        }

        private void btn_refresh_Click(object sender, EventArgs e)
        {
            loadDevices();
            loadDeviceMaps();
        }

        private void comboBox_midi_in_changed(object sender, EventArgs e)
        {
            int selectedIndex = ((ComboBox)sender).SelectedIndex;
           
            if (selectedIndex > 0) {
                var selectedDevice = (InputDeviceItem)((ComboBox)sender).SelectedItem;
                if (selectedDevice.Type == InputDeviceType.UNKNOWN)
                {
                    ((ComboBox)sender).SelectedIndex = 0;
                    comboBox_midi_srf.Enabled = false;
                    comboBox_midi_out.Enabled = false;
                }
                else
                {
                    comboBox_midi_srf.Enabled = true;
                    if (comboBox_midi_srf.SelectedIndex == 0)
                    {
                        int i = 1;
                        foreach (var surf in deviceMaps)
                        {
                            if (selectedDevice != null && surf.deviceNames.IndexOf(selectedDevice.Text) > -1)
                            {
                                comboBox_midi_srf.SelectedIndex = i;
                                break;
                            }
                            i++;
                        }
                    }

                    if (selectedDevice.Type == InputDeviceType.MIDI) {
                        comboBox_midi_out.Enabled = true;
                        if (comboBox_midi_out.SelectedIndex == 0)
                        {
                            for (int i = 0; i < comboBox_midi_out.Items.Count; i++)
                            {
                                if (comboBox_midi_out.Items[i].ToString() == selectedDevice.Text)
                                {
                                    comboBox_midi_out.SelectedIndex = i;
                                }
                            }
                        }
                    }else
                    {
                        comboBox_midi_out.SelectedIndex = 0;
                        comboBox_midi_out.Enabled = false;
                    }
                }
            }
        }

        private void comboBox_midi_srf_changed(object sender, EventArgs e)
        {
            midi_map_info.Text = "";
            if (((ComboBox)sender).SelectedIndex > 0) { 
                var midisrf = deviceMaps[((ComboBox)sender).SelectedIndex-1];
                midi_map_info.Text = Program.res.GetString("midi_author")+": "+ midisrf.author + " | "+ Program.res.GetString("midi_version") + ": " + midisrf.version +" | ";
                midi_map_info.Text += midisrf.website + "\r\n";
                midi_map_info.Text += midisrf.description;
            }
        }

        private void midi_map_info_Click(object sender, EventArgs e)
        {
            if (comboBox_midi_srf.SelectedIndex>0)
            {
                var midisrf = deviceMaps[comboBox_midi_srf.SelectedIndex - 1];
                if (midisrf.website != null)
                {
                    System.Diagnostics.Process.Start(midisrf.website);
                }
            }
        }

        private void btn_start_Click(object sender, EventArgs e)
        {
            if (Program.OBSgw.Connected() || Program.OBSgw.Connecting())
            {
                Program.OBSgw.Stop();
            }
            else { 
                if (!String.IsNullOrEmpty(text_obsurl.Text))
                {
                    if (!String.IsNullOrEmpty(text_obsport.Text) && Int16.Parse(text_obsport.Text) > 0 )
                    {
                        if (comboBox_midi_in.SelectedIndex > 0)
                        {
                            if (comboBox_midi_srf.SelectedIndex > 0) {
                                var selectedDevice = (InputDeviceItem)comboBox_midi_in.SelectedItem;
                                if (Program.OBSgw.loadSchema(deviceMaps[comboBox_midi_srf.SelectedIndex - 1].filename))
                                {
                                    if (selectedDevice.Type == InputDeviceType.MIDI)
                                    {
                                        Program.OBSgw.SetDevice(selectedDevice, (InputDeviceItem)comboBox_midi_out.SelectedItem);
                                    }
                                    else
                                    {
                                        Program.OBSgw.SetDevice(selectedDevice);
                                    }
                                    Program.OBSgw.Start("ws://" + text_obsurl.Text + ":" + text_obsport.Text, text_obspwd.Text);
                                }
                                else
                                {
                                    MessageBox.Show(Program.res.GetString("error_mapfile"), Program.res.GetString("error"), MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                                }
                            }else
                            {
                                MessageBox.Show(Program.res.GetString("error_nomapfile"), Program.res.GetString("error"), MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                            }
                        }
                        else
                        {
                            MessageBox.Show(Program.res.GetString("error_noinputdevice"), Program.res.GetString("error"), MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        }
                    }else
                    {
                        MessageBox.Show(Program.res.GetString("error_obsport"), Program.res.GetString("error"), MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    }
                }else
                {
                    MessageBox.Show(Program.res.GetString("error_obsurl"), Program.res.GetString("error"), MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
            }
        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void linkLabel1_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("https://github.com/Palakis/obs-websocket");
        }


        private void linkLabel2_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("https://obsproject.com/forum/resources/obs-websocket-remote-control-of-obs-studio-made-easy.466/");
        }

        private void WinForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (Program.OBSgw.Connected() || Program.OBSgw.Connecting())
            {
                e.Cancel = true;
                this.Hide();
            }
        }

        private void OnClose()
        {
            
        }

        private void obsport_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void FSettings_Load(object sender, EventArgs e)
        {

        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {

        }

        private void text_obspwd_TextChanged(object sender, EventArgs e)
        {

        }

        private void text_obsurl_TextChanged(object sender, EventArgs e)
        {

        }

        private void text_obsport_TextChanged(object sender, EventArgs e)
        {

        }

        private void server_label_Click(object sender, EventArgs e)
        {

        }

        private void password_label_Click(object sender, EventArgs e)
        {

        }

        private void obs_info_Click(object sender, EventArgs e)
        {

        }

        private void groupBox2_Enter(object sender, EventArgs e)
        {

        }
    }
}
