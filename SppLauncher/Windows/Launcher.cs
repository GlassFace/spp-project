﻿using System;
using System.Collections;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Text;
using System.IO;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Xml;
using Ionic.Zip;
using MySql.Data.MySqlClient;
using MySQLClass;
using SppLauncher.OnlineBot;
using SppLauncher.Windows;
using SppLauncher.Class;

namespace SppLauncher
{
    public partial class Launcher : Form
    {
        #region Variable

        public static string wowExePath,
                             wowFolderPath,
                             wowVersion,
                             ipAdress,
                             online,
                             resetBots,
                             randomizeBots,
                             realmListPath,
                             realmDialogPath,
                             OLDrealmList,
                             UpdLink,
                             importFile,importFolder,
                             exportfile,exportFolder,
                             autostart;

        readonly string getTemp = Path.GetTempPath();
        readonly PerformanceCounter cpuCounter;
        readonly PerformanceCounter ramCounter;
        public static string RemoteProgVer, currProgVer = "1.0.6",lang;
        public static double CurrEmuVer, RemoteEmuVer;
        public static bool Available, Updater  = false, AllowUpdaterQuestion = false, allowupdaternorunwow = false;
        private bool _startStop;
        private Process _cmd, _cmd1, _cmd3;
        private bool _allowdev, _allowtext, _restart;
        private DateTime _dt   = DateTime.Now;
        private string lwPath  = "SingleCore\\";
        private Process _p     = new Process();
        private string _realm, _mangosdMem, _realmdMem, _sqlMem;
        private string _sql;
        private string sqlpath   = "Database\\bin\\";
        private DateTime _start1 = DateTime.Now;
        private string _status;
        private bool _update, _updateNo;
        private bool _updateYes;
        private string _world;
        private System.Collections.Hashtable LangHash = new Hashtable();
        private System.Resources.ResourceManager rm;
        public static string[] button = new string[80];

        private string tooltip             = "Auto Changed RealmList.wtf and after exit SPP change back original realmlist.";
        private String Msgloadworld        = "Loading World";
        private string Msgstartingrealm    = "Starting Realm";
        private string Msgstartingmy       = "Starting Mysqlm";
        string Msgresetbots                = "Reset bots";
        private string Msgserverres        = "Servers Started";
        private string Msgreadytoplay      = "Ready to play";
        private string Msgwould            = "Would you like to start World of Warcraft?";
        private string Msgupstart          = "Update started.\nDo lag for several times.";
        private string Msgmangoscrash      = "Mangosd Crashed";
        private string Msgrestart          = "The process is automatically restart.";
        private string Msgcheckup          = "Checking Update...";
        private string Msgnewver           = "New Version Available: V";
        private string Msgyouwant          = "You want to download?";
        private string Msgnewver2          = "New Version";
        private string Msgnewservera       = "New Server Update Available";
        private string Msgstatusup         = "Up to date";
        private string Msgimportchar       = "Import Characters";
        private string Msgdecompress       = "Decompress";
        private string Msgimportacc        = "Import Accounts";
        private string Msgimportcomp       = "Import Completed";
        private string Msgexportchar       = "Export Characters";
        private string Msgexportchar1      = "Exporting Characters";
        private string Msgexportacc        = "Export Accounts";
        private string Msgcompress         = "Compressing";
        private string Msgexportcomp       = "Export Completed";
        public static string Msgaboutmsg1  = "Launcher and tools by Whit33r";
        public static string Msgaboutmsg2  = "Server and database by";
        public static string Msgaboutver   = "Versions";
        public static string Msgstarting   = "Starting Options";
        public static string Msgserverrate = "Server Rates";
        public static string Msgitemdrop   = "Item Drop Rates";
        public static string Msgreport     = "Report";
        public static string Msgyourmail   = "Your Mail";
        public static string Msgbugtype    = "Bug Type:";
        public static string Msgdesc       = "Description";
        public static string Msgsysinfo    = "System Information";
        public static string Msgsendreport = "Send Report";
        public static string Msgtitle      = "Bug Report";
        public static string Msgbotsettings = "Bot Settings";
        public static string Msgbots = "Bots:";
        public static string Msgminbots = "Min bots:";
        public static string Msgmaxbots = "Max bots:";
        public static string Msgminbotsi = "Min bots/intervall:";
        public static string Msgmaxbotsi = "Max bots/intervall";
        public static string Msgbotacc = "Bot account:";
        public static string Msgupinter = "Update intervall:";
        public static string Msgbotmaps = "Bot Maps";
        public static string Msgother = "Other";
        public static string Msgbotsacc = "Delete bot accounts";
        public static string Msgbotlfg = "Bot Join LFG";
        public static string Msgrnd = "Rnd bot login";
        public static string Msgloaddef = "Load Default";
        public static string Msgsave = "Save";

        #endregion

        #region Initializems
        

        public Launcher()
        {



            //rm = new System.Resources.ResourceManager(typeof (Launcher));
            //LangHash.Add("English", Thread.CurrentThread.CurrentCulture);
            //LangHash.Add("Hungarian", new System.Globalization.CultureInfo("hu"));

            InitializeComponent();

            cpuCounter = new PerformanceCounter();
            GetLocalSrvVer();
            cpuCounter.CategoryName = "Processor";
            cpuCounter.CounterName  = "% Processor Time";
            cpuCounter.InstanceName = "_Total";
            ramCounter              = new PerformanceCounter("Memory", "Available MBytes");
            tmrUsage.Start();
            StatusBarUpdater.Start();
            MenuItemsDisableAfterLoad();
            WasThisUpdate();
            rtWorldDev.Visible = false;
            WindowSize(true);
            font();
            ReadXML();
            Loadsettlang();
            SearchProcess();
            StatusIcon();

            if (autostart == "1")
            {

                startstopToolStripMenuItem.Image                 = Properties.Resources.Button_stop_icon;
                startToolStripMenuItem.Image                     = Properties.Resources.Button_stop_icon;
                startstopToolStripMenuItem.Text                  = "Stop";
                startToolStripMenuItem.Text                      = "Stop";
                startstopToolStripMenuItem.Enabled               = false;
                startToolStripMenuItem.Enabled                   = false;
                _startStop                                       = true;
                autostartToolStripMenuItem.Checked               = true;
                autorunToolStripMenuItem.Checked                 = true;
                exportCharactersToolStripMenuItem.Enabled        = false;
                exportImportCharactersToolStripMenuItem.Enabled  = false;
                StartAll();
            }
            else
            {
                bwUpdate.RunWorkerAsync();
                exportCharactersToolStripMenuItem.Enabled       = false;
                exportImportCharactersToolStripMenuItem.Enabled = false;
                startstopToolStripMenuItem.Enabled              = true;
                startToolStripMenuItem.Enabled                  = true;
                restartToolStripMenuItem1.Enabled               = false;
                restartToolStripMenuItem2.Enabled               = false;
            }

            SppTray.Visible = true;
            startWowToolStripMenuItem.ToolTipText = tooltip;

        }

        public void Loadsettlang()
        {
            Language.ReadXML(lang);
            Loadlang();
        }

