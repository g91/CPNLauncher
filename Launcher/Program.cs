using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DemoLauncher
{
    static class Program
    {
        static void loadgames()
        {
            string data = Global.server + "/games.txt";

            if (Helpers.DownloadFile(data))
            {
                Preload.DeleteGame();
                foreach (string Game in Global.DownloadFile)
                {
                    Preload.GameList.Add(Game.Replace("\r", ""));
                }
            }

            if (Preload.GameList.Count() > 0)
            {
                string Gdata;
                foreach (string Game in Preload.GameList)
                {
                    string test = Directory.GetCurrentDirectory() + "\\gaming\\games\\" + Game;
                    if (!Directory.Exists(test))
                    {
                        Directory.CreateDirectory(test);
                        Gdata = Global.server + Game + ".txt";
                        if (Helpers.DownloadFile(Gdata))
                        {
                            Preload.DeletePre();

                            foreach (string theGames in Global.DownloadFile)
                            {
                                try
                                {
                                    if (theGames != string.Empty)
                                    {

                                        string[] parts = theGames.Split('=');
                                        if (parts.Length != 2) continue;
                                        string what = parts[0].Trim().ToLower();
                                        switch (what)
                                        {
                                            case "name":
                                                Preload.name = parts[1].Trim();
                                                break;
                                            case "thegame":
                                                Preload.theGame = parts[1].Trim();
                                                break;
                                            case "version":
                                                Preload.version = parts[1].Trim();
                                                break;
                                            case "path":
                                                Preload.path = parts[1].Trim();
                                                break;
                                            case "exe":
                                                Preload.exe = parts[1].Trim();
                                                break;
                                            case "args":
                                                Preload.args = parts[1].Trim();
                                                break;
                                            case "download":
                                                Preload.download = parts[1].Trim();
                                                break;
                                        }


                                    }

                                }
                                catch (Exception)
                                {
                                    Logger.Log("File: " + Game + " cannot be Create!", System.Drawing.Color.Red);

                                }
                            }
                            //Add
                            Games.Refresh();
                            Games.Create(Preload.name, Preload.theGame, Preload.version, Preload.path, Preload.exe, Preload.args, Preload.download);
                            Games.Refresh();
                        }
                    }
                }
            }
        }


        /// <summary>
        /// Der Haupteinstiegspunkt für die Anwendung.
        /// </summary>
        [STAThread]
        static void Main()
        {
            //FTP Main Server Path
            Global.server = "https://Launcher.chickenpickle.ninja/";
            //Path for Update the Software
            Global.updateServer = "https://Launcher.chickenpickle.ninja/";
            //Path for the News Tap
            Global.NewsServer = "https://Launcher.chickenpickle.ninja/";


            string path = Directory.GetCurrentDirectory();
            Gameload.Download2(path, Global.server + "download/CPNDownloader3.exe", "CPNDownloader3.exe");


            using (WebClient web1 = new WebClient())
            {
                string Version = web1.DownloadString(Global.server + "version.txt");
                if (File.Exists("version.txt"))
                {
                    string BuildVersion = Helpers.getVersion("version.txt");

                    File.WriteAllText("version.txt", Version);
                    if (Version != BuildVersion)

                        Helpers.RunShell("CPNDownloader3.exe", "update");
                }
                else
                {
                    File.WriteAllText("version.txt", Version);
                    Helpers.RunShell("CPNDownloader3.exe", "update");
                }
            }


            loadgames();
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form3());
        }
    }
}
