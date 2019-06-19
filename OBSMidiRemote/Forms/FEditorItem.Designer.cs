namespace OBSMidiRemote.Forms
{
    partial class FEditorItem
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FEditorItem));
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label4 = new System.Windows.Forms.Label();
            this.connectTo_combo = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.modifier_combo = new System.Windows.Forms.ComboBox();
            this.obsMode_combo = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.actionType_combo = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.btn_in_add = new System.Windows.Forms.Button();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.type = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.cmd = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.channel = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.data1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.data2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.test = new System.Windows.Forms.DataGridViewButtonColumn();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.btn_out_add = new System.Windows.Forms.Button();
            this.dataGridView2 = new System.Windows.Forms.DataGridView();
            this.btn_ok = new System.Windows.Forms.Button();
            this.btn_cancel = new System.Windows.Forms.Button();
            this.type1 = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.cmd1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.channel1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.data11 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.data12 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.test1 = new System.Windows.Forms.DataGridViewButtonColumn();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.groupBox3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView2)).BeginInit();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.connectTo_combo);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.modifier_combo);
            this.groupBox1.Controls.Add(this.obsMode_combo);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.actionType_combo);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Location = new System.Drawing.Point(12, 165);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(559, 129);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Action / Parameters";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(269, 23);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(14, 13);
            this.label4.TabIndex = 7;
            this.label4.Text = "#";
            // 
            // connectTo_combo
            // 
            this.connectTo_combo.FormattingEnabled = true;
            this.connectTo_combo.Location = new System.Drawing.Point(287, 20);
            this.connectTo_combo.Name = "connectTo_combo";
            this.connectTo_combo.Size = new System.Drawing.Size(121, 21);
            this.connectTo_combo.TabIndex = 6;
            this.connectTo_combo.SelectedIndexChanged += new System.EventHandler(this.connectTo_combo_SelectedIndexChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(29, 94);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(44, 13);
            this.label3.TabIndex = 5;
            this.label3.Text = "Modifier";
            // 
            // modifier_combo
            // 
            this.modifier_combo.FormattingEnabled = true;
            this.modifier_combo.Location = new System.Drawing.Point(78, 90);
            this.modifier_combo.Name = "modifier_combo";
            this.modifier_combo.Size = new System.Drawing.Size(185, 21);
            this.modifier_combo.TabIndex = 4;
            // 
            // obsMode_combo
            // 
            this.obsMode_combo.FormattingEnabled = true;
            this.obsMode_combo.Location = new System.Drawing.Point(78, 55);
            this.obsMode_combo.Name = "obsMode_combo";
            this.obsMode_combo.Size = new System.Drawing.Size(185, 21);
            this.obsMode_combo.TabIndex = 3;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(15, 59);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(59, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "OBS Mode";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // actionType_combo
            // 
            this.actionType_combo.FormattingEnabled = true;
            this.actionType_combo.Location = new System.Drawing.Point(78, 20);
            this.actionType_combo.Name = "actionType_combo";
            this.actionType_combo.Size = new System.Drawing.Size(185, 21);
            this.actionType_combo.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(42, 24);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(37, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Action";
            this.label1.Click += new System.EventHandler(this.label1_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.btn_in_add);
            this.groupBox2.Controls.Add(this.dataGridView1);
            this.groupBox2.Location = new System.Drawing.Point(13, 4);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(559, 151);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Input rules";
            // 
            // btn_in_add
            // 
            this.btn_in_add.Location = new System.Drawing.Point(522, 19);
            this.btn_in_add.Name = "btn_in_add";
            this.btn_in_add.Size = new System.Drawing.Size(31, 23);
            this.btn_in_add.TabIndex = 1;
            this.btn_in_add.Text = "+";
            this.btn_in_add.UseVisualStyleBackColor = true;
            this.btn_in_add.Click += new System.EventHandler(this.btn_in_add_Click);
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.AllowUserToDeleteRows = false;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.type,
            this.cmd,
            this.channel,
            this.data1,
            this.data2,
            this.test});
            this.dataGridView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView1.Location = new System.Drawing.Point(3, 16);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.Size = new System.Drawing.Size(553, 132);
            this.dataGridView1.TabIndex = 0;
            this.dataGridView1.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView1_CellContentClick);
            // 
            // type
            // 
            this.type.DataPropertyName = "TypeIn";
            this.type.DisplayStyle = System.Windows.Forms.DataGridViewComboBoxDisplayStyle.ComboBox;
            this.type.HeaderText = "Type";
            this.type.Name = "type";
            this.type.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            // 
            // cmd
            // 
            this.cmd.DataPropertyName = "Cmd";
            this.cmd.HeaderText = "Cmd";
            this.cmd.Name = "cmd";
            this.cmd.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.cmd.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.cmd.Width = 80;
            // 
            // channel
            // 
            this.channel.DataPropertyName = "Channel";
            this.channel.HeaderText = "Channel";
            this.channel.Name = "channel";
            this.channel.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.channel.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.channel.Width = 50;
            // 
            // data1
            // 
            this.data1.DataPropertyName = "Data1";
            this.data1.HeaderText = "Data1";
            this.data1.Name = "data1";
            this.data1.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.data1.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.data1.Width = 80;
            // 
            // data2
            // 
            this.data2.DataPropertyName = "Data2";
            this.data2.HeaderText = "Data2";
            this.data2.Name = "data2";
            this.data2.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.data2.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.data2.Width = 80;
            // 
            // test
            // 
            this.test.HeaderText = "Test";
            this.test.Name = "test";
            this.test.Width = 50;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.btn_out_add);
            this.groupBox3.Controls.Add(this.dataGridView2);
            this.groupBox3.Location = new System.Drawing.Point(13, 300);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(559, 152);
            this.groupBox3.TabIndex = 2;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Output rules (for visual feedback)";
            // 
            // btn_out_add
            // 
            this.btn_out_add.Location = new System.Drawing.Point(522, 19);
            this.btn_out_add.Name = "btn_out_add";
            this.btn_out_add.Size = new System.Drawing.Size(31, 23);
            this.btn_out_add.TabIndex = 2;
            this.btn_out_add.Text = "+";
            this.btn_out_add.UseVisualStyleBackColor = true;
            this.btn_out_add.Click += new System.EventHandler(this.btn_out_add_Click);
            // 
            // dataGridView2
            // 
            this.dataGridView2.AllowUserToAddRows = false;
            this.dataGridView2.AllowUserToDeleteRows = false;
            this.dataGridView2.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView2.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.type1,
            this.cmd1,
            this.channel1,
            this.data11,
            this.data12,
            this.test1});
            this.dataGridView2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView2.Location = new System.Drawing.Point(3, 16);
            this.dataGridView2.Name = "dataGridView2";
            this.dataGridView2.Size = new System.Drawing.Size(553, 133);
            this.dataGridView2.TabIndex = 0;
            this.dataGridView2.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView2_CellContentClick);
            // 
            // btn_ok
            // 
            this.btn_ok.Location = new System.Drawing.Point(408, 472);
            this.btn_ok.Name = "btn_ok";
            this.btn_ok.Size = new System.Drawing.Size(75, 23);
            this.btn_ok.TabIndex = 3;
            this.btn_ok.Text = "Ok";
            this.btn_ok.UseVisualStyleBackColor = true;
            this.btn_ok.Click += new System.EventHandler(this.btn_ok_Click);
            // 
            // btn_cancel
            // 
            this.btn_cancel.Location = new System.Drawing.Point(495, 471);
            this.btn_cancel.Name = "btn_cancel";
            this.btn_cancel.Size = new System.Drawing.Size(75, 23);
            this.btn_cancel.TabIndex = 4;
            this.btn_cancel.Text = "Cancel";
            this.btn_cancel.UseVisualStyleBackColor = true;
            this.btn_cancel.Click += new System.EventHandler(this.btn_cancel_Click);
            // 
            // type1
            // 
            this.type1.DataPropertyName = "TypeOut";
            this.type1.DisplayStyle = System.Windows.Forms.DataGridViewComboBoxDisplayStyle.ComboBox;
            this.type1.HeaderText = "Type";
            this.type1.Name = "type1";
            // 
            // cmd1
            // 
            this.cmd1.DataPropertyName = "Cmd";
            this.cmd1.HeaderText = "Cmd";
            this.cmd1.Name = "cmd1";
            this.cmd1.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.cmd1.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.cmd1.Width = 80;
            // 
            // channel1
            // 
            this.channel1.DataPropertyName = "Channel";
            this.channel1.HeaderText = "Channel";
            this.channel1.Name = "channel1";
            this.channel1.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.channel1.Width = 50;
            // 
            // data11
            // 
            this.data11.DataPropertyName = "Data1";
            this.data11.HeaderText = "Data1";
            this.data11.Name = "data11";
            this.data11.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.data11.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.data11.Width = 80;
            // 
            // data12
            // 
            this.data12.DataPropertyName = "Data2";
            this.data12.HeaderText = "Data2";
            this.data12.Name = "data12";
            this.data12.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.data12.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.data12.Width = 80;
            // 
            // test1
            // 
            this.test1.HeaderText = "Test";
            this.test1.Name = "test1";
            this.test1.Text = "Test";
            this.test1.Width = 50;
            // 
            // FEditorItem
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(584, 507);
            this.Controls.Add(this.btn_cancel);
            this.Controls.Add(this.btn_ok);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(600, 546);
            this.MinimumSize = new System.Drawing.Size(600, 546);
            this.Name = "FEditorItem";
            this.Text = "FEditorItem";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.groupBox3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView2)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.ComboBox obsMode_combo;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox actionType_combo;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.DataGridView dataGridView2;
        private System.Windows.Forms.ComboBox modifier_combo;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox connectTo_combo;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button btn_in_add;
        private System.Windows.Forms.DataGridViewComboBoxColumn type;
        private System.Windows.Forms.DataGridViewTextBoxColumn cmd;
        private System.Windows.Forms.DataGridViewTextBoxColumn channel;
        private System.Windows.Forms.DataGridViewTextBoxColumn data1;
        private System.Windows.Forms.DataGridViewTextBoxColumn data2;
        private System.Windows.Forms.DataGridViewButtonColumn test;
        private System.Windows.Forms.Button btn_out_add;
        private System.Windows.Forms.Button btn_ok;
        private System.Windows.Forms.Button btn_cancel;
        private System.Windows.Forms.DataGridViewComboBoxColumn type1;
        private System.Windows.Forms.DataGridViewTextBoxColumn cmd1;
        private System.Windows.Forms.DataGridViewTextBoxColumn channel1;
        private System.Windows.Forms.DataGridViewTextBoxColumn data11;
        private System.Windows.Forms.DataGridViewTextBoxColumn data12;
        private System.Windows.Forms.DataGridViewButtonColumn test1;
    }
}