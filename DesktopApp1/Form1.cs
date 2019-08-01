using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Net.Http;
using System.Net;
using System.Runtime.CompilerServices;
using MetroFramework;
using Rebex.IO;
using System.IO.Compression;
using Rebex.Net;
using Microsoft.Win32;

/*Copyright Darksoftware (c) 2019*/

namespace DesktopApp1
{
    public partial class Form1 : MetroFramework.Forms.MetroForm
    {
        public Form1()
        {
            InitializeComponent();
            metroTextBox1.Text = Properties.Settings.Default.IP;
            Rebex.Licensing.Key = "==AnKxIZnJ2NXyRRk/MrXLh5vsLbImP/JhMGERReY23qIk==";

        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
 

        }

        private void button1_Click(object sender, EventArgs e)
        {
           
        }

        private void helloWorldLabel_Click(object sender, EventArgs e)
        {

        }

        private void openFileDialog1_FileOk(object sender, CancelEventArgs e)
        {
  
        }


        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void contextMenuStrip1_Opening(object sender, CancelEventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }


        public static System.Drawing.Bitmap BytesToBitmap(byte[] ImgBytes)
        {
            System.Drawing.Bitmap result = null;
            if (ImgBytes != null)
            {
                MemoryStream stream = new MemoryStream(ImgBytes);
                result = (System.Drawing.Bitmap)System.Drawing.Bitmap.FromStream(stream);
            }
            return result;
        }


