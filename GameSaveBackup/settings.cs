using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Resources;
using System.IO;
using System.Text.RegularExpressions;
using IWshRuntimeLibrary;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Net;

namespace GameSaveBackup
{
    public partial class settings : Form
    {
        public settings()
        {
            InitializeComponent();
        }

        string steamReg;

        /*public string getGOGInstallPath()
        {
            RegistryKey regKey = Registry.LocalMachine;
            regKey = regKey.OpenSubKey(@"SOFTWARE\WOW6432NODE\GOG.com");

            if (regKey != null)
                return regKey.GetValue("DefaultPackPath").ToString();
            else
                return null;
        }

        public string getGOGGalaxyPath()
        {
            RegistryKey regKey = Registry.LocalMachine;
            regKey = regKey.OpenSubKey(@"SOFTWARE\WOW6432NODE\GOG.com\GalaxyClient\paths");

            if (regKey != null)
                return regKey.GetValue("client").ToString();
            else
                return null;
        }*/

        private void settings_Load(object sender, EventArgs e)
        {
            if (Properties.Settings.Default.language == "")
                Properties.Settings.Default.language = "English"; //debug has an issue with loading this.

            autoDL.Checked = Properties.Settings.Default.autoDL;
            miniStartup.Checked = Properties.Settings.Default.miniStartup;
            autoRestore.Checked = Properties.Settings.Default.autoRestore;
            winstartup.Checked = Properties.Settings.Default.winStartup;
            langSelect.Text = Properties.Settings.Default.language;
            maxBackups.Text = Properties.Settings.Default.maxBackups.ToString();
            debugExport.Checked = Properties.Settings.Default.debugExport;
            doubleClick.Checked = Properties.Settings.Default.doubleClick;
            hideNotifications.Checked = Properties.Settings.Default.hideNotifications;

            try {
                RegistryKey regKey = Registry.CurrentUser;
                regKey = regKey.OpenSubKey(@"Software\Valve\Steam");

                if (regKey != null)
                    steamReg = regKey.GetValue("SteamPath").ToString();
            } catch
            {

            }

            //language string loading
            Assembly a = Assembly.Load("GameSaveBackup");
            ResourceManager rm = new ResourceManager("GameSaveBackup.Lang." + Properties.Settings.Default.language, a);
            steamLabel.Text = rm.GetString("steamLabel");
            langLabel.Text = rm.GetString("langLabel");
            autoRestore.Text = rm.GetString("autoRestore");
            miniStartup.Text = rm.GetString("miniStartup");
            winstartup.Text = rm.GetString("winstartup");
            mbLabel.Text = rm.GetString("mbLabel");
            debugExport.Text = rm.GetString("debugExport");
            this.Text = rm.GetString("settings");

            if (Properties.Settings.Default.gogDirs != null)
            {
                foreach (string i in Properties.Settings.Default.gogDirs)
                {
                    if (Directory.Exists(i))
                        gogList.Items.Add(i);
                }
            }
            
            if (steamReg != null)
            {
                if (!Directory.Exists(steamReg.Replace(@"/", @"\") + @"\steamapps\common\"))
                    return;
                steamList.Items.Add(steamReg.Replace(@"/", @"\") + @"\steamapps\common\");
                try {
                    foreach (string line in System.IO.File.ReadLines(steamReg + @"\config\config.vdf"))
                    {
                        if (line.Contains("BaseInstallFolder_"))
                            steamList.Items.Add(getVal(line) + @"\steamapps\common\");
                    }
                }
                catch
                {
                    Debug.WriteLine("Steam config.vdf was not readable at this moment.");
                }
            }
            /*if (getGOGInstallPath() != null)
            {
                gogList.Items.Add(getGOGInstallPath());
                gogList.Items.Add(getGOGGalaxyPath() + @"\Games\");
            }*/
        }

        private string getVal(string s)
        {
            s = Regex.Replace(s, @"\s+", "");
            string[] words = s.Split('"');
            return words[3].Replace(@"\\",@"\");
        }

        bool detectChange() //true if a setting changed
        {
            if (miniStartup.Checked != Properties.Settings.Default.miniStartup)
                return true;
            if (autoDL.Checked != Properties.Settings.Default.autoDL)
                return true;
            if (Properties.Settings.Default.debugExport != debugExport.Checked)
                return true;
            if (Properties.Settings.Default.doubleClick != doubleClick.Checked)
                return true;
            if (Properties.Settings.Default.winStartup != winstartup.Checked)
                return true;
            if (Properties.Settings.Default.autoRestore != autoRestore.Checked)
                return true;
            if (Properties.Settings.Default.language != langSelect.Text)
                return true;
            if (maxBackups.Text == "")
                maxBackups.Text = Properties.Settings.Default.maxBackups.ToString();
            if (Properties.Settings.Default.maxBackups != Convert.ToInt32(maxBackups.Text))
                return true;
            if (Properties.Settings.Default.hideNotifications != hideNotifications.Checked)
                return true;

            // check the list to the save info, and the save info to the list. This ensures we are checking both added and removed information.
            foreach (object i in gogList.Items)
            {
                if (!Properties.Settings.Default.gogDirs.Contains(i.ToString()))
                    return true;
            }
            foreach (string i in Properties.Settings.Default.gogDirs)
            {
                if (!gogList.Items.Contains(i.ToString()))
                    return true;
            }

            return false;
        }

        private void formClosing(object sender, FormClosingEventArgs e)
        {
            bool anythingChanged = detectChange();

            Properties.Settings.Default.miniStartup = miniStartup.Checked;
            Properties.Settings.Default.autoDL = autoDL.Checked;
            Properties.Settings.Default.autoRestore = autoRestore.Checked;
            Properties.Settings.Default.hideNotifications = hideNotifications.Checked;

            if (!winstartup.Checked)
            {
                Properties.Settings.Default.winStartup = false;
                if (IsAutoStartEnabled())
                    UnSetAutoStart();
            }
            else
                Properties.Settings.Default.winStartup = winstartup.Checked;
            
            Properties.Settings.Default.debugExport = debugExport.Checked;
            Properties.Settings.Default.doubleClick = doubleClick.Checked;

            Properties.Settings.Default.language = langSelect.Text;
            Properties.Settings.Default.maxBackups = Convert.ToInt32(maxBackups.Text);

            StringCollection gogListItems = new StringCollection();
            foreach (object i in gogList.Items)
            {
                gogListItems.Add(i.ToString());
            }
            Properties.Settings.Default.gogDirs = gogListItems;

            Properties.Settings.Default.Save();

            if (winstartup.Checked)
            {
                if (IsAutoStartEnabled() == false)
                    SetAutoStart();
            }
            else
            {
                if (IsAutoStartEnabled())
                    UnSetAutoStart();
            }

            if (anythingChanged) // restart application if a setting changed
                Application.Restart();
        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
                e.Handled = true;
        }

        public static void SetAutoStart()
        {
            //RegistryKey key = Registry.CurrentUser.CreateSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run");
            //key.SetValue("SteamCloudSave", "\"" + Application.ExecutablePath.ToString() + "\"");
            try
            {
                string shortcutLocation = Path.Combine(Environment.ExpandEnvironmentVariables(@"%APPDATA%\Microsoft\Windows\Start Menu\Programs\Startup"), "GameSaveBackup" + ".lnk");
                WshShell shell = new WshShell();
                IWshShortcut shortcut = (IWshShortcut)shell.CreateShortcut(shortcutLocation);

                shortcut.Description = "GameSaveBackup startup";               // The description of the shortcut
                shortcut.IconLocation = Application.StartupPath + @"\cloud3.ico";                       // The icon of the shortcut
                shortcut.TargetPath = Application.ExecutablePath;    // The path of the file that will launch when the shortcut is run
                shortcut.Save();
            }
            catch
            {
                MessageBox.Show(@"ERROR: Can't find %appdata%\Microsoft\Windows\Start Menu\Programs\Startup");
            }
        }

        public bool IsAutoStartEnabled()
        {
            /*RegistryKey key = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run");
            if (key == null)
                return false;

            string value = (string)key.GetValue("SteamCloudSave");
            if (value == null)
                return false;*/

            if (!System.IO.File.Exists(@"C:\ProgramData\Microsoft\Windows\Start Menu\Programs\Startup\GameSaveBackup.lnk"))
                return false;

            return true;//(value == "\"" + Application.ExecutablePath.ToString() + "\"");
        }

        public void UnSetAutoStart()
        {
            //RegistryKey key = Registry.CurrentUser.CreateSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run");
            //key.DeleteValue("SteamCloudSave");
            System.IO.File.Delete(@"C:\ProgramData\Microsoft\Windows\Start Menu\Programs\Startup\GameSaveBackup.lnk");
        }

        private void gogRemove_btn_Click(object sender, EventArgs e)
        {
            if (gogList.SelectedItem != null)
                gogList.Items.RemoveAt(gogList.SelectedIndex);
        }

        private void gogAdd_btn_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog folderBrowserDialog1 = new FolderBrowserDialog();
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                string path = folderBrowserDialog1.SelectedPath;
                if (!path.EndsWith(Path.DirectorySeparatorChar.ToString()))
                    path = path + @"\";
                gogList.Items.Add(path);
            }
        }

        private void maxBackups_TextChanged(object sender, EventArgs e)
        {

        }

        private void signUp_btn_Click(object sender, EventArgs e)
        {
            Process.Start("https://patreon.com/newage");
        }

        private void getToken_btn_Click(object sender, EventArgs e)
        {
            Process.Start("https://update.newagesoftware.net/patreon.php");
        }
    }
}
