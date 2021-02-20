using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NotepadApplication
{
    static class Program
    {

        [STAThread]
        static void Main(string[] args)
        {
            //MessageBox.Show(args[0]);
            //Console.WriteLine(args[0]);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            Application.Run(new Form1(args));

        }
    }
}
