using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Net;
using System.Windows.Forms;

namespace DemoLauncher
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            SystemLog.Clear(); // Make Empy LogFile.
            Logger.box = rtb1; // Set OutputBox for Log.
            webBrowser1.ScriptErrorsSuppressed = true;
            webBrowser1.Url = new Uri(Global.NewsServer); //Set the News Page
            toolStripStatusLabel3.Text =  Assembly.GetExecutingAssembly().GetName().Version.ToString();
            label1.Text = Global.user;


            RefreshGames();
            ItemSelect();
        }

        private void RefreshGames()
        {
            Games.Refresh();
            toolStripComboBox1.Items.Clear();
            foreach (Game p in Games.games)
                toolStripComboBox1.Items.Add(p.theGame);
        }

        private void ItemSelect()
        {
            if (toolStripComboBox1.Items.Count > 0)
                toolStripComboBox1.SelectedIndex = 0;
            if (toolStripComboBox1.Items.Count > 1)
                toolStripComboBox1.SelectedIndex = 1;
        }


        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            timer1.Stop();
            Application.Exit();
        }

        private void checkUpdatesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            toolStripStatusLabel4.Text = "(" + 0 + "/" + 1 + ")";
            string path = Helpers.GetDirectory();
            WebClient webClient = new WebClient();
            webClient.DownloadFileCompleted += new AsyncCompletedEventHandler(Completed);
            webClient.DownloadProgressChanged += new DownloadProgressChangedEventHandler(ProgressChanged);
            webClient.DownloadFileAsync(new Uri(Global.server + "/version.txt"), path + "/version.txt");
        }

        private void ProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            toolStripProgressBar1.Value = e.ProgressPercentage;
        }

        private void Completed(object sender, AsyncCompletedEventArgs e)
        {
            toolStripStatusLabel4.Text = "(" + 1 + "/" + 1 + ")";
            string BuildVersion = Helpers.getVersion("version.txt");

            if (BuildVersion != Assembly.GetExecutingAssembly().GetName().Version.ToString())
            {
                MessageBox.Show("Update available!");
                System.Diagnostics.Process.Start(Global.updateServer);
                toolStripStatusLabel4.Text = "(" + 0 + "/" + 0 + ")";
                toolStripProgressBar1.Value = 0;
            }
        }

        private void gamesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form2 f = new Form2();
            f.ShowDialog();
            RefreshGames();
            ItemSelect();
        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            //RefreshGames();
            //ItemSelect();
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            //Start
            int n = toolStripComboBox1.SelectedIndex;
            if (n == -1)
            {
                return;
            }

            Game p = Games.games[n];
            string exe = p.exe;
            string path = p.path;
            string args =  p.args;
            Helpers.RunShell(path + exe, args);
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            toolStripStatusLabel4.Text = "(" + Gameload.counter_down.ToString() + "/" + Gameload.counter_files.ToString() + ")";

            if (Gameload.LoadFile())
            {
                Global.percent = (100 / Gameload.counter_files) * Gameload.counter_down;
                toolStripProgressBar1.Value = Convert.ToInt32(Global.percent);

                if (Global.percent == 100)
                {
                    MessageBox.Show("Download completed!");
                    toolStripStatusLabel4.Text = "(" + 0 + "/" + 0 + ")";
                    toolStripProgressBar1.Value = 0;
                    Gameload.counter_files = 0;
                    Gameload.counter_down = 0;
                    timer1.Stop();
                }

            }

        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            //Download
            int n = toolStripComboBox1.SelectedIndex;
            if (n == -1)
            {
                return;
            }

            Game p = Games.games[n];
            string path = p.path;  //Game Path
            string download = p.download;

            Gameload.path_directory = path;
            Gameload.path_download = Global.server + "/" + download + "/";

            string data = Global.server + "/" + download + "/updater.txt";

            if (Helpers.DownloadFile(data))
            {
                Gameload.DeleteGameDown();

                foreach (string Game in Global.DownloadFile)
                {
                    try
                    {
                        if (Game != string.Empty)
                        {
                            Gameload.GameDownList.Add(Game);
                        }

                    }
                    catch(Exception)
                    {
                        Logger.Log("Download failed.", System.Drawing.Color.Red);
                        return;
                    }
                }
                Gameload.counter_files = Gameload.GameDownList.Count();
            }

            Gameload.MakeFolders(Gameload.path_directory);
            timer1.Start();
     

        }

        private void tabPage2_Enter(object sender, EventArgs e)
        {

        }

        private void toolStripComboBox1_Click(object sender, EventArgs e)
        {
            ;
            webBrowser2.Navigate("http://launcher.chickenpickle.ninja/cp/" + toolStripComboBox1.Text + "/index.php?username=" + Global.user + "&key="+ Global.apikey);
        }

        private void menuStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }
    }
}
