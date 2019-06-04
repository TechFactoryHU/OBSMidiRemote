namespace OBSMidiRemote
{
    partial class FSettings
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
            this.components = new System.ComponentModel.Container();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.text_obsport = new System.Windows.Forms.TextBox();
            this.linkLabel2 = new System.Windows.Forms.LinkLabel();
            this.obs_info = new System.Windows.Forms.Label();
            this.linkLabel1 = new System.Windows.Forms.LinkLabel();
            this.text_obspwd = new System.Windows.Forms.TextBox();
            this.password_label = new System.Windows.Forms.Label();
            this.server_label = new System.Windows.Forms.Label();
            this.text_obsurl = new System.Windows.Forms.TextBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.midi_map_info = new System.Windows.Forms.TextBox();
            this.btn_refresh = new System.Windows.Forms.Button();
            this.comboBox_midi_srf = new System.Windows.Forms.ComboBox();
            this.midi_scheme_label = new System.Windows.Forms.Label();
            this.comboBox_midi_out = new System.Windows.Forms.ComboBox();
            this.midi_out_label = new System.Windows.Forms.Label();
            this.midi_in_label = new System.Windows.Forms.Label();
            this.comboBox_midi_in = new System.Windows.Forms.ComboBox();
            this.btn_start = new System.Windows.Forms.Button();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.ws_info = new System.Windows.Forms.TextBox();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.text_obsport);
            this.groupBox1.Controls.Add(this.linkLabel2);
            this.groupBox1.Controls.Add(this.obs_info);
            this.groupBox1.Controls.Add(this.linkLabel1);
            this.groupBox1.Controls.Add(this.text_obspwd);
            this.groupBox1.Controls.Add(this.password_label);
            this.groupBox1.Controls.Add(this.server_label);
            this.groupBox1.Controls.Add(this.text_obsurl);
            this.groupBox1.Location = new System.Drawing.Point(10, 192);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(410, 113);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "OBS settings (WebSocket)";
            this.groupBox1.Enter += new System.EventHandler(this.groupBox1_Enter);
            // 
            // text_obsport
            // 
            this.text_obsport.Location = new System.Drawing.Point(330, 20);
            this.text_obsport.MaxLength = 6;
            this.text_obsport.Name = "text_obsport";
            this.text_obsport.Size = new System.Drawing.Size(71, 20);
            this.text_obsport.TabIndex = 13;
            this.text_obsport.Text = "4444";
            this.text_obsport.TextChanged += new System.EventHandler(this.text_obsport_TextChanged);
            // 
            // linkLabel2
            // 
            this.linkLabel2.AutoSize = true;
            this.linkLabel2.Location = new System.Drawing.Point(265, 85);
            this.linkLabel2.Name = "linkLabel2";
            this.linkLabel2.Size = new System.Drawing.Size(87, 13);
            this.linkLabel2.TabIndex = 12;
            this.linkLabel2.TabStop = true;
            this.linkLabel2.Text = "OBS Websocket";
            this.linkLabel2.Click += new System.EventHandler(this.linkLabel2_Click);
            // 
            // obs_info
            // 
            this.obs_info.AutoSize = true;
            this.obs_info.Location = new System.Drawing.Point(7, 85);
            this.obs_info.Name = "obs_info";
            this.obs_info.Size = new System.Drawing.Size(222, 13);
            this.obs_info.TabIndex = 11;
            this.obs_info.Text = "obs-websocket plugin required for midi remote";
            this.obs_info.Click += new System.EventHandler(this.obs_info_Click);
            // 
            // linkLabel1
            // 
            this.linkLabel1.AutoSize = true;
            this.linkLabel1.Location = new System.Drawing.Point(357, 85);
            this.linkLabel1.Name = "linkLabel1";
            this.linkLabel1.Size = new System.Drawing.Size(40, 13);
            this.linkLabel1.TabIndex = 10;
            this.linkLabel1.TabStop = true;
            this.linkLabel1.Text = "GitHub";
            this.linkLabel1.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel1_LinkClicked);
            this.linkLabel1.Click += new System.EventHandler(this.linkLabel1_Click);
            // 
            // text_obspwd
            // 
            this.text_obspwd.Location = new System.Drawing.Point(115, 52);
            this.text_obspwd.Name = "text_obspwd";
            this.text_obspwd.Size = new System.Drawing.Size(286, 20);
            this.text_obspwd.TabIndex = 9;
            this.text_obspwd.UseSystemPasswordChar = true;
            this.text_obspwd.TextChanged += new System.EventHandler(this.text_obspwd_TextChanged);
            // 
            // password_label
            // 
            this.password_label.AutoSize = true;
            this.password_label.Location = new System.Drawing.Point(6, 55);
            this.password_label.Name = "password_label";
            this.password_label.Size = new System.Drawing.Size(86, 13);
            this.password_label.TabIndex = 8;
            this.password_label.Text = "Server password";
            this.password_label.Click += new System.EventHandler(this.password_label_Click);
            // 
            // server_label
            // 
            this.server_label.AutoSize = true;
            this.server_label.Location = new System.Drawing.Point(6, 23);
            this.server_label.Name = "server_label";
            this.server_label.Size = new System.Drawing.Size(78, 13);
            this.server_label.TabIndex = 7;
            this.server_label.Text = "Server address";
            this.server_label.Click += new System.EventHandler(this.server_label_Click);
            // 
            // text_obsurl
            // 
            this.text_obsurl.Location = new System.Drawing.Point(115, 20);
            this.text_obsurl.Name = "text_obsurl";
            this.text_obsurl.Size = new System.Drawing.Size(208, 20);
            this.text_obsurl.TabIndex = 0;
            this.text_obsurl.Text = "localhost";
            this.text_obsurl.TextChanged += new System.EventHandler(this.text_obsurl_TextChanged);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.midi_map_info);
            this.groupBox2.Controls.Add(this.btn_refresh);
            this.groupBox2.Controls.Add(this.comboBox_midi_srf);
            this.groupBox2.Controls.Add(this.midi_scheme_label);
            this.groupBox2.Controls.Add(this.comboBox_midi_out);
            this.groupBox2.Controls.Add(this.midi_out_label);
            this.groupBox2.Controls.Add(this.midi_in_label);
            this.groupBox2.Controls.Add(this.comboBox_midi_in);
            this.groupBox2.Location = new System.Drawing.Point(10, 7);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(410, 178);
            this.groupBox2.TabIndex = 2;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Hardware settings";
            this.groupBox2.Enter += new System.EventHandler(this.groupBox2_Enter);
            // 
            // midi_map_info
            // 
            this.midi_map_info.BackColor = System.Drawing.SystemColors.Control;
            this.midi_map_info.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.midi_map_info.Cursor = System.Windows.Forms.Cursors.Hand;
            this.midi_map_info.Location = new System.Drawing.Point(10, 116);
            this.midi_map_info.Multiline = true;
            this.midi_map_info.Name = "midi_map_info";
            this.midi_map_info.ReadOnly = true;
            this.midi_map_info.Size = new System.Drawing.Size(391, 51);
            this.midi_map_info.TabIndex = 9;
            this.midi_map_info.Click += new System.EventHandler(this.midi_map_info_Click);
            // 
            // btn_refresh
            // 
            this.btn_refresh.Font = new System.Drawing.Font("Microsoft Sans Serif", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.btn_refresh.Location = new System.Drawing.Point(355, 20);
            this.btn_refresh.Name = "btn_refresh";
            this.btn_refresh.Size = new System.Drawing.Size(46, 86);
            this.btn_refresh.TabIndex = 8;
            this.btn_refresh.Text = "R";
            this.btn_refresh.UseVisualStyleBackColor = true;
            this.btn_refresh.Click += new System.EventHandler(this.btn_refresh_Click);
            // 
            // comboBox_midi_srf
            // 
            this.comboBox_midi_srf.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox_midi_srf.FormattingEnabled = true;
            this.comboBox_midi_srf.Location = new System.Drawing.Point(115, 84);
            this.comboBox_midi_srf.Name = "comboBox_midi_srf";
            this.comboBox_midi_srf.Size = new System.Drawing.Size(234, 21);
            this.comboBox_midi_srf.TabIndex = 6;
            this.comboBox_midi_srf.SelectedIndexChanged += new System.EventHandler(this.comboBox_midi_srf_changed);
            // 
            // midi_scheme_label
            // 
            this.midi_scheme_label.AutoSize = true;
            this.midi_scheme_label.Location = new System.Drawing.Point(6, 87);
            this.midi_scheme_label.Name = "midi_scheme_label";
            this.midi_scheme_label.Size = new System.Drawing.Size(76, 13);
            this.midi_scheme_label.TabIndex = 5;
            this.midi_scheme_label.Text = "Hardware map";
            this.midi_scheme_label.Click += new System.EventHandler(this.label3_Click);
            // 
            // comboBox_midi_out
            // 
            this.comboBox_midi_out.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox_midi_out.FormattingEnabled = true;
            this.comboBox_midi_out.Location = new System.Drawing.Point(115, 52);
            this.comboBox_midi_out.Name = "comboBox_midi_out";
            this.comboBox_midi_out.Size = new System.Drawing.Size(234, 21);
            this.comboBox_midi_out.TabIndex = 4;
            // 
            // midi_out_label
            // 
            this.midi_out_label.AutoSize = true;
            this.midi_out_label.Location = new System.Drawing.Point(6, 55);
            this.midi_out_label.Name = "midi_out_label";
            this.midi_out_label.Size = new System.Drawing.Size(95, 13);
            this.midi_out_label.TabIndex = 3;
            this.midi_out_label.Text = "Midi Device (OUT)";
            this.midi_out_label.Click += new System.EventHandler(this.label2_Click);
            // 
            // midi_in_label
            // 
            this.midi_in_label.AutoSize = true;
            this.midi_in_label.Location = new System.Drawing.Point(6, 24);
            this.midi_in_label.Name = "midi_in_label";
            this.midi_in_label.Size = new System.Drawing.Size(61, 13);
            this.midi_in_label.TabIndex = 2;
            this.midi_in_label.Text = "Device (IN)";
            this.midi_in_label.Click += new System.EventHandler(this.label1_Click);
            // 
            // comboBox_midi_in
            // 
            this.comboBox_midi_in.Cursor = System.Windows.Forms.Cursors.Default;
            this.comboBox_midi_in.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox_midi_in.FormattingEnabled = true;
            this.comboBox_midi_in.Location = new System.Drawing.Point(115, 21);
            this.comboBox_midi_in.Name = "comboBox_midi_in";
            this.comboBox_midi_in.Size = new System.Drawing.Size(234, 21);
            this.comboBox_midi_in.TabIndex = 1;
            this.comboBox_midi_in.SelectedIndexChanged += new System.EventHandler(this.comboBox_midi_in_changed);
            // 
            // btn_start
            // 
            this.btn_start.Location = new System.Drawing.Point(296, 313);
            this.btn_start.Name = "btn_start";
            this.btn_start.Size = new System.Drawing.Size(126, 50);
            this.btn_start.TabIndex = 4;
            this.btn_start.Text = "Connect";
            this.btn_start.UseVisualStyleBackColor = true;
            this.btn_start.Click += new System.EventHandler(this.btn_start_Click);
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(61, 4);
            // 
            // ws_info
            // 
            this.ws_info.BackColor = System.Drawing.SystemColors.Info;
            this.ws_info.Location = new System.Drawing.Point(10, 313);
            this.ws_info.Multiline = true;
            this.ws_info.Name = "ws_info";
            this.ws_info.ReadOnly = true;
            this.ws_info.Size = new System.Drawing.Size(278, 50);
            this.ws_info.TabIndex = 10;
            this.ws_info.Visible = false;
            // 
            // FSettings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(432, 374);
            this.Controls.Add(this.ws_info);
            this.Controls.Add(this.btn_start);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Name = "FSettings";
            this.Text = "OBSMidiRemote";
            this.Load += new System.EventHandler(this.FSettings_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label midi_in_label;
        private System.Windows.Forms.ComboBox comboBox_midi_in;
        private System.Windows.Forms.ComboBox comboBox_midi_out;
        private System.Windows.Forms.Label midi_out_label;
        private System.Windows.Forms.Label midi_scheme_label;
        private System.Windows.Forms.Label server_label;
        private System.Windows.Forms.TextBox text_obsurl;
        private System.Windows.Forms.ComboBox comboBox_midi_srf;
        private System.Windows.Forms.Label obs_info;
        private System.Windows.Forms.LinkLabel linkLabel1;
        private System.Windows.Forms.TextBox text_obspwd;
        private System.Windows.Forms.Label password_label;
        private System.Windows.Forms.Button btn_start;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.LinkLabel linkLabel2;
        private System.Windows.Forms.Button btn_refresh;
        private System.Windows.Forms.TextBox text_obsport;
        private System.Windows.Forms.TextBox midi_map_info;
        private System.Windows.Forms.TextBox ws_info;
    }
}

