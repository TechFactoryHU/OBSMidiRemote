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

namespace OBSMidiRemote.Forms
{
    public partial class FEditorItem : Form
    {
        private SMidiOBSAction Data;
        private int Index;

        private FEditor parent;

        private List<CFEditorItem_GridItem> datalist_in = new List<CFEditorItem_GridItem>();
        private List<CFEditorItem_GridItem> datalist_out = new List<CFEditorItem_GridItem>();

        public FEditorItem(FEditor form)
        {
            parent = form;
            InitializeComponent();
            dataGridView1.CellValueChanged += gridViewDataChanged;
            dataGridView1.MouseClick += dataGridView_MouseClick;
            dataGridView1.DataError += DataGridView1_DataError;
            dataGridView2.CellValueChanged += gridViewDataChanged;
            dataGridView2.CellContentClick += dataGridView2_CellContentClick;
            dataGridView2.MouseClick += dataGridView_MouseClick;
            dataGridView2.DataError += DataGridView1_DataError;
        }

        private void DataGridView1_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
          
        }

        public void SetData(SMidiOBSAction d, int i)
        {
            this.Text = "ItemEditor ("+i+")";
            //copy object
            Data = new SMidiOBSAction();
            Data.InActions = new List<SMidiInput>();
            if (d.InActions != null) { Data.InActions = d.InActions.ToList(); }
            Data.Index = d.Index;
            Data.Modifier = d.Modifier;
            Data.OutActions = new List<SMidiOutput>();
            if (d.OutActions != null) { Data.OutActions = d.OutActions.ToList(); }
            Data.ObsMode = d.ObsMode;
            Data.Type = d.Type;
            Data.IsModifier = d.IsModifier;

            Index = i;
            RenderForm();
            FormChanged();
            SelectValues();

        }

        private void SelectValues()
        {
            if (modifier_combo.Enabled)
            {
                modifier_combo.SelectedIndex = 0;
                for (int i=0; i<modifier_combo.Items.Count; i++)
                {
                    if (Data.Modifier == ((StandardListItem)modifier_combo.Items[i]).Value)
                    {
                        modifier_combo.SelectedIndex = i;
                    }
                }
            }

            if (connectTo_combo.Enabled)
            {

                connectTo_combo.SelectedIndex = 0;
                for (int i = 0; i < connectTo_combo.Items.Count; i++)
                {
                    if (Data.Type == EMidiOBSItemType.Modifier)
                    {
                        if (Data.Modifier == ((StandardListItem)connectTo_combo.Items[i]).Value)
                        {
                            connectTo_combo.SelectedIndex = i;
                        }
                    }
                    else
                    {
                        if (Data.Index == ((StandardListItem)connectTo_combo.Items[i]).Value)
                        {
                            connectTo_combo.SelectedIndex = i;
                        }
                    }
                }
            }
        }

        public void RenderForm()
        {
            dataGridView1.Columns[0].ValueType = typeof(EMidiOBSInputType);
            ((DataGridViewComboBoxColumn)dataGridView1.Columns[0]).DataSource = Enum.GetValues(typeof(EMidiOBSInputType));

            dataGridView2.Columns[0].ValueType = typeof(EMidiOBSOutputType);
            ((DataGridViewComboBoxColumn)dataGridView2.Columns[0]).DataSource = Enum.GetValues(typeof(EMidiOBSOutputType));

            actionType_combo.DataSource = Enum.GetValues(typeof(EMidiOBSItemType));
            actionType_combo.SelectedItem = Data.Type;
            actionType_combo.SelectedIndexChanged += ActionType_combo_SelectedIndexChanged;

            obsMode_combo.DataSource = Enum.GetValues(typeof(EMidiObsMode));
            obsMode_combo.SelectedItem = (EMidiObsMode)Data.ObsMode;

            var modifiers = parent.GetModifiers();

            modifier_combo.Items.Add(new StandardListItem("ANY", -1));
            modifier_combo.Items.Add(new StandardListItem("No modifier", 0));

            for (int i = 0; i < modifiers.Count(); i++)
            {
                modifier_combo.Items.Add(new StandardListItem(modifiers[i].Modifier + "# modifier", modifiers[i].Modifier));
            }
            RenderGridView();
        }

        private void ActionType_combo_SelectedIndexChanged(object sender, EventArgs e)
        {
            FormChanged();
        }

