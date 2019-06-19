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
using Newtonsoft.Json.Linq;

namespace OBSMidiRemote.Forms
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

    public partial class FMain : Form
    {
        private List<SMidiXMLInfo> deviceMaps;
        private FEditor editor = new FEditor();
        private COBSMidiRemote obsmidi;
        private bool locked = false;

        private List<CFMain_GridItem> schemeFiles;

        public FMain()
        {
            obsmidi = new COBSMidiRemote();
            obsmidi.StatusChanged += Obsmidi_StatusChanged;

            this.Icon = new Icon("icons/icon.ico");

            deviceMaps = new List<SMidiXMLInfo>();
            schemeFiles = new List<CFMain_GridItem>();
            InitializeComponent();
            FormClosing += WinForm_FormClosing;

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

            tabPage2.Text = Program.res.GetString("tab2_title");
            tabPage1.Text = Program.res.GetString("tab1_title");

            comboBox_midi_srf.Enabled = false;
            comboBox_midi_out.Enabled = false;

            btn_refresh.Text = "\uD83D\uDDD8";
            btn_reload.Text = Program.res.GetString("refresh");
            btn_newscheme.Text = Program.res.GetString("new_scheme");

            comboBox_obs_version.Items.Add(new StandardListItem("OBS", (int)EOBSConnectorType.OBS));
            comboBox_obs_version.Items.Add(new StandardListItem("StreamLabs OBS", (int)EOBSConnectorType.SLOBS));
            comboBox_obs_version.SelectedIndex = 0;

            tabControl1.Deselecting += TabControl1_Deselecting;
            gridview_files.AutoGenerateColumns = false;
            gridview_files.DataError += Gridview_files_DataError;
            gridview_files.MultiSelect = false;
            gridview_files.MouseClick += Gridview_files_MouseClick;
            gridview_files.MouseDoubleClick += Gridview_files_MouseDoubleClick;
        }

        private void EditScheme(string filename)
        {
            var editor = new FEditor();
            editor.LoadXMLFile(filename);
            editor.ShowDialog();
            loadDeviceMaps();
        }

        private void NewScheme()
        {
            var editor = new FEditor();
            if (editor.OpenFileDialog())
            {
                editor.ShowDialog();
            }
        }

        private void Gridview_files_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            
        }

        private void Gridview_files_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            int currentMouseOverRow = gridview_files.HitTest(e.X, e.Y).RowIndex;
            if (currentMouseOverRow >= 0)
            {
                var filename = schemeFiles[currentMouseOverRow].FileName;
                EditScheme(filename);
            }   
        }

        private void Gridview_files_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                ContextMenu m = new ContextMenu();
                int currentMouseOverRow = gridview_files.HitTest(e.X, e.Y).RowIndex;
                if (currentMouseOverRow >= 0)
                {
                    gridview_files.Rows[currentMouseOverRow].Selected = true;
                    var editmenu = new CGridMenuItem("Edit");
                    editmenu.SelectedIndex = currentMouseOverRow;
                    editmenu.Click += Gridview_Editmenu_Click;
                    m.MenuItems.Add(editmenu);
                    m.Show(gridview_files, new Point(e.X, e.Y));
                }
            }
        }

        private void Gridview_Editmenu_Click(object sender, EventArgs e)
        {
            var filename = schemeFiles[((CGridMenuItem)sender).SelectedIndex].FileName;
            EditScheme(filename);
        }

        private void TabControl1_Deselecting(object sender, TabControlCancelEventArgs e)
        {
            if (locked) { e.Cancel = true; }
        }

        private delegate void Obsmidi_StatusChangedInvoked(COBSMidiRemote sender, EOBSCStatus type);
        private void Obsmidi_StatusChanged(COBSMidiRemote sender, EOBSCStatus type)
        {
            if (InvokeRequired)
            {
                var d = new Obsmidi_StatusChangedInvoked(Obsmidi_StatusChanged);
                Invoke(d, new object[] { sender, type });
                return;
            }

            ws_info.Visible = false;
            if (type == EOBSCStatus.Connected || type == EOBSCStatus.Connecting || type == EOBSCStatus.DeviceReady)
            {
                ws_info.Visible = true;
                LockFormElements();
                Program.ChangeTrayIcon(true);

                if (type == EOBSCStatus.Connecting || type == EOBSCStatus.DeviceReady)
                {
                    btn_start.Text = "...";
                    ws_info.Visible = true;
                    ws_info.Text = type == EOBSCStatus.DeviceReady ? "HW Ready ..." : Program.res.GetString("connecting");
                }
                else if (type == EOBSCStatus.Connected)
                {
                    ws_info.Text = obsmidi.GetVersionInfo();
                    btn_start.Text = Program.res.GetString("disconnect");
                }
            }
            else
            {
                if (type == EOBSCStatus.WrongAuth)
                {
                    MessageBox.Show(
                        Program.res.GetString("error_password"),
                        Program.res.GetString("error"), MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
                else if (type == EOBSCStatus.Error || type == EOBSCStatus.Unknown)
                {
                    MessageBox.Show(
                        Program.res.GetString("error_connect"),
                        Program.res.GetString("error"), MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
                else if (type == EOBSCStatus.DeviceError)
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
            gridview_files.DataSource = null;
            CMidiXMLSchema parser = new CMidiXMLSchema();
            deviceMaps.Clear();
            schemeFiles.Clear();
            comboBox_midi_srf.Items.Clear();
            DirectoryInfo d = new DirectoryInfo(Program.GetMapsDir());
            if (d.Exists) { 
                FileInfo[] Files = d.GetFiles("*.xml");
                foreach (FileInfo file in Files)
                {
                    try
                    {
                        SMidiXMLInfo? info = parser.LoadHeaders(file.Name);
                        if (info != null)
                        {
                            deviceMaps.Add((SMidiXMLInfo)info);
                            schemeFiles.Add(new CFMain_GridItem((SMidiXMLInfo)info));
                        }
                    }
                    catch (System.Xml.XmlException e)
                    {}
                }
            }

            comboBox_midi_srf.Items.Add(Program.res.GetString("select_midi_map"));
            comboBox_midi_srf.SelectedIndex = 0;
            foreach (var surf in deviceMaps)
            {
                comboBox_midi_srf.Items.Add(surf.Name);
            }

            gridview_files.DataSource = schemeFiles;
        }

        private void LockFormElements()
        {
            comboBox_midi_srf.Enabled = false;
            comboBox_midi_in.Enabled = false;
            comboBox_midi_out.Enabled = false;
            comboBox_obs_version.Enabled = false;
            text_obsurl.Enabled = false;
            text_obsport.Enabled = false;
            text_obspwd.Enabled = false;
            btn_refresh.Enabled = false;
            locked = true;
        }

        private void UnlockFormElements()
        {
            comboBox_midi_srf.Enabled = true;
            comboBox_midi_in.Enabled = true;
            comboBox_midi_out.Enabled = true;
            comboBox_obs_version.Enabled = true;
            text_obsurl.Enabled = true;
            text_obsport.Enabled = true;
            text_obspwd.Enabled = true;
            btn_refresh.Enabled = true;
            locked = false;
        }

        private async void btn_refresh_Click(object sender, EventArgs e)
        {
            loadDevices();
            loadDeviceMaps();
        }

        private void comboBox_obs_version_changed(object sender, EventArgs e)
        {
            var selectedValue = ((StandardListItem)((ComboBox)sender).SelectedItem);
            if ((EOBSConnectorType)selectedValue.Value == EOBSConnectorType.SLOBS) {
                text_obsport.Text = "59650";
                linkLabel1.Visible = false;
                linkLabel2.Visible = false;
                obs_info.Text = Program.res.GetString("slobs_info");
            }
            else if ((EOBSConnectorType)selectedValue.Value == EOBSConnectorType.OBS)
            {
                text_obsport.Text = "4444";
                linkLabel1.Visible = true;
                linkLabel2.Visible = true;
                obs_info.Text = Program.res.GetString("obs_info");
            }
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
                            if (selectedDevice != null && surf.DeviceName.IndexOf(selectedDevice.Text) > -1)
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
                midi_map_info.Text = Program.res.GetString("midi_author")+": "+ midisrf.Author + " | "+ Program.res.GetString("midi_version") + ": " + midisrf.Version.ToString() +" | ";
                midi_map_info.Text += midisrf.Website + "\r\n";
                midi_map_info.Text += midisrf.Description;
            }
        }

        private void midi_map_info_Click(object sender, EventArgs e)
        {
            if (comboBox_midi_srf.SelectedIndex>0)
            {
                var midisrf = deviceMaps[comboBox_midi_srf.SelectedIndex - 1];
                if (midisrf.Website != null)
                {
                    System.Diagnostics.Process.Start(midisrf.Website);
                }
            }
        }

        private void btn_start_Click(object sender, EventArgs e)
        {
            if (obsmidi.Connected || obsmidi.Connecting)
            {
                obsmidi.Disconnect();
            }
            else { 
                if (!String.IsNullOrEmpty(text_obsurl.Text))
                {
                    if (!String.IsNullOrEmpty(text_obsport.Text) && Int32.Parse(text_obsport.Text) > 0 )
                    {
                        if (comboBox_midi_in.SelectedIndex > 0)
                        {

                            var connector = (StandardListItem)comboBox_obs_version.SelectedItem;
                            if (connector != null)
                            {
                                obsmidi.SetConnector((EOBSConnectorType)connector.Value);
                            }else
                            {
                                obsmidi.SetConnector(EOBSConnectorType.OBS);
                            }

                            if (comboBox_midi_srf.SelectedIndex > 0) {
                                var selectedDevice = (InputDeviceItem)comboBox_midi_in.SelectedItem;
                                if (obsmidi.LoadSchema(deviceMaps[comboBox_midi_srf.SelectedIndex - 1].Filename))
                                {
                                    if (selectedDevice.Type == InputDeviceType.MIDI)
                                    {
                                        obsmidi.SetDevice(selectedDevice, (InputDeviceItem)comboBox_midi_out.SelectedItem);
                                    }
                                    else
                                    {
                                        obsmidi.SetDevice(selectedDevice);
                                    }
                                    obsmidi.SetEndoint(text_obsurl.Text, Int32.Parse(text_obsport.Text), !String.IsNullOrEmpty(text_obspwd.Text) ? text_obspwd.Text : null);
                                    obsmidi.Connect();
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
            if (obsmidi.Connected || obsmidi.Connecting)
            {
                e.Cancel = true;
                this.Hide();
            }
        }

        private void obsport_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void btn_newscheme_Click(object sender, EventArgs e)
        {
            NewScheme();
        }

        private void btn_reload_Click(object sender, EventArgs e)
        {
            loadDeviceMaps();
        }
    }


    public class CFMain_GridItem
    {
        private SMidiXMLInfo SchemaInfo;
          
        public string Name {
            get {
                return SchemaInfo.Name;
            }
        }

        public string FileName {
            get
            {
                return SchemaInfo.Filename;
            }
        }
        public string FileMTime {
            get
            {
                return SchemaInfo.FileMTime.ToString("dd/MM/yy HH:mm:ss");
            }
        }

        public CFMain_GridItem(SMidiXMLInfo a)
        {
            SchemaInfo = a;
        }
    }
}
