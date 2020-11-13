using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

namespace GameSaveBackup
{
    public partial class supported_games : Form
    {
        public supported_games()
        {
            InitializeComponent();
        }

        public List<string> supportedGamesList() //get a list of our supported games
        {
            List<string> listRange = new List<string>();
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(System.IO.File.ReadAllText(Form1.xmlFile));
            XmlNodeList nodes = doc.SelectNodes("/backups/game");
            foreach (XmlNode node in nodes)
            {
                listRange.Add(node.Attributes["name"].Value);
            }

            return listRange;
        }

        public List<string> supportedGamesList2() //get a list of our supported games
        {
            List<string> listRange = new List<string>();
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(System.IO.File.ReadAllText(Form1.GOGxmlFile));
            XmlNodeList nodes = doc.SelectNodes("/backups/game");
            foreach (XmlNode node in nodes)
            {
                listRange.Add(node.Attributes["name"].Value);
            }

            return listRange;
        }

        private void supported_games_Load(object sender, EventArgs e)
        {
            foreach (var item in supportedGamesList())
            {
                textBox1.Text += item + System.Environment.NewLine;
            }
            foreach (var item in supportedGamesList2())
            {
                textBox2.Text += item + System.Environment.NewLine;
            }
        }

        private void label1_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("mailto:web@newagesoftware.net?subject=GameSaveBackup%20Support%20Game%20Request&body=Could%20you%20please%20add%20support%20for");
        }
    }
}
