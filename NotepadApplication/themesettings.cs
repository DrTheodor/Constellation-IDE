using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NotepadApplication
{
    public partial class themesettings : Form
    {
        private bool initialized = false;
        public themesettings(bool theme)
        {
            InitializeComponent();
            checkBox1.Checked = theme;
            checkBox2.Checked = Form1.DiscordRPC;
            checkBox1.BringToFront();
            button1.BringToFront();
            initialized = true;
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (initialized)
            {
                Form1.especiallyRestart = true;
                Form1.changeTheme(checkBox1.Checked);
                Application.Restart();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            if (initialized)
            {
                //Form1.especiallyRestart = true;
                Form1.DiscordRPC = ((CheckBox)sender).Checked;
                Application.Restart();
            }
        }
    }
}
