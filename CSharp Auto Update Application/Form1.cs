using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CSharp_Auto_Update_Application
{
    public partial class Update_Form : Form
    {
        public Update_Form()
        {
            InitializeComponent();
        }

        public const String Raw_Link =
            ""; //Github Gist Raw Link
        public String Application_Name = "Mithalos"; //Project Name

        private bool Update_Check()
        {
            const String Update_Link = Raw_Link; //We Define Update Link, Based on Raw Link
            int New_Version; //We Define New Version

            try
            {
                //The following thing helps us to extract the text we wrote from the raw link with WebClient and get the first issue as a version.
                New_Version = int.Parse(new WebClient().DownloadString(Update_Link)[0].ToString());
            }
            catch (Exception Err)
            {
                MessageBox.Show($"The new version is not entered as an integer! please enter 2, 3. - {Err.Message}", "Ups!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            //It will pull the current version of the app. It might return something like 1.0.0.2, but we'll take that 1 at the beginning.
            int Application_Version = int.Parse(System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString()[0].ToString());
            // The version of the app is smaller than the new version.Update available.
            return Application_Version < New_Version;
        }

        private void End_Version_Update()
        {
            const String Raw = Raw_Link;
            WebClient WebC = new WebClient();
            String File_Name = Application_Name.EndsWith(".exe") ? Application_Name : Application_Name += ".exe"; //.exe Add No Extension
            String New_Version_Link = WebC.DownloadString(Raw).Split('|').Last(); //Divide By Separator And '|' Get Text After Char.

            try
            {
                WebC.DownloadFile(New_Version_Link, Application_Name);
            }
            catch (Exception Err)
            {
                MessageBox.Show($"An error occurred while downloading the new version. - {Err.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            /*
            
            The new version has been downloaded here. Now let's delete this version and open the new version, but we have a there is an obstacle, 
            we cannot delete the program while the program is running, so we will first terminate the program with the Process.Kill method.

             */

            Process.Start(Application_Name); //Run Application
            Process.Start(new ProcessStartInfo() //Deleting Current Application
            {
                Arguments = "/C choice /C Y /N /D Y /T 3 & Del \"" + Application.ExecutablePath + "\"",
                WindowStyle = ProcessWindowStyle.Hidden,
                CreateNoWindow = true,
                FileName = "cmd.exe"
            });
            Process.GetCurrentProcess().Kill();
        }

        private void Update_Form_Load(object sender, EventArgs e)
        {
            int Info_Version = int.Parse(System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString()[0].ToString());
            Lbl_Version_Info.Text = "Version: " + Info_Version;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (Update_Check())
            {
                DialogResult rn = MessageBox.Show("There is an update, update it?", "Update Alert", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
                if (rn == DialogResult.Yes)
                {
                    End_Version_Update();
                }
            }
            else
            {
                MessageBox.Show("No updates, thanks for using the latest version.", "No Updates", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
    }
}
