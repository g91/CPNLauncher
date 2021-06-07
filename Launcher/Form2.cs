using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DemoLauncher
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            RefreshGames();
        }

        private void Form2_FormClosing(object sender, FormClosingEventArgs e)
        {

        }

        private void RefreshGames()
        {
            Games.Refresh();
            listBox1.Items.Clear();
            foreach (Game p in Games.games)
                listBox1.Items.Add(p.ToString());
        }



        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            string data = Global.server + "/games.txt";

            if (Helpers.DownloadFile(data))
            {
                Preload.DeleteGame();
                foreach (string Game in Global.DownloadFile)
                {
                    Preload.GameList.Add(Game);
                }
            }

            if (Preload.GameList.Count() > 0)
            {
                string Gdata;

                foreach (string Game in Preload.GameList)
                {
                    Gdata = Global.server + "/" + Game + ".txt";

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
                            catch(Exception)
                            {
                                Logger.Log("File: " + Game + " cannot be Create!", System.Drawing.Color.Red);

                            }
                        }
                        //Add
                        Games.Refresh();
                        Games.Create(Preload.name,Preload.theGame, Preload.version,Preload.path,Preload.exe, Preload.args, Preload.download);
                        Games.Refresh();
                        RefreshGames();
                    }
                    
                }
            }
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            //Removed
            int n = listBox1.SelectedIndex;
            if (n == -1)
                return;
            Game p = Games.games[n];
            string path = Games.getProfilePath(p.id);
            if (File.Exists(path))
                File.Delete(path);
            Games.Refresh();
            RefreshGames();
        }

        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            RefreshGames();
        }
    }
}
