using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace TS3Patcher
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }

        public static bool ReplaceBytes(byte[] fileBytes, byte[] replaceWith, int position)
        {
            for (var i = 0; i < replaceWith.Length; i++)
            {
                fileBytes[position + i] = replaceWith[i];
            }
            return true;
        }

        //thanks https://social.msdn.microsoft.com/Forums/vstudio/en-US/15514c1a-b6a1-44f5-a06c-9b029c4164d7/searching-a-byte-array-for-a-pattern-of-bytes?forum=csharpgeneral
        public static int IndexOf(byte[] arrayToSearchThrough, byte[] patternToFind)
        {
            if (patternToFind.Length > arrayToSearchThrough.Length)
                return -1;
            for (int i = 0; i < arrayToSearchThrough.Length - patternToFind.Length; i++)
            {
                bool found = true;
                for (int j = 0; j < patternToFind.Length; j++)
                {
                    if (arrayToSearchThrough[i + j] != patternToFind[j])
                    {
                        found = false;
                        break;
                    }
                }
                if (found)
                {
                    return i;
                }
            }
            return -1;
        }

        public static void PatchExe(string exeFile)
        {
            var bakpath = Path.Combine(Path.GetDirectoryName(exeFile), Path.GetFileNameWithoutExtension(exeFile) + ".bak");
            if (!File.Exists(bakpath))
            {
                try
                {
                    File.Copy(exeFile, bakpath);
                }
                catch (Exception exception)
                {
                    var diag = MessageBox.Show("Can't create a backup. Proceed anyway?", "Info", MessageBoxButtons.YesNo);
                    if (diag == DialogResult.No)
                        return;
                }
            }

            var fileBytes = File.ReadAllBytes(exeFile);

            var customFunction = new byte[] { 0xC3 };
            var lookup = new byte[] { 0x8B, 0x44, 0x24, 0x04, 0x8B, 0x08, 0x6A, 0x01, 0x51, 0xFF };
            var index = IndexOf(fileBytes, lookup);
            if (index >= 0)
            {
                ReplaceBytes(fileBytes, customFunction, index);
                try
                {
                    File.WriteAllBytes(exeFile, fileBytes);
                }
                catch (Exception e)
                {
                    MessageBox.Show("Couldn't patch executable. Try launching me as administrator?");
                    return;
                }
                MessageBox.Show("Game Patched Successfully");
            }
            else
            {
                MessageBox.Show("Can't find pattern, sorry :(");
            }
        }
    }
}
