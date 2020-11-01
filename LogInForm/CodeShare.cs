using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace FTPModule
{
	public class Settings
	{
		public static string server;
		public static string UID;
		public static string PWD;
	}
	public class FTPModules
	{
		public List<string> serverDirectory = new List<string>();
		public List<string> CheckingPermissions = new List<string>();
		public List<string> SubFolderList = new List<string>();

		public void CheckFilePermission(string ftpserver, string ftpUID, string ftpPWD)
		{
			FtpWebRequest request = (FtpWebRequest)WebRequest.Create(ftpserver);
			request.Credentials = new NetworkCredential(ftpUID, ftpPWD);
			request.UseBinary = true;
			request.UsePassive = true;
			request.KeepAlive = true;

			request.Method = WebRequestMethods.Ftp.ListDirectoryDetails;
			StreamReader reader = new StreamReader(request.GetResponse().GetResponseStream());

			string pattern =
	@"^([\w-]+)\s+(\d+)\s+(\w+)\s+(\w+)\s+(\d+)\s+" +
	@"(\w+\s+\d+\s+\d+|\w+\s+\d+\s+\d+:\d+)\s+(.+)$";
			Regex regex = new Regex(pattern);

			while (!reader.EndOfStream)
			{
				string line = reader.ReadLine();
				Match match = regex.Match(line);
				string permissions = match.Groups[1].Value;
				string name = match.Groups[7].Value;
				CheckingPermissions.Add(permissions + " " + name);
			}
				
		}

		public void GetDirectoryList(string ftpserver, string ftpUID, string ftpPWD)
		{
			FtpWebRequest request = (FtpWebRequest)WebRequest.Create(ftpserver);
			request.Credentials = new NetworkCredential(ftpUID, ftpPWD);
			request.UseBinary = true;
			request.UsePassive = true;
			request.KeepAlive = true;

			request.Method = WebRequestMethods.Ftp.ListDirectory;
			StreamReader reader = new StreamReader(request.GetResponse().GetResponseStream());
					while (!reader.EndOfStream)
					{
						serverDirectory.Add(reader.ReadLine());
					}
		}

		public long GetFileSize(string path, string ftpserver, string ftpUID, string ftpPWD)
		{
			FtpWebRequest request = (FtpWebRequest)WebRequest.Create(ftpserver + "/" + path);
			request.Credentials = new NetworkCredential(ftpUID, ftpPWD);
			request.UseBinary = true;
			request.UsePassive = true;
			request.KeepAlive = true;

			request.Method = WebRequestMethods.Ftp.GetFileSize;

			return request.GetResponse().ContentLength;
		}

		
		public void OpenSubFolder(string ftpserver, string ftpUID, string ftpPWD)
		{
			SubFolderList = new List<string>();
			FtpWebRequest request = (FtpWebRequest)WebRequest.Create(ftpserver);
			request.Credentials = new NetworkCredential(ftpUID, ftpPWD);
			request.UseBinary = true;
			request.UsePassive = true;
			request.KeepAlive = true;

			request.Method = WebRequestMethods.Ftp.ListDirectory;
			StreamReader reader = new StreamReader(request.GetResponse().GetResponseStream());
			while (!reader.EndOfStream)
			{
				string[] SplitSub = reader.ReadLine().Split('/');
				string SubFolder = SplitSub[SplitSub.Length-1];
				SubFolderList.Add(SubFolder);
			}
		}

		public void MakeDirectory(string currentDir, string server, string username, string password, string newDirectory)
		{
			FtpWebRequest request = FtpWebRequest.Create(server + currentDir + "/" + newDirectory) as FtpWebRequest;

			request.Credentials = new NetworkCredential(username, password);
			request.UseBinary = true;
			request.UsePassive = true;
			request.KeepAlive = true;
			request.Method = WebRequestMethods.Ftp.MakeDirectory;
			FtpWebResponse response = (FtpWebResponse)request.GetResponse();
			Stream ftpstream = response.GetResponseStream();
			ftpstream.Close();
			response.Close();
		}


		public List<string> DirectoryListing(string Path, string ServerAdress, string Login, string Password)
		{
			FtpWebRequest request = (FtpWebRequest)WebRequest.Create(ServerAdress + "/" + Path);
			request.Credentials = new NetworkCredential(Login, Password);

			request.Method = WebRequestMethods.Ftp.ListDirectory;

			FtpWebResponse response = (FtpWebResponse)request.GetResponse();
			Stream responseStream = response.GetResponseStream();
			StreamReader reader = new StreamReader(responseStream);

			List<string> result = new List<string>();

			while (!reader.EndOfStream)
			{
				result.Add(reader.ReadLine());
			}

			reader.Close();
			response.Close();

			return result;
		}

		public void DeleteDirectory(string path, string server, string username, string password)
		{
			FtpWebRequest request = FtpWebRequest.Create(server + "/" + path) as FtpWebRequest;
			request.Credentials = new NetworkCredential(username, password);
			request.UseBinary = true;
			request.UsePassive = true;
			request.KeepAlive = true;
			request.Method = WebRequestMethods.Ftp.RemoveDirectory;
			FtpWebResponse response = (FtpWebResponse)request.GetResponse();
			Stream ftpstream = response.GetResponseStream();
			ftpstream.Close();
			response.Close();
		}

		public void DeleteFile(string path, string server, string username, string password)
		{
			FtpWebRequest request = FtpWebRequest.Create(server + "/" + path) as FtpWebRequest;
			request.Credentials = new NetworkCredential(username, password);
			request.UseBinary = true;
			request.UsePassive = true;
			request.KeepAlive = true;
			request.Method = WebRequestMethods.Ftp.DeleteFile;
			FtpWebResponse response = (FtpWebResponse)request.GetResponse();
			Stream ftpstream = response.GetResponseStream();
			ftpstream.Close();
			response.Close();
		}

		public void Download(string server, string username,string password,ProgressBar progressBar1, long filesize, string downloadpath, string filename)
		{
			int size = (int)filesize;

			progressBar1.Invoke(
				(MethodInvoker)(() => progressBar1.Maximum = size));

			FtpWebRequest request = FtpWebRequest.Create(server + "/" + filename) as FtpWebRequest;
				request.Credentials = new NetworkCredential(username, password);
				request.UseBinary = true;
				request.UsePassive = true;
				request.KeepAlive = true;
			request.Method = WebRequestMethods.Ftp.DownloadFile;

			using (Stream ftpStream = request.GetResponse().GetResponseStream())
			using (Stream fileStream = File.Create(@downloadpath))
			{
				byte[] buffer = new byte[10240];
				int read;
				while ((read = ftpStream.Read(buffer, 0, buffer.Length)) > 0)
				{
					fileStream.Write(buffer, 0, read);
					int position = (int)fileStream.Position;
					progressBar1.Invoke(
						(MethodInvoker)(() => progressBar1.Value = position));
				}
			}
		}

		public void Upload(string server, string username, string password, ProgressBar progressBar1, string filepath, string filename)
		{
			FtpWebRequest request = FtpWebRequest.Create(server + "/" + filename) as FtpWebRequest;
			request.Credentials = new NetworkCredential(username, password);
			request.UseBinary = true;
			request.UsePassive = true;
			request.KeepAlive = true;
			request.Method = WebRequestMethods.Ftp.UploadFile;

			using (Stream fileStream = File.OpenRead(@filepath))
			using (Stream ftpStream = request.GetRequestStream())
			{
				progressBar1.Invoke(
							(MethodInvoker)delegate { progressBar1.Maximum = (int)fileStream.Length; });

				byte[] buffer = new byte[10240];
				int read;
				while ((read = fileStream.Read(buffer, 0, buffer.Length)) > 0)
				{
					ftpStream.Write(buffer, 0, read);
					progressBar1.Invoke(
						(MethodInvoker)delegate {
							progressBar1.Value = (int)fileStream.Position;
						});
				}
			}
		}

	}
}
