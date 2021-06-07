using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DemoLauncher
{
    public partial class Form3 : Form
    {
        private IniFile MyIni = new IniFile("Settings.ini");
        public Form3()
        {
            InitializeComponent();
            username.Text = MyIni.Read("username", "Settings");
            password.Text = MyIni.Read("password", "Settings");
        }

        private void button1_Click(object sender, EventArgs e)
        {

            Global.user = username.Text;

            MyIni.Write("username", Global.user, "Settings");
            MyIni.Write("password", password.Text, "Settings");


            using (WebClient web1 = new WebClient())
            {
                Global.apikey = web1.DownloadString("http://chickenpickle.ninja/api.php?login-api=go&login=" + Global.user + "&password=" + password.Text+"&v=2");
   
                if (string.Compare(Global.apikey, "Invalid") == 1)
                    label3.Text = "Error: Invalid username or password.";
                else if(string.Compare(Global.apikey, "BADV") == 1)
                    label3.Text = "Error: download update ";
                else
                {
                    this.Hide();
                    
                    MyIni.Write("webkey", Global.apikey, "Settings");
                    Form1 f = new Form1();
                    f.ShowDialog();
                    this.Close();

                }
            }

            
        }
    }
}
