using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using FTPModule;
using System.IO;
using System.Runtime.InteropServices;

namespace LogInForm
{
    public partial class MainFrm : Form
    {
		FTPModules ftp = new FTPModules();
		string[] path = new string[0];


		public const int WM_NCLBUTTONDOWN = 0xA1;
		public const int HT_CAPTION = 0x2;

		[DllImportAttribute("user32.dll")]
		public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
		[DllImportAttribute("user32.dll")]
		public static extern bool ReleaseCapture();

		public MainFrm()
        {
			InitializeComponent();
            btnClose.Click+=new EventHandler(btnClose_Click);
			this.Load += MainFrm_Load;
			btnCheck.Click += BtnCheck_Click;
			label1.MouseMove += Label1_MouseMove;
			panel1.MouseMove += Panel1_MouseMove;
			this.MouseMove += MainFrm_MouseMove;
			btnSize.Click += BtnSize_Click;
			btnSubFolder.Click += BtnSubFolder_Click;
			btnBack.Click += BtnBack_Click;
			btnCreate.Click += BtnCreate_Click;
			btnDelete.Click += BtnDelete_Click;
			btnDownload.Click += BtnDownload_Click;
			btnUpload.Click += BtnUpload_Click;
            btnRefresh.Click += new EventHandler(btnRefresh_Click);
		}

        void btnRefresh_Click(object sender, EventArgs e)
        {
            RefreshListBox();
        }

		private void BtnUpload_Click(object sender, EventArgs e)
		{
			string filePath = "";
			string savepath = "";
			try
			{
				OpenFileDialog openFileDialog = new OpenFileDialog();
				openFileDialog.InitialDirectory = @"C:\";
				openFileDialog.Filter = "All files (*.*)|*.*|Text files (*.txt)|*.txt|Word (*.docx)|*.docx|MP4 (*.mp4)|*.mp4|MP3 (*.mp3)|*.mp3|RAR (*.rar, *.zip)|*.rar, *.zip|Image files (*.jpg, *.jpeg, *.jpe, *.jfif, *.png) | *.jpg; *.jpeg; *.jpe; *.jfif; *.png";
				if (openFileDialog.ShowDialog() == DialogResult.OK)
				{
					filePath = openFileDialog.FileName;
				}

				string[] splitpath = filePath.Split('\\');
				string filename = splitpath[splitpath.Length - 1];
				string selected = listBox1.SelectedItem.ToString();
				if (selected == string.Empty)
				{
					savepath = CreateFinalPath(path);
				}
				else savepath = CreateFinalPath(path) + "/" + selected;

				ftp.Upload(savepath, Settings.UID, Settings.PWD, progressBar1, filePath, filename);

				MessageBox.Show("File Upload Complete!");
				progressBar1.Value = 0;
			}
			catch(Exception error)
			{
				MessageBox.Show(error.Message);
			}
		}

		private void BtnDownload_Click(object sender, EventArgs e)
		{
			string filePath = "";	
			try
			{
			
				SaveFileDialog saveFileDialog = new SaveFileDialog();
				saveFileDialog.InitialDirectory = @"C:\";
				saveFileDialog.Filter = "All files (*.*)|*.*|Text files (*.txt)|*.txt|Word (*.docx)|*.docx|MP4 (*.mp4)|*.mp4|MP3 (*.mp3)|*.mp3|Image files (*.jpg, *.jpeg, *.jpe, *.jfif, *.png) | *.jpg; *.jpeg; *.jpe; *.jfif; *.png";
				if (saveFileDialog.ShowDialog() == DialogResult.OK)
				{
					filePath = saveFileDialog.FileName;
				}
				string selected = listBox1.SelectedItem.ToString();
				//MessageBox.Show(filePath + "\\" + selected);
				long size = ftp.GetFileSize(selected, CreateFinalPath(path), Settings.UID, Settings.PWD);
				ftp.Download(CreateFinalPath(path), Settings.UID, Settings.PWD, progressBar1, size, filePath, selected);
				MessageBox.Show("File Download Complete!");
				progressBar1.Value = 0;
			}
			catch(Exception error)
			{
				MessageBox.Show(error.Message);
			}
			
		}

