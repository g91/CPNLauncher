using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CPNDownloader3
{
    internal class Program
    {
        //WebClient webClient = new WebClient();
        //webClient.DownloadFileCompleted += new AsyncCompletedEventHandler(Completed);
        //webClient.DownloadProgressChanged += new DownloadProgressChangedEventHandler(ProgressChanged);
        //webClient.DownloadFileAsync(new Uri(Global.server + "/version.txt"), path + "/version.txt");

        private static void ProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            //e.ProgressPercentage;
            Logger.Log("Downloaded: " + e.ProgressPercentage);
        }

        private static void Completed(object sender, AsyncCompletedEventArgs e)
        {
            //toolStripStatusLabel4.Text = "(" + 1 + "/" + 1 + ")";
            //string BuildVersion = Helpers.getVersion("version.txt");
            //if (BuildVersion != Assembly.GetExecutingAssembly().GetName().Version.ToString());
        }

        public static void Download(string directory, string Path, string FileX)
        {

            //WebClient webClient = new WebClient();
            //webClient.DownloadFileCompleted += new AsyncCompletedEventHandler(Completed);
            //webClient.DownloadProgressChanged += new DownloadProgressChangedEventHandler(ProgressChanged);
            //webClient.DownloadFileAsync(new Uri(Path), directory + "\\" + FileX);

            DateTime startTime = DateTime.UtcNow;

            WebRequest request = WebRequest.Create(Path);
            WebResponse response = request.GetResponse();

            using (Stream responseStream = response.GetResponseStream())
            {
                using (Stream fileStream = File.OpenWrite(directory + "\\" + FileX))
                {
                    byte[] buffer = new byte[4096];
                    int bytesRead = responseStream.Read(buffer, 0, 4096);
                    while (bytesRead > 0)
                    {
                        fileStream.Write(buffer, 0, bytesRead);
                        DateTime nowTime = DateTime.UtcNow;
                        if ((nowTime - startTime).TotalMinutes > 5)
                        {
                            throw new ApplicationException(
                                "Download timed out");
                        }
                        bytesRead = responseStream.Read(buffer, 0, 4096);
                    }
                }
            }
        }


        public static string GetMD5(string fileName)
        {
            using (var md5 = MD5.Create())
            {
                using (var stream = File.OpenRead(fileName))
                {
                    return BitConverter.ToString(md5.ComputeHash(stream)).Replace("-", string.Empty);
                }
            }
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

        static async Task Main(string[] args)
        {
            Logger.Log(args[0]);
            if (args[0] == "update")
            {
                Logger.Log("performing update");
                Logger.Log("killing CPNLauncher Process");

                foreach (var process in Process.GetProcessesByName("CPNLauncher"))
                {
                        process.Kill();
                }

                Logger.Log("downloading update");
                string path = Directory.GetCurrentDirectory();
                Download(path, "https://launcher.chickenpickle.ninja/download/CPNLauncher.exe", "CPNLauncher.exe");
                Logger.Log("10 seconds the updater will close and CPN launcher as resume as normal");
                Thread.Sleep(100);
                RunShell("CPNLauncher.exe", "");

            }
            else {
                if (File.Exists("dl"))
                {
                    string path = Directory.GetCurrentDirectory() + "\\gaming\\games\\";
                    string cdir = "\\";
                    string[] DownloadFile = File.ReadAllLines("dl");
                    foreach (string Game in DownloadFile)
                    {
                        string[] MyGame = Game.Split('#');
                        string path2 = path + MyGame[4];
                        if (MyGame[1] == "d")
                        {
                            cdir = MyGame[0];
                            string path3 = path2 + MyGame[4] + "\\" + cdir;
                            if (!Directory.Exists(path3))
                            {
                                Directory.CreateDirectory(path3);
                            }
                            Logger.Log("testing Directory: " + MyGame[0]);
                        }

                        if (MyGame[1] == "f")
                        {
                            Logger.Log("testing Files: " + MyGame[0]);
                            if (!File.Exists(path2 + "\\" + MyGame[0]))
                            {
                                Logger.Log("Downloading: " + MyGame[0]);
                                Download(path2, MyGame[3] + MyGame[0], MyGame[0]);
                            }
                            else
                            {
                                if (MyGame[2] != GetMD5(path2 + "\\" + MyGame[0]))
                                {
                                    Logger.Log("Downloading: " + MyGame[0]);
                                    Download(path2, MyGame[3] + MyGame[0], MyGame[0]);
                                }
                            }
                        }
                    }
                    File.Delete("dl");
                }
            }
        }
    }
}
