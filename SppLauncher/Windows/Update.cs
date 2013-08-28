﻿using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Net;
using System.Threading;
using System.Windows.Forms;
using Ionic.Zip;

namespace SppLauncher
{
    public partial class Update : Form
    {
        public Update()
        {
            InitializeComponent();
        }

        readonly Stopwatch sw = new Stopwatch();

        private void Update_Shown(object sender, EventArgs e)
        {
            bw_LauncherUpdate.RunWorkerAsync();
        }

        public void DownloadUpdate(string URL, string Save)
        {
            lbl_status.Text = "Status: Connecting";
            Thread.Sleep(100);
            string exePath = AppDomain.CurrentDomain.FriendlyName;

            if (File.Exists("SppLauncher_OLD.exe"))
            {
                File.Delete("SppLauncher_OLD.exe");
            }

            File.Move(exePath, @"SppLauncher_OLD.exe");
            File.SetAttributes("SppLauncher_OLD.exe", FileAttributes.Hidden);

            var client                      = new WebClient();
            client.DownloadProgressChanged += client_DownloadProgressChanged;
            client.DownloadFileCompleted   += client_DownloadFileCompleted;
            lbl_status.Text                 = "Status: Downloading";
            client.DownloadFileAsync(
                new Uri(URL), Save);
            sw.Start();
        }
        
        public void DownloadLangUpdate(string URL, string Save)
        {
            lbl_status.Text = "Status: Connecting";
            Thread.Sleep(100);
            var client = new WebClient();
            client.DownloadProgressChanged += clientLang_DownloadProgressChanged;
            client.DownloadFileCompleted += clientLang_DownloadFileCompleted;
            lbl_status.Text = "Status: Downloading Languages";
            client.DownloadFileAsync(
                new Uri(URL), Save);
            sw.Start();
        }

        private void client_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            double bytesIn    = double.Parse(e.BytesReceived.ToString());
            double totalBytes = double.Parse(e.TotalBytesToReceive.ToString());
            double percentage = bytesIn / totalBytes * 100;
            lbl_downByte.Text = e.BytesReceived / 1024 + " / " + e.TotalBytesToReceive / 1024 + " Kb";
            lbl_speed.Text    = "Speed: " + (bytesIn / 1024 / sw.Elapsed.TotalSeconds).ToString("0.00") + " Kb/s";
            pb_down.Value     = int.Parse(Math.Truncate(percentage).ToString(CultureInfo.InvariantCulture));
            lbl_Perecent.Text = int.Parse(Math.Truncate(percentage).ToString(CultureInfo.InvariantCulture)) + "%";
        }

        private void clientLang_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            double bytesIn = double.Parse(e.BytesReceived.ToString());
            double totalBytes = double.Parse(e.TotalBytesToReceive.ToString());
            double percentage = bytesIn / totalBytes * 100;
            lbl_downByte.Text = e.BytesReceived / 1024 + " / " + e.TotalBytesToReceive / 1024 + " Kb";
            lbl_speed.Text = "Speed: " + (bytesIn / 1024 / sw.Elapsed.TotalSeconds).ToString("0.00") + " Kb/s";
            pb_down.Value = int.Parse(Math.Truncate(percentage).ToString(CultureInfo.InvariantCulture));
            lbl_Perecent.Text = int.Parse(Math.Truncate(percentage).ToString(CultureInfo.InvariantCulture)) + "%";
        }

        private void client_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            bw_LangUpdate.RunWorkerAsync();
        }

        private void clientLang_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            ImportExtract();
            lbl_status.Text = "Status: Completed";


            Thread.Sleep(1500);
            try
            {
                File.Delete("Languages.zip");
            }
            catch (Exception)
            {
            }
            Process.Start("SppLauncher.exe");
            Application.Exit();
        }

        private void bw_updater_DoWork(object sender, DoWorkEventArgs e)
        {
            DownloadUpdate("http://dl.dropbox.com/u/7587303/Updates/SppLauncher_new.exe", "SppLauncher.exe"); //? Release.
            //DownloadUpdate("http://dl.dropbox.com/u/7587303/Updates/SppLauncher_new_test.exe", "SppLauncher.exe"); //? Test
        }

        private void pb_down_Click(object sender, EventArgs e)
        {
        }

        private void bw_LangUpdate_DoWork(object sender, DoWorkEventArgs e)
        {
            DownloadLangUpdate("https://dl.dropboxusercontent.com/u/7587303/Updates/Languages.zip", "Languages.zip"); //? Release.
            //DownloadLangUpdate("https://dl.dropboxusercontent.com/u/7587303/Updates/Languages_test.zip", "Languages.zip"); //? Test
            lbl_status.Text = "Status: Decompress";
        }

        public void ImportExtract() //.sppbackup extract
        {
            string unpck = "Languages.zip";
            string unpckDir = "";
            using (ZipFile zip = ZipFile.Read(unpck))
            {
                foreach (ZipEntry e in zip)
                {
                    e.Extract(unpckDir, ExtractExistingFileAction.OverwriteSilently);
                }
            }
        }

        private void bw_LangUpdate_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
        }
    }
}