        public void Loadlang()
        {
            runWoWToolStripMenuItem.Text             = button[0];
            toolStripMenuItem1.Text                  = button[1];
            toolsToolStripMenuItem.Text              = button[2];
            restartToolStripMenuItem.Text            = button[3];
            reportBugToolStripMenuItem.Text          = button[4];
            tsmHelpUs.Text                           = button[5];
            aboutToolStripMenuItem.Text              = button[6];
            exitToolStripMenuItem2.Text              = button[7];
            worldSettingsToolStripMenuItem.Text      = button[8];
            botSettingsToolStripMenuItem.Text        = button[9];
            changeWoWPathToolStripMenuItem1.Text     = button[10];
            resetAllRandomBotsToolStripMenuItem.Text = button[11];
            randomizeBotsToolStripMenuItem.Text      = button[12];
            exportCharactersToolStripMenuItem.Text   = button[13];
            lanSwitcherToolStripMenuItem1.Text       = button[14];
            accountToolToolStripMenuItem1.Text       = button[15];
            startstopToolStripMenuItem.Text          = button[16];
            restartToolStripMenuItem1.Text           = button[17];
            autostartToolStripMenuItem.Text          = button[18];
            Msgloadworld                             = button[19];
            Msgstartingrealm                         = button[20];
            Msgstartingmy                            = button[21];
            Msgresetbots                             = button[22];
            Msgreadytoplay                           = button[23];
            Msgwould                                 = button[24];
            Msgupstart                               = button[25];
            Msgmangoscrash                           = button[26];
            Msgrestart                               = button[27];
            Msgcheckup                               = button[28];
            Msgnewver                                = button[29];
            Msgnewver2                               = button[30];
            Msgnewservera                            = button[31];
            Msgstatusup                              = button[32];
            Msgimportchar                            = button[33];
            Msgdecompress                            = button[34];
            Msgimportacc                             = button[35];
            Msgimportcomp                            = button[36];
            Msgexportchar                            = button[37];
            Msgexportchar1                           = button[38];
            Msgexportacc                             = button[39];
            Msgcompress                              = button[40];
            Msgexportcomp                            = button[41];
            Msgaboutmsg1                             = button[42];
            Msgaboutmsg2                             = button[43];
            Msgaboutver                              = button[44];
            Msgstarting                              = button[45];
            Msgserverrate                            = button[46];
            Msgitemdrop                              = button[47];
            Msgyourmail                              = button[48];
            Msgbugtype                               = button[49];
            Msgdesc                                  = button[50];
            Msgsysinfo                               = button[51];
            Msgsendreport                            = button[52];
            Msgtitle                                 = button[53];
            Msgbotsettings                           = button[54];
            Msgbots                                  = button[55];
            Msgreport                                = button[56];
            Msgminbots                               = button[57];
            Msgmaxbots                               = button[58];
            Msgminbotsi                              = button[59];
            Msgmaxbotsi                              = button[60];
            Msgbotsacc                               = button[61];
            Msgupinter                               = button[62];
            Msgbotmaps                               = button[63];
            Msgother                                 = button[64];
            Msgbotacc                                = button[65];
            Msgbotlfg                                = button[66];
            Msgrnd                                   = button[67];
            Msgloaddef                               = button[68];
            Msgsave                                  = button[69];
        }

        public void Land()
        {
            button[0]  = runWoWToolStripMenuItem.Text;
            button[1]  = toolStripMenuItem1.Text;
            button[2]  = toolsToolStripMenuItem.Text;
            button[3]  = restartToolStripMenuItem.Text;
            button[4]  = reportBugToolStripMenuItem.Text;
            button[5]  = tsmHelpUs.Text;
            button[6]  = aboutToolStripMenuItem.Text;
            button[7]  = exitToolStripMenuItem2.Text;
            button[8]  = worldSettingsToolStripMenuItem.Text;
            button[9]  = botSettingsToolStripMenuItem.Text;
            button[10] = changeWoWPathToolStripMenuItem1.Text;
            button[11] = resetAllRandomBotsToolStripMenuItem.Text;
            button[12] = randomizeBotsToolStripMenuItem.Text;
            button[13] = exportCharactersToolStripMenuItem.Text;
            button[14] = lanSwitcherToolStripMenuItem1.Text;
            button[15] = accountToolToolStripMenuItem1.Text;
            button[16] = startstopToolStripMenuItem.Text;
            button[17] = restartToolStripMenuItem1.Text;
            button[18] = autostartToolStripMenuItem.Text;
            button[19] = Msgloadworld;
            button[20] = Msgstartingrealm;
            button[21] = Msgstartingmy;
            button[22] = Msgresetbots;
            button[23] = Msgreadytoplay;
            button[24] = Msgwould;
            button[25] = Msgupstart;
            button[26] = Msgmangoscrash;
            button[27] = Msgrestart;
            button[28] = Msgcheckup;
            button[29] = Msgnewver;
            button[30] = Msgnewver2;
            button[31] = Msgnewservera;
            button[32] = Msgstatusup;
            button[33] = Msgimportchar;
            button[34] = Msgdecompress;
            button[35] = Msgimportacc;
            button[36] = Msgimportcomp;
            button[37] = Msgexportchar;
            button[38] = Msgexportchar1;
            button[39] = Msgexportacc;
            button[40] = Msgcompress;
            button[41] = Msgexportcomp;
            button[42] = Msgaboutmsg1;
            button[43] = Msgaboutmsg2;
            button[44] = Msgaboutver;
            button[45] = Msgstarting;
            button[46] = Msgserverrate;
            button[47] = Msgitemdrop;
            button[48] = Msgyourmail;
            button[49] = Msgbugtype;
            button[50] = Msgdesc;
            button[51] = Msgsysinfo;
            button[52] = Msgsendreport;
            button[53] = Msgtitle;
            button[54] = Msgbotsettings;
            button[55] = Msgbots;
            button[56] = Msgreport;
            button[57] = Msgminbots;
            button[58] = Msgmaxbots;
            button[59] = Msgminbotsi;
            button[60] = Msgmaxbotsi;
            button[61] = Msgbotacc;
            button[62] = Msgupinter;
            button[63] = Msgbotmaps;
            button[64] = Msgother;
            button[65] = Msgbotsacc;
            button[66] = Msgbotlfg;
            button[67] = Msgrnd;
            button[68] = Msgloaddef;
            button[69] = Msgsave;
        }

        #endregion

        #region Servers
        
        private void SetText(string text)
        {
            _realm += text;
        }


        public float getCurrentCpuUsage()
        {
            return cpuCounter.NextValue();
            
        }

        public int getAvailableRAM()
        {
            return Convert.ToInt32(ramCounter.NextValue());
        } 

        private void SetText3(string text1)
        {
            if (richTextBox1.InvokeRequired)
            {
                SetTextCallback d = SetText3;
                Invoke(d, new object[] {text1});
            }
            else
            {
                _sql               += text1;
                richTextBox1.Text  += text1 + Environment.NewLine;
                File.WriteAllText("logs\\mysql.txt", richTextBox1.Text);
            }
        }

        private void SetText1(string text)
        {
            if (rtWorldDev.InvokeRequired)
            {
                SetTextCallback d = SetText1;
                Invoke(d, new object[] {text});
            }
            else
            {
                _world += text;

                if (_allowtext)
                {
                    rtWorldDev.Text          += _dt.ToString("(" + "HH:mm" + ") ") + text + Environment.NewLine;
                    rtWorldDev.SelectionStart = rtWorldDev.Text.Length;
                    rtWorldDev.ScrollToCaret();
                }
            }
        }

