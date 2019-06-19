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
using System.IO;
using System.IO.Ports;
using OBSMidiRemote.Lib.PureMidi.DeviceInfo;
using OBSMidiRemote.Lib.Device;

namespace OBSMidiRemote.Forms
{
    public partial class FEditor : Form
    {
        private SMidiXMLInfo XMLInfo;
        private SMidiXMLData XMLData;
        public IOBSDevice Device;
        private bool changed = false;

        public List<SMidiOBSAction> midiActions = new List<SMidiOBSAction>();
        private List<CFEditor_GridItem> datalist = new List<CFEditor_GridItem>();
        private SMidiOBSAction selected;
        private int selectedIndex;

        private FEditorMidiLog logWindow;
        private FEditorItem editWindow;

        public FEditor()
        {
            InitializeComponent();
            datalist.Clear();

            this.Text = "XML Scheme editor";
            dataGridView1.AutoGenerateColumns = false;
            dataGridView1.DataError += new DataGridViewDataErrorEventHandler(dgvCombo_DataError);
            dataGridView1.MultiSelect = false;
            dataGridView1.MouseClick += dataGridView1_MouseClick;
            dataGridView1.MouseDoubleClick += DataGridView1_MouseDoubleClick;
            btn_refresh.Text = "\uD83D\uDDD8";

            cbaudrate_combo.Items.Clear();
            baudrate_combo.Items.Clear();
            for (int i=0; i< CMidiFields.BaudRates.Length; i++)
            {
                cbaudrate_combo.Items.Add(CMidiFields.BaudRates[i]);
                baudrate_combo.Items.Add(CMidiFields.BaudRates[i]);
                if (CMidiFields.DefaultBaudRate == CMidiFields.BaudRates[i])
                {
                    baudrate_combo.SelectedIndex  = i;
                    cbaudrate_combo.SelectedIndex = i;
                }
            }
            comboBox_midi_in.SelectedIndexChanged += comboBox_midi_in_changed;
            loadDevices();

            logWindow = new FEditorMidiLog();
            lockForm();
        }

        private void loadDevices()
        {
            comboBox_midi_in.Items.Clear();
            comboBox_midi_in.Items.Add(new InputDeviceItem(Program.res.GetString("select_in_device")));
            comboBox_midi_in.Items.Add(new InputDeviceItem("--" + Program.res.GetString("select_in_serial") + "--"));
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


        public bool NewXMLFile(String file)
        {
            changed = false;
            XMLInfo = new SMidiXMLInfo();
            XMLInfo.Filename = file;
            XMLData = new SMidiXMLData();

            num_limit_packet.Value = -1;
            num_limit_ms.Value = 0;

            return true;
        }

        public bool LoadXMLFile(String file)
        {
            CMidiXMLSchema parser = new CMidiXMLSchema();
            if (parser.LoadSchema(file,"en"))
            {
                text_filename.Text = file;
                changed = false;
                midiActions = parser.MidiActions;
                XMLInfo = parser.MidiMapInfo;
                XMLData = parser.MidiMapData;

                int baudIndex = Array.IndexOf(CMidiFields.BaudRates, XMLData.BaudRate);
                baudrate_combo.SelectedIndex = baudIndex;
                cbaudrate_combo.SelectedIndex = baudIndex;

                num_limit_packet.Value = XMLData.PacketCount;
                num_limit_ms.Value = XMLData.PacketInterval;

                text_author.Text = XMLInfo.Author;
                text_schemaname.Text = XMLInfo.Name;
                text_devices.Text = XMLInfo.Name;

                if (XMLInfo.Description.Count > 0)
                {
                    for (int i=0; i<XMLInfo.Description.Count; i++)
                    {
                        if (XMLInfo.Description[i].Lang == "en") {
                            text_description.Text = XMLInfo.Description[i].Text;
                        }
                    }
                }

                text_website.Text = XMLInfo.Website;
                text_version.Text = XMLInfo.Version.ToString();
                renderData();
                unlockForm();

                return true;
            }else
            {
                text_filename.Text = "";
            }
            return false;
        }

        private void CollectXMLData()
        {
            XMLInfo.Author = text_author.Text;
            XMLInfo.Name = text_schemaname.Text;
            XMLInfo.DeviceName = text_devices.Text;
            XMLInfo.Version = Version.Parse(text_version.Text);
            XMLInfo.VersionString = XMLInfo.Version.ToString();
            XMLInfo.Website = text_website.Text;

            if (XMLInfo.Description == null)
            {
                XMLInfo.Description = new List<SMidiXMLInfoDescription>();
            }
            bool found = false;
            if (XMLInfo.Description.Count > 0)
            {
                for (int i = 0; i < XMLInfo.Description.Count; i++)
                {
                    if (XMLInfo.Description[i].Lang == "en")
                    {
                        var item = XMLInfo.Description[i];
                        item.Text = text_description.Text;
                        XMLInfo.Description[i] = item;
                        found = true;
                    }
                }
            }
            if (!found)
            {
                XMLInfo.Description.Add(new SMidiXMLInfoDescription { Text = text_description.Text, Lang = "en" });
            }

            int selected = baudrate_combo.SelectedIndex;
            if (selected >= 0) {
                 XMLData.BaudRate = CMidiFields.BaudRates[selected];
            }
            XMLData.PacketCount = (int)num_limit_packet.Value;
            XMLData.PacketInterval = (int)num_limit_ms.Value;
        }

        public bool SaveXMLFile()
        {
            if (!String.IsNullOrEmpty(text_filename.Text)) {
                var filename = text_filename.Text;
                CMidiXMLSchema writer = new CMidiXMLSchema();
                CollectXMLData();
                writer.SetMidiInfo(XMLInfo);
                writer.SetMidiData(XMLData);
                writer.SetMidiActions(midiActions);
                return writer.WriteSchema(filename);
            }
            else
            {
                Console.WriteLine("empty filename");
            }

            return false;
        }

        public void EditRow(int index, SMidiOBSAction d)
        {
            if (index >= 0 && index < midiActions.Count()) {
                midiActions[index] = d;
                renderData();
                changed = true;
            }
        }

        public void AddRow()
        {
            midiActions.Add(new SMidiOBSAction());
            renderData();
            changed = true;
        }

        public void DeleteRow(int index)
        {
            midiActions.RemoveAt(index);
            renderData();
            changed = true;
        }

        private void lockForm()
        {
            text_author.Enabled = false;
            text_description.Enabled = false;
            text_devices.Enabled = false;
            text_schemaname.Enabled = false;
            text_version.Enabled = false;
            text_website.Enabled = false;
            btn_addrow.Enabled = false;
        }

        private void unlockForm()
        {
            text_author.Enabled = true;
            text_description.Enabled = true;
            text_devices.Enabled = true;
            text_schemaname.Enabled = true;
            text_version.Enabled = true;
            text_website.Enabled = true;
            btn_addrow.Enabled = true;
        }

        void dgvCombo_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            // (No need to write anything in here)
            // Console.WriteLine(e.RowIndex + ":"+e.ColumnIndex+": " + e.Exception.Message);
        }

