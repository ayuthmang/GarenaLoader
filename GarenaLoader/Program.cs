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
            // Initial objects
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

                // Waited delay for start GarenaMessenger and move BBTalk
                System.Threading.Thread.Sleep(3500);
            
                if (File.Exists(GarenaTalkPath))
                {
                    File.Move(GarenaTalkPath, Path.GetDirectoryName(GarenaTalkPath) + @"\BBTalk.exe.lod");
                }
                Write("Starting GarenaMessenger ... ", ConsoleColor.Yellow);
                Process.Start(GarenaMessengerPath);
                WriteLine("Success", ConsoleColor.Green);
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
            // if file not exists then create a file
            if (!File.Exists(SETTING_FILE_FULLPATH))
            {
                File.Create(SETTING_FILE_FULLPATH).Close();
            }

            OpenFileDialog fileDialog;
            // read garenapath from file
            if (iniFile.IniReadValue("GarenaTalk", "Path") == "" | iniFile.IniReadValue("GarenaTalk", "Path") == "")
            {
                // if read path failed or returned null
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

        public bool Close()
        {
            Console.WriteLine("\n");
            WriteLine("[Closing]", ConsoleColor.Green);
            // kill all process about garena
            foreach (Process proc in Process.GetProcesses())
            {
                if (proc.ProcessName == "GarenaMessenger" || proc.ProcessName == "BBTalk")
                {
                    proc.Kill();   
                }
            }

            // Waited delay for start GarenaMessenger and move BBTalk
            System.Threading.Thread.Sleep(3500);
            try
            {
                if (File.Exists(Path.GetDirectoryName(GarenaTalkPath) + @"\BBTalk.exe.lod"))
                {
                    Write("Restoring file ... ", ConsoleColor.Yellow);
                    File.Move(Path.GetDirectoryName(GarenaTalkPath) + @"\BBTalk.exe.lod", GarenaTalkPath);
                    WriteLine("Completed", ConsoleColor.Green);
                }
                else{
                    WriteLine("Restore file failed", ConsoleColor.Red);
                    return false;
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine("Garena --> Close()");
                WriteLine(ex.Message, ConsoleColor.Red);
                return false;
            }
            return true;
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

#region "trap ctr + c"
        //ref: http://geekswithblogs.net/mrnat/archive/2004/09/23/11594.aspx

        // Declare the SetConsoleCtrlHandler function
        // as external and receiving a delegate.
        [DllImport("Kernel32")]
        public static extern bool SetConsoleCtrlHandler(HandlerRoutine Handler, bool Add);

        // A delegate type to be used as the handler routine 
        // for SetConsoleCtrlHandler.
        public delegate bool HandlerRoutine(CtrlTypes CtrlType);

        // An enumerated type for the control messages
        // sent to the handler routine.
        public enum CtrlTypes
        {
            CTRL_C_EVENT = 0,
            CTRL_BREAK_EVENT,
            CTRL_CLOSE_EVENT,
            CTRL_LOGOFF_EVENT = 5,
            CTRL_SHUTDOWN_EVENT
        }

        private static bool ConsoleCtrlCheck(CtrlTypes ctrlType)
        {
            if (garena.Close())
                Environment.Exit(0);
            return true;
        }
#endregion

        public static readonly String VERSION = "0.1.2";
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
            Console.Title = String.Format("GarenaLoader v{0}",VERSION);
            WriteLine("[GarenaLoader]", ConsoleColor.Magenta);
            WriteLine("Developed by blackSource",ConsoleColor.White);
            Write("Source code can be found at: ", ConsoleColor.Yellow); WriteLine("https://github.com/blackSourcez/GarenaLoader", ConsoleColor.Green);
            Console.WriteLine();

            // init
            WriteLine("[Initial Loader]", ConsoleColor.Green);

            // console event callback
            Write("Setting console event callback ... ", ConsoleColor.Yellow);
            {
                handler = new ConsoleEventDelegate(ConsoleEventCallback);
                SetConsoleCtrlHandler(handler, true);
            }
            WriteLine("Success", ConsoleColor.Green);


            // console ctrl callback
            Write("Setting console ctrl callback ... ", ConsoleColor.Yellow);
            { 
                SetConsoleCtrlHandler(new HandlerRoutine(ConsoleCtrlCheck), true);
            }
            WriteLine("Success", ConsoleColor.Green);

            // Find all and kill all process Garena Messenger and TalkTalk
            Write("Finding and kill all garena processes ... ", ConsoleColor.Yellow);
            foreach (Process proc in Process.GetProcesses())
            {
                if (proc.ProcessName == "GarenaMessenger" || proc.ProcessName == "BBTalk")
                {
                    proc.Kill();
                }
            }
            WriteLine("Success", ConsoleColor.Green);

            garena = new Garena();
            garena.SetUp();
            garena.Start();

            String input;
            while (true)
            {
                Write("Press 'y' to exit loader and terminate GarenaMessenger: ", ConsoleColor.Red);
                input = Console.ReadLine();
                if (input == "y")
                {
                    // by Environment.Exit(0) exit will not catch by EventCallBack
                    // so we gonna close garena by self
                    if (garena.Close())
                        Environment.Exit(0);
                }
            }
        }
    }
}
