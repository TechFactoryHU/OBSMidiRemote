namespace OBSMidiRemote.Forms
{
    partial class FEditor
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FEditor));
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.ruletype = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.obsmode = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.modifier = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Action = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.index = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.input = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.output = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label14 = new System.Windows.Forms.Label();
            this.btn_openfile = new System.Windows.Forms.Button();
            this.text_filename = new System.Windows.Forms.TextBox();
            this.label12 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.baudrate_combo = new System.Windows.Forms.ComboBox();
            this.num_limit_ms = new System.Windows.Forms.NumericUpDown();
            this.num_limit_packet = new System.Windows.Forms.NumericUpDown();
            this.label8 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.text_description = new System.Windows.Forms.TextBox();
            this.text_website = new System.Windows.Forms.TextBox();
            this.text_version = new System.Windows.Forms.TextBox();
            this.text_author = new System.Windows.Forms.TextBox();
            this.text_devices = new System.Windows.Forms.TextBox();
            this.text_schemaname = new System.Windows.Forms.TextBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.btn_addrow = new System.Windows.Forms.Button();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.label13 = new System.Windows.Forms.Label();
            this.cbaudrate_combo = new System.Windows.Forms.ComboBox();
            this.btn_connect = new System.Windows.Forms.Button();
            this.btn_disconnect = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.btn_refresh = new System.Windows.Forms.Button();
            this.comboBox_midi_out = new System.Windows.Forms.ComboBox();
            this.comboBox_midi_in = new System.Windows.Forms.ComboBox();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.btn_cancel = new System.Windows.Forms.Button();
            this.btn_save = new System.Windows.Forms.Button();
            this.btn_saveclose = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.num_limit_ms)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.num_limit_packet)).BeginInit();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.SuspendLayout();
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.AllowUserToDeleteRows = false;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.ruletype,
            this.obsmode,
            this.modifier,
            this.Action,
            this.index,
            this.input,
            this.output});
            this.dataGridView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView1.Location = new System.Drawing.Point(2, 15);
            this.dataGridView1.Margin = new System.Windows.Forms.Padding(10);
            this.dataGridView1.MaximumSize = new System.Drawing.Size(982, 0);
            this.dataGridView1.MinimumSize = new System.Drawing.Size(982, 382);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.ReadOnly = true;
            this.dataGridView1.Size = new System.Drawing.Size(982, 384);
            this.dataGridView1.TabIndex = 0;
            this.dataGridView1.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView1_CellContentClick);
            // 
            // ruletype
            // 
            this.ruletype.DataPropertyName = "ruletype";
            this.ruletype.HeaderText = "Type";
            this.ruletype.Name = "ruletype";
            this.ruletype.ReadOnly = true;
            this.ruletype.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.ruletype.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // obsmode
            // 
            this.obsmode.DataPropertyName = "obsmode";
            this.obsmode.HeaderText = "ObsMode";
            this.obsmode.Name = "obsmode";
            this.obsmode.ReadOnly = true;
            this.obsmode.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.obsmode.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // modifier
            // 
            this.modifier.DataPropertyName = "modifier";
            this.modifier.HeaderText = "Modifier";
            this.modifier.Name = "modifier";
            this.modifier.ReadOnly = true;
            this.modifier.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            // 
            // Action
            // 
            this.Action.DataPropertyName = "action";
            this.Action.HeaderText = "Action";
            this.Action.Name = "Action";
            this.Action.ReadOnly = true;
            this.Action.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.Action.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // index
            // 
            this.index.DataPropertyName = "connect";
            this.index.HeaderText = "Connect to ...";
            this.index.Name = "index";
            this.index.ReadOnly = true;
            this.index.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            // 
            // input
            // 
            this.input.DataPropertyName = "input";
            this.input.HeaderText = "Input rule(s)";
            this.input.Name = "input";
            this.input.ReadOnly = true;
            // 
            // output
            // 
            this.output.DataPropertyName = "output";
            this.output.HeaderText = "Output rule(s)";
            this.output.Name = "output";
            this.output.ReadOnly = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label14);
            this.groupBox1.Controls.Add(this.btn_openfile);
            this.groupBox1.Controls.Add(this.text_filename);
            this.groupBox1.Controls.Add(this.label12);
            this.groupBox1.Controls.Add(this.label11);
            this.groupBox1.Controls.Add(this.label10);
            this.groupBox1.Controls.Add(this.label9);
            this.groupBox1.Controls.Add(this.baudrate_combo);
            this.groupBox1.Controls.Add(this.num_limit_ms);
            this.groupBox1.Controls.Add(this.num_limit_packet);
            this.groupBox1.Controls.Add(this.label8);
            this.groupBox1.Controls.Add(this.label7);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.text_description);
            this.groupBox1.Controls.Add(this.text_website);
            this.groupBox1.Controls.Add(this.text_version);
            this.groupBox1.Controls.Add(this.text_author);
            this.groupBox1.Controls.Add(this.text_devices);
            this.groupBox1.Controls.Add(this.text_schemaname);
            this.groupBox1.Location = new System.Drawing.Point(13, 13);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(624, 204);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Schema properties";
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(32, 30);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(49, 13);
            this.label14.TabIndex = 21;
            this.label14.Text = "Filename";
            this.label14.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // btn_openfile
            // 
            this.btn_openfile.Location = new System.Drawing.Point(248, 26);
            this.btn_openfile.Name = "btn_openfile";
            this.btn_openfile.Size = new System.Drawing.Size(57, 23);
            this.btn_openfile.TabIndex = 20;
            this.btn_openfile.Text = "Open";
            this.btn_openfile.UseVisualStyleBackColor = true;
            this.btn_openfile.Click += new System.EventHandler(this.btn_openfile_Click);
            // 
            // text_filename
            // 
            this.text_filename.Location = new System.Drawing.Point(84, 27);
            this.text_filename.Name = "text_filename";
            this.text_filename.ReadOnly = true;
            this.text_filename.Size = new System.Drawing.Size(158, 20);
            this.text_filename.TabIndex = 19;
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(331, 173);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(50, 13);
            this.label12.TabIndex = 18;
            this.label12.Text = "Baudrate";
            this.label12.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(584, 143);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(20, 13);
            this.label11.TabIndex = 17;
            this.label11.Text = "ms";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(464, 143);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(48, 13);
            this.label10.TabIndex = 16;
            this.label10.Text = "packet /";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(320, 143);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(61, 13);
            this.label9.TabIndex = 15;
            this.label9.Text = "Packet limit";
            this.label9.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // baudrate_combo
            // 
            this.baudrate_combo.FormattingEnabled = true;
            this.baudrate_combo.Location = new System.Drawing.Point(384, 169);
            this.baudrate_combo.Name = "baudrate_combo";
            this.baudrate_combo.Size = new System.Drawing.Size(121, 21);
            this.baudrate_combo.TabIndex = 14;
            // 
            // num_limit_ms
            // 
            this.num_limit_ms.Increment = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.num_limit_ms.Location = new System.Drawing.Point(515, 139);
            this.num_limit_ms.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.num_limit_ms.Name = "num_limit_ms";
            this.num_limit_ms.Size = new System.Drawing.Size(67, 20);
            this.num_limit_ms.TabIndex = 13;
            this.num_limit_ms.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.num_limit_ms.ValueChanged += new System.EventHandler(this.numericUpDown2_ValueChanged);
            // 
            // num_limit_packet
            // 
            this.num_limit_packet.Location = new System.Drawing.Point(384, 139);
            this.num_limit_packet.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            -2147483648});
            this.num_limit_packet.Name = "num_limit_packet";
            this.num_limit_packet.Size = new System.Drawing.Size(77, 20);
            this.num_limit_packet.TabIndex = 12;
            this.num_limit_packet.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.num_limit_packet.Value = new decimal(new int[] {
            1,
            0,
            0,
            -2147483648});
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(322, 54);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(60, 13);
            this.label8.TabIndex = 11;
            this.label8.Text = "Description";
            this.label8.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(352, 23);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(30, 13);
            this.label7.TabIndex = 10;
            this.label7.Text = "Web";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(39, 173);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(42, 13);
            this.label6.TabIndex = 9;
            this.label6.Text = "Version";
            this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(43, 142);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(38, 13);
            this.label5.TabIndex = 8;
            this.label5.Text = "Author";
            this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(36, 111);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(46, 13);
            this.label4.TabIndex = 7;
            this.label4.Text = "Devices";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(7, 80);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(75, 13);
            this.label3.TabIndex = 6;
            this.label3.Text = "Schema name";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // text_description
            // 
            this.text_description.Location = new System.Drawing.Point(384, 51);
            this.text_description.Multiline = true;
            this.text_description.Name = "text_description";
            this.text_description.Size = new System.Drawing.Size(221, 82);
            this.text_description.TabIndex = 5;
            // 
            // text_website
            // 
            this.text_website.Location = new System.Drawing.Point(384, 20);
            this.text_website.Name = "text_website";
            this.text_website.Size = new System.Drawing.Size(221, 20);
            this.text_website.TabIndex = 4;
            // 
            // text_version
            // 
            this.text_version.Location = new System.Drawing.Point(84, 170);
            this.text_version.Name = "text_version";
            this.text_version.Size = new System.Drawing.Size(221, 20);
            this.text_version.TabIndex = 3;
            // 
            // text_author
            // 
            this.text_author.Location = new System.Drawing.Point(84, 139);
            this.text_author.Name = "text_author";
            this.text_author.Size = new System.Drawing.Size(221, 20);
            this.text_author.TabIndex = 2;
            // 
            // text_devices
            // 
            this.text_devices.Location = new System.Drawing.Point(84, 108);
            this.text_devices.Name = "text_devices";
            this.text_devices.Size = new System.Drawing.Size(221, 20);
            this.text_devices.TabIndex = 1;
            // 
            // text_schemaname
            // 
            this.text_schemaname.Location = new System.Drawing.Point(84, 77);
            this.text_schemaname.Name = "text_schemaname";
            this.text_schemaname.Size = new System.Drawing.Size(221, 20);
            this.text_schemaname.TabIndex = 0;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.btn_addrow);
            this.groupBox2.Controls.Add(this.dataGridView1);
            this.groupBox2.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.groupBox2.Location = new System.Drawing.Point(0, 224);
            this.groupBox2.Margin = new System.Windows.Forms.Padding(0);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Padding = new System.Windows.Forms.Padding(2);
            this.groupBox2.Size = new System.Drawing.Size(988, 401);
            this.groupBox2.TabIndex = 2;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Rules";
            // 
            // btn_addrow
            // 
            this.btn_addrow.Location = new System.Drawing.Point(924, 18);
            this.btn_addrow.Name = "btn_addrow";
            this.btn_addrow.Size = new System.Drawing.Size(39, 40);
            this.btn_addrow.TabIndex = 1;
            this.btn_addrow.Text = "+";
            this.btn_addrow.UseVisualStyleBackColor = true;
            this.btn_addrow.Click += new System.EventHandler(this.button1_Click);
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.label13);
            this.groupBox3.Controls.Add(this.cbaudrate_combo);
            this.groupBox3.Controls.Add(this.btn_connect);
            this.groupBox3.Controls.Add(this.btn_disconnect);
            this.groupBox3.Controls.Add(this.label2);
            this.groupBox3.Controls.Add(this.label1);
            this.groupBox3.Controls.Add(this.btn_refresh);
            this.groupBox3.Controls.Add(this.comboBox_midi_out);
            this.groupBox3.Controls.Add(this.comboBox_midi_in);
            this.groupBox3.Location = new System.Drawing.Point(643, 13);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(333, 153);
            this.groupBox3.TabIndex = 12;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Test device";
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(8, 89);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(55, 13);
            this.label13.TabIndex = 20;
            this.label13.Text = "BaudRate";
            // 
            // cbaudrate_combo
            // 
            this.cbaudrate_combo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbaudrate_combo.FormattingEnabled = true;
            this.cbaudrate_combo.Location = new System.Drawing.Point(66, 85);
            this.cbaudrate_combo.Name = "cbaudrate_combo";
            this.cbaudrate_combo.Size = new System.Drawing.Size(209, 21);
            this.cbaudrate_combo.TabIndex = 19;
            // 
            // btn_connect
            // 
            this.btn_connect.Location = new System.Drawing.Point(171, 118);
            this.btn_connect.Name = "btn_connect";
            this.btn_connect.Size = new System.Drawing.Size(75, 23);
            this.btn_connect.TabIndex = 18;
            this.btn_connect.Text = "Connect";
            this.btn_connect.UseVisualStyleBackColor = true;
            this.btn_connect.Click += new System.EventHandler(this.btn_connect_Click);
            // 
            // btn_disconnect
            // 
            this.btn_disconnect.Location = new System.Drawing.Point(252, 118);
            this.btn_disconnect.Name = "btn_disconnect";
            this.btn_disconnect.Size = new System.Drawing.Size(75, 23);
            this.btn_disconnect.TabIndex = 17;
            this.btn_disconnect.Text = "Disconnect";
            this.btn_disconnect.UseVisualStyleBackColor = true;
            this.btn_disconnect.Click += new System.EventHandler(this.btn_disconnect_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(8, 58);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(30, 13);
            this.label2.TabIndex = 16;
            this.label2.Text = "OUT";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(20, 27);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(18, 13);
            this.label1.TabIndex = 15;
            this.label1.Text = "IN";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // btn_refresh
            // 
            this.btn_refresh.Font = new System.Drawing.Font("Microsoft Sans Serif", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.btn_refresh.Location = new System.Drawing.Point(281, 22);
            this.btn_refresh.Name = "btn_refresh";
            this.btn_refresh.Size = new System.Drawing.Size(46, 84);
            this.btn_refresh.TabIndex = 14;
            this.btn_refresh.Text = "R";
            this.btn_refresh.UseVisualStyleBackColor = true;
            this.btn_refresh.Click += new System.EventHandler(this.btn_refresh_Click);
            // 
            // comboBox_midi_out
            // 
            this.comboBox_midi_out.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox_midi_out.FormattingEnabled = true;
            this.comboBox_midi_out.Location = new System.Drawing.Point(41, 54);
            this.comboBox_midi_out.Name = "comboBox_midi_out";
            this.comboBox_midi_out.Size = new System.Drawing.Size(234, 21);
            this.comboBox_midi_out.TabIndex = 13;
            // 
            // comboBox_midi_in
            // 
            this.comboBox_midi_in.Cursor = System.Windows.Forms.Cursors.Default;
            this.comboBox_midi_in.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox_midi_in.FormattingEnabled = true;
            this.comboBox_midi_in.Location = new System.Drawing.Point(41, 23);
            this.comboBox_midi_in.Name = "comboBox_midi_in";
            this.comboBox_midi_in.Size = new System.Drawing.Size(234, 21);
            this.comboBox_midi_in.TabIndex = 12;
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.btn_cancel);
            this.groupBox4.Controls.Add(this.btn_save);
            this.groupBox4.Controls.Add(this.btn_saveclose);
            this.groupBox4.Location = new System.Drawing.Point(643, 172);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(333, 45);
            this.groupBox4.TabIndex = 13;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Save";
            // 
            // btn_cancel
            // 
            this.btn_cancel.Location = new System.Drawing.Point(252, 14);
            this.btn_cancel.Name = "btn_cancel";
            this.btn_cancel.Size = new System.Drawing.Size(75, 23);
            this.btn_cancel.TabIndex = 21;
            this.btn_cancel.Text = "Cancel";
            this.btn_cancel.UseVisualStyleBackColor = true;
            this.btn_cancel.Click += new System.EventHandler(this.btn_cancel_click);
            // 
            // btn_save
            // 
            this.btn_save.Location = new System.Drawing.Point(46, 13);
            this.btn_save.Name = "btn_save";
            this.btn_save.Size = new System.Drawing.Size(75, 23);
            this.btn_save.TabIndex = 20;
            this.btn_save.Text = "Save";
            this.btn_save.UseVisualStyleBackColor = true;
            this.btn_save.Click += new System.EventHandler(this.btn_save_Click);
            // 
            // btn_saveclose
            // 
            this.btn_saveclose.Location = new System.Drawing.Point(126, 13);
            this.btn_saveclose.Name = "btn_saveclose";
            this.btn_saveclose.Size = new System.Drawing.Size(122, 23);
            this.btn_saveclose.TabIndex = 19;
            this.btn_saveclose.Text = "Save and Close";
            this.btn_saveclose.UseVisualStyleBackColor = true;
            this.btn_saveclose.Click += new System.EventHandler(this.btn_saveclose_Click);
            // 
            // FEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(988, 625);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximumSize = new System.Drawing.Size(1004, 1000);
            this.MinimumSize = new System.Drawing.Size(1004, 624);
            this.Name = "FEditor";
            this.Text = "FEditor";
            this.Load += new System.EventHandler(this.FEditor_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.num_limit_ms)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.num_limit_packet)).EndInit();
            this.groupBox2.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.DataGridViewTextBoxColumn ruletype;
        private System.Windows.Forms.DataGridViewTextBoxColumn obsmode;
        private System.Windows.Forms.DataGridViewTextBoxColumn modifier;
        private System.Windows.Forms.DataGridViewTextBoxColumn Action;
        private System.Windows.Forms.DataGridViewTextBoxColumn index;
        private System.Windows.Forms.DataGridViewTextBoxColumn input;
        private System.Windows.Forms.DataGridViewTextBoxColumn output;
        private System.Windows.Forms.Button btn_addrow;
        private System.Windows.Forms.TextBox text_description;
        private System.Windows.Forms.TextBox text_website;
        private System.Windows.Forms.TextBox text_version;
        private System.Windows.Forms.TextBox text_author;
        private System.Windows.Forms.TextBox text_devices;
        private System.Windows.Forms.TextBox text_schemaname;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Button btn_connect;
        private System.Windows.Forms.Button btn_disconnect;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btn_refresh;
        private System.Windows.Forms.ComboBox comboBox_midi_out;
        private System.Windows.Forms.ComboBox comboBox_midi_in;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.Button btn_cancel;
        private System.Windows.Forms.Button btn_save;
        private System.Windows.Forms.Button btn_saveclose;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.ComboBox baudrate_combo;
        private System.Windows.Forms.NumericUpDown num_limit_ms;
        private System.Windows.Forms.NumericUpDown num_limit_packet;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.ComboBox cbaudrate_combo;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.Button btn_openfile;
        private System.Windows.Forms.TextBox text_filename;
    }
}