using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

//used to compile files
using Microsoft.CSharp;
using System.CodeDom.Compiler;
using System.CodeDom;

//used to modify files(read,write)
using System.IO;
using System.Collections;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using FastColoredTextBoxNS;
using System.Text.RegularExpressions;
using DiscordRPC;
using DiscordRPC.Logging;


namespace NotepadApplication
{
    /*
     *
            i will leave this code in description
            
     */

    public partial class Form1 : Form
    {
        public static Form1 form;
        AutocompleteMenu popupMenu;
        private ColorDialog cd = new ColorDialog();
        private TextStyle greenStyle = new TextStyle(Brushes.DarkSeaGreen, null, FontStyle.Regular);
        AutocompleteMenu popupMenu2;
        public string AppPath = AppContext.BaseDirectory;
        public DiscordRpcClient client;
        private int lastIndex = 0;
        public static bool DiscordRPC = false;
        private List<string> paths = new List<string>();
        private List<string> tbd = new List<string>();
        string[] keywords = { "abstract", "as", "base", "bool", "break", "byte", "case", "catch", "char", "checked", "class", "const", "continue", "decimal", "default", "delegate", "do", "double", "else", "enum", "event", "explicit", "extern", "false", "finally", "fixed", "float", "for", "foreach", "goto", "if", "implicit", "in", "int", "interface", "internal", "is", "lock", "long", "namespace", "new", "null", "object", "operator", "out", "override", "params", "private", "protected", "public", "readonly", "ref", "return", "sbyte", "sealed", "short", "sizeof", "stackalloc", "static", "string", "struct", "switch", "this", "throw", "true", "try", "typeof", "uint", "ulong", "unchecked", "unsafe", "ushort", "using", "virtual", "void", "volatile", "while", "add", "alias", "ascending", "descending", "dynamic", "from", "get", "global", "group", "into", "join", "let", "orderby", "partial", "remove", "select", "set", "value", "var", "where", "yield", "Mathf", "Vector3", "Vector2", "Text", "GameObject", "Image", "Transform", "Object", "Quaternion", "Random", "NavMeshAgent" };
        //string[] methods = { "Equals();", "GetHashCode();", "GetType();", "ToString();", "SetActive(false);" };
        string[] snippets = { "if(^)\n{\n;\n}", "if(^)\n{\n;\n}\nelse\n{\n;\n}", "for(^;;)\n{\n;\n}", "while(^)\n{\n;\n}", "do${\n^;\n}while();", "switch(^)\n{\ncase : break;\n}", "GetComponent<T>();" };
        string[] declarationSnippets = {
               "public class ^\n{\n}", "private class ^\n{\n}", "internal class ^\n{\n}",
               "public struct ^\n{\n;\n}", "private struct ^\n{\n;\n}", "internal struct ^\n{\n;\n}",
               "public void ^()\n{\n;\n}", "private void ^()\n{\n;\n}", "internal void ^()\n{\n;\n}", "protected void ^()\n{\n;\n}",
               "public ^{ get; set; }", "private ^{ get; set; }", "internal ^{ get; set; }", "protected ^{ get; set; }", "private class Class : MonoBehaviour ^\n{\n}"
               };
        static readonly string[] sources = new string[]{
            "UnityEngine",
            "UnityEngine.UI",
            "UnityEngine.AI",
            "Debug",
            "Debug.Log();",
            "Time",
            "Time.deltaTime",
            "com.company.Class3",
            "com.example",
            "com.example.ClassX",
            "com.example.ClassX.Method1",
            "com.example.ClassY",
            "com.example.ClassY.Method1"
        };

        List<KeyValuePair<string, string>> list = new List<KeyValuePair<string, string>>() {
            new KeyValuePair<string, string>("Log", "Write a line to Unity console."),
            new KeyValuePair<string, string>("AddForce", "Applies an impulse force to the GameObject's Rigidbody."),
            new KeyValuePair<string, string>("GameObject", "Base class for all entities in Unity Scenes.")
        };
        //private static Color ThemeColor;

        public Form1(string[] args)
        {
            //try
            //{
                InitializeComponent();
                form = this;

                LoadData();
                LoadSettings();
                this.tabControl1.DrawMode = TabDrawMode.OwnerDrawFixed;

                SetTabHeader(tabControl1.SelectedTab, Color.FromArgb(255, 39, 44, 53));
                tabControl1.DrawMode = TabDrawMode.OwnerDrawFixed;
                tabControl1.SizeMode = TabSizeMode.Fixed;
                tabControl1.ItemSize = new Size(90, 20);
                //tabArea = tabControl1.GetTabRect(0);
                //tabTextArea = (RectangleF)tabControl1.GetTabRect(0);
                //tabControl1..BorderSize = FlatStyle.Flat;
                this.BackColor = ColorTranslator.FromHtml("#434C5B");
                paths.Add("");
                tbd.Add("");
                tabControl1.BackColor = Color.Transparent;
                this.tabControl1.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.tabControl1_DrawItem);
                popupMenu = new AutocompleteMenu(fastColoredTextBox);
                popupMenu.Items.ImageList = imageList1;
                popupMenu.SearchPattern = @"[\w\.:=!<>]";
                popupMenu.AllowTabKey = true;
                //tbd.Add(new TextBoxData { text = fastColoredTextBox.Text, cursor = fastColoredTextBox.Cursor });
                //
                BuildAutocompleteMenu();
                
                //create autocomplete popup menu
                popupMenu2 = new AutocompleteMenu(fastColoredTextBox);
                popupMenu2.SearchPattern = @"[\w\.]";
                popupMenu2.Items.ImageList = imageList1;

            //
                var items = new List<AutocompleteItem>();
                foreach (var item in sources)
                    items.Add(new MethodAutocompleteItem2(item) { ImageIndex = 3 });

                popupMenu2.Items.SetAutocompleteItems(items);

                if(themedSaved)
                {
                    popupMenu.BackColor = Color.FromArgb(255, 39, 44, 53);
                    popupMenu.ForeColor = Color.White;
                    popupMenu.SelectedColor = Color.FromArgb(60, 255, 255, 255);

                    popupMenu2.BackColor = Color.FromArgb(255, 39, 44, 53);
                    popupMenu2.ForeColor = Color.White;
                    popupMenu2.SelectedColor = Color.FromArgb(60, 255, 255, 255);
                }
                fastColoredTextBox.Language = Language.CSharp;
                menuStrip1.Renderer = new MyRenderer();

                try
                {
                    LoadTree(treeView1);
                }
                catch
                {
                    File.Create("SavedState.cfg");
                    LoadTree(treeView1);
                }

