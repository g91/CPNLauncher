using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Diagnostics;
using System.Windows.Forms;
using Microsoft.Win32;
using System.Net;
using System.Security.Cryptography;


namespace DemoLauncher
{
    public static class Helpers
    {

        public static string getVersion(string path)
        {
            string Versions = "";

            if (File.Exists(path))
            {
                string[] readText = File.ReadAllLines(path);
                foreach (string s in readText)
                {
                    Versions = s;
                }
            }
            return Versions;
        }


        public static string GetDirectory()
        {
            string path = Directory.GetCurrentDirectory();
            return path;
        }

        public static void RunShell(string file, string command)
        {
            Process process = new System.Diagnostics.Process();
            ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
            startInfo.FileName = file;
            startInfo.Arguments = command;
            process.StartInfo = startInfo;
            process.Start();
        }

        public static bool DownloadFile(string file)
        {
            Global.DownloadFile = null;
            try
            {
                using (WebClient wc = new WebClient())
                {
                    //Global.DownloadFile = wc.DownloadString(file).Split(new string[] { Environment.NewLine }, StringSplitOptions.None);
                    Global.DownloadFile = wc.DownloadString(file).Split('\n');
                }
            }
            catch (Exception)
            {
                Logger.Log("Host: " + file + " cannot be reached!", System.Drawing.Color.Red);
                return false;
            }
            return true;
        }

    }
}
