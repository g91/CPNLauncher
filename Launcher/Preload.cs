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
    public static class Preload
    {
        public static string name;
        public static string theGame;
        public static string version;
        public static string path;
        public static string exe;
        public static string args;
        public static string download;


        public static List<string> GameList = new List<string>();

        public static void DeleteGame()
        {
            GameList.Clear();
        }

        public static void DeletePre()
        {
            name = null;
            theGame = null;
            version = null;
            path = null;
            exe = null;
            args = null;
            download = null;

        }

    }
}
