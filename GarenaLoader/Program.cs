using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Ini;
using System.Runtime.InteropServices;
using System.Diagnostics;

namespace GarenaLoader
{
    
    class Garena
    {
        public static void Write(String message, ConsoleColor color)
        {
            Console.ForegroundColor = color;
            Console.Write(message);
            Console.ResetColor();
        }

        public static void WriteLine(String message, ConsoleColor color)
        {
            Console.ForegroundColor = color;
            Console.WriteLine(message);
            Console.ResetColor();
        }
        private INIFile iniFile;
        private readonly String SETTING_FILE_FULLPATH = Directory.GetCurrentDirectory() + @"\GarenaSetting.txt";
        private readonly String CURRENT_DIRECTORY = Directory.GetCurrentDirectory() + @"\";
        public String GarenaMessengerPath
        {
            get;
            set;
        }

        public String GarenaTalkPath
        {
            get;
            set;
        }

        public Garena()
        {
            iniFile = new INIFile( SETTING_FILE_FULLPATH );
        }

        public void Start()
        {
            try
            {
                foreach (Process proc in Process.GetProcesses())
                {
                    if (proc.ProcessName == "GarenaMessenger" || proc.ProcessName == "BBTalk")
                    {
                        proc.Kill();
                    }
                }

                if (File.Exists(GarenaTalkPath))
                {
                    File.Move(GarenaTalkPath, Path.GetDirectoryName(GarenaTalkPath) + @"\BBTalk.exe.lod");
                }
                Process.Start(GarenaMessengerPath);
            }
            catch (FileNotFoundException ex)
            {
                WriteLine(ex.Message, ConsoleColor.Red);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Garena --> Start()");
                WriteLine(ex.Message, ConsoleColor.Red);
            }
        }

        public void SetUp()
        {
            //if file not exists then create a file
            if (!File.Exists(SETTING_FILE_FULLPATH))
            {
                File.Create(SETTING_FILE_FULLPATH).Close();
            }

            //read garenapath from file
            if (iniFile.IniReadValue("GarenaTalk", "Path") == "" | iniFile.IniReadValue("GarenaTalk", "Path") == "")
            {
                OpenFileDialog fileDialog;
                do
                {
                    fileDialog = new OpenFileDialog();
                    fileDialog.Title = "Please choose GarenaMessenger.exe path";
                    fileDialog.FileName = "GarenaMessenger.exe";
                    fileDialog.Filter = "GarenaMessenger.exe | GarenaMessenger.exe";
                } while (fileDialog.ShowDialog() != DialogResult.OK);

                GarenaMessengerPath = fileDialog.FileName;
                GarenaTalkPath = Path.GetDirectoryName(fileDialog.FileName) + @"\bbtalk\BBTalk.exe";
                iniFile.IniWriteValue("GarenaMessenger", "Path", GarenaMessengerPath);
                iniFile.IniWriteValue("GarenaTalk", "Path", GarenaTalkPath);
            }
            else
            {
                if (iniFile.IniReadValue("GarenaTalk", "Path") == "" | iniFile.IniReadValue("GarenaTalk", "Path") == "")
                {
                    OpenFileDialog fileDialog;
                    do
                    {
                        fileDialog = new OpenFileDialog();
                        fileDialog.Title = "Please choose GarenaMessenger.exe path";
                        fileDialog.FileName = "GarenaMessenger.exe";
                        fileDialog.Filter = "GarenaMessenger.exe | GarenaMessenger.exe";
                    } while (fileDialog.ShowDialog() != DialogResult.OK);

                    GarenaMessengerPath = fileDialog.FileName;
                    GarenaTalkPath = Path.GetDirectoryName(fileDialog.FileName) + @"\bbtalk\BBTalk.exe";
                    iniFile.IniWriteValue("GarenaMessenger", "Path", GarenaMessengerPath);
                    iniFile.IniWriteValue("GarenaTalk", "Path", GarenaTalkPath);
                }
                else
                {
                    GarenaMessengerPath = iniFile.IniReadValue("GarenaMessenger", "Path");
                    GarenaTalkPath = iniFile.IniReadValue("GarenaTalk", "Path");
                }
            }
        }

        public void Close()
        {
            foreach (Process proc in Process.GetProcesses())
            {
                if (proc.ProcessName == "GarenaMessenger" || proc.ProcessName == "BBTalk")
                {
                    proc.Kill();
                }
            }
            System.Threading.Thread.Sleep(2000);
            try
            {
                if (File.Exists(Path.GetDirectoryName(GarenaTalkPath) + @"\BBTalk.exe.lod"))
                {
                    File.Move(Path.GetDirectoryName(GarenaTalkPath) + @"\BBTalk.exe.lod", GarenaTalkPath);
                    Console.WriteLine("Restore file completed", ConsoleColor.Green);
                }
                else{
                    WriteLine("Restore file failed", ConsoleColor.Red);
                    Environment.Exit(-1);
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine("Garena --> Close()");
                WriteLine(ex.Message, ConsoleColor.Red);
            }

        }
    }
    
    class Program
    {
        public static Garena garena;
        //Thanks: https://stackoverflow.com/questions/4646827/on-exit-for-a-console-application
        private delegate bool ConsoleEventDelegate(int eventType);
        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool SetConsoleCtrlHandler(ConsoleEventDelegate callback, bool add);

        static ConsoleEventDelegate handler;   // Keeps it from getting garbage collected
                                               // Pinvoke

        static bool ConsoleEventCallback(int eventType)
        {
            if (eventType == 2) // console is closing
            {

                garena.Close();
            }
            return false;
        }

        public static readonly String VERSION = "0.1";
        public static void Write(String message, ConsoleColor color)
        {
            Console.ForegroundColor = color;
            Console.Write(message);
            Console.ResetColor();
        }

        public static void WriteLine(String message, ConsoleColor color)
        {
            Console.ForegroundColor = color;
            Console.WriteLine(message);
            Console.ResetColor();
        }

        [STAThread] // Thanks https://stackoverflow.com/questions/15270387/browse-for-folder-in-console-application
        static void Main(string[] args)
        {
            handler = new ConsoleEventDelegate(ConsoleEventCallback);
            SetConsoleCtrlHandler(handler, true);
            Console.WriteLine("Finding and kill all garena processes ...");

            garena = new Garena();
            // Find all and kill all process Garena Messenger and TalkTalk
            foreach (Process proc in Process.GetProcesses())
            {
                if (proc.ProcessName == "GarenaMessenger" || proc.ProcessName == "BBTalk")
                {
                    proc.Kill();
                }
            }
            garena.SetUp();
            garena.Start();
            while (true)
            {
                WriteLine("Garena will be terminated are you sure? (y/n)", ConsoleColor.Red);
                if (Console.ReadLine().Trim().ToLower() == "y")
                {
                    garena.Close();
                    Environment.Exit(0);
                }
            }
        }
    }
}