		private void BtnDelete_Click(object sender, EventArgs e)
		{
			try
			{
				ftp.serverDirectory.Clear();
				ftp.CheckingPermissions.Clear();
				ftp.SubFolderList.Clear();
				string selected = listBox1.SelectedItem.ToString();
				string identity = Permission(selected);
				if (identity == "Folder")
				{
					
					ftp.GetDirectoryList(CreateFinalPath(path) + "/" + selected, Settings.UID, Settings.PWD);
					for (int i = 0; i < ftp.serverDirectory.Count; i++)
					{
						string[] SplitSub = ftp.serverDirectory[i].Split('/');
						string SubFolder = SplitSub[SplitSub.Length - 1];
						string identiti = Permission(SubFolder);
						MessageBox.Show(identiti);
						if (identiti == "Folder")
						{
							ftp.DeleteDirectory(ftp.serverDirectory[i], CreateFinalPath(path), Settings.UID, Settings.PWD);
						}
						else
						{
							ftp.DeleteFile(ftp.serverDirectory[i], CreateFinalPath(path), Settings.UID, Settings.PWD);
						}
							
					}
					ftp.DeleteDirectory(selected, CreateFinalPath(path), Settings.UID, Settings.PWD);
				
				}
				else if (identity == "File")
				{
					ftp.DeleteFile(selected, CreateFinalPath(path), Settings.UID, Settings.PWD);
				}
				MessageBox.Show("Successful!\nItem Deleted!");
				RefreshListBox();
			}
			catch (Exception error)
			{
				MessageBox.Show(error.Message);
			}
				
			
		}

		private void BtnCreate_Click(object sender, EventArgs e)
		{
			string selected = "";

			if (txtCreateDIR.Text.Equals("")) return;
			string newDirectoryName = txtCreateDIR.Text;

			try
			{
				selected = listBox1.SelectedItem.ToString();
				ftp.MakeDirectory(selected, CreateFinalPath(path) + "/", Settings.UID, Settings.PWD, newDirectoryName);
			}
			catch
			{
				ftp.MakeDirectory(selected, CreateFinalPath(path) + "/", Settings.UID, Settings.PWD, newDirectoryName);
			}

			MessageBox.Show("Directory Created to " + CreateFinalPath(path) + "/" + selected);
			txtCreateDIR.Clear();
			RefreshListBox();
		}

		string CreateFinalPath(string[] path)
		{
			string finalpath = "ftp://";
			for (int i = 0; i < path.Length; i++)
			{
				finalpath += path[i];
			}
			return finalpath;
		}

		private void BtnBack_Click(object sender, EventArgs e)
		{
			Array.Resize<string>(ref path, path.Length - 1);
			Open(path);
		}

		private void BtnSubFolder_Click(object sender, EventArgs e)
		{
			try
			{
				ftp.serverDirectory.Clear();
				ftp.CheckingPermissions.Clear();
				ftp.SubFolderList.Clear();
				string fileName = listBox1.SelectedItem.ToString();
				if (Permission(fileName) == "Folder")
				{
					Array.Resize<string>(ref path, path.Length + 1);
					path[path.Length - 1] = "/" + fileName;
					Open(path);
				}
				else
				{
					MessageBox.Show("Please choose a FOLDER");
				}
			}
			catch(Exception error)
			{
				MessageBox.Show("Please choose a something");
			}
			
		}

		private void BtnSize_Click(object sender, EventArgs e)
		{
			string fileName = listBox1.SelectedItem.ToString();
			//FileSize
			try
			{
				ftp.serverDirectory.Clear();
				ftp.CheckingPermissions.Clear();
				ftp.SubFolderList.Clear();
				long size = ftp.GetFileSize(fileName, CreateFinalPath(path), Settings.UID, Settings.PWD);
				int kb = (int)size / 1024;
				if (kb >= 1)
				{
					int mb = kb / 1000;

					if (mb >= 1)
					{
						int gb = mb / 1000;

						if (gb >= 1)
						{
							if (gb >= 1)
								MessageBox.Show("File Name: " + fileName + "\nFile Size: " + gb + " GB");
						}
						else
							MessageBox.Show("File Name: " + fileName + "\nFile Size: " + mb + " MB");
					}
					else
						MessageBox.Show("File Name: " + fileName + "\nFile Size: " + kb + " KB");
				}
				else
					MessageBox.Show("File Name: " + fileName + "\nFile Size: " + size + " Bytes");
			}
			catch(Exception error)
			{
				MessageBox.Show("Please choose a FILE");
			}
		}