        public List<SMidiOBSAction> GetModifiers()
        {
            List<SMidiOBSAction> modifiers = new List<SMidiOBSAction>();
            for (int i=0; i<midiActions.Count(); i++)
            {
                if (midiActions[i].Type == EMidiOBSItemType.Modifier)
                {
                    modifiers.Add(midiActions[i]);

                }
            }
            return modifiers;
        }

        private void renderData()
        {
            dataGridView1.DataSource = null;
            datalist.Clear();
            for (int i= midiActions.Count()-1; i>=0 ; i--)
            {
                datalist.Add( new CFEditor_GridItem(i, midiActions[i]) );
            }
            dataGridView1.DataSource = datalist;
        }

        private bool selectData(int index)
        {
            if (index >= 0 && index < midiActions.Count())
            {
                int realindex = getRealIndex(index);
                selected = midiActions[realindex];
                selectedIndex = realindex;
                return true;
            }
            return false;
        }

        private int getRealIndex(int index)
        {
            if (index >= 0 && index < midiActions.Count())
            {
                int realindex = (midiActions.Count() - 1) - index;
                return realindex;
            }
            return -1;
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void DataGridView1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                int currentMouseOverRow = dataGridView1.HitTest(e.X, e.Y).RowIndex;
                if (selectData(currentMouseOverRow))
                {
                    OpenRowEditor();
                }
            }
        }

        private void dataGridView1_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                ContextMenu m = new ContextMenu();
                int currentMouseOverRow = dataGridView1.HitTest(e.X, e.Y).RowIndex;