                setVisible(false);
                    if (args != null && args.Length == 1)
                    {
                //SaveData();
                        //Console.WriteLine("Args - length! Args: " + args[0] + " exists: " + File.Exists(args[0]));
                        if (File.Exists(args[0]))
                        {
                            //Console.WriteLine("Args - file!");
                            StreamReader sr = new StreamReader(args[0]);
                            tabControl1.TabPages.Add(Path.GetFileName(args[0]));
                            paths.Insert(tabControl1.TabPages.Count - 1, args[0]);
                            tabControl1.SelectedIndex = tabControl1.TabPages.Count - 1;

                            //Console.WriteLine(this.Controls.Find("tabPage1", true)[0]);
                            //this.Controls.Remove(this.Controls.Find("tabPage1", true)[0]);
                            lastIndex = tabControl1.SelectedIndex;
                            //this.Controls.Remove(tabPage1.Con);
                            fastColoredTextBox.Text = sr.ReadToEnd();
                            tbd.Insert(tabControl1.SelectedIndex, fastColoredTextBox.Text);
                            sr.Close();
                            this.Text = args[0] + " — Constellation IDE";
                            if (DiscordRPC)
                            {
                                client.UpdateDetails("Editing " + Path.GetFileName(args[0]));
                            }
                        }
                        else
                        {
                            MessageBox.Show("File not found!");
                        }
                    }
            //} catch (Exception e)
            //{
            //    MessageBox.Show("ERROR: " + e.Message);
            //}
            DocumentMap dm = new DocumentMap();

            
        }

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //fastColoredTextBox.Text = "";
            tabControl1.TabPages.Add("Empty");
            tabControl1.SelectedIndex = tabControl1.TabPages.Count - 1;
            fastColoredTextBox.Text = "";
        }

        //method,to open file
        private void OpenDlg()
        {
            //create new open file dialog
            OpenFileDialog of = new OpenFileDialog();
            //open file dialog files extension filter
            if (fastColoredTextBox.Language == FastColoredTextBoxNS.Language.CSharp)
                of.Filter = "C# File|*.cs|Any File|*.*";
            /*else if (fastColoredTextBox1.Language == FastColoredTextBoxNS.Language.VB)
                of.Filter = "VB File|*.vb|Any File|*.*";
            else if (fastColoredTextBox1.Language == FastColoredTextBoxNS.Language.HTML)
                of.Filter = "HTML File|*.html|Any File|*.*";
            else if (fastColoredTextBox1.Language == FastColoredTextBoxNS.Language.PHP)
                of.Filter = "PHP File|*.php|Any File|*.*";
            else if (fastColoredTextBox1.Language == FastColoredTextBoxNS.Language.XML)
                of.Filter = "XML File|*.xml|Any File|*.*";
            else if (fastColoredTextBox1.Language == FastColoredTextBoxNS.Language.JS)
                of.Filter = "JS File|*.js|Any File|*.*";
            else if (fastColoredTextBox1.Language == FastColoredTextBoxNS.Language.Lua)
                of.Filter = "Lua File|*.lua|Any File|*.*";*/
            else
                of.Filter = "Any File|*.*";
            //if after showing dialog,clicked ok
            if (of.ShowDialog() == DialogResult.OK)
            {
                //open file
                StreamReader sr = new StreamReader(of.FileName);
                //place file text to text box

                //tbd.Insert(tabControl1.SelectedIndex, fastColoredTextBox.Text);
                tabControl1.TabPages.Add(Path.GetFileName(of.FileName));
                paths.Insert(tabControl1.TabPages.Count - 1, of.FileName);
                tabControl1.SelectedIndex = tabControl1.TabPages.Count - 1;
                //tabControl1.TabPages.Add(Path.GetFileName(of.FileName));

                fastColoredTextBox.Text = sr.ReadToEnd();
                tbd.Insert(tabControl1.SelectedIndex, fastColoredTextBox.Text);
                lastIndex = tabControl1.SelectedIndex;
                //close file
                Console.WriteLine(of.FileName);
                sr.Close();
                //text of this window = path of currently opened file
                this.Text = of.FileName + " — Constellation IDE";
                if (DiscordRPC)
                {
                    client.UpdateDetails("Editing " + Path.GetFileName(of.FileName));
                }

                //Console.WriteLine(this.Controls.Find("tabPage1", true)[0]);
                //this.Controls.Remove(this.Controls.Find("tabPage1", true)[0]);


            }
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenDlg();
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Console.WriteLine(this.Text != "Empty*");
            try
            {
                if (tabControl1.SelectedTab.Text != "Empty*")
                {
                    //save file
                    StreamWriter sw = new StreamWriter(paths[tabControl1.SelectedIndex]);
                    sw.Write(fastColoredTextBox.Text);
                    sw.Close();
                    tabControl1.SelectedTab.Text = tabControl1.SelectedTab.Text.Remove(tabControl1.SelectedTab.Text.Length - 1);
                } else
                {
                    SaveDlg();
                }
            }
            catch
            {
                SaveDlg();
            }
        }

        //save file method
        private void SaveDlg()
        {
            //new save file dialog
            SaveFileDialog of = new SaveFileDialog();
            //filter
            if (fastColoredTextBox.Language == FastColoredTextBoxNS.Language.CSharp)
                of.Filter = "C# File|*.cs|Any File|*.*";
            /*else if (fastColoredTextBox1.Language == FastColoredTextBoxNS.Language.VB)
                of.Filter = "VB File|*.vb|Any File|*.*";
            else if (fastColoredTextBox1.Language == FastColoredTextBoxNS.Language.HTML)
                of.Filter = "HTML File|*.html|Any File|*.*";
            else if (fastColoredTextBox1.Language == FastColoredTextBoxNS.Language.PHP)
                of.Filter = "PHP File|*.php|Any File|*.*";
            else if (fastColoredTextBox1.Language == FastColoredTextBoxNS.Language.XML)
                of.Filter = "XML File|*.xml|Any File|*.*";
            else if (fastColoredTextBox1.Language == FastColoredTextBoxNS.Language.JS)
                of.Filter = "JS File|*.js|Any File|*.*";
            else if (fastColoredTextBox1.Language == FastColoredTextBoxNS.Language.Lua)
                of.Filter = "Lua File|*.lua|Any File|*.*";*/
            else
                of.Filter = "Any File|*.*";
            //if after showing dialog,user clicked ok
            if (of.ShowDialog() == DialogResult.OK)
            {
                StreamWriter sr = new StreamWriter(of.FileName);
                sr.Write(fastColoredTextBox.Text);
                sr.Close();
                tabControl1.SelectedTab.Text = Path.GetFileName(of.FileName);
                if (tabControl1.SelectedTab.Text.Contains("*"))
                {
                    tabControl1.SelectedTab.Text = tabControl1.SelectedTab.Text.Remove(tabControl1.SelectedTab.Text.Length - 1);
                }
                this.Text = of.FileName + " — Constellation IDE";
            }
        }

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveDlg();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //close application
            Application.Exit();
        }

        private void cutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            fastColoredTextBox.Cut();
        }

        private void copyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            fastColoredTextBox.Copy();
        }

        private void pasteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            fastColoredTextBox.Paste();
        }

        private void backgroundColorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //if after showing dialog,user clicked ok
            if (cd.ShowDialog()==DialogResult.OK)
            {
                //set background color to text box
                fastColoredTextBox.BackColor = cd.Color;
            }
        }

        private void textColorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //if after showing dialog,user clicked ok
            if (cd.ShowDialog() == DialogResult.OK)
            {
                //set text color to text box
                fastColoredTextBox.ForeColor = cd.Color;
            }
        }

        private void fontToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //new font choosing dialog
            FontDialog fd = new FontDialog();
            //if after showing dialog,user clicked ok
            if (fd.ShowDialog() == DialogResult.OK)
            {
                //set font to text box
                fastColoredTextBox.Font = fd.Font;
            }
            SaveData();
        }

        private void undoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            fastColoredTextBox.Undo();
        }

        private void redoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            fastColoredTextBox.Redo();
        }

        private void selectAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            fastColoredTextBox.SelectAll();
        }

        private void cutToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            fastColoredTextBox.Cut();
        }

        private void copyToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            fastColoredTextBox.Copy();
        }

        private void pastToolStripMenuItem_Click(object sender, EventArgs e)
        {
            fastColoredTextBox.Paste();
        }

        private void findToolStripMenuItem_Click(object sender, EventArgs e)
        {
            fastColoredTextBox.ShowFindDialog();
        }

        private void goToToolStripMenuItem_Click(object sender, EventArgs e)
        {
            fastColoredTextBox.ShowGoToDialog();
        }

        private void replaceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            fastColoredTextBox.ShowReplaceDialog();
        }

        private void cToolStripMenuItem_Click(object sender, EventArgs e)
        {
            fastColoredTextBox.Language = FastColoredTextBoxNS.Language.CSharp;
        }

        /*private void vBToolStripMenuItem_Click(object sender, EventArgs e)
        {
            fastColoredTextBox1.Language = FastColoredTextBoxNS.Language.VB;
        }*/

        private void hTMLToolStripMenuItem_Click(object sender, EventArgs e)
        {
            fastColoredTextBox.Language = FastColoredTextBoxNS.Language.HTML;
        }

        private void pHPToolStripMenuItem_Click(object sender, EventArgs e)
        {
            fastColoredTextBox.Language = FastColoredTextBoxNS.Language.PHP;
        }

        private void jSToolStripMenuItem_Click(object sender, EventArgs e)
        {
            fastColoredTextBox.Language = FastColoredTextBoxNS.Language.JS;
        }

        /*private void sQLToolStripMenuItem_Click(object sender, EventArgs e)
        {
            fastColoredTextBox1.Language = FastColoredTextBoxNS.Language.SQL;
        }

        private void lUAToolStripMenuItem_Click(object sender, EventArgs e)
        {
            fastColoredTextBox1.Language = FastColoredTextBoxNS.Language.Lua;
        }*/

        private void xMLToolStripMenuItem_Click(object sender, EventArgs e)
        {
            fastColoredTextBox.Language = FastColoredTextBoxNS.Language.XML;
        }

        private void runToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if(fastColoredTextBox.Language== FastColoredTextBoxNS.Language.HTML) //if language is html
            {
                HTMLPreview h = new HTMLPreview(fastColoredTextBox.Text);
                h.Show();
            }
            else if(fastColoredTextBox.Language == FastColoredTextBoxNS.Language.CSharp) //if language is c#
            {
                SaveFileDialog sf = new SaveFileDialog();
                sf.Filter = "Executable File|*.exe";
                string OutPath = "?.exe";
                if(sf.ShowDialog() == DialogResult.OK)
                {
                    OutPath = sf.FileName;
                    //compile code:
                    //create c# code compiler
                    CSharpCodeProvider codeProvider = new CSharpCodeProvider();
                    //create new parameters for compilation and add references(libs) to compiled app
                    CompilerParameters parameters = new CompilerParameters(new string[] { "System.dll" });
                    //is compiled code will be executable?(.exe)
                    parameters.GenerateExecutable = true;
                    //output path
                    parameters.OutputAssembly = OutPath;
                    //code sources to compile
                    string[] sources = { fastColoredTextBox.Text };
                    //results of compilation
                    CompilerResults results = codeProvider.CompileAssemblyFromSource(parameters, sources);

                    //if has errors
                    if (results.Errors.Count > 0)
                    {
                        string errsText = "";
                        foreach (CompilerError CompErr in results.Errors)
                        {
                            errsText = "(" + CompErr.ErrorNumber +
                                        ")Line " + CompErr.Line +
                                        ",Column " + CompErr.Column +
                                        ":" + CompErr.ErrorText + "" +
                                        Environment.NewLine;
                        }
                        //show error message
                        MessageBox.Show(errsText, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else
                    {
                        //run compiled app
                        System.Diagnostics.Process.Start(OutPath);
                    }
                }
            }
            else
            {
                MessageBox.Show("Cannot run this file");
            }
        }

        private void colorDialogToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //if after showing dialog,user clicked ok
            if (cd.ShowDialog() == DialogResult.OK)
            {
                //set background color to text box
                fastColoredTextBox.BackColor = cd.Color;
            }
        }

        private void hexToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //UserControl1 us = new UserControl1();
            //us.Show();
            hex_picker hexPick = new hex_picker("backColor");
            hexPick.Show();
        }

        private void colorPickerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //new color choosing dialog
            //if after showing dialog,user clicked ok
            if (cd.ShowDialog() == DialogResult.OK)
            {
                //set text color to text box
                fastColoredTextBox.ForeColor = cd.Color;
            }
        }

        private void hexToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            hex_picker hexPick = new hex_picker("textColor");
            hexPick.Show();
        }

        public static void refreshBack(Color arg0)
        {
            form.fastColoredTextBox.BackColor = arg0;
            form.fastColoredTextBox.Refresh();
            SaveData();
        }

        public static void refreshText(Color arg0)
        {
            form.fastColoredTextBox.ForeColor = arg0;
            form.fastColoredTextBox.Refresh();
            SaveData();
        }


        public static void SaveData()
        {
            // Create a hashtable of values that will eventually be serialized.
            Hashtable addresses = new Hashtable();
            addresses.Add("backColor", form.fastColoredTextBox.BackColor);
            addresses.Add("textColor", form.fastColoredTextBox.ForeColor);
            addresses.Add("font", form.fastColoredTextBox.Font);
            //addresses.Add("basePath", form.BasePath);
            //addresses.Add("isPrjVisible", form.treeView1.Visible);
            //addresses.Add("discord", Form1.DiscordRPC);
            // To serialize the hashtable and its key/value pairs,   
            // you must first open a stream for writing.  
            // In this case, use a file stream.
            FileStream fs = new FileStream(form.AppPath + @"\Data\DataFile.cfg", FileMode.Create);

            // Construct a BinaryFormatter and use it to serialize the data to the stream.
            BinaryFormatter formatter = new BinaryFormatter();
            try
            {
                formatter.Serialize(fs, addresses);
            }
            catch (SerializationException e)
            {
                Console.WriteLine("Failed to serialize. Reason: " + e.Message);
                throw;
            }
            finally
            {
                fs.Close();
            }
        }

        public static void SaveSettings()
        {
            // Create a hashtable of values that will eventually be serialized.
            Hashtable addresses = new Hashtable();
            addresses.Add("basePath", form.BasePath);
            addresses.Add("isPrjVisible", form.treeView1.Visible);
            addresses.Add("discord", Form1.DiscordRPC);
            // To serialize the hashtable and its key/value pairs,   
            // you must first open a stream for writing.  
            // In this case, use a file stream.
            FileStream fs = new FileStream(form.AppPath + @"\Data\SettingsFile.cfg", FileMode.Create);

            // Construct a BinaryFormatter and use it to serialize the data to the stream.
            BinaryFormatter formatter = new BinaryFormatter();
            try
            {
                formatter.Serialize(fs, addresses);
            }
            catch (SerializationException e)
            {
                Console.WriteLine("Failed to serialize. Reason: " + e.Message);
                throw;
            }
            finally
            {
                fs.Close();
            }
        }

        public static void SaveTree(TreeView tree)
        {
            using (Stream file = File.Open(form.AppPath + @"\Data\SavedState.cfg", FileMode.Create))
            {
                BinaryFormatter bf = new BinaryFormatter();
                //bf.Serialize(file, form.BasePath);
                bf.Serialize(file, tree.Nodes.Cast<TreeNode>().ToList());
            }
        }

        public static void LoadData()
        {
            // Declare the hashtable reference.
            Hashtable addresses = null;

            FileStream fs = new FileStream(form.AppPath + @"\Data\DataFile.cfg", FileMode.Open);


            try
            {
                BinaryFormatter formatter = new BinaryFormatter();

                // Deserialize the hashtable from the file and  
                // assign the reference to the local variable.
                addresses = (Hashtable)formatter.Deserialize(fs);
            }
            catch (SerializationException e)
            {
                Console.WriteLine("Failed to deserialize. Reason: " + e.Message);
                //throw;
            }
            finally
            {
                fs.Close();
            }

            // To prove that the table deserialized correctly,  
            // display the key/value pairs. 
            if (addresses != null)
            {
                foreach (DictionaryEntry de in addresses)
                {
                    Console.WriteLine("{0} lives at {1}.", de.Key, de.Value);
                    switch (de.Key)
                    {
                        case "backColor":
                            Console.WriteLine(de.Value);
                            form.fastColoredTextBox.BackColor = (Color)de.Value;
                            break;

                        case "textColor":
                            Console.WriteLine(de.Value);
                            form.fastColoredTextBox.ForeColor = (Color)de.Value;
                            break;

                        case "font":
                            Console.WriteLine(de.Value);
                            form.fastColoredTextBox.Font = (Font)de.Value;
                            break;

                        /*case "basePath":
                            form.BasePath = (string)de.Value;
                            break;

                        case "isPrjVisible":
                            form.treeView1.Visible = (bool)de.Value;
                            break;

                        case "discord":
                            Console.WriteLine("DiscordRPC is " + (bool)de.Value);
                            Form1.DiscordRPC = (bool)de.Value;
                            if (Form1.DiscordRPC)
                            {
                                form.Initialize();
                            }
                            break;*/
                    }
                }
                if (form.fastColoredTextBox.BackColor == Color.FromArgb(255, 39, 44, 53)) {
                    DarkTheme = true;
                    UpdateThemeColors(true);
                } else
                {
                    form.menuStrip1.BackColor = SystemColors.Control;
                }
            }
        }

        public static void LoadSettings()
        {
            // Declare the hashtable reference.
            Hashtable addresses = null;

            FileStream fs = new FileStream(form.AppPath + @"\Data\SettingsFile.cfg", FileMode.Open);
            

            try
            {
                BinaryFormatter formatter = new BinaryFormatter();

                // Deserialize the hashtable from the file and  
                // assign the reference to the local variable.
                addresses = (Hashtable)formatter.Deserialize(fs);
            }
            catch (SerializationException e)
            {
                Console.WriteLine("Failed to deserialize. Reason: " + e.Message);
                //throw;
            }
            finally
            {
                fs.Close();
            }

            // To prove that the table deserialized correctly,  
            // display the key/value pairs. 
            if (addresses != null)
            {
                foreach (DictionaryEntry de in addresses)
                {
                    Console.WriteLine("{0} lives at {1}.", de.Key, de.Value);
                    switch (de.Key)
                    {
                        case "basePath":
                            form.BasePath = (string)de.Value;
                            break;

                        case "isPrjVisible":
                            form.treeView1.Visible = (bool)de.Value;
                            break;

                        case "discord":
                            Console.WriteLine("DiscordRPC is " + (bool)de.Value);
                            Form1.DiscordRPC = (bool)de.Value;
                            if (Form1.DiscordRPC)
                            {
                                form.Initialize();
                            }
                            break;
                    }
                }
            }
        }

        public static void LoadTree(TreeView tree)
        {
            try
            {
                using (Stream file = File.Open(form.AppPath + @"\Data\SavedState.cfg", FileMode.Open))
                {
                    BinaryFormatter bf = new BinaryFormatter();
                    object obj = bf.Deserialize(file);

                    TreeNode[] nodeList = (obj as IEnumerable<TreeNode>).ToArray();
                    tree.Nodes.AddRange(nodeList);

                }
            } catch
            {

            }
        }


        private void BuildAutocompleteMenu()
        {
            List<AutocompleteItem> items = new List<AutocompleteItem>();

            foreach (var item in snippets)
                items.Add(new SnippetAutocompleteItem(item) { ImageIndex = 1 });
            foreach (var item in declarationSnippets)
                items.Add(new DeclarationSnippet(item) { ImageIndex = 0 });
            //foreach (var item in methods)
                //items.Add(new MethodAutocompleteItem(item) { ImageIndex = 2 });
            foreach (var item in keywords)
                items.Add(new AutocompleteItem(item) { ImageIndex = 4 });

            items.Add(new InsertSpaceSnippet());
            items.Add(new InsertSpaceSnippet(@"^(\w+)([=<>!:]+)(\w+)$"));
            items.Add(new InsertSpaceSnippet());

            //set as autocomplete source
            popupMenu.Items.SetAutocompleteItems(items);
        }

        class DeclarationSnippet : SnippetAutocompleteItem
        {
            public DeclarationSnippet(string snippet)
                : base(snippet)
            {
            }

            public override CompareResult Compare(string fragmentText)
            {
                var pattern = Regex.Escape(fragmentText);
                if (Regex.IsMatch(Text, "\\b" + pattern, RegexOptions.IgnoreCase))
                    return CompareResult.Visible;
                return CompareResult.Hidden;
            }
        }

        public class MethodAutocompleteItem2 : MethodAutocompleteItem
        {
            string firstPart;
            string lastPart;

            public MethodAutocompleteItem2(string text)
                : base(text)
            {
                var i = text.LastIndexOf('.');
                if (i < 0)
                    firstPart = text;
                else
                {
                    firstPart = text.Substring(0, i);
                    lastPart = text.Substring(i + 1);
                }
            }

            public override CompareResult Compare(string fragmentText)
            {
                int i = fragmentText.LastIndexOf('.');

                if (i < 0)
                {
                    if (firstPart.StartsWith(fragmentText) && string.IsNullOrEmpty(lastPart))
                        return CompareResult.VisibleAndSelected;
                    //if (firstPart.ToLower().Contains(fragmentText.ToLower()))
                    //  return CompareResult.Visible;
                }
                else
                {
                    var fragmentFirstPart = fragmentText.Substring(0, i);
                    var fragmentLastPart = fragmentText.Substring(i + 1);


                    if (firstPart != fragmentFirstPart)
                        return CompareResult.Hidden;

                    if (lastPart != null && lastPart.StartsWith(fragmentLastPart))
                        return CompareResult.VisibleAndSelected;

                    if (lastPart != null && lastPart.ToLower().Contains(fragmentLastPart.ToLower()))
                        return CompareResult.Visible;

                }

                return CompareResult.Hidden;
            }

            public override string GetTextForReplace()
            {
                if (lastPart == null)
                    return firstPart;

                return firstPart + "." + lastPart;
            }

            public override string ToString()
            {
                if (lastPart == null)
                    return firstPart;

                return lastPart;
            }
        }


        class InsertSpaceSnippet : AutocompleteItem
        {
            string pattern;

            public InsertSpaceSnippet(string pattern) : base("")
            {
                this.pattern = pattern;
            }

            public InsertSpaceSnippet()
                : this(@"^(\d+)([a-zA-Z_]+)(\d*)$")
            {
            }

            public override CompareResult Compare(string fragmentText)
            {
                if (Regex.IsMatch(fragmentText, pattern))
                {
                    Text = InsertSpaces(fragmentText);
                    if (Text != fragmentText)
                        return CompareResult.Visible;
                }
                return CompareResult.Hidden;
            }

            public string InsertSpaces(string fragment)
            {
                var m = Regex.Match(fragment, pattern);
                if (m == null)
                    return fragment;
                if (m.Groups[1].Value == "" && m.Groups[3].Value == "")
                    return fragment;
                return (m.Groups[1].Value + " " + m.Groups[2].Value + " " + m.Groups[3].Value).Trim();
            }

            public override string ToolTipTitle
            {
                get
                {
                    return Text;
                }
            }
        }


        class InsertEnterSnippet : AutocompleteItem
        {
            Place enterPlace = Place.Empty;

            public InsertEnterSnippet()
                : base("[Line break]")
            {
            }

            public override CompareResult Compare(string fragmentText)
            {
                var r = Parent.Fragment.Clone();
                while (r.Start.iChar > 0)
                {
                    if (r.CharBeforeStart == '}')
                    {
                        enterPlace = r.Start;
                        return CompareResult.Visible;
                    }

                    r.GoLeftThroughFolded();
                }

                return CompareResult.Hidden;
            }

            public override string GetTextForReplace()
            {
                //extend range
                Range r = Parent.Fragment;
                Place end = r.End;
                r.Start = enterPlace;
                r.End = r.End;
                //insert line break
                return Environment.NewLine + r.Text;
            }

            public override void OnSelected(AutocompleteMenu popupMenu, SelectedEventArgs e)
            {
                base.OnSelected(popupMenu, e);
                if (Parent.Fragment.tb.AutoIndent)
                    Parent.Fragment.tb.DoAutoIndent();
            }

            public override string ToolTipTitle
            {
                get
                {
                    return "Insert line break after '}'";
                }
            }
        }



        private class MyRenderer : ToolStripProfessionalRenderer
        {
            public MyRenderer() : base(new MyColors()) { }
        }


        private void aboutToolStripMenuItem_MouseEnter(object sender, EventArgs e)
        {
            ToolStripMenuItem TSMI = sender as ToolStripMenuItem;
            TSMI.BackColor = Color.Black;
        }

        private void aboutToolStripMenuItem_MouseLeave(object sender, EventArgs e)
        {
            ToolStripMenuItem TSMI = sender as ToolStripMenuItem;
            TSMI.BackColor = Color.White;
        }


        private class MyColors : ProfessionalColorTable
        {
            public override Color MenuItemSelected
            {
                get { return Color.FromArgb(60, 255, 255, 255); }
            }
            public override Color MenuItemSelectedGradientBegin
            {
                get { return Color.FromArgb(60, 255, 255, 255); }
            }
            public override Color MenuItemSelectedGradientEnd
            {
                get { return Color.FromArgb(60, 255, 255, 255); }
            }

            public override Color ToolStripGradientBegin
            { get { return Color.FromArgb(60, 255, 255, 255); } }

            public override Color ToolStripGradientMiddle
            { get { return Color.FromArgb(60, 255, 255, 255); } }

            public override Color ToolStripGradientEnd
            { get { return Color.FromArgb(60, 255, 255, 255); } }

            public override Color MenuStripGradientBegin
            { get { return Color.FromArgb(60, 255, 255, 255); } }

            public override Color MenuStripGradientEnd
            { get { return Color.FromArgb(60, 255, 255, 255); } }

        }

        private void settingsToolStripMenuItem_DropDownOpened(object sender, EventArgs e)
        {
            if (DarkTheme)
            {
                settingsToolStripMenuItem.ForeColor = Color.Black;
            }
        }
        private void settingsToolStripMenuItem_DropDownClosed(object sender, EventArgs e)
        {
            if (DarkTheme)
            {
                settingsToolStripMenuItem.ForeColor = Color.White;
            }
        }


        private void fileToolStripMenuItem_DropDownOpened(object sender, EventArgs e)
        {
            if (DarkTheme)
            {
                fileToolStripMenuItem.ForeColor = Color.Black;
            }
        }
        private void fileToolStripMenuItem_DropDownClosed(object sender, EventArgs e)
        {
            if (DarkTheme)
            {
                fileToolStripMenuItem.ForeColor = Color.White;
            }
        }

        private void editToolStripMenuItem_DropDownOpened(object sender, EventArgs e)
        {
            if (DarkTheme)
            {
                editToolStripMenuItem.ForeColor = Color.Black;
            }
        }
        private void editToolStripMenuItem_DropDownClosed(object sender, EventArgs e)
        {
                if (DarkTheme)
                {
                    editToolStripMenuItem.ForeColor = Color.White;
                }
        }

        private void langToolStripMenuItem_DropDownOpened(object sender, EventArgs e)
        {
                    if (DarkTheme)
                    {
                        languageToolStripMenuItem.ForeColor = Color.Black;
                    }
        }
        private void langToolStripMenuItem_DropDownClosed(object sender, EventArgs e)
        {
                        if (DarkTheme)
                        {
                            languageToolStripMenuItem.ForeColor = Color.White;
                        }
        }

        void Initialize()
        {
            /*
            Create a Discord client
            NOTE: 	If you are using Unity3D, you must use the full constructor and define
                     the pipe connection.
            */
            Console.WriteLine("Discord RPC is initializing!");

            client = new DiscordRpcClient("809118165111013376");

            //Set the logger
            client.Logger = new ConsoleLogger() { Level = LogLevel.Warning };

            //Subscribe to events
            client.OnReady += (sender, e) =>
            {
                Console.WriteLine("Received Ready from user {0}", e.User.Username);
            };

            client.OnPresenceUpdate += (sender, e) =>
            {
                Console.WriteLine("Received Update! {0}", e.Presence);
            };

            //Connect to the RPC
            client.Initialize();

            //Set the rich presence
            //Call this as many times as you want and anywhere in your code.
            client.SetPresence(new RichPresence()
            {
                Details = "Just empty file",
                State = "",
                Assets = new Assets()
                {
                    LargeImageKey = "image_large",
                    //LargeImageText = "Testing",
                    SmallImageKey = "image_small"
                }
            });
        }


        private void fastColoredTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

            e.ChangedRange.ClearStyle(greenStyle);
            //highlight tags
            e.ChangedRange.SetStyle(greenStyle, "<[^>]+>");
        }



        private int howMuchTime = 0;
        private void tabControl1_Selected(object sender, TabControlEventArgs e)
        {
            //tabControl1.TabPages[tabControl1.SelectedIndex].Text;
            if (tabControl1.TabPages.Count != 0)
            {
                if (tabControl1.TabPages[tabControl1.SelectedIndex].Text != "Empty" || tabControl1.TabPages[tabControl1.SelectedIndex].Text != "Empty*")
                {
                    try
                    {
                        if (!paths[tabControl1.SelectedIndex].Equals(""))
                        {
                            this.Text = paths[tabControl1.SelectedIndex] + " — Constellation IDE";
                        } else
                        {
                            this.Text = "Empty File — Constellation IDE";
                        }
                    } catch
                    {
                        this.Text = "Empty File — Constellation IDE";
                    }

                }
                else
                {
                    this.Text = "Empty File — Constellation IDE";
                }
                //tbd.Insert(lastIndex, fastColoredTextBox.Text);

                //Console.WriteLine(fastColoredTextBox.Text);
                    //Console.WriteLine(tbd);

                try
                {
                    //Console.WriteLine(lastIndex);
                    if (!tbd[lastIndex].Contains(fastColoredTextBox.Text) && notDeleting)
                    {
                        Console.WriteLine("Inserted!");
                        try
                        {
                            tbd.RemoveAt(lastIndex);
                        } catch {

                        }
                        tbd.Insert(lastIndex, fastColoredTextBox.Text);

                    } else
                    {
                        if(howMuchTime == 0)
                        {
                            //Console.WriteLine("Inserted!");
                            tbd.Insert(lastIndex, fastColoredTextBox.Text);
                            notDeleting = true;
                            howMuchTime++;
                        }
                        notDeleting = true;
                    }
                    fastColoredTextBox.Text = tbd[tabControl1.SelectedIndex];
                }
                catch
                {
                    fastColoredTextBox.Text = "";
                }
                lastIndex = tabControl1.SelectedIndex;
            }
        }

        private void fastColoredTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!tabControl1.SelectedTab.Text.Contains("*"))
            {
                tabControl1.SelectedTab.Text = tabControl1.SelectedTab.Text + "*";
            }
        }

        private Dictionary<TabPage, Color> TabColors = new Dictionary<TabPage, Color>();
        //private Rectangle tabArea;
        //private RectangleF tabTextArea;
        private string BasePath;
        private static bool DarkTheme = false;
        public static bool especiallyRestart = false;

        private void SetTabHeader(TabPage page, Color color)
        {
            TabColors[page] = color;
            tabControl1.Invalidate();
        }

        private void tabControl1_DrawItem(object sender, DrawItemEventArgs e)
        {
            Color bckColor;
            if(DarkTheme)
            {
                bckColor = Color.FromArgb(255, 39, 44, 53);
            } else
            {
                bckColor = SystemColors.Control;
            }
            using (Brush br = new SolidBrush(bckColor))
            {
                Graphics g = e.Graphics;
                Pen p = new Pen(Color.FromArgb(255, 105, 119, 142), 4);
                if (tabControl1.TabCount != 0)
                {
                    g.DrawRectangle(p, tabControl1.TabPages[tabControl1.SelectedIndex].Bounds);
                }

                tabControl1.Padding = new Point(0);
                //tabControl1.Appearance = TabAppearance.FlatButtons;

                e.Graphics.FillRectangle(br, e.Bounds);
                SizeF sz = e.Graphics.MeasureString(tabControl1.TabPages[e.Index].Text, e.Font);
                SolidBrush fillbrush;
                if (DarkTheme)
                {
                    e.Graphics.DrawString(tabControl1.TabPages[e.Index].Text, e.Font, Brushes.White, e.Bounds.Left + (e.Bounds.Width - sz.Width) / 2, e.Bounds.Top + (e.Bounds.Height - sz.Height) / 2 + 1);
                    fillbrush = new SolidBrush(Color.FromArgb(255, 39, 44, 53));
                } else
                {
                    e.Graphics.DrawString(tabControl1.TabPages[e.Index].Text, e.Font, Brushes.Black, e.Bounds.Left + (e.Bounds.Width - sz.Width) / 2, e.Bounds.Top + (e.Bounds.Height - sz.Height) / 2 + 1);
                    fillbrush = new SolidBrush(SystemColors.Control);
                }



                //draw rectangle behind the tabs
                Rectangle lasttabrect = tabControl1.GetTabRect(tabControl1.TabPages.Count - 1);
                Rectangle background = new Rectangle();
                background.Location = new Point(lasttabrect.Right, 0);

                //pad the rectangle to cover the 1 pixel line between the top of the tabpage and the start of the tabs
                background.Size = new Size(tabControl1.Right - background.Left, lasttabrect.Height + 10);
                e.Graphics.FillRectangle(fillbrush, background);



                Rectangle rect = e.Bounds;
                rect.Offset(0, 1);
                rect.Inflate(0, -1);
                //DarkGray
                e.Graphics.DrawRectangle(new Pen(Color.FromArgb(255, 39, 44, 53)), rect);
                e.DrawFocusRectangle();

                /*Graphics g = e.Graphics;
                Pen p = new Pen(Color.FromArgb(255, 39, 44, 53));
                Font font = new Font("Arial", 10.0f);
                SolidBrush brush = new SolidBrush(Color.Red);

                g.DrawRectangle(p, tabArea);
                g.DrawString("tabPage1", font, brush, tabTextArea);*/
            }
            TabPage CurrentTab = tabControl1.TabPages[e.Index];
            Rectangle ItemRect = tabControl1.GetTabRect(e.Index);
            SolidBrush FillBrush;
            SolidBrush TextBrush;
            if (DarkTheme)
            {
                FillBrush = new SolidBrush(Color.FromArgb(255, 39, 44, 53));
                TextBrush = new SolidBrush(Color.White);
            } else
            {
                FillBrush = new SolidBrush(SystemColors.Control);
                TextBrush = new SolidBrush(Color.Black);
            }
            StringFormat sf = new StringFormat();
            sf.Alignment = StringAlignment.Center;
            sf.LineAlignment = StringAlignment.Center;

            //If we are currently painting the Selected TabItem we'll
            //change the brush colors and inflate the rectangle.
            if (System.Convert.ToBoolean(e.State & DrawItemState.Selected))
            {
                //FillBrush.Color = Color.White;
                //TextBrush.Color = Color.Red;
                ItemRect.Inflate(2, 2);
            }

            //Set up rotation for left and right aligned tabs
            if (tabControl1.Alignment == TabAlignment.Left || tabControl1.Alignment == TabAlignment.Right)
            {
                float RotateAngle = 90;
                if (tabControl1.Alignment == TabAlignment.Left)
                    RotateAngle = 270;
                PointF cp = new PointF(ItemRect.Left + (ItemRect.Width / 2), ItemRect.Top + (ItemRect.Height / 2));
            e.Graphics.TranslateTransform(cp.X, cp.Y);
                e.Graphics.RotateTransform(RotateAngle);
                ItemRect = new Rectangle(-(ItemRect.Height / 2), -(ItemRect.Width / 2), ItemRect.Height, ItemRect.Width);
            }

            //Next we'll paint the TabItem with our Fill Brush
            e.Graphics.FillRectangle(FillBrush, ItemRect);

            //Now draw the text.
            e.Graphics.DrawString(CurrentTab.Text, e.Font, TextBrush, (RectangleF)ItemRect, sf);

            //Reset any Graphics rotation
            e.Graphics.ResetTransform();

            //Finally, we should Dispose of our brushes.
            FillBrush.Dispose();
            TextBrush.Dispose();


        }

        /*protected override void OnPaintBackground(PaintEventArgs e)
        {
            base.OnPaintBackground(e);
            Rectangle lasttabrect = tabControl1.GetTabRect(tabControl1.TabPages.Count - 1);
            RectangleF emptyspacerect = new RectangleF(
                    lasttabrect.X + lasttabrect.Width + tabControl1.Left,
                    tabControl1.Top + lasttabrect.Y,
                    tabControl1.Width - (lasttabrect.X + lasttabrect.Width),
                    lasttabrect.Height);

            Brush b = Brushes.BlueViolet; // the color you want
            e.Graphics.FillRectangle(b, emptyspacerect);
        }*/

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            base.OnPaintBackground(e);

                Rectangle lasttabrect = tabControl1.GetTabRect(tabControl1.TabPages.Count - 1);
                RectangleF emptyspacerect = new RectangleF(
                        lasttabrect.X + lasttabrect.Width + tabControl1.Left,
                        tabControl1.Top + lasttabrect.Y,
                        tabControl1.Width - (lasttabrect.X + lasttabrect.Width),
                        lasttabrect.Height);
                Brush b = new SolidBrush(Color.FromArgb(255, 39, 44, 53)); // the color you want
                e.Graphics.FillRectangle(b, emptyspacerect);
            
            //base.OnPaint(e);
            //e.Graphics.DrawRectangle(new Pen(this.BackColor, 5), this.ClientRectangle);
        }

        private void showProjectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (treeView1.Visible)
            {
                treeView1.Visible = false;
                //fastColoredTextBox.Location = new Point(0, 44);
                fastColoredTextBox.Location = new Point(0, 42);
                //fastColoredTextBox.Size = new Size(980, 410);
                fastColoredTextBox.Size = new Size(870, 410);
                fastColoredTextBox.BringToFront();
                documentMap1.BringToFront();
            } else
            {
                treeView1.Visible = true;
                //fastColoredTextBox.Location = new Point(182, 44);
                fastColoredTextBox.Location = new Point(182, 42);
                //fastColoredTextBox.Size = new Size(798, 410);
                fastColoredTextBox.Size = new Size(690, 410);
                fastColoredTextBox.BringToFront();
                documentMap1.BringToFront();
            }
        }

        private static void setVisible(bool vis)
        {
            if (vis)
            {
                form.treeView1.Visible = true;
                //form.fastColoredTextBox.Location = new Point(182, 35);
                form.fastColoredTextBox.Location = new Point(182, 35);
                //form.fastColoredTextBox.Size = new Size(798, 410);
                form.fastColoredTextBox.Size = new Size(690, 410);
                form.fastColoredTextBox.BringToFront();
                form.documentMap1.BringToFront();
            }
            else
            {
                form.treeView1.Visible = false;
                //form.fastColoredTextBox.Location = new Point(0, 42);
                form.fastColoredTextBox.Location = new Point(0, 42);
                //form.fastColoredTextBox.Size = new Size(980, 410);
                form.fastColoredTextBox.Size = new Size(870, 410);
                form.fastColoredTextBox.BringToFront();
                form.documentMap1.BringToFront();
            }
        }

        private void openProjectToolStripMenuItem_Click(object sender, EventArgs e)
        {

            FolderBrowserDialog fbd = new FolderBrowserDialog();
            if(fbd.ShowDialog() == DialogResult.OK)
            {
                setVisible(true);
                treeView1.Nodes.Clear();

                BasePath = fbd.SelectedPath;
                var stack = new Stack<TreeNode>();
                var rootDirectory = new DirectoryInfo(BasePath);
                var node = new TreeNode(rootDirectory.Name) { Tag = rootDirectory };
                stack.Push(node);

                while (stack.Count > 0)
                {
                    var currentNode = stack.Pop();
                    var directoryInfo = (DirectoryInfo)currentNode.Tag;
                    foreach (var directory in directoryInfo.GetDirectories())
                    {
                        var childDirectoryNode = new TreeNode(directory.Name) { Tag = directory };
                        currentNode.Nodes.Add(childDirectoryNode);
                        stack.Push(childDirectoryNode);
                    }
                    foreach (var file in directoryInfo.GetFiles())
                        currentNode.Nodes.Add(new TreeNode(file.Name));
                }

                treeView1.Nodes.Add(node);
            }


        }




        private void treeView1_MouseMove(object sender, MouseEventArgs e)
        {

           // Get the node at the current mouse pointer location.  
            TreeNode theNode = this.treeView1.GetNodeAt(e.X, e.Y);

            // Set a ToolTip only if the mouse pointer is actually paused on a node.  
            if (theNode != null && theNode.Tag != null)
            {
                // Change the ToolTip only if the pointer moved to a new node.  
                if (theNode.Tag.ToString() != this.toolTip1.GetToolTip(this.treeView1))
                    this.toolTip1.SetToolTip(this.treeView1, theNode.Tag.ToString());

            }
            else     // Pointer is not over a node so clear the ToolTip.  
            {
                this.toolTip1.SetToolTip(this.treeView1, "");
            }
        }

        private void treeView1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            string pathtofile = Directory.GetParent(BasePath) + @"\" + ((TreeView)sender).SelectedNode.FullPath;
            Console.WriteLine(pathtofile);

            if (File.Exists(pathtofile)) {
                StreamReader sr = new StreamReader(pathtofile);
                tabControl1.TabPages.Add(Path.GetFileName(pathtofile));
                paths.Insert(tabControl1.TabPages.Count - 1, pathtofile);
                tabControl1.SelectedIndex = tabControl1.TabPages.Count - 1;
                fastColoredTextBox.Text = sr.ReadToEnd();
                sr.Close();
                this.Text = pathtofile + " — Constellation IDE";
                client.UpdateDetails("Editing " + Path.GetFileName(pathtofile));
            }
        }


        private void closeTabToolStripMenuItem1_Click(object sender, EventArgs e)
        {

            if (tabControl1.SelectedTab.Text != "Empty")
            {
                try
                {
                    paths.RemoveAt(tabControl1.SelectedIndex);
                }
                catch
                {

                }
                try
                {
                    Console.WriteLine("Removed from text list");
                    tbd.RemoveAt(tabControl1.SelectedIndex);
                    notDeleting = false;
                    //displaceItems(tabControl1.SelectedIndex);
                }
                catch
                {

                }
                try
                {

                    tabControl1.TabPages.Remove(tabControl1.SelectedTab);
                    lastIndex = tabControl1.SelectedIndex;
                }
                catch
                {

                }
                if(tabControl1.TabPages.Count == 0)
                {
                    fastColoredTextBox.Text = "";
                }
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!especiallyRestart)
            {
                SaveData();
                SaveTree(treeView1);
                SaveSettings();
                //client.Deinitialize();
            }
        }

        private void themeSettingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            
            themesettings ts = new themesettings(DarkTheme);
            ts.Show();
        }

        public static void changeTheme(bool Theme)
        {
            themedSaved = Theme;
            if(Theme)
            {
                // Create a hashtable of values that will eventually be serialized.
                Hashtable addresses = new Hashtable();
                addresses.Add("backColor", Color.FromArgb(255, 39, 44, 53));
                addresses.Add("textColor", Color.White);
                addresses.Add("font", new Font("JetBrains Mono", 18.0F));
                // To serialize the hashtable and its key/value pairs,   
                // you must first open a stream for writing.  
                // In this case, use a file stream.
                FileStream fs = new FileStream(form.AppPath + @"\Data\DataFile.cfg", FileMode.Create);

                // Construct a BinaryFormatter and use it to serialize the data to the stream.
                BinaryFormatter formatter = new BinaryFormatter();
                try
                {
                    formatter.Serialize(fs, addresses);
                }
                catch (SerializationException e)
                {
                    Console.WriteLine("Failed to serialize. Reason: " + e.Message);
                    throw;
                }
                finally
                {
                    fs.Close();
                }
            } else
            {
                // Create a hashtable of values that will eventually be serialized.
                Hashtable addresses = new Hashtable();
                addresses.Add("backColor", Color.White);
                addresses.Add("textColor", Color.Black);
                addresses.Add("font", new Font("Arial", 12.0f));
                //addresses.Add("basePath", form.BasePath);
                //addresses.Add("isPrjVisible", form.treeView1.Visible);
                // To serialize the hashtable and its key/value pairs,   
                // you must first open a stream for writing.  
                // In this case, use a file stream.
                FileStream fs = new FileStream(form.AppPath + @"\Data\DataFile.cfg", FileMode.Create);

                // Construct a BinaryFormatter and use it to serialize the data to the stream.
                BinaryFormatter formatter = new BinaryFormatter();
                try
                {
                    formatter.Serialize(fs, addresses);
                }
                catch (SerializationException e)
                {
                    Console.WriteLine("Failed to serialize. Reason: " + e.Message);
                    throw;
                }
                finally
                {
                    fs.Close();
                }
            }
        }

        private static void UpdateThemeColors(bool Theme)
        {
            if(Theme)
            {
                    form.fastColoredTextBox.SelectionColor = Color.FromArgb(60, 255, 255, 255);
                    form.fastColoredTextBox.IndentBackColor = Color.FromArgb(255, 39, 44, 53);
                    form.fastColoredTextBox.LineNumberColor = Color.FromArgb(255, 63, 71, 87);
                    form.fastColoredTextBox.ServiceLinesColor = Color.FromArgb(255, 39, 44, 53);
                    form.fastColoredTextBox.CaretColor = Color.FromArgb(60, 255, 255, 255);
                    form.menuStrip1.BackColor = Color.FromArgb(255, 32, 37, 44);
                    form.menuStrip1.ForeColor = Color.FromArgb(60, 255, 255, 255);

                    form.fileToolStripMenuItem.BackColor = Color.FromArgb(255, 32, 37, 44);
                    form.editToolStripMenuItem.BackColor = Color.FromArgb(255, 32, 37, 44);
                    form.languageToolStripMenuItem.BackColor = Color.FromArgb(255, 32, 37, 44);
                    form.runToolStripMenuItem.BackColor = Color.FromArgb(255, 32, 37, 44);
                    form.settingsToolStripMenuItem.BackColor = Color.FromArgb(255, 32, 37, 44);
                    form.tabControl1.BackColor = Color.FromArgb(255, 39, 44, 53);
                    //ThemeColor = Color.FromArgb(255, 39, 44, 53);
                    form.tabControl1.ForeColor = Color.FromArgb(60, 255, 255, 255);
                    form.BackColor = Color.FromArgb(255, 39, 44, 53);
                    form.treeView1.BackColor = Color.FromArgb(255, 39, 44, 53);
                    form.treeView1.ForeColor = Color.FromArgb(60, 255, 255, 255);
            } else {
                File.Delete(form.AppPath + @"\Data\DataFile.cfg");
                //Application.Restart();
            }
        }



        List<string> allKeys;
        List<string> values;
        private void fctb_ToolTipNeeded(object sender, ToolTipNeededEventArgs e)
        {
            allKeys = (from kvp in list select kvp.Key).Distinct().ToList();
            if (!string.IsNullOrEmpty(e.HoveredWord))
            {
                if (allKeys.Contains(e.HoveredWord))
                {
                    foreach (var index in allKeys)
                    {
                        if(e.HoveredWord == index)
                        {
                            List<string> values = (from kvp in list where kvp.Key == index select kvp.Value).ToList();
                            e.ToolTipTitle = e.HoveredWord;
                            e.ToolTipText = values[0];
                        }
                    }
                }
            }

            /*
             * Also you can get any fragment of the text for tooltip.
             * Following example gets whole line for tooltip:
            
            var range = new Range(sender as FastColoredTextBox, e.Place, e.Place);
            string hoveredWord = range.GetFragment("[^\n]").Text;
            e.ToolTipTitle = hoveredWord;
            e.ToolTipText = "This is tooltip for '" + hoveredWord + "'";
             */
        }

        public static void changeTabSize(Size size)
        {

        }

        string temp;
        private bool notDeleting = true;
        private static bool themedSaved;

        private void displaceItems(int index)
        {
            foreach(string item in tbd)
            {
                if(tbd[index] == item)
                {
                    tbd.Remove(item);
                    index++;
                    displacement(index);
                }
            }
        }

        private void displacement(int index)
        {
            foreach (string item in tbd)
            {
                temp = tbd[index];
                tbd.Remove(item);
                tbd.Insert(index - 1, temp);
                index++;
            }
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AboutForm af = new AboutForm();
            af.Show();
        }
    }

    public class TextBoxData
    {
        public string text;
    }
}