		private void BtnCheck_Click(object sender, EventArgs e)
		{
			try
			{
				ftp.serverDirectory.Clear();
				ftp.CheckingPermissions.Clear();
				ftp.SubFolderList.Clear();
				string selected = listBox1.SelectedItem.ToString();
				string Identity = Permission(selected);
				selected = "";
				if(Identity == "Folder")
				{
					MessageBox.Show("This is a Folder!");
				}
				else if (Identity == "File")
				{
					MessageBox.Show("This is a File!");
				}

			}
			catch(Exception error)
			{
				MessageBox.Show(error.Message);
			}

		}

		private void MainFrm_Load(object sender, EventArgs e)
		{
			Array.Resize<string>(ref path, path.Length + 1);
			path[path.Length - 1] = Settings.server;
			RefreshListBox();
		}

		//EncapsulatedSubfolder
		public void Open(string[] path)
		{
			ftp.OpenSubFolder(CreateFinalPath(path), Settings.UID, Settings.PWD);

			if (ftp.SubFolderList.Count == 0)
			{
				listBox1.Enabled = false;
				listBox1.Items.Clear();
				listBox1.Items.Add("This Folder is empty.");
				btnBack.Enabled = true;
				btnBack.BackColor = Color.LightSteelBlue;
			}
			else
			{
				listBox1.Enabled = true;
				listBox1.Items.Clear();
				for (int i = 0; i < ftp.SubFolderList.Count; i++)
				{

					listBox1.Items.Add(ftp.SubFolderList[i].ToString());
				}

				if (path.Length == 1)
				{
					btnBack.Enabled = false;
					btnBack.BackColor = Color.DarkGray;
				}
				else
				{
					btnBack.Enabled = true;
					btnBack.BackColor = Color.LightSteelBlue;
				}
			}
		}

		//EncapsulatedCheckFile
		public string Permission(string selected)
		{
			Settings.server = CreateFinalPath(path);
			ftp.CheckFilePermission(Settings.server, Settings.UID, Settings.PWD);
			string CheckedPermission = "";
			for (int i = 0; i < ftp.CheckingPermissions.Count; i++)
			{
				string[] selectsplit = selected.Split(' ');
				string[] split = ftp.CheckingPermissions[i].Split(' ');
				
				if (split[split.Length-1] == selectsplit[selectsplit.Length -1])
				{
					if (split[0] == "drwxr-xr-x")
					{
						CheckedPermission = "Folder";	
					}
					else
					{
						CheckedPermission = "File";
					}
				}
			}
			return CheckedPermission;
		}
		
		//EncapsulatedRefreshListBox
		public void RefreshListBox()
		{
			ftp.serverDirectory.Clear();
			ftp.CheckingPermissions.Clear();
			ftp.SubFolderList.Clear();
			listBox1.Items.Clear();
			ftp.GetDirectoryList(CreateFinalPath(path), Settings.UID, Settings.PWD);
			if (ftp.serverDirectory.Count == 0)
			{
				listBox1.Enabled = false;
				listBox1.Items.Clear();
				listBox1.Items.Add("This Folder is empty.");
			}
			else
			{
				for (int i = 0; i < ftp.serverDirectory.Count; i++)
				{
					listBox1.Items.Add(Path.GetFileName(ftp.serverDirectory[i]));
				}
			}
			
		}

		private void Panel1_MouseMove(object sender, MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Left)
			{
				ReleaseCapture();
				SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
			}
		}

		private void MainFrm_MouseMove(object sender, MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Left)
			{
				ReleaseCapture();
				SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
			}
		}

		private void Label1_MouseMove(object sender, MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Left)
			{
				ReleaseCapture();
				SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
			}
		}

		private void btnClose_Click(object sender, EventArgs e)
        {
            this.Hide();
            LogInForm frm = new LogInForm();
            frm.Show();
        }
    }
}