using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;


namespace DemoLauncher
{
    public class Game
    {
        public string _raw;
        public string name;
        public long id;
        public string theGame;
        public string version;
        public string path;
        public string exe;
        public string args;
        public string download;

        public static Game Load(string filename)
        {
            string[] lines = File.ReadAllLines(filename);
            Game p = new Game();
            p._raw = File.ReadAllText(filename);
            foreach (string line in lines)
            {
                string[] parts = line.Split('=');
                if (parts.Length != 2) continue;
                string what = parts[0].Trim().ToLower();
                switch (what)
                {
                    case "name":
                        p.name = parts[1].Trim();
                        break;
                    case "id":
                        p.id = Convert.ToInt32(parts[1].Trim());
                        break;
                    case "thegame":
                        p.theGame = parts[1].Trim();
                        break;
                    case "version":
                        p.version = parts[1].Trim();
                        break;
                    case "path":
                        p.path = parts[1].Trim();
                        break;
                    case "exe":
                        p.exe = parts[1].Trim();
                        break;
                    case "args":
                        p.args = parts[1].Trim();
                        break;
                    case "download":
                        p.download = parts[1].Trim();
                        break;
                }
            }
            if (p.name == null || p.id == 0)
                return null;
            return p;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("ID = " + id + ", ");
            sb.Append("Name = " + name + ", ");
            sb.Append("theGame = " + theGame + ", ");
            sb.Append("Version = " + version + ", ");
            sb.Append("Path = " + path + ", ");
            sb.Append("Exe = " + exe + ", ");
            sb.Append("Args = " + args + ", ");
            sb.Append("Download = " + download);
            return sb.ToString();
        }

    }

    public static class Games
    {
        public static List<Game> games = new List<Game>();

        public static void Refresh()
        {
            games = new List<Game>();

            if (!Directory.Exists("gaming"))
                Directory.CreateDirectory("gaming");
            if (!Directory.Exists("gaming\\games"))
                Directory.CreateDirectory("gaming\\games");
            string[] files = Directory.GetFiles("gaming\\games\\", "*.txt");
            foreach (string file in files)
            {
                Game p = Game.Load(file);
                if (p != null)
                    games.Add(p);
            }
            Logger.Log("Loaded " + games.Count + " game profiles");
        }

        public static string getProfilePath(long id)
        {
            return "gaming\\games\\" + id.ToString("X8") + "_profile.txt";
        }

        public static Game Create(string name, string theGame, string version, string path, string exe, string args, string download)
        {
            long id = 1000;
            while (File.Exists(getProfilePath(id)))
                id++;
            return Create(name, id, theGame, version, path, exe, args, download);
        }

        public static Game Create(string name, long id, string theGame, string version, string path, string exe, string args, string download)
        {
            string profileContent = "name=" + name + "\nid=" + id + "\ntheGame=" + theGame + "\nVersion=" + version + "\nPath=" + path + "\nExe=" + exe + "\nArgs=" + args + "\nDownload=" + download;
            string filename = getProfilePath(id);
            File.WriteAllText(filename, profileContent, Encoding.Unicode);
            return Game.Load(filename);
        }





    }



}
