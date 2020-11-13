using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GameSaveBackup
{
    public partial class closeQuestion : Form
    {
        public closeQuestion()
        {
            InitializeComponent();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            foreach (Form childForm in Application.OpenForms)
            {
                childForm.WindowState = FormWindowState.Minimized;
                childForm.Hide();
            }
            Close();
        }

        private void closeQuestion_Load(object sender, EventArgs e)
        {

        }
    }
}