        public void FormChanged()
        {
            if (actionType_combo.SelectedIndex == (int)EMidiOBSItemType.Modifier)
            {
                if (modifier_combo.Enabled) { 
                    modifier_combo.Enabled = false;
                    modifier_combo.SelectedIndex = 0;
                    connectTo_combo.Items.Clear();
                    connectTo_combo.Enabled = true;
                    var modifiers = parent.GetModifiers();
                  
                    for (int x=1; x<20; x++)
                    {
                        bool found = false;
                        for (int i=0; i< modifiers.Count(); i++)
                        {
                            if (modifiers[i].Modifier == x && Data.Modifier != x) { found = true;  break; }
                        }
                        if (!found)
                        {
                            connectTo_combo.Items.Add(new StandardListItem(x.ToString(),x));
                        }
                    }
                }
            }
            else
            {
                if (!modifier_combo.Enabled)
                {
                    modifier_combo.Enabled = true;
                }

                if (CMidiFields.Ranges.Contains((EMidiOBSItemType)actionType_combo.SelectedIndex))
                {
                    connectTo_combo.Enabled = true;
                    connectTo_combo.Items.Clear();
                    for (int i=0; i<20; i++)
                    {
                        connectTo_combo.Items.Add(new StandardListItem((i+1).ToString()+". "+ Enum.GetName(typeof(EMidiOBSItemType), (EMidiOBSItemType)actionType_combo.SelectedIndex) , i));
                    }
                }else
                {
                    connectTo_combo.Enabled = false;
                }
            }
        }

        public void MidiData(SMidiAction data)
        {
            if (!midiData2Gridview(dataGridView1, data))
            {
                midiData2Gridview(dataGridView2, data);
            }
        }

        private void RenderGridView()
        {
            dataGridView1.DataSource = null;
            dataGridView1.AutoGenerateColumns = false;
            datalist_in.Clear();

            if (Data.InActions != null && Data.InActions.Count > 0)
            {
                for (int i = 0; i < Data.InActions.Count; i++)
                {
                    var item = new CFEditorItem_GridItem();
                    item.InputData(Data.InActions[i]);
                    datalist_in.Add(item);
                }
                dataGridView1.DataSource = datalist_in;
            }

            dataGridView2.DataSource = null;
            dataGridView2.AutoGenerateColumns = false;
            datalist_out.Clear();

            if (Data.OutActions != null && Data.OutActions.Count>0)
            {
                for (int i = 0; i < Data.OutActions.Count; i++)
                {
                    var item = new CFEditorItem_GridItem();
                    item.OutputData(Data.OutActions[i]);
                    datalist_out.Add(item);
                }
                dataGridView2.DataSource = datalist_out;
            }
        }
        
