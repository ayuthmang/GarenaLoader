using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ini;

namespace GarenaLoader
{
    class Garena
    {
        private readonly String SETTING_FILENAME = "GarenaSetting.txt";
        public String Path
        {
            get;
            set;
        }

        public void StartProgram()
        {

            String CurrentDirectory = Directory.GetCurrentDirectory();

            //find settings file and load
            try
            {

            }
            catch (FileNotFoundException ex)
            {
                
            }

        }
    }
    
    class Program
    {
        public void StartUp()
        {


            Garena garena = new Garena();
            garena.Path = "eiei";
            Console.WriteLine(garena.Path);
            Console.Read();
        }
        static void Main(string[] args)
        {
            new Program().StartUp();







        }
    }
}
