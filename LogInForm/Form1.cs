using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using FTPModule;


namespace LogInForm
{
    public partial class LogInForm : Form
    {
		FTPModules ftp = new FTPModules();

		public const int WM_NCLBUTTONDOWN = 0xA1;
		public const int HT_CAPTION = 0x2;

		[DllImportAttribute("user32.dll")]
		public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
		[DllImportAttribute("user32.dll")]
		public static extern bool ReleaseCapture();

		public LogInForm()
        {
            InitializeComponent();
			this.Load += Form1_Load;
            btnClose.Click += new EventHandler(btnClose_Click);
            btnConnect.Click += new EventHandler(btnConnect_Click);
			txtServer.KeyDown += TxtServer_KeyDown;
			txtUserName.KeyDown += TxtUserName_KeyDown;
			txtPassword.KeyDown += TxtPassword_KeyDown;
			txtServer.Leave += TxtServer_Leave;
			txtServer.Enter += TxtServer_Enter;
			txtUserName.Enter += TxtUserName_Enter;
			txtUserName.Leave += TxtUserName_Leave;
			txtPassword.Enter += TxtPassword_Enter;
			txtPassword.Leave += TxtPassword_Leave;
			this.MouseMove += LogInForm_MouseMove;
			panel1.MouseMove += Panel1_MouseMove;
			label1.MouseMove += Label1_MouseMove;
		}

		void btnConnect_Click(object sender, EventArgs e)
		{
			Settings.server = txtServer.Text.ToString();
			Settings.UID = txtUserName.Text.ToString();
			Settings.PWD = txtPassword.Text.ToString();
			MainFrm frm = new MainFrm();
			this.Hide();
			frm.Show();
		}

		private void Label1_MouseMove(object sender, MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Left)
			{
				ReleaseCapture();
				SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
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

		private void LogInForm_MouseMove(object sender, MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Left)
			{
				ReleaseCapture();
				SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
			}
		}

		private void TxtPassword_Leave(object sender, EventArgs e)
		{
			if (txtPassword.Text == String.Empty)
			{
				txtPassword.PasswordChar = '\0';
				txtPassword.Text = "PASSWORD";
			}
			
		}

		private void TxtPassword_Enter(object sender, EventArgs e)
		{
			if (txtPassword.Text == "PASSWORD")
			{
				txtPassword.PasswordChar = '*';
				txtPassword.Text = "";
			}
		}

		private void TxtUserName_Leave(object sender, EventArgs e)
		{
			if (txtUserName.Text == String.Empty)
			{
				txtUserName.Text = "USERNAME";
			}
		}

		private void TxtUserName_Enter(object sender, EventArgs e)
		{
			if (txtUserName.Text == "USERNAME")
			{
				txtUserName.Text = "";
			}
		}

		private void TxtServer_Enter(object sender, EventArgs e)
		{
			if (txtServer.Text == "FTP SERVER")
			{
				txtServer.Text = "";
			}
		}

		private void TxtServer_Leave(object sender, EventArgs e)
		{
			if (txtServer.Text == String.Empty)
			{
				txtServer.Text = "FTP SERVER";
			}
		}

		private void Form1_Load(object sender, EventArgs e)
		{
			txtServer.Text = "FTP SERVER";
			txtUserName.Text = "USERNAME";
			txtPassword.Text = "PASSWORD";
		}

		private void TxtPassword_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Enter)
			{
				btnConnect.PerformClick();
			}
		}

		private void TxtUserName_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Enter)
			{
				btnConnect.PerformClick();
			}
		}

		private void TxtServer_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Enter)
			{
				btnConnect.PerformClick();
			}
		}



        void btnClose_Click(object sender, EventArgs e)
        {
            Environment.Exit(0);
        }
    }
}
