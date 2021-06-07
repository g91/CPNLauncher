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

        public static bool LoadFile()
        {
            int count = GameDownList.Count() -1;

            if (count > -1)
            {
                string file = GameDownList[count].ToString();
                GetDownload(path_directory, file, path_download);
                GameDownList.RemoveAt(count);
                counter_down++;
                return true;
            }
            return false;
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


        public static void Download(string directory, string Path, string FileX)
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
    }
}
