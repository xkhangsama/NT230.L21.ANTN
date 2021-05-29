using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;


namespace B1._4
{
    public partial class Service1 : ServiceBase
    {
        public Service1()
        {
            InitializeComponent();
            this.CanHandleSessionChangeEvent = true;
        }
        [DllImport("wtsapi32.dll", SetLastError = true)]
        static extern bool WTSSendMessage(
                IntPtr hServer,
                [MarshalAs(UnmanagedType.I4)] int SessionId,
                String pTitle,
                [MarshalAs(UnmanagedType.U4)] int TitleLength,
                String pMessage,
                [MarshalAs(UnmanagedType.U4)] int MessageLength,
                [MarshalAs(UnmanagedType.U4)] int Style,
                [MarshalAs(UnmanagedType.U4)] int Timeout,
                [MarshalAs(UnmanagedType.U4)] out int pResponse,
                bool bWait);

        protected override void OnSessionChange(SessionChangeDescription changeDescription)
        {
            Thread.Sleep(3000);
            switch (changeDescription.Reason)
            {
                case SessionChangeReason.SessionLogon:
                    WriteToFile(changeDescription.SessionId + "User Logon");
                    bool result = false;
                    String title = "Alert";
                    int titleLength = title.Length;
                    String msg = "Passcode: 18520071 - Nguyen Xuan Khang";
                    int msgLength = msg.Length;
                    int resp = 7;
                    var WTS_CURRENT_SERVER_HANDLE = IntPtr.Zero;
                    result = WTSSendMessage(WTS_CURRENT_SERVER_HANDLE, changeDescription.SessionId, title, titleLength, msg, msgLength, 4, 3, out resp, true);
                    break;
                case SessionChangeReason.SessionLogoff:
                    WriteToFile("User has Session id " +changeDescription.SessionId + " logoff");
                    break;
                case SessionChangeReason.SessionLock:
                    WriteToFile("User has Session id "+ changeDescription.SessionId + " lock");
                    break;
                case SessionChangeReason.SessionUnlock:
                    WriteToFile("User has Session id "+changeDescription.SessionId + " unlock");
                    break;
            }

            base.OnSessionChange(changeDescription);
        }
        protected override void OnStart(string[] args)
        {
            WriteToFile("Service is started at " + DateTime.Now);
        }

        protected override void OnStop()
        {
            WriteToFile("Service is stopped at " + DateTime.Now);
        }
        public void WriteToFile(string Message)
        {
            string path = AppDomain.CurrentDomain.BaseDirectory + "\\Logs";
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            string filepath = AppDomain.CurrentDomain.BaseDirectory +
           "\\Logs\\ShellLog_" + DateTime.Now.Date.ToShortDateString().Replace('/', '_') +
           ".txt";
            if (!File.Exists(filepath))
            {
                // Create a file to write to.
                using (StreamWriter sw = File.CreateText(filepath))
                {
                    sw.WriteLine(Message);
                }
            }
            else
            {
                using (StreamWriter sw = File.AppendText(filepath))
                {
                    sw.WriteLine(Message);
                }
            }
        }
    }
}