        private void metroButton2_Click(object sender, EventArgs e)
        {

            OpenFileDialog opendialog = new OpenFileDialog();
            opendialog.CheckFileExists = true;
            //opendialog.AddExtension 
            opendialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            opendialog.Multiselect = false;
            opendialog.Filter = "PS4 Package File (*.pkg) | *.pkg";
            if (opendialog.ShowDialog() == DialogResult.OK)
            {
                string FilePath = opendialog.FileName;
                metroTextBox2.Text = FilePath;

           //Adding PS4 Tools so we can get an image pkg information ext
                //xDPx
                try
                {
                    var pkgfile = PS4_Tools.PKG.SceneRelated.Read_PKG(FilePath);
                    pbPkg.Image = BytesToBitmap(pkgfile.Image);
                    lblPackageInfo.Text = pkgfile.PS4_Title + "\n" + pkgfile.PKG_Type.ToString() + "\n" +
                                          pkgfile.Param.TitleID; //display whatever info youd like here
                }
                catch (Exception ee)
                {
                    MessageBox.Show("Invaild Package!");
                }

        }



        private bool _isWorking;                    // determines whether any operation is running
        private DateTime _lastTransferProgressTime;      // last TransferProgress event call time
        private DateTime _transferTime;             // transfer launch-time
        private bool _cccMode = false;

        private void label1_Click_1(object sender, EventArgs e)
        {
            // create an FTP clien
  

        }

        private void TransferCompleted()
        {
            _isWorking = false;
            MessageBox.Show("Finished");
            SetProgressValue(0);
        }

        /// <summary>
        /// show transfer status: files, bytes, time, speed
        /// </summary>
        private void ShowTransferStatus(long bytesTransferred, int filesTransferred)
        {
            // unknown bytes transferred


            if (bytesTransferred == 0)
                return;

            // files and bytes transferred
            string outstring = string.Format("{0} file{1} ({2} byte{3}) transferred in",
                filesTransferred, (filesTransferred > 1 ? "s" : null),
                bytesTransferred, (bytesTransferred > 1 ? "s" : null));

            // time spent
            TimeSpan ts = DateTime.Now - _transferTime;

            // speed
            if (ts.TotalSeconds > 1)
            {
                outstring += (ts.Days > 0 ? " " + ts.Days + " day" + (ts.Days > 1 ? "s" : null) : null);
                outstring += (ts.Hours > 0 ? " " + ts.Hours + " hour" + (ts.Hours > 1 ? "s" : null) : null);
                outstring += (ts.Minutes > 0 ? " " + ts.Minutes + " min" + (ts.Minutes > 1 ? "s" : null) : null);
                outstring += (ts.Seconds > 0 ? " " + ts.Seconds + " sec" + (ts.Seconds > 1 ? "s" : null) : null);
            }
            else
            {
                outstring += " " + ts.TotalSeconds + " sec";
            }

            double speed = bytesTransferred / ts.TotalSeconds;
            if (speed < 1)
                outstring += string.Format(" at {0:F3} B/s", speed);
            else if (speed < 1024)
                outstring += string.Format(" at {0:F0} B/s", speed);
            else
                outstring += string.Format(" at {0:F0} KB/s", speed / 1024);

        }

        void Traversing(object sender, FtpTraversingEventArgs e)
        {
            if (e.Action == TransferAction.Listing)
                return;

            string strBatchInfo = string.Format("({0} file{1} traversed)    ",
                e.FilesTotal, (e.FilesTotal > 1 ? "s" : null));

            switch (e.TraversingState)
            {
                case TraversingState.DirectoryRetrieved:
                    strBatchInfo += "Directory retrieved.";
                    break;
                case TraversingState.DirectoryRetrieving:
                    strBatchInfo += "Retrieving directory...";
                    break;
                case TraversingState.HierarchyRetrieved:
                    strBatchInfo += string.Format("Hierarchy retrieved ({0} byte{1} in {2} file{3}).",
                        e.BytesTotal, (e.BytesTotal > 1 ? "s" : null),
                        e.FilesTotal, (e.FilesTotal > 1 ? "s" : null));
                    break;
                case TraversingState.HierarchyRetrieving:
                    strBatchInfo += "Retrieving hierarchy...";
                    break;
            }

            SetText(strBatchInfo);
        }

        /// <summary>
        /// handles the transfer progress changed event
        /// </summary>
        void TransferProgressChanged(object sender, FtpTransferProgressChangedEventArgs e)
        {
            string strBatchInfo = string.Format("({0} / {1} file{2} processed)    ",
                e.FilesProcessed, e.FilesTotal, (e.FilesProcessed > 1 ? "s" : null));

            SetProgressValue(Convert.ToInt32(e.ProgressPercentage));

            switch (e.TransferState)
            {
                case TransferProgressState.DataBlockProcessed:
                    strBatchInfo += e.BytesTransferred + " bytes";
                    break;
                case TransferProgressState.DirectoryProcessing:
                    strBatchInfo += "Processing directory...";
                    break;
                case TransferProgressState.FileTransferring:
                    strBatchInfo += "Transferring file...";
                    break;
                case TransferProgressState.FileTransferred:
                    strBatchInfo += "File transferred.";
                    break;
                case TransferProgressState.TransferCompleted:
                    strBatchInfo += "Transfer completed.";
                    ShowTransferStatus(e.BytesTransferred, e.FilesTransferred);
                    break;
            }

            SetText(strBatchInfo);
        }


        private void SetProgressValue(int value)
        {
            // workaround for progress bar smoothing

            Invoke(new Action(() => {
                if (value == 100)
            {
                progressBar1.Maximum = 101;
                progressBar1.Value = 101;
                progressBar1.Maximum = 100;
                System.Threading.Thread.Sleep(200);
            }
            else
            {
                progressBar1.Value = value;
            }
            }));
        }

        private void SetText(string text)
        {
            Invoke(new Action(() => { lblProgress.Text = text; }));
        }

        private void SetTex(string text)
        {
            Invoke(new Action(() => { label15.Text = text; }));
        }

        private void StateChanged(object sender, FtpStateChangedEventArgs e)
        {
            switch (e.NewState)
            {
                case FtpState.Disconnected:
                case FtpState.Disposed:
                    SetTex("Disconnected");
                    break;
                case FtpState.Ready:
                    SetTex("Ready");
                    break;
            }
        }

        private void DeleteProgressChanged(object sender, FtpDeleteProgressChangedEventArgs e)
        {
            string strDeleteInfo = string.Format("({0} / {1} file{2} deleted)    ",
                e.FilesProcessed, e.FilesTotal, (e.FilesProcessed > 1 ? "s" : null));

            switch (e.DeleteState)
            {
                case DeleteProgressState.DeleteCompleted:
                    strDeleteInfo += "Delete completed.";
                    break;
                case DeleteProgressState.DirectoryDeleted:
                    strDeleteInfo += "Directory deleted.";
                    break;
                case DeleteProgressState.DirectoryProcessing:
                    strDeleteInfo += "Processing directory...";
                    break;
                case DeleteProgressState.FileDeleted:
                    strDeleteInfo += "File deleted.";
                    break;
                case DeleteProgressState.FileDeleting:
                    strDeleteInfo += "Deleting file...";
                    break;
            }

            SetText(strDeleteInfo);
        }

        private void ProblemDetected(object sender, FtpProblemDetectedEventArgs e)
        {
            // MessageBox.Show("Problem Detected " + e);
        }

        private void god(string path, string dir)
        {
            using (Ftp client = new Ftp())
            {

                try
                {
                    // connect and login to the FTP
                    client.Connect(path);
                    client.Login("anonymous", "DONT-LOOK@MYCODE");

                    client.StateChanged += StateChanged;
                    client.Traversing += Traversing;
                    client.TransferProgressChanged += TransferProgressChanged;
                    client.DeleteProgressChanged += DeleteProgressChanged;
                    client.ProblemDetected += ProblemDetected;

                   client.PutFile(dir, @"/user/app/temp.pkg");

                    client.SendCommand("installpkg");

                    var response = client.ReadResponse();

                    MessageBox.Show(response.Raw);

                    SetText("Package Sent");



                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }

                client.Disconnect();
            }
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            Properties.Settings.Default.IP = metroTextBox1.Text;
            Properties.Settings.Default.Save();
        }

        private void metroButton1_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Started");
            Task.Run(() => god(metroTextBox1.Text, metroTextBox2.Text));
        }


    }
}
