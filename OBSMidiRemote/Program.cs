using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Drawing;
using OBSMidiRemote.Lib;

namespace OBSMidiRemote
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        /// 

        public static CDeviceObsGw OBSgw;
        private static string deviceMapDir;
        public static FSettings mainForm;
        public static NotifyIcon trayIcon;
        public static System.Resources.ResourceManager res;

        private static string[] languages = new string[] { "en", "hu" };
        public static string lang;

        [STAThread]
        static void Main()
        {
            if (ProcIsRunning())
            {
                Application.Exit();
            }

            var cultureInfo = System.Globalization.CultureInfo.CurrentCulture;
            var found = false;
            foreach (var lng in languages)
            {
                if (lng == cultureInfo.TwoLetterISOLanguageName)
                {
                    found = true;
                    lang = lng;
                    res = new System.Resources.ResourceManager("OBSMidiRemote.lang." + cultureInfo.TwoLetterISOLanguageName, System.Reflection.Assembly.GetExecutingAssembly());
                }
            }

            if (!found)
            {
                lang = "en";
                res = new System.Resources.ResourceManager("OBSMidiRemote.lang.en", System.Reflection.Assembly.GetExecutingAssembly());
           }

           string path = System.Reflection.Assembly.GetExecutingAssembly().Location;
           deviceMapDir = Path.Combine(Application.StartupPath, "devicemaps");
           OBSgw = new CDeviceObsGw();

           Application.EnableVisualStyles();
           Application.SetCompatibleTextRenderingDefault(false);
           AppDomain.CurrentDomain.ProcessExit += new EventHandler(OnProcessExit);

           PrepareTray();

           mainForm = new FSettings();
           Application.Run(mainForm);
        }

        static void PrepareTray()
        {
            trayIcon = new NotifyIcon()
            {
                Text = res.GetString("OBSMidiRemote"),
                ContextMenu = new ContextMenu(new MenuItem[] {
                    new MenuItem( res.GetString("open"),    ShowApp),
                    new MenuItem( res.GetString("exit"),     CloseApp)
                }),
                Visible = true
            };
            trayIcon.DoubleClick += new System.EventHandler(ShowApp);
            ChangeTrayIcon(false);
        }

        public static void ChangeTrayIcon(bool busy)
        {
            trayIcon.Icon = busy ? 
                    new Icon(Path.Combine(Application.StartupPath, "icons", "icon_c.ico")) : new Icon(Path.Combine(Application.StartupPath, "icons", "icon.ico"));
        }

        public static string GetMapsDir()
        {
            return deviceMapDir;
        }

        static void ShowApp(object sender, EventArgs e)
        {
            mainForm.Show();
        }

        static void HideApp()
        {
            mainForm.Hide();
        }

        static void CloseApp(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show(res.GetString("are_you_sure"), res.GetString("exit"), MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
            if (result == DialogResult.Yes) {
                OBSgw.Stop();
                Application.Exit();
            }
        }

        [DllImport("user32.dll")]
        private static extern bool ShowWindowAsync(IntPtr hWnd, int nCmdShow);
        [DllImport("user32.dll")]
        private static extern int SetForegroundWindow(IntPtr hWnd);
        [DllImport("User32.dll")]
        private static extern bool IsIconic(IntPtr handle);

        public static Boolean ProcIsRunning()
        {
            /*
             *  Hide = 0,
            ShowNormal = 1, ShowMinimized = 2, ShowMaximized = 3,
            Maximize = 3, ShowNormalNoActivate = 4, Show = 5,
            Minimize = 6, ShowMinNoActivate = 7, ShowNoActivate = 8,
            Restore = 9, ShowDefault = 10, ForceMinimized = 11 */

            Process curr = Process.GetCurrentProcess();
            Process[] procs = Process.GetProcessesByName(curr.ProcessName);
            foreach (Process p in procs)
            {
                if ((p.Id != curr.Id) &&
                    (p.MainModule.FileName == curr.MainModule.FileName))
                {
                    IntPtr handle = p.MainWindowHandle;
                    ShowWindowAsync(handle, 1);
                    SetForegroundWindow(handle);
                    return true;
                }
            }
            return false;
        }


        static void OnProcessExit(object sender, EventArgs e)
        {

        }
    }
}
