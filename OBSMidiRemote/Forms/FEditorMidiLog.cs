using OBSMidiRemote.Lib;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OBSMidiRemote.Forms
{
    public partial class FEditorMidiLog : Form
    {
        private struct LogItem
        {
            public string text;
            public SMidiAction? data;
        }

        private int logstore = 50;
        private LogItem[] logLines;

        public FEditorMidiLog()
        {
            logLines = new LogItem[logstore];
            InitializeComponent();
            FormClosing += WinForm_FormClosing;
            Shown += WinForm_Shown;
            this.Text = "MidiLog";
        }

        public void Log(string line)
        {
            shiftItems();
            LogItem item = new LogItem();
            item.text = line;
            item.data = null;
            logLines[0] = item;
            Render();
        }

        public void Data(SMidiAction d)
        {
            shiftItems();
            LogItem item = new LogItem();
            item.text = null;
            item.data = d;
            logLines[0] = item;
            Render();
        }

       
        public void Render()
        {
            string html = "<html>";
            html += "<style> html,body { margin:0; padding:0; font-family: Verdana,sans-serif; font-size:11pt;  background:black;} TABLE THEAD TD { background:#008DB0; color: white; } TABLE TBODY TD { font-size:11pt; background:black; color:white; border-bottom:#333 1px solid; }";
            html += "TR.noteon TD { background:black; color:#4FB851; } TR.noteoff TD { background:black; color:#2F7165; } TR.control TD { background:black; color:#f0f0f0; } </style>";
            html += "<body><table width='100%' cellspacing='0' cellpadding='5'>";
            html += "<thead><tr><td>CMD</td><td>Channel</td><td>Data1</td><td>Data2</td></tr></thead>";
            html += "<tbody>";

            for (int i = 0; i < logLines.Length; i++)
            {
                if (logLines[i].data != null)
                {

                    html += "<tr class=\"";
                    switch (((SMidiAction)logLines[i].data).Cmd)
                    {
                        case (int)EMidiActionType.NoteOn:
                            html += "noteon";
                            break;
                        case (int)EMidiActionType.NoteOff:
                            html += "noteoff";
                            break;
                        case (int)EMidiActionType.ControlChange:
                            html += "control";
                            break;
                    }


                    html += "\"><td>0x";
                    html += ((SMidiAction)logLines[i].data).Cmd.ToString("X2");
                    html += "<small>("+((SMidiAction)logLines[i].data).Cmd+")</small>";
                    html += "</td><td>0x";
                    html += ((SMidiAction)logLines[i].data).Channel.ToString("X2");
                    html += "<small>(" + ((SMidiAction)logLines[i].data).Channel + ")</small>";
                    html += "</td><td>0x";
                    html += ((SMidiAction)logLines[i].data).Data1.ToString("X2");
                    html += "<small>(" + ((SMidiAction)logLines[i].data).Data1 + ")</small>";
                    html += "</td><td>0x";
                    html += ((SMidiAction)logLines[i].data).Data2.ToString("X2");
                    html += "<small>(" + ((SMidiAction)logLines[i].data).Data2 + ")</small>";
                    html += "</td></tr>";
                }

                if (logLines[i].text != null)
                {
                    html += "<tr><td colspan='4'>" + logLines[i].text + "</td></tr>";
                   
                }
            }

            html += "</tbody></table></body></html>";
            webBrowser1.DocumentText = html;
            webBrowser1.Refresh();
        }

        private void shiftItems()
        {
            for (int i = logLines.Length-1; i >= 0 ; i--)
            {
                if (i-1 >= 0)
                {
                    logLines[i] = logLines[i-1];
                }
            }
        }

        private void WinForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
        }

        private void WinForm_Shown(Object sender, EventArgs e)
        {
            webBrowser1.Refresh();
        }

    }
}