                if (currentMouseOverRow >= 0)
                {
                    dataGridView1.Rows[currentMouseOverRow].Selected = true;

                    var editmenu = new CGridMenuItem("Edit");
                    editmenu.SelectedIndex = currentMouseOverRow;
                    editmenu.Click += Edit_Click;
                    m.MenuItems.Add(editmenu);

                    var delmenu = new CGridMenuItem("Delete");
                    delmenu.SelectedIndex = currentMouseOverRow;
                    delmenu.Click += Delete_Click;
                    m.MenuItems.Add(delmenu);

                }else
                {
                    var add = new MenuItem("Add");
                    add.Click += Add_Click;
                    m.MenuItems.Add(add);
                }
                m.Show(dataGridView1, new Point(e.X, e.Y));
            }
        }

        private void OpenRowEditor()
        {
            editWindow = new FEditorItem(this);
            editWindow.SetData(selected, selectedIndex);
            editWindow.ShowDialog(this);
        }

        private void Edit_Click(object sender, EventArgs e)
        {
            CGridMenuItem dataholder = (CGridMenuItem)sender;
            if (dataholder.SelectedIndex > -1)
            {
                if (selectData(dataholder.SelectedIndex))
                {
                    OpenRowEditor();
                }
            }
        }

        private void Delete_Click(object sender, EventArgs e)
        {
            CGridMenuItem dataholder = (CGridMenuItem)sender;
            if (dataholder.SelectedIndex > -1)
            {
                DeleteRow(getRealIndex(dataholder.SelectedIndex));
            }
        }

        private void Add_Click(object sender, EventArgs e)
        {
            AddRow();
        }

        private void button2_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            AddRow();
        }

        private void btn_refresh_Click(object sender, EventArgs e)
        {
            loadDevices();
        }

        private void comboBox_midi_in_changed(object sender, EventArgs e)
        {
            int selectedIndex = ((ComboBox)sender).SelectedIndex;
            if (selectedIndex > 0)
            {
                var selectedDevice = (InputDeviceItem)((ComboBox)sender).SelectedItem;
                if (selectedDevice.Type == InputDeviceType.UNKNOWN)
                {
                    ((ComboBox)sender).SelectedIndex = 0;
                    comboBox_midi_out.Enabled = false;
                    cbaudrate_combo.Enabled = false;
                }
                else
                {
                    if (selectedDevice.Type == InputDeviceType.MIDI)
                    {
                        comboBox_midi_out.Enabled = true;
                        cbaudrate_combo.Enabled = false;
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
                    }
                    else
                    {
                        comboBox_midi_out.SelectedIndex = 0;
                        comboBox_midi_out.Enabled = false;
                        cbaudrate_combo.Enabled = true;
                    }
                }
            }
        }

        private void btn_connect_Click(object sender, EventArgs e)
        {
            if (Device == null)
            {
                //Device
                if (comboBox_midi_in.SelectedIndex > 0)
                {
                    var selectedDevice = (InputDeviceItem)comboBox_midi_in.SelectedItem;
                    if (selectedDevice.Type == InputDeviceType.MIDI)
                    {
                        Device = new CObsDeviceMidi();
                        Device.Input = selectedDevice;
                        if (comboBox_midi_out.SelectedIndex > 0)
                        {
                            Device.Output = (InputDeviceItem)comboBox_midi_out.SelectedItem;
                        }
                    }
                    else if(selectedDevice.Type == InputDeviceType.SERIAL)
                    {
                        Device = new CObsDeviceMidi();
                        Device.BaudRate = int.Parse(baudrate_combo.SelectedText);
                        Device.Input = selectedDevice;
                    }
                    else
                    {

                    }

                    if (Device != null)
                    {
                        Device.OnData += Device_OnData; ;
                        Device.OnStatusChanged += Device_OnStatusChanged;

                        comboBox_midi_in.Enabled = false;
                        comboBox_midi_out.Enabled = false;
                        baudrate_combo.Enabled = false;
                        btn_connect.Enabled = false;
                        btn_disconnect.Enabled = false;

                        Device.Connect();
                    }
                }
            }
        }

        private void Device_OnStatusChanged(IOBSDevice sender, EMidiEvent eventType)
        {
            logWindow.Log("Event: " + Enum.GetName(typeof(EMidiEvent), eventType));
            if (eventType == EMidiEvent.DeviceDisconnected || eventType == EMidiEvent.DeviceError || eventType == EMidiEvent.Unknown)
            {
                comboBox_midi_in.Enabled = true;
                comboBox_midi_out.Enabled = true;
                baudrate_combo.Enabled = true;
                btn_connect.Enabled = true;
                btn_disconnect.Enabled = true;
                logWindow.Hide();
            }
            else if(eventType == EMidiEvent.DeviceReady)
            {
                btn_disconnect.Enabled = true;
                logWindow.Show(this);
            }
        }

        private void Device_OnData(IOBSDevice sender, SMidiAction data)
        {
            logWindow.Data(data);
            if (editWindow != null)
            {
                editWindow.MidiData(data);
            }
        }

        private void btn_disconnect_Click(object sender, EventArgs e)
        {
            if (Device != null) { Device.Disconnect(); }
            Device = null;
        }

        private void FEditor_Load(object sender, EventArgs e)
        {

        }

        private void numericUpDown2_ValueChanged(object sender, EventArgs e)
        {

        }

        private void btn_openfile_Click(object sender, EventArgs e)
        {
            OpenFileDialog();
        }

        public bool OpenFileDialog()
        {
            OpenFileDialog fdlg = new OpenFileDialog();
            fdlg.Title = "Select or create new XML file";
            fdlg.InitialDirectory = Program.GetMapsDir();
            fdlg.Filter = "XML file (*.xml)|*.xml";
            fdlg.FilterIndex = 2;
            fdlg.RestoreDirectory = true;
            fdlg.CheckFileExists = false;
            fdlg.CheckPathExists = true;

            if (fdlg.ShowDialog() == DialogResult.OK)
            {
                text_filename.Text = fdlg.SafeFileName;
                if (File.Exists(System.IO.Path.Combine(Program.GetMapsDir(), fdlg.SafeFileName)))
                {
                    LoadXMLFile(fdlg.SafeFileName);
                }
                else
                {
                    NewXMLFile(fdlg.SafeFileName);
                }

                return true;
            }
            return false;
        }

        private void btn_cancel_click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btn_saveclose_Click(object sender, EventArgs e)
        {
            if (SaveXMLFile())
            {
                this.Close();
            }else
            {
                MessageBox.Show("Cannot save file ...", Program.res.GetString("error"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btn_save_Click(object sender, EventArgs e)
        {
            if (!SaveXMLFile())
            {
                MessageBox.Show("Cannot save file ...", Program.res.GetString("error"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }

    public class CGridMenuItem : MenuItem
    {
        public int SelectedIndex { get; set; }
        public string SelectedGrid { get; set; }

        public CGridMenuItem(String name)
        {
            SelectedIndex = -1;
            this.Text = name;
        }
    }

    public class CFEditor_GridItem
    {
        private int _index;
        public string ruletype { get; set; }
        public string obsmode { get; set; }
        public string modifier { get; set; }
        public string action { get; set; }
        public string connect { get; set; }
        public string input { get; set; }
        public string output { get; set; }

        public CFEditor_GridItem(int index, SMidiOBSAction a)
        {
            _index = index;

            ruletype = a.Type == EMidiOBSItemType.Modifier ? "Modifier" : "Item";
            obsmode = ((EMidiObsMode)a.ObsMode).ToString();
            if (a.Type == EMidiOBSItemType.Modifier)
            {
                modifier = "--";
            }
            else
            {
                if (a.Modifier == -1) { modifier = "ANY"; }
                else if (a.Modifier == 0) { modifier = "None"; }
                else { modifier = "Modifier#" + a.Modifier.ToString(); }
            }

            action = a.Type.ToString();
            connect = (a.Index + 1).ToString();

            switch (a.Type)
            {
                case EMidiOBSItemType.AudioItem:
                    connect += ". Audio item";
                    break;
                case EMidiOBSItemType.AudioVolume:
                    connect += ". Audio slider";
                    break;
                case EMidiOBSItemType.ConnectionStatus:
                    connect = "Connection status";
                    break;
                case EMidiOBSItemType.Mode:
                    connect = "OBS Mode";
                    break;
                case EMidiOBSItemType.Pscene:
                    connect += ". Preview Scene";
                    break;
                case EMidiOBSItemType.PsceneItem:
                    connect += ". Preview Source";
                    break;
                case EMidiOBSItemType.Scene:
                    connect += ". Scene";
                    break;
                case EMidiOBSItemType.SceneItem:
                    connect += ". Source";
                    break;
                case EMidiOBSItemType.Record:
                    connect = "Recording";
                    break;
                case EMidiOBSItemType.Stream:
                    connect = "Streaming";
                    break;
                case EMidiOBSItemType.ReloadObsData:
                    connect = "Reload OBS data";
                    break;
                case EMidiOBSItemType.Transition:
                    connect += ". Transition";
                    break;
                case EMidiOBSItemType.None:
                    connect = "None (dummy)";
                    break;
            }

            if (a.InActions != null)
            {
                input = "["+a.InActions.Count().ToString()+"]";
                for (int i=0; i<a.InActions.Count(); i++)
                {
                    input += " ";
                    input += a.InActions[i].Type.ToString();
                    input += ":0x";
                    input += a.InActions[i].Action.Data1.ToString("X2");
                    input += a.InActions[i].Action.Data2.ToString("X2");
                }
            }else { input = "--";  }

            if (a.OutActions != null)
            {
                output = "[" + a.OutActions.Count().ToString() + "]";
                for (int i = 0; i < a.OutActions.Count(); i++)
                {
                    output += " ";
                    output += a.OutActions[i].Type.ToString();
                    output += ":0x";
                    output += a.OutActions[i].Action.Data1.ToString("X2");
                    output += a.OutActions[i].Action.Data2.ToString("X2");
                }
            }
            else { output = "0"; }
        }
    }
}
