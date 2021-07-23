using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace TS3Patcher
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            MessageBox.Show("Please locate your TS3W.exe");
            var fileResult = openFileDialog1.ShowDialog();
            if (fileResult == DialogResult.OK && File.Exists(openFileDialog1.FileName))
            {
                Program.PatchExe(openFileDialog1.FileName);
            }
            Environment.Exit(0);
        }
    }
}