        public void WorldStart()
        {
            _start1              = DateTime.Now;
            pbarWorld.Visible    = true;
            pbTempW.Visible      = true;
            pbNotAvailW.Visible  = false;
            _status              = Msgloadworld;
            WindowSize(false);
            tmrWorld.Start();
            var cmdStartInfo                    = new ProcessStartInfo(lwPath + "mangosd.exe");
            cmdStartInfo.CreateNoWindow         = true;
            cmdStartInfo.RedirectStandardInput  = true;
            cmdStartInfo.RedirectStandardOutput = true;
            cmdStartInfo.RedirectStandardError  = true;
            cmdStartInfo.UseShellExecute        = false;
            cmdStartInfo.WindowStyle            = ProcessWindowStyle.Hidden;

            _cmd1 = new Process();
            _cmd1.StartInfo = cmdStartInfo;


            if (_cmd1.Start())
            {
                _cmd1.OutputDataReceived += _cmd_OutputDataReceived1;
                _cmd1.ErrorDataReceived  += _cmd_ErrorDataReceived1;
                _cmd1.Exited             += _cmd_Exited1;
                _cmd1.BeginOutputReadLine();
                _cmd1.BeginErrorReadLine();
            }
            else
            {
                _cmd1 = null;
            }
        }

        public void RealmdStart()
        {
            _start1              = DateTime.Now;
            _status              = Msgstartingrealm;
            tssStatus.Image = Properties.Resources.search_animation;
            pbTempR.Visible      = true;
            pbNotAvailR.Visible  = false;
            tmrRealm.Start();
            var cmdStartInfo                    = new ProcessStartInfo(lwPath + "login.exe");
            cmdStartInfo.CreateNoWindow         = true;
            cmdStartInfo.RedirectStandardInput  = true;
            cmdStartInfo.RedirectStandardOutput = true;
            cmdStartInfo.RedirectStandardError  = true;
            cmdStartInfo.UseShellExecute        = false;
            cmdStartInfo.WindowStyle            = ProcessWindowStyle.Hidden;
            _cmd                                = new Process();
            _cmd.StartInfo                      = cmdStartInfo;

            if (_cmd.Start())
            {
                _cmd.OutputDataReceived += _cmd_OutputDataReceived;
                _cmd.ErrorDataReceived  += _cmd_ErrorDataReceived;
                _cmd.Exited             += _cmd_Exited;
                _cmd.BeginOutputReadLine();
                _cmd.BeginErrorReadLine();
            }
            else
            {
                _cmd = null;
            }
        }

        private void _cmd_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            UpdateConsole(e.Data);
        }

        private void _cmd_ErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
            UpdateConsole(e.Data, Brushes.Red);
        }

        private void _cmd_Exited(object sender, EventArgs e)
        {
            _cmd.OutputDataReceived -= _cmd_OutputDataReceived;
            _cmd.Exited             -= _cmd_Exited;
        }

        private void UpdateConsole(string text)
        {
            UpdateConsole(text, null);
        }

        private void UpdateConsole(string text, Brush color)
        {
            WriteLine(text, color);
        }

        private void WriteLine(string text, Brush color)
        {
            if (text != null)
            {
                SetText(text);
            }
        }

        private void _cmd_OutputDataReceived1(object sender, DataReceivedEventArgs e)
        {
            UpdateConsole1(e.Data);
        }

        private void _cmd_ErrorDataReceived1(object sender, DataReceivedEventArgs e)
        {
            UpdateConsole1(e.Data, Brushes.Red);
        }

        private void _cmd_Exited1(object sender, EventArgs e)
        {
            _cmd1.OutputDataReceived -= _cmd_OutputDataReceived1;
            _cmd1.Exited             -= _cmd_Exited1;
        }

        private void UpdateConsole1(string text)
        {
            UpdateConsole1(text, null);
        }

        private void UpdateConsole1(string text, Brush color)
        {
            WriteLine1(text, color);
        }

        private void WriteLine1(string text, Brush color)
        {
            if (text != null)
            {
                SetText1(text);
            }
        }

        private void Launcher_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                Hide();
            }
        }

        private void NotifyBallon(int timeout, string title, string msg, bool crashed)
        {
            if (crashed)
                SppTray.ShowBalloonTip(timeout, title, msg, ToolTipIcon.Warning);
            else
                SppTray.ShowBalloonTip(timeout, title, msg, ToolTipIcon.Info);
        }

        public void StartAll()
        {
            _start1 = DateTime.Now;
            if (!_restart)
            {
                _status              = Msgstartingmy;
                animateStatus(true);
                pbTempM.Visible      = true;
                pbNotAvailM.Visible  = false;
                SqlStartCheck.Start();
                var cmdStartInfo = new ProcessStartInfo("cmd.exe",
                                                        "/C " + sqlpath + "mysqld.exe" +
                                                        " --defaults-file=" + sqlpath + "my.ini" +
                                                        " --standalone --console");
                cmdStartInfo.CreateNoWindow         = true;
                cmdStartInfo.RedirectStandardInput  = true;
                cmdStartInfo.RedirectStandardOutput = true;
                cmdStartInfo.RedirectStandardError  = true;
                cmdStartInfo.UseShellExecute        = false;
                cmdStartInfo.WindowStyle            = ProcessWindowStyle.Hidden;

                _cmd3 = new Process();
                try
                {
                    _cmd3.StartInfo = cmdStartInfo;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Some exception: mysql\n" + ex.Message);
                }

                if (_cmd3.Start())
                {
                    _cmd3.OutputDataReceived += _cmd_OutputDataReceived3;
                    _cmd3.ErrorDataReceived  += _cmd_ErrorDataReceived3;
                    _cmd3.Exited             += _cmd_Exited3;
                    _cmd3.BeginOutputReadLine();
                    _cmd3.BeginErrorReadLine();
                }
                else
                {
                    _cmd3 = null;
                }
            }
            else
            {
                pbAvailableM.Visible = true;
                RealmdStart();
            }
        }

        private void _cmd_OutputDataReceived3(object sender, DataReceivedEventArgs e)
        {
            UpdateConsole3(e.Data);
        }

        private void _cmd_ErrorDataReceived3(object sender, DataReceivedEventArgs e)
        {
            UpdateConsole3(e.Data, Brushes.Red);
        }

        private void _cmd_Exited3(object sender, EventArgs e)
        {
            _cmd3.OutputDataReceived -= _cmd_OutputDataReceived3;
            _cmd3.Exited             -= _cmd_Exited3;
        }

        private void UpdateConsole3(string text)
        {
            UpdateConsole3(text, null);
        }

        private void UpdateConsole3(string text, Brush color)
        {
            WriteLine3(text, color);
        }

        private void WriteLine3(string text, Brush color)
        {
            if (text != null)
            {
                SetText3(text);
            }
        }

        private delegate void SetTextCallback(string text);

        private delegate void SetTextCallback1(string text);

        private delegate void SetTextCallback3(string text);

        private void Check_Tick(object sender, EventArgs e)
        {
            if (!CheckRunwow())
            {
                Check.Stop();
                RealmRestore();
            }
        }

        private void rstchck_DoWork(object sender, DoWorkEventArgs e)
        {
            _status = Msgresetbots;
            tssStatus.Image = Properties.Resources.search_animation;
            Thread.Sleep(10000);
        }

        private void rstchck_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            MenuItemsDisableAfterLoad();
            RealmWorldRestart();
        }

        #endregion

        #region ProcessTasks

        public void CloseProcess(bool restart)
        {
            try
            {
                foreach (Process proc in Process.GetProcessesByName("mangosd"))
                {
                    _cmd1.StandardInput.WriteLine("save");
                    Thread.Sleep(1000);

                    _status = "World Shutdown";
                    _cmd1.StandardInput.WriteLine("server shutdown 0");
                    Thread.Sleep(300);
                    proc.Kill();
                }

                foreach (Process proc in Process.GetProcessesByName("login"))
                {
                    tssStatus.IsLink  = false;
                    _status           = "Login Shutdown";
                    proc.Kill();
                }

                foreach (Process proc in Process.GetProcessesByName("mysqld"))
                {
                    if (!restart)
                        ShutdownSql();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Some Exception: CloseProcess\n" +
                                ex.Message, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void SearchProcess()
        {
            try
            {
                foreach (Process proc in Process.GetProcessesByName("mangosd"))
                {
                    DialogResult result =
                        MessageBox.Show("Mangosd process is running.\nYou want close the running process?", "Warning",
                                        MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                    if (result == DialogResult.Yes)
                    {
                        proc.Kill();
                    }
                    else
                    {
                        Loader.Kill = true;
                    }
                }

                Thread.Sleep(100);

                foreach (Process proc in Process.GetProcessesByName("login"))
                {
                    DialogResult result =
                        MessageBox.Show("Login process is running.\nYou want close the running process?", "Warning",
                                        MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                    if (result == DialogResult.Yes)
                    {
                        proc.Kill();
                    }
                    else
                    {
                        Loader.Kill = true;
                    }
                }

                foreach (Process proc in Process.GetProcessesByName("mysqld"))
                {
                    DialogResult result =
                        MessageBox.Show("Mysqld process is running.\nYou want close the running process?", "Warning",
                                        MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                    if (result == DialogResult.Yes)
                    {
                        proc.Kill();
                    }
                    else
                    {
                        Loader.Kill = true;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Some Exception: SearchProcess\n" +
                                ex.Message, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        #endregion

        #region OtherMethods

        public int getmemory() {return Convert.ToInt32(new Microsoft.VisualBasic.Devices.ComputerInfo().TotalPhysicalMemory / 1024 / 1024);}

        public void MenuItemsDisableAfterLoad()
        {
            tssStatus.IsLink                                = false;
            resetAllRandomBotsToolStripMenuItem.Enabled     = false;
            exportCharactersToolStripMenuItem.Enabled       = false;
            exportImportCharactersToolStripMenuItem.Enabled = false;
            startstopToolStripMenuItem.Enabled              = false;
            randomizeBotsToolStripMenuItem.Enabled          = false;
            resetBotsToolStripMenuItem.Enabled              = false;
            randomizeBotsToolStripMenuItem1.Enabled         = false;
            restartToolStripMenuItem1.Enabled               = false;
            restartToolStripMenuItem2.Enabled               = false;
            lanSwitcherToolStripMenuItem1.Enabled           = false;
            lanSwitcherToolStripMenuItem.Enabled            = false;
        }

        public void StatusIcon()
        {
            if (!_restart)
            {
                pbNotAvailM.Visible = true;
            }
            //red
            pbNotAvailR.Visible = true;
            pbNotAvailW.Visible = true;
            //yellow
            pbTempM.Visible = false;
            pbTempR.Visible = false;
            pbTempW.Visible = false;
            //green
            pbAvailableM.Visible = false;
            pbAvailableR.Visible = false;
            pbAvailableW.Visible = false;
        }

        public void font()
        {
            try
            {
                var pfc = new PrivateFontCollection();
                pfc.AddFontFile("data\\font\\Wfont.ttf");
                lblMysql.Font = new Font(pfc.Families[0], 26, FontStyle.Regular);
                lblRealm.Font = new Font(pfc.Families[0], 26, FontStyle.Regular);
                lblWorld.Font = new Font(pfc.Families[0], 26, FontStyle.Regular);
            }
            catch (Exception)
            {
            }
        }

        private void WindowSize(bool min)
        {
            if (min)
            {
                groupBox1.Height = 140;
                Height           = 230;
            }
            else
            {
                groupBox1.Height = 174;
                Height           = 265;
            }
        }

        public void Traymsg()
        {
            if (!_restart)
            {
                NotifyBallon(1000, Msgserverres, Msgreadytoplay, false);
            }
            else
            {
                NotifyBallon(1000, Msgserverres, Msgreadytoplay, false);
                _restart = false;
            }
            _world = "";
        }

        public void startWow()
        {
            CheckLanIpInRealmDatabase();

            if (wowExePath == "" || realmListPath == "")
            {
                DialogMethod();
            }
            else
            {
                RealmChange();
                Process.Start(wowExePath);
                Check.Start();
                
            }
        }

        public void DialogMethod()
        {
            var dialog = new OpenFileDialog();

            dialog.InitialDirectory = "c:\\";
            dialog.Filter           = "Executable (*wow.exe)|*wow.exe";
            dialog.FilterIndex      = 2;
            dialog.RestoreDirectory = true;

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    wowExePath      = dialog.FileName;
                    realmDialogPath = Path.GetDirectoryName(wowExePath);
                    RealmDialog();

                }
                catch (Exception)
                {

                }
            }
        }

        public void RealmDialog()
        {
            var dialog = new OpenFileDialog();

            dialog.InitialDirectory = realmDialogPath;
            dialog.Filter           = "Realmlist File (*realmlist.wtf)|*realmlist.wtf";
            dialog.FilterIndex      = 2;
            dialog.RestoreDirectory = true;

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                realmListPath = dialog.FileName;
                saveMethod();
                RealmChange();
                Process.Start(wowExePath);
                Check.Start();
            }
        }

        public void RealmChange()
        {
            try
            {
                OLDrealmList = File.ReadAllText(realmListPath, Encoding.UTF8);
                NotifyBallon(1000, "Realmlist", "Set 127.0.0.1", false);

            }
            catch (Exception ex)
            {
                MessageBox.Show("Some Exception: RealmChange\n" + ex.Message);
            }

            try
            {
                File.WriteAllText(realmListPath, "set realmlist 127.0.0.1");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Some Exception: RealmWrite\n" + ex.Message);

            }
        }

        public void RealmRestore()
        {
            try
            {
                File.WriteAllText(realmListPath,OLDrealmList);
                NotifyBallon(1000,"Realmlist","Restored",false);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Some Exception: RealmRestore\n" + ex.Message);
            }
        }

        public static void saveMethod()
        {
            var writer = new XmlTextWriter("config\\SppPathConfig.xml", Encoding.UTF8);
            writer.Formatting = Formatting.Indented;
            writer.WriteStartDocument();
            writer.WriteStartElement("Config");
            writer.WriteElementString("GamePath", wowExePath);
            writer.WriteElementString("RealmWTF", realmListPath);
            writer.WriteElementString("ResetBots", resetBots);
            writer.WriteElementString("RandomizeBots", randomizeBots);
            writer.WriteElementString("Autostart", autostart);
            writer.WriteElementString("Lang",lang);

            writer.WriteEndElement();
            writer.Close();
        }

        private void ReadXML()
        {
            try
            {
                var doc           = new XmlDocument();
                doc.Load("config\\SppPathConfig.xml");
                XmlElement root   = doc.DocumentElement;
                XmlNodeList nodes = root.SelectNodes("/Config");

                foreach (XmlNode node in nodes)
                {
                    wowExePath    = node["GamePath"].InnerText;
                    realmListPath = node["RealmWTF"].InnerText;
                    autostart     = node["Autostart"].InnerText;
                    lang = node["Lang"].InnerText;
                }

                switch (lang)
                {
                    case "Hungarian":
                                    groupBox1.Width = 683;
            Width = 715;
                        pbNotAvailM.Location = new Point(167, 64);
                        pbNotAvailR.Location = new Point(313, 64);
                        pbNotAvailW.Location = new Point(464, 64);
                        pbTempM.Location = new Point(167, 64);
                        pbTempR.Location = new Point(313, 64);
                        pbTempW.Location = new Point(464, 64);
                        pbAvailableM.Location = new Point(167, 64);
                        pbAvailableR.Location = new Point(313, 64);
                        pbAvailableW.Location = new Point(464, 64);
                        pbarWorld.Location = new Point(167, 136);
                        lblMysql.Location = new Point(134, 26);
                        lblRealm.Location = new Point(297, 26);
                        lblWorld.Location = new Point(444, 26);
                        magyarToolStripMenuItem.Checked = true;
                        break;
                    case "English":
                                                groupBox1.Width = 575;
            Width = 605;
                                    pbNotAvailM.Location = new Point(105, 64);

            pbNotAvailR.Location = new Point(251, 64);

            pbNotAvailW.Location = new Point(402, 64);

            pbTempM.Location = new Point(105, 64);

            pbTempR.Location = new Point(251, 64);

            pbTempW.Location = new Point(402, 64);

            pbAvailableM.Location = new Point(105, 64);

            pbAvailableR.Location = new Point(251, 64);

            pbAvailableW.Location = new Point(402, 64);


            pbarWorld.Location = new Point(105, 136);

            lblMysql.Location = new Point(72, 26);

            lblRealm.Location = new Point(235, 26);

            lblWorld.Location = new Point(382, 26);
                        englishToolStripMenuItem.Checked = true;
                        break;
                    case "French":
                        frenchToolStripMenuItem.Checked = true;
                        break;
                    case "German":
                        germanToolStripMenuItem.Checked = true;
                        break;
                }
            }
            catch
            {
                saveMethod();
            }
        }

        private void CheckLanIpInRealmDatabase()
        {
            try
            {
                var client = new MySQLClient("127.0.0.1", "realmd", "root", "123456", 3310);
                ipAdress   = client.Select("realmlist", "id='1'")["address"];
            }
            catch (Exception)
            {
            }
        }

        private void input_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyData)
            {
                case Keys.Enter:
                    _cmd1.StandardInput.WriteLine(txbWorldDev.Text);
                    txbWorldDev.Text = "";
                    break;
                case Keys.Escape:
                    txbWorldDev.Text = "";
                    break;
            }
        }

        private static void ShutdownSql()
        {
            var startInfo             = new ProcessStartInfo();
            startInfo.CreateNoWindow  = true;
            startInfo.UseShellExecute = false;
            startInfo.FileName        = "Database\\bin\\mysqladmin.exe";
            startInfo.Arguments       = "-u root -p123456 --port=3310 shutdown";
            startInfo.WindowStyle     = ProcessWindowStyle.Hidden;

            try
            {
                using (Process exeProcess = Process.Start(startInfo))
                {
                    exeProcess.WaitForExit();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public void DevWindow(bool active)
        {
            if (!active)
            {
                Size               = new Size(537, 460);
                rtWorldDev.Visible = true;
            }

            if (active)
            {
                Size               = new Size(537, 251);
                rtWorldDev.Visible = false;
            }
        }

        public void ResetBots()
        {
            _cmd1.StandardInput.WriteLine("rndbot reset");

            DialogResult dialog = MessageBox.Show("Restart is required for reset bots.\nRestart now?", "Question",
                                                  MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (dialog == DialogResult.Yes)
            {
                rstchck.RunWorkerAsync();
            }
        }

        public void RandomizeBotsMethod()
        {
            _cmd1.StandardInput.WriteLine("rndbot update");
            MessageBox.Show(Msgupstart);
        }

        public bool CheckRunwow()
        {
            foreach (Process proc in Process.GetProcessesByName("Wow"))
            {
                return true;
            }
            return false;
        }

        public bool ProcessView()
        {
            foreach (Process proc in Process.GetProcessesByName("mangosd"))
            {
                return false;
            }
            return true;
        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {
            if (rtWorldDev.TextLength >= 10000)
            {
                rtWorldDev.Clear();
            }
        }

        private void StatusBarUpdater_Tick(object sender, EventArgs e)
        {
            tssStatus.Text = _status;
        }

        public void GetLocalSrvVer()
        {
            try
            {
                CurrEmuVer = Convert.ToDouble(File.ReadAllText("SingleCore\\version"));
            }
            catch (Exception)
            { }
        }

        #endregion

        #region WindowMenuItems

        private void exportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            export();
        }

        private void importToolStripMenuItem_Click(object sender, EventArgs e)
        {
            import();
        }

        private void showToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Show();
            Application.OpenForms["Launcher"].Activate();
        }

        private void exitToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            Hide();
            GetSqlOnlineBot.Stop();
            Thread.Sleep(200);
            CloseProcess(false);
            SppTray.Visible = false;
            Loader.Kill     = true;
        }

        private void worldSettingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Application.OpenForms["WorldConf"] == null)
            {
                var show = new WorldConf();
                show.Show();
            }
            else
            {
                Application.OpenForms["WorldConf"].Activate();
            }
        }

        private void worldSettingsToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (Application.OpenForms["WorldConf"] == null)
            {
                var show = new WorldConf();
                show.Show();
            }
            else
            {
                Application.OpenForms["WorldConf"].Activate();
            }
        }

        private void tsmHelpUs_Click(object sender, EventArgs e)
        {
            if (Application.OpenForms["HelpUsWindow"] == null)
            {
                var show = new HelpUsWindow();
                show.Show();
            }
            else
            {
                Application.OpenForms["HelpUsWindow"].Activate();
            }
        }

        private void reportBugToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Application.OpenForms["BugReport"] == null)
            {
                var show = new BugReport();
                show.Show();
            }
            else
            {
                Application.OpenForms["BugReport"].Activate();
            }
        }

        private void reportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var sReport = new BugReport();
            sReport.Show();

            if (Application.OpenForms["BugReport"] == null)
            {
                var show = new BugReport();
                show.Show();
            }
            else
            {
                Application.OpenForms["BugReport"].Activate();
            }
        }

        private void aboutToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (Application.OpenForms["AboutBox"] == null)
            {
                var show = new AboutBox();
                show.Show();
            }
            else
            {
                Application.OpenForms["Aboutbot"].Activate();
            }
        }

        private void tssStatus_Click(object sender, EventArgs e)
        {
            Process.Start(UpdLink);
        }

        #endregion

        #region TrayMenu

        private void botSettingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var botConf = new BotConf();
            botConf.Show();
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Application.OpenForms["AboutBox"] == null)
            {
                var show = new AboutBox();
                show.Show();
            }
            else
            {
                Application.OpenForms["Aboutbox"].Activate();
            }
        }

        private void botSettingsToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            var bot = new BotConf();
            bot.Show();
        }

        private void SppTray_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                SppTray.ContextMenuStrip = cmsTray;
                MethodInfo mi = typeof (NotifyIcon).GetMethod("ShowContextMenu",BindingFlags.Instance | BindingFlags.NonPublic);
                mi.Invoke(SppTray, null);
                SppTray.ContextMenuStrip = cmsTray;
            }
        }

        private void SppTray_DoubleClick(object sender, EventArgs e)
        {
            Show();
            Application.OpenForms["Launcher"].Activate();
        }

        private void startWowToolStripMenuItem_Click(object sender, EventArgs e)
        {
            startWow();
        }

        private void lanSwitcherToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Process.Start("Tools\\IP_Changer.exe");
        }

        private void accountToolToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Process.Start("Tools\\AccountCreator.exe");
        }

        private void accountToolAdvancedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Process.Start("Tools\\AccountCreator_adv.exe");
        }

        private void changeWoWPathToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogMethod();
        }

        private void runWoWToolStripMenuItem_Click(object sender, EventArgs e)
        {
            startWow();
        }

        private void lanSwitcherToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            //Process.Start("Tools\\IP_Changer.exe");

            var LanSwitcher = new LanSwitcher();
            LanSwitcher.Show();

        }

        private void accountToolToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            Process.Start("Tools\\AccountCreator.exe");
        }

        private void changeWoWPathToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            DialogMethod();
        }

        private void exitToolStripMenuItem2_Click(object sender, EventArgs e)
        {
            Hide();
            CheckMangosCrashed.Stop();
            GetSqlOnlineBot.Stop();
            Thread.Sleep(200);
            CloseProcess(false);

            SppTray.Visible = false;
            Loader.Kill     = true;
        }

        public void RealmWorldRestart()
        {
           
            rtWorldDev.Visible = false;
            CheckMangosCrashed.Stop();
            _allowtext          = false;
            _restart            = true;
            CloseProcess(true);
            StatusIcon();
            WindowState        = FormWindowState.Normal;
            Show();
            StartAll();
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == (Keys.Control | Keys.D))
            {
                if (!_allowdev)
                {
                    _allowdev = true;
                    DevWindow(true);
                }
                else
                {
                    _allowdev = false;
                    DevWindow(false);
                }


                return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void resetAllRandomBotsToolStripMenuItem_Click(object sender, EventArgs e)
        {ResetBots();}

        private void randomizeBotsToolStripMenuItem_Click(object sender, EventArgs e)
        {RandomizeBotsMethod();}

        private void resetBotsToolStripMenuItem_Click(object sender, EventArgs e)
        {ResetBots();}

        private void randomizeBotsToolStripMenuItem1_Click(object sender, EventArgs e)
        {RandomizeBotsMethod();}

        private void cmsTray_DoubleClick(object sender, EventArgs e)
        {WindowState = FormWindowState.Normal;}

        #endregion

        #region Timers

        private void tmrUsage_Tick(object sender, EventArgs e)
        {
            int ramavail  = getAvailableRAM();
            int ramtoltal = Convert.ToInt32(getmemory());
            int ramfree   = ramtoltal - ramavail;
            int ramp      = ramtoltal / 100;
            int perecent  = ramfree / ramp;
            Process proc  = Process.GetCurrentProcess();

            try
            {
                Process[] SqlProcesses;
                SqlProcesses = Process.GetProcessesByName("mysqld");
                _sqlMem       = Convert.ToString(SqlProcesses[0].WorkingSet64 / 1024 / 1024) + "MB";
            }
            catch (Exception)
            {
                _sqlMem = "N/A";
            }

            try
            {
                Process[] mangosdProcesses;
                mangosdProcesses = Process.GetProcessesByName("mangosd");
                _mangosdMem       = Convert.ToString(mangosdProcesses[0].WorkingSet64 / 1024 / 1024) + "MB";
            }
            catch (Exception)
            {
                _mangosdMem = "N/A";
            }

            try
            {
                Process[] realmdProcesses;
                realmdProcesses = Process.GetProcessesByName("login");
                _realmdMem       = Convert.ToString(realmdProcesses[0].WorkingSet64 / 1024 / 1024) + "MB";
            }
            catch (Exception)
            {
                _realmdMem = "N/A";
            }

            tssUsage.ToolTipText = "Launcher: " + Convert.ToString(proc.WorkingSet64 / 1024 / 1024) + "MB" + "\nMysql Server: " + _sqlMem + "\nLogin Server: " + _realmdMem + "\nGame Server: " + _mangosdMem;

            tssUsage.Text = "CPU: " + Convert.ToInt32(getCurrentCpuUsage()) + "% |" + " RAM: " + perecent + "%" + " (" + ramfree + "/" + ramtoltal + "MB)";


        }

        //private void timer1_Tick(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        double version = Convert.ToDouble(File.ReadAllText("SingleCore\\version"));

        //        if (version > CurrEmuVer)
        //        {
        //            UpdateCompleteChecker.Stop();
        //            if (RemoteEmuVer > version)
        //            {
        //                AllowUpdaterQuestion = true;
        //                bwUpdate.RunWorkerAsync();
        //            }


        //            if (!AllowUpdaterQuestion)
        //            {
        //                Thread.Sleep(20);
        //                foreach (Process proc in Process.GetProcessesByName("Updater"))
        //                {
        //                    proc.Kill();
        //                }

        //                Show();
        //                NotifyBallon(1000, "Update Completed", "Server starting...", false);
        //                try
        //                {
        //                    Process.Start("notepad.exe", "Update\\changelog");
        //                }
        //                catch (Exception)
        //                {
        //                }
        //                RealmWorldRestart();

        //                allowupdaternorunwow = false;
        //            }
        //        }
        //    }
        //    catch (Exception)
        //    {
        //    }
        //}

        private void tmrRealm_Tick(object sender, EventArgs e)
        {
            try
            {
                if (_realm.Contains("realmd process priority class set to HIGH"))
                {
                    tmrRealm.Stop();
                    DateTime end1          = DateTime.Now;
                    lblRealmStartTime.Text = (end1 - _start1).TotalSeconds.ToString();
                    pbAvailableR.Visible   = true;
                    pbTempR.Visible        = false;
                    WorldStart();
                }
            }
            catch
            {
            }
        }

        private void tmrWorld_Tick(object sender, EventArgs e)
        {
            try
            {
                if (_world.Contains("Loading Script Names..."))
                {
                    pbarWorld.Value = 10;
                }
                if (_world.Contains("Loading Item converts..."))
                {
                    pbarWorld.Value = 20;
                    _world = "";
                }
                if (_world.Contains("Loading pet levelup spells..."))
                {
                    pbarWorld.Value = 30;
                    _world = "";
                }
                if (_world.Contains("Loading Quests Relations..."))
                {
                    pbarWorld.Value = 40;
                    _world = "";
                }
                if (_world.Contains("Loading the max pet number..."))
                {
                    pbarWorld.Value = 50;
                    _world = "";
                }
                if (_world.Contains("Loading Achievements..."))
                {
                    pbarWorld.Value = 60;
                    _world = "";
                }
                if (_world.Contains("Waypoint templates loaded"))
                {
                    pbarWorld.Value = 70;
                    _world = "";
                }
                if (_world.Contains("Loading GameTeleports..."))
                {
                    pbarWorld.Value = 80;
                    _world = "";
                }
                if (_world.Contains("Initialize AuctionHouseBot..."))
                {
                    pbarWorld.Value = 90;
                }
                if (_world.Contains("WORLD: World initialized"))
                {
                    tmrWorld.Stop();

                    DateTime end1           = DateTime.Now;
                    lblWorldStartTime.Text  = (end1 - _start1).TotalSeconds.ToString();
                    pbarWorld.Value         = 100;
                    _status                 = "Online";
                    animateStatus(false);
                    pbAvailableW.Visible    = true;
                    pbTempW.Visible         = false;
                    pbarWorld.Visible       = false;
                    WindowSize(true);
                    pbarWorld.Value       = 0;
                    _allowtext            = true;
                    _allowdev              = true;

                    resetAllRandomBotsToolStripMenuItem.Enabled     = true;
                    randomizeBotsToolStripMenuItem.Enabled          = true;
                    resetBotsToolStripMenuItem.Enabled              = true;
                    randomizeBotsToolStripMenuItem1.Enabled         = true;
                    lanSwitcherToolStripMenuItem1.Enabled           = true;
                    lanSwitcherToolStripMenuItem.Enabled            = true;
                    startstopToolStripMenuItem.Enabled              = true;
                    startToolStripMenuItem.Enabled                  = true;
                    restartToolStripMenuItem1.Enabled               = true;
                    restartToolStripMenuItem1.Enabled               = true;
                    exportCharactersToolStripMenuItem.Enabled       = true;
                    exportImportCharactersToolStripMenuItem.Enabled = true;
                    
                    bwUpdate.RunWorkerAsync(); //check update

                    Traymsg();
                    GetSqlOnlineBot.Start(); //get online bots

                    if (Updater)
                    {
                        Updater = false;
                    }

                    CheckMangosCrashed.Start();
                    SrvAnnounce.Start();
                }
            }
            catch
            {
            }
        }

        private void SqlStartCheck_Tick(object sender, EventArgs e)
        {
            try
            {
                if (_sql.Contains("Version: '10.0.3-MariaDB'  socket: ''  port: 3310  mariadb.org binary distribution"))
                {
                    SqlStartCheck.Stop();
                    DateTime end1         = DateTime.Now;
                    lblSqlStartTime.Text  = (end1 - _start1).TotalSeconds.ToString();
                    pbAvailableM.Visible  = true;
                    pbTempM.Visible       = false;
                    _sql                  = "";
                    RealmdStart();
                }
            }
            catch
            {
            }
        }

        private void CheckWowRun_Tick(object sender, EventArgs e)
        {
            CheckWowRun.Stop();

            Traymsg();

            if (!_restart && !CheckRunwow())
            {
                DialogResult dialog = MessageBox.Show(Msgwould, "Question",
                                                      MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (dialog == DialogResult.Yes)
                {
                    startWow();
                }
            }
        }

        private void GetSqlOnlineBot_Tick(object sender, EventArgs e)
        {
            tssLOnline.Text = new Onlinebot().GetBot();
            SppTray.Text    = tssLOnline.Text;
        }

        private void SrvAnnounce_Tick(object sender, EventArgs e)
        {
            SrvAnnounce.Stop();
            _cmd1.StandardInput.WriteLine("announce " + online);
        }

        private void CheckMangosCrashed_Tick(object sender, EventArgs e)
        {
            if (ProcessView() && !Updater)
            {
                _restart = true;
                NotifyBallon(1000, Msgmangoscrash, Msgrestart, true);
                RealmWorldRestart();
            }
        }

        #endregion

        #region Update

        private void WasThisUpdate()
        {
            if (File.Exists("SppLauncher_OLD.exe"))
            {
                File.Delete("SppLauncher_OLD.exe");
                var uC = new Update_Completed();
                uC.Show();
                _update = true;
            }
        }

        private void bwUpdate_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                _status               = Msgcheckup;
                animateStatus(true);
                var client            = new WebClient();
                Stream stream         = client.OpenRead("https://raw.github.com/conan513/SingleCore/SPP/Tools/update.txt");
                var reader            = new StreamReader(stream);
                String content        = reader.ReadToEnd();
                string[] parts        = content.Split(';');
                content               = parts[0];
                RemoteEmuVer          = Convert.ToDouble(parts[1]);
                UpdLink               = parts[2];
                Available             = true;
                RemoteProgVer         = content;
                allowupdaternorunwow  = true;

                if (content != currProgVer)
                {
                    if (
                        MessageBox.Show(Msgnewver + content + "\n" + Msgyouwant,
                                        Msgnewver2, MessageBoxButtons.YesNo,
                                        MessageBoxIcon.Asterisk, MessageBoxDefaultButton.Button1,
                                        MessageBoxOptions.DefaultDesktopOnly) == DialogResult.Yes)
                    {
                        _updateYes = true;
                    }
                    else
                    {
                        _updateNo = true;
                    }
                }
            }
            catch (Exception)
            {
                Available = false;
                _status   = "ERROR";
            }

            CurrEmuVer = Convert.ToDouble(File.ReadAllText("SingleCore\\version"));
            
            if (RemoteEmuVer > CurrEmuVer)
            {
                    _status           = Msgnewservera;
                    tssStatus.IsLink  = true;
                    _updateNo         = false;
            }
        }

        private void bwUpdate_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (!_updateNo)
            {
                _status = Msgstatusup;
                animateStatus(false);
            }

            //else
            //{
            //    updateNO = false;
            //    status = "New Version Available";
            //}

            AllowUpdaterQuestion = false;
            UpdateCompleteChecker.Start();

            if (_updateYes)
            {
                CheckMangosCrashed.Stop();
                GetSqlOnlineBot.Stop();
                CloseProcess(false);
                pbAvailableW.Visible = false;
                pbNotAvailW.Visible  = true;
                pbAvailableR.Visible = false;
                pbNotAvailR.Visible  = true;
                Thread.Sleep(1000);
                var update = new Update();
                update.Show();
            }
        }

        #endregion

        #region Backup

        private void import()
        {
            OpenFileDialog openFile = new OpenFileDialog();
            openFile.Title          = Msgimportchar;
            openFile.Filter         = "SPP Backup (*.sppbackup)|*.sppbackup|All files (*.*)|*.*";

            if (openFile.ShowDialog() == DialogResult.OK)
            {
                MenuItemsDisableAfterLoad();
                importFile         = openFile.FileName;
                importFolder       = Path.GetDirectoryName(importFile);
                rtWorldDev.Visible = false;
                CheckMangosCrashed.Stop();
                _allowtext = false;
                _restart   = true;
                CloseProcess(true);
                StatusIcon();
                WindowState = FormWindowState.Normal;
                Show();
                pbAvailableM.Visible = true;
                _status              = Msgdecompress;
                animateStatus(true);
                bwImport.RunWorkerAsync();
            }
        }

        private void bwImport_DoWork(object sender, DoWorkEventArgs e)
        {
            ImportExtract();
            _status = Msgimportchar;

            string conn            = "server=127.0.0.1;user=root;pwd=123456;database=characters;port=3310;convertzerodatetime=true;";
            MySqlBackup mb         = new MySqlBackup(conn);
            mb.ImportInfo.FileName = getTemp + "\\save01";
            mb.Import();

            _status = Msgimportacc;

            string conn1            = "server=127.0.0.1;user=root;pwd=123456;database=realmd;port=3310;convertzerodatetime=true;";
            MySqlBackup mb1         = new MySqlBackup(conn1);
            mb1.ImportInfo.FileName = getTemp + "\\save02";
            mb1.Import();
        }

        private void bwImport_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            _status = Msgimportcomp;
            animateStatus(false);
            File.Delete(getTemp + "\\save01");
            File.Delete(getTemp + "\\save02");
            StartAll();
        }

        private void ImportExtract() //.sppbackup extract
        {
            string unpck       = importFile;
            string unpckDir    = getTemp;
            using (ZipFile zip = ZipFile.Read(unpck))
            {  
                foreach (ZipEntry e in zip)
                {
                    e.Password = "89765487";
                    e.Extract(unpckDir, ExtractExistingFileAction.OverwriteSilently);
                }
            }
        }

        private void export()
        {
            SaveFileDialog saveFile = new SaveFileDialog();
            saveFile.Title          = Msgexportchar;
            tssStatus.Image = Properties.Resources.search_animation;
            saveFile.Filter         = "SPP Backup (*.sppbackup)|*.sppbackup|All files (*.*)|*.*";
            saveFile.FileName       = "characters";

            if (saveFile.ShowDialog() == DialogResult.OK)
            {
                _status       = Msgexportchar1;
                exportfile    = saveFile.FileName;
                exportFolder  = Path.GetDirectoryName(exportfile);
                bwExport.RunWorkerAsync();
            }
        }

        private void bwExport_DoWork(object sender, DoWorkEventArgs e)
        {
            string conn            = "server=127.0.0.1;user=root;pwd=123456;database=characters;port=3310;convertzerodatetime=true;";
            MySqlBackup mb         = new MySqlBackup(conn);
            mb.ExportInfo.FileName = getTemp + "\\save01";
            mb.Export();

            _status = Msgexportacc;

            string conn1            = "server=127.0.0.1;user=root;pwd=123456;database=realmd;port=3310;convertzerodatetime=true;";
            MySqlBackup mb1         = new MySqlBackup(conn1);
            mb1.ExportInfo.FileName = getTemp + "\\save02";
            mb1.Export();

            _status = Msgcompress;

            using (ZipFile zip = new ZipFile()) //create .sppbackup
            {
                zip.Password = "89765487";
                zip.AddFile(getTemp + "\\save01", @"\");
                zip.AddFile(getTemp + "\\save02", @"\");
                zip.Save(exportfile);
            }
        }

        private void bwExport_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            _status = Msgexportcomp;
            animateStatus(false);
            File.Delete(getTemp + "\\save01");
            File.Delete(getTemp + "\\save02");
        }

        #endregion

        private void startstopToolStripMenuItem_Click(object sender, EventArgs e)
        {
            startNStop();
        }

        private void autostartToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (autostartToolStripMenuItem.Checked)
            {
                autostartToolStripMenuItem.Checked = false;
                autorunToolStripMenuItem.Checked   = false;
                autostart                          = "0";
                saveMethod();
            }
            else
            {
                autostartToolStripMenuItem.Checked = true;
                autorunToolStripMenuItem.Checked   = true;
                autostart                          = "1";
                saveMethod();
            }
        }

        private void restartToolStripMenuItem1_Click_1(object sender, EventArgs e)
        {
            MenuItemsDisableAfterLoad();
            RealmWorldRestart();
        }

        private void SppTray_MouseDoubleClick(object sender, MouseEventArgs e)
        {

        }

        private void autorunToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (autorunToolStripMenuItem.Checked)
            {
                autostartToolStripMenuItem.Checked = false;
                autorunToolStripMenuItem.Checked   = false;
                autostart                          = "0";
                saveMethod();
            }
            else
            {
                autostartToolStripMenuItem.Checked = true;
                autorunToolStripMenuItem.Checked   = true;
                autostart                          = "1";
                saveMethod();
            }
        }

        public void startNStop()
        {
            if (!_startStop)
            {
                tssStatus.IsLink                   = false;
                startstopToolStripMenuItem.Enabled = false;
                startToolStripMenuItem.Enabled     = false;
                restartToolStripMenuItem1.Enabled  = false;
                restartToolStripMenuItem2.Enabled  = false;
                StartAll();
                _startStop                        = true;
                startstopToolStripMenuItem.Text   = "Stop";
                startToolStripMenuItem.Text       = "Stop";
                startstopToolStripMenuItem.Image  = Properties.Resources.Button_stop_icon;
                startToolStripMenuItem.Image      = Properties.Resources.Button_stop_icon;
            }
            else
            {
                resetAllRandomBotsToolStripMenuItem.Enabled = false;
                randomizeBotsToolStripMenuItem.Enabled      = false;
                resetBotsToolStripMenuItem.Enabled          = false;
                randomizeBotsToolStripMenuItem1.Enabled     = false;

                tssStatus.IsLink                                 = false;
                restartToolStripMenuItem1.Enabled                = false;
                restartToolStripMenuItem2.Enabled                = false;
                exportCharactersToolStripMenuItem.Enabled        = false;
                exportImportCharactersToolStripMenuItem.Enabled  = false;
                startstopToolStripMenuItem.Image                 = Properties.Resources.Play_1_Hot_icon;
                startToolStripMenuItem.Image                     = Properties.Resources.Play_1_Hot_icon;
                startstopToolStripMenuItem.Text                  = "Start";
                startToolStripMenuItem.Text                      = "Start";
                _startStop                                       = false;
                CheckMangosCrashed.Stop();
                GetSqlOnlineBot.Stop();
                tssLOnline.Text = "Online bot: N/A";
                CloseProcess(false);
                StatusIcon();
            }
        }

        private void startToolStripMenuItem_Click(object sender, EventArgs e)
        {
            startNStop();
        }

        private void restartToolStripMenuItem2_Click(object sender, EventArgs e)
        {
            MenuItemsDisableAfterLoad();
            RealmWorldRestart();
        }

        private void exportToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            export();
        }

        private void importToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            import();
        }

        private void animateStatus(bool animate)
        {
            if (animate)
            {
                tssStatus.Image = Properties.Resources.search_animation;   
            }
            else
            {
                tssStatus.Image = null;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Language.saveMethod();
        }

        private void magyarToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            magyarToolStripMenuItem.Checked  = true;
            englishToolStripMenuItem.Checked = false;
            frenchToolStripMenuItem.Checked  = false;
            germanToolStripMenuItem.Checked  = false;
            Language.ReadXML("Hungarian");
            lang = "Hungarian";
            saveMethod();
            Loadlang();

            groupBox1.Width = 683;
            Width = 715;
            pbNotAvailM.Location = new Point(167, 64);
            pbNotAvailR.Location = new Point(313, 64);
            pbNotAvailW.Location = new Point(464, 64);
            pbTempM.Location = new Point(167, 64);
            pbTempR.Location = new Point(313, 64);
            pbTempW.Location = new Point(464, 64);
            pbAvailableM.Location = new Point(167, 64);
            pbAvailableR.Location = new Point(313, 64);
            pbAvailableW.Location = new Point(464, 64);
            pbarWorld.Location = new Point(167, 136);
            lblMysql.Location = new Point(134, 26);
            lblRealm.Location = new Point(297, 26);
            lblWorld.Location = new Point(444, 26);
        }

        private void englishToolStripMenuItem_Click(object sender, EventArgs e)
        {
            magyarToolStripMenuItem.Checked  = false;
            englishToolStripMenuItem.Checked = true;
            frenchToolStripMenuItem.Checked  = false;
            germanToolStripMenuItem.Checked  = false;
            Language.ReadXML("English");
            lang = "English";
            saveMethod();
            Loadlang();

            groupBox1.Width = 575;
            Width = 605;
            pbNotAvailM.Location = new Point(105, 64);
            pbNotAvailR.Location = new Point(251, 64);
            pbNotAvailW.Location = new Point(402, 64);
            pbTempM.Location = new Point(105, 64);
            pbTempR.Location = new Point(251, 64);
            pbTempW.Location = new Point(402, 64);
            pbAvailableM.Location = new Point(105, 64);
            pbAvailableR.Location = new Point(251, 64);
            pbAvailableW.Location = new Point(402, 64);
            pbarWorld.Location = new Point(105, 136);
            lblMysql.Location = new Point(72, 26);
            lblRealm.Location = new Point(235, 26);
            lblWorld.Location = new Point(382, 26);

        }

        private void frenchToolStripMenuItem_Click(object sender, EventArgs e)
        {
            magyarToolStripMenuItem.Checked  = false;
            englishToolStripMenuItem.Checked = false;
            frenchToolStripMenuItem.Checked  = true;
            germanToolStripMenuItem.Checked  = false;
            Language.ReadXML("French");
            lang = "French";
            saveMethod();
            Loadlang();
        }

        private void germanToolStripMenuItem_Click(object sender, EventArgs e)
        {
            magyarToolStripMenuItem.Checked  = false;
            englishToolStripMenuItem.Checked = false;
            frenchToolStripMenuItem.Checked  = false;
            germanToolStripMenuItem.Checked  = true;
            Language.ReadXML("German");
            lang = "German";
            saveMethod();
            Loadlang();
        }

        public void Setlang()
        {
            
        }
    }
}