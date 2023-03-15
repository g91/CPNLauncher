using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Threading;

namespace DemoLauncher
{
    public static class Gameload
    {
        public static Double counter_files;
        public static Double counter_down;

        public static string path_download;
        public static string path_directory;


        public static List<string> GameDownList = new List<string>();

        public static void DeleteGameDown()
        {
            GameDownList.Clear();
        }


        public static void threadFunc3(object arg)
        {
            int count = GameDownList.Count() - 1;

            if (count > -2)
            {
                string file = GameDownList[count].ToString();
                GetDownload(path_directory, file, path_download);
                GameDownList.RemoveAt(count);
                counter_down++;
                //return true;
            }
        }

        public static bool LoadFile()
        {
            Thread tParm = new Thread(new ParameterizedThreadStart(threadFunc3));
            tParm.Start();

            //int count = GameDownList.Count() -1;

            //if (count > -1)
            //{
            //    string file = GameDownList[count].ToString();
            //    GetDownload(path_directory, file, path_download);
            //    GameDownList.RemoveAt(count);
            //    counter_down++;
            //    return true;
            //}
            return true;
        }


        public static bool SetDirectory(string path)
        {
            string dir = path;
            Directory.CreateDirectory(dir);

            try
            {
                //Set the current directory.
                Directory.SetCurrentDirectory(dir);
            }
            catch (DirectoryNotFoundException e)
            {
                Logger.LogError("The specified directory does not exist!", e);
                return false;
            }

            return true;
        }

        public static void MakeFolders(string directory)
        {
            Directory.CreateDirectory(directory);

            foreach (string files in GameDownList)
            {
                try
                {
                    if (files != string.Empty)
                    {
                        string[] Download_file = files.Split('#');
                        string file = Download_file[0].Replace('/', '\\');
                        string type = Download_file[1];
                        string checksum = Download_file[2];

                        // If is directory
                        if (type == "d")
                        {
                            if (!Directory.Exists(directory + "\\" + file))
                            {
                                Logger.Log("Create Folder: " + file, System.Drawing.Color.Blue);
                                Directory.CreateDirectory(directory + "\\" + file);
                            }
                            else
                            {
                                Logger.Log("Folder: " + file + " Exists!", System.Drawing.Color.DarkGreen);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logger.LogError("Download Error", ex);
                }
            }
        }


        public static void GetDownload(string directory, string files, string Download_path)
        {
            Directory.CreateDirectory(directory);

            try
            {

                string[] Download_file = files.Split('#');
                string file = Download_file[0].Replace('/', '\\');
                string type = Download_file[1];
                string checksum = Download_file[2];

                // If is directory
                if (type == "d")
                {
                    if (!Directory.Exists(directory + "\\" + file))
                    {
                        Logger.Log("Create Folder: " + file, System.Drawing.Color.Blue);
                        Directory.CreateDirectory(directory + "\\" + file);
                    }
                    else
                    {
                        Logger.Log("Folder: " + file + " Exists!", System.Drawing.Color.DarkGreen);
                    }
                }

                // If is file
                if (type == "f" && file != "Updater.exe" && file != "updater.txt")
                {

                    if (!File.Exists(directory + "\\" + file))
                    {
                        Logger.Log("Download: " + file, System.Drawing.Color.DarkOliveGreen);
                        Download(directory, Download_path + Download_file[0], file);
                        Logger.Log("Download: " + file + " successfully!", System.Drawing.Color.Green);
                    }
                    else
                    {
                        Logger.Log("File: " + file + " Exists!", System.Drawing.Color.DarkGreen);
                        Logger.Log("Checking: " + file + " MD5 Hash: " + checksum, System.Drawing.Color.DarkGreen);

                        if (checksum != GetMD5(directory + "\\" + file))
                        {
                            File.Delete(directory + "\\" + file);
                            Logger.Log("Update: " + file, System.Drawing.Color.DarkOrchid);
                            Download(directory, Download_path + Download_file[0], file);
                            Logger.Log("Download: " + file + " successfully!", System.Drawing.Color.Green);
                        }

                    }


                }

            }
            catch (Exception ex)
            {
                Logger.LogError("Download Error", ex);
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
        public static void threadFunc(object arg)
        {
            string[] myArg = arg.ToString().Split('|');
            string directory = myArg[0];
            string Path = myArg[1];
            string FileX = myArg[2];

            //Logger.Log("test" + directory + " " + Path + " " + FileX);
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

        public static void Download2(string directory, string Path, string FileX)
        {
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

        public static Process process = new Process();

        public static void CPNDownloader()
        {
            process.EnableRaisingEvents = true;
            process.OutputDataReceived += new System.Diagnostics.DataReceivedEventHandler(process_OutputDataReceived);
            process.ErrorDataReceived += new System.Diagnostics.DataReceivedEventHandler(process_ErrorDataReceived);
            process.Exited += new System.EventHandler(process_Exited);

            process.StartInfo.FileName = "CPNDownloader3.exe";
            process.StartInfo.Arguments = "";
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardError = true;
            process.StartInfo.RedirectStandardOutput = true;

            process.Start();
            process.BeginErrorReadLine();
            process.BeginOutputReadLine();

            //below line is optional if we want a blocking call
            //process.WaitForExit();
        }

        public static void process_Exited(object sender, EventArgs e)
        {
            Console.WriteLine(string.Format("process exited with code {0}\n", process.ExitCode.ToString()));
        }

        public static void process_ErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
            //Console.WriteLine(e.Data + "\n");
            Logger.Log(e.Data + "\n");
        }

        public static void process_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            //Console.WriteLine(e.Data + "\n");
            Logger.Log(e.Data + "\n");
        }



        public static void Download(string directory, string Path, string FileX)
        {
            Helpers.RunShell("CPNDownloader3.exe", "games");
            //DateTime startTime = DateTime.UtcNow;

            //WebRequest request = WebRequest.Create(Path);
            //WebResponse response = request.GetResponse();

            //using (Stream responseStream = response.GetResponseStream())
            //{
            //    using (Stream fileStream = File.OpenWrite(directory + "\\" + FileX))
            //    {
            //        byte[] buffer = new byte[4096];
            //        int bytesRead = responseStream.Read(buffer, 0, 4096);
            //        while (bytesRead > 0)
            //        {
            //            fileStream.Write(buffer, 0, bytesRead);
            //            DateTime nowTime = DateTime.UtcNow;
            //            if ((nowTime - startTime).TotalMinutes > 5)
            //            {
            //                throw new ApplicationException(
            //                    "Download timed out");
            //            }
            //            bytesRead = responseStream.Read(buffer, 0, 4096);
            //        }
            //    }
            //}
            //string arg = directory + "|" + Path + "|" + FileX;
            //Thread tParm = new Thread(new ParameterizedThreadStart(threadFunc));
            //tParm.Start(arg);
        }
    }
}