        private bool midiData2Gridview(DataGridView gv, SMidiAction data)
        {
            for (int r = 0; r < gv.RowCount; r++)
            {
                if (gv.Rows[r].Selected)
                {
                    gv.Rows[r].Cells[1].Value = "0x" + data.Cmd.ToString("X2");
                    gv.Rows[r].Cells[2].Value = "0x" + data.Channel.ToString("X2");
                    gv.Rows[r].Cells[3].Value = "0x" + data.Data1.ToString("X2");
                    gv.Rows[r].Cells[4].Value = "0x" + data.Data2.ToString("X2");
                    gv.Rows[r].Selected = false;
                    return true;
                }
                else
                {
                    for (int c = 1; c < gv.Rows[r].Cells.Count; c++)
                    {
                        if (gv.Rows[r].Cells[c].Selected)
                        {
                            switch (c)
                            {
                                case 1:
                                    gv.Rows[r].Cells[1].Value = "0x" + data.Cmd.ToString("X2");
                                    break;
                                case 2:
                                    gv.Rows[r].Cells[2].Value = "0x" + data.Channel.ToString("X2");
                                    break;
                                case 3:
                                    gv.Rows[r].Cells[3].Value = "0x" + data.Data1.ToString("X2");
                                    break;
                                case 4:
                                    gv.Rows[r].Cells[4].Value = "0x" + data.Data2.ToString("X2");
                                    break;
                            }
                            gv.Rows[r].Cells[c].Selected = false;
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void dataGridView2_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (parent.Device != null && parent.Device.IsReady())
            {
                var senderGrid = (DataGridView)sender;
                if (senderGrid.Columns[e.ColumnIndex] is DataGridViewButtonColumn)
                {
                    SMidiOutput data = datalist_out[e.RowIndex].ToOutput();
                    parent.Device.Send(data.Action);
                }
            }
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void connectTo_combo_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void gridViewDataChanged(object sender, DataGridViewCellEventArgs e)
        {
            var gv = (DataGridView)sender;
            if (gv.Name == "dataGridView1")
            {
                if (datalist_in[e.RowIndex] != null) { 
                   Data.InActions[e.RowIndex] = datalist_in[e.RowIndex].ToInput();
                }
            }
            else
            {
                if (datalist_out[e.RowIndex] != null)
                {
                    Data.OutActions[e.RowIndex] = datalist_out[e.RowIndex].ToOutput();
                }
            }         
        }

        private void collectData()
        {
            Data.Type = (EMidiOBSItemType)actionType_combo.SelectedIndex;
            if (modifier_combo.Enabled) { 
                Data.Modifier = ((StandardListItem)modifier_combo.SelectedItem).Value;
            }
            Data.ObsMode = (int)obsMode_combo.SelectedIndex;

            if ((EMidiOBSItemType)actionType_combo.SelectedIndex == EMidiOBSItemType.Modifier)
            {
                Data.Modifier = connectTo_combo.SelectedIndex;
            }else
            {
                if (connectTo_combo.Enabled)
                {
                    Data.Index = connectTo_combo.SelectedIndex;
                }else
                {
                    Data.Index = 0;
                }
            }
        }

        private void dataGridView_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                ((DataGridView)sender).ClearSelection();
                ((DataGridView)sender).EndEdit();
                ContextMenu m = new ContextMenu();
                int currentMouseOverRow = ((DataGridView)sender).HitTest(e.X, e.Y).RowIndex;

                if (currentMouseOverRow >= 0)
                {
                    ((DataGridView)sender).Rows[currentMouseOverRow].Selected = true;
                    var delmenu = new CGridMenuItem("Delete");
                    delmenu.SelectedIndex = currentMouseOverRow;
                    delmenu.SelectedGrid = ((DataGridView)sender).Name;
                    delmenu.Click += menu_delete_click;
                    m.MenuItems.Add(delmenu);
                }
                else
                {
                    var add = new MenuItem("Add");
                    add.Click += btn_out_add_Click;
                    m.MenuItems.Add(add);
                }
                m.Show(((DataGridView)sender), new Point(e.X, e.Y));
            }
        }

        private void menu_delete_click(object sender, EventArgs e)
        {
            if (((CGridMenuItem)sender).SelectedGrid.Equals("dataGridView2"))
            {
                Data.OutActions.RemoveAt(((CGridMenuItem)sender).SelectedIndex);
            }
            else if (((CGridMenuItem)sender).SelectedGrid.Equals("dataGridView1"))
            {
                Data.InActions.RemoveAt(((CGridMenuItem)sender).SelectedIndex);
            }
            RenderGridView();
        }

        private void btn_in_add_Click(object sender, EventArgs e)
        {
            if (Data.InActions == null) { Data.InActions = new List<SMidiInput>(); }
            Data.InActions.Add(new SMidiInput());
            RenderGridView();
        }

        private void btn_out_add_Click(object sender, EventArgs e)
        {
            if (Data.OutActions == null) { Data.OutActions = new List<SMidiOutput>(); }
            Data.OutActions.Add(new SMidiOutput { Type = EMidiOBSOutputType.Unknown, Action = { Channel = 0, Cmd = 0, Data1 = 0,Data2 = 0 } });
            RenderGridView();
        }

        private void btn_cancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btn_ok_Click(object sender, EventArgs e)
        {
            collectData();
            parent.EditRow(Index, Data);
            this.Close();
        }
    }

    public class CFEditorItem_GridItem
    {
        public EMidiOBSInputType TypeIn { get; set; }
        public EMidiOBSOutputType TypeOut { get; set; }


        private int _cmd;
        private int _channel;
        private int _data1;
        private int _data2;

        public string Cmd
        {
            get
            {
                return "0x"+_cmd.ToString("X2");
            }

            set
            {
                _cmd = _string2int(value);
            }
        }

        public string Channel
        {
            get
            {
                return "0x" + _channel.ToString("X2");
            }
            set
            {
                _channel = _string2int(value);
            }
        }

        public string Data1
        {
            get
            {
                return "0x" + _data1.ToString("X2");
            }
            set
            {
                _data1 = _string2int(value);
            }
        }

        public string Data2
        {
            get
            {
                return "0x" + _data2.ToString("X2");
            }
            set
            {
                _data2 = _string2int(value);
            }
        }

        private int _string2int(string value)
        {
            int retval = 0;
            if (!String.IsNullOrEmpty(value)) { 
                if (value.StartsWith("0x"))
                {
                    try
                    {
                        retval = Convert.ToInt32(value, 16);
                    }
                    catch (FormatException e)
                    {}
                    catch (Exception e)
                    {}
                }
                else
                {
                    retval = int.Parse(value);
                }
                if (retval <= 0 || retval > 255)
                {
                    retval = 0;
                }
            }
            return retval;
        }

        public CFEditorItem_GridItem() {}

        public void InputData (SMidiInput d)
        {
            TypeIn = d.Type;
            setData(d.Action);
        }

        public void OutputData(SMidiOutput d)
        {
            TypeOut = d.Type;
            setData(d.Action);
        }

        private void setData(SMidiAction a)
        {
            _cmd = a.Cmd;
            _channel = a.Channel;
            _data1 = a.Data1;
            _data2 = a.Data2;
        }

        public SMidiOutput ToOutput()
        {
            var r = new SMidiOutput();
            r.Type = TypeOut;
            r.Action = new SMidiAction {
                Channel = _channel, Cmd = _cmd, Data1 = _data1, Data2 = _data2
            };
            return r;
        }

        public SMidiInput ToInput()
        {
            var r = new SMidiInput();
            r.Type = TypeIn;
            r.Action = new SMidiAction
            {
                Channel = _channel,
                Cmd = _cmd,
                Data1 = _data1,
                Data2 = _data2
            };
            return r;
        }
    }
}
