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
    public partial class hex_picker : Form
    {
        private string option;
        public hex_picker(string option)
        {
            InitializeComponent();
            this.option = option;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            switch(option)
            {
                case "backColor":
                    Form1.refreshBack(ColorTranslator.FromHtml(textBox1.Text));
                    this.Close();
                    break;

                case "textColor":
                    Form1.refreshText(ColorTranslator.FromHtml(textBox1.Text));
                    this.Close();
                    break;
            }

        }

    }
}
