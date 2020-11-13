using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GameSaveBackup
{

    public partial class storageSource : Form
    {
        private const int EM_SETCUEBANNER = 0x1501;

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern Int32 SendMessage(IntPtr hWnd, int msg, int wParam, [MarshalAs(UnmanagedType.LPWStr)]string lParam);

        public storageSource()
        {
            InitializeComponent();
            SendMessage(hostBox.Handle, EM_SETCUEBANNER, 0, "IP Address, Hostname or Domain");
            SendMessage(userBox.Handle, EM_SETCUEBANNER, 0, "FTP Username");
            SendMessage(passBox.Handle, EM_SETCUEBANNER, 0, "FTP Password");
            SendMessage(dirBox.Handle, EM_SETCUEBANNER, 0, "FTP Directory");
            SendMessage(movePath.Handle, EM_SETCUEBANNER, 0, "Local or Mapped Directory");
            hostBox.Text = Properties.Settings.Default.ftpIP;
            userBox.Text = Properties.Settings.Default.ftpUser;
            passBox.Text = Properties.Settings.Default.ftpPass;
            dirBox.Text = Properties.Settings.Default.ftpDir;
            movePath.Text = Properties.Settings.Default.moveDir;

            System.Windows.Forms.ToolTip ToolTip1 = new System.Windows.Forms.ToolTip();
            ToolTip1.SetToolTip(this.pictureBox1, "Click here to use GoogleDrive as your Game Save Backup server.");
            System.Windows.Forms.ToolTip ToolTip2 = new System.Windows.Forms.ToolTip();
            ToolTip2.SetToolTip(this.pictureBox2, "Click here to use DropBox as your Game Save Backup server.");
            System.Windows.Forms.ToolTip ToolTip3 = new System.Windows.Forms.ToolTip();
            ToolTip3.SetToolTip(this.pictureBox3, "Click here to use OneDrive as your Game Save Backup server.");
            System.Windows.Forms.ToolTip ToolTip4 = new System.Windows.Forms.ToolTip();
            ToolTip4.SetToolTip(this.pictureBox4, "Click here to use FTP as your Game Save Backup server.");
            System.Windows.Forms.ToolTip ToolTip5 = new System.Windows.Forms.ToolTip();
            ToolTip5.SetToolTip(this.pictureBox5, "Click here to use file moving as your Game Save Backup solution.");

            if (Properties.Settings.Default.cloudSource == "googledrive")
            {
                pictureBox1.BorderStyle = BorderStyle.Fixed3D;
                pictureBox1.BackColor = Color.Gainsboro;
                pictureBox1.Cursor = Cursors.No;
                ToolTip1.SetToolTip(this.pictureBox1, "GoogleDrive is your Game Save Backup server.");
            }
            if (Properties.Settings.Default.cloudSource == "dropbox")
            {
                pictureBox2.BorderStyle = BorderStyle.Fixed3D;
                pictureBox2.BackColor = Color.Gainsboro;
                pictureBox2.Cursor = Cursors.No;
                ToolTip2.SetToolTip(this.pictureBox2, "DropBox is your Game Save Backup server.");
            }
            if (Properties.Settings.Default.cloudSource == "onedrive")
            {
                pictureBox3.BorderStyle = BorderStyle.Fixed3D;
                pictureBox3.BackColor = Color.Gainsboro;
                pictureBox3.Cursor = Cursors.No;
                ToolTip3.SetToolTip(this.pictureBox3, "OneDrive is your Game Save Backup server.");
            }
            if (Properties.Settings.Default.cloudSource == "ftp")
            {
                pictureBox4.BorderStyle = BorderStyle.Fixed3D;
                pictureBox4.BackColor = Color.Gainsboro;
                pictureBox4.Cursor = Cursors.No;
                ToolTip4.SetToolTip(this.pictureBox4, "FTP is your Game Save Backup server.");
            }
            if (Properties.Settings.Default.cloudSource == "local")
            {
                pictureBox5.BorderStyle = BorderStyle.Fixed3D;
                pictureBox5.BackColor = Color.Gainsboro;
                pictureBox5.Cursor = Cursors.No;
                ToolTip5.SetToolTip(this.pictureBox5, "Local Storage is your Game Save Backup solution.");
            }
        }

        void savenClose(string source, string host = null, string user = null, string pass = null, string dir = null)
        {
            Properties.Settings.Default.cloudSource = source;
            if (movePath.Text != "")
            {
                Properties.Settings.Default.moveDir = host;
            }
            if (hostBox.Text != "" && userBox.Text != "" && passBox.Text != "" && dirBox.Text != "")
            {
                Properties.Settings.Default.ftpIP = host;
                Properties.Settings.Default.ftpUser = user;
                Properties.Settings.Default.ftpPass = pass;
                Properties.Settings.Default.ftpDir = dir;
            }
            Properties.Settings.Default.Save();
            Application.Restart();
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            savenClose("googledrive");
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            savenClose("dropbox");
        }

        private void pictureBox3_Click(object sender, EventArgs e)
        {
            savenClose("onedrive");
        }

        private void storageSource_Load(object sender, EventArgs e)
        {
            server_label.Text = "Current Server: " + Properties.Settings.Default.cloudSource;
        }

        private void pictureBox4_Click(object sender, EventArgs e)
        {
            if (hostBox.Text == "" || userBox.Text == "" || passBox.Text == "" || dirBox.Text == "")
                MessageBox.Show("Hostname, Username, Password and Directory are all required to use FTP.");
            else
            {
                if (!dirBox.Text.EndsWith("/"))
                    dirBox.Text = dirBox.Text + "/";
                savenClose("ftp", hostBox.Text, userBox.Text, passBox.Text, dirBox.Text);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                movePath.Text = folderBrowserDialog1.SelectedPath;
            }
        }

        private void pictureBox5_Click(object sender, EventArgs e)
        {
            if (movePath.Text == "")
                MessageBox.Show("The folder path cannot be empty. Click the browse button.");
            else
            {
                if (!movePath.Text.EndsWith(@"\"))
                    movePath.Text = movePath.Text + @"\";
                savenClose("local", movePath.Text);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
