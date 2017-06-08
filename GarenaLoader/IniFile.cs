using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Ini
{

    /*
     Reference: https://www.codeproject.com/Articles/1990/INI-Class-using-C
     Steps to use the Ini class:

    1. In your project namespace definition add 

        using INI;

    2. Create a INIFile like this

        INIFile ini = new INIFile("C:\\test.ini");

    3. Use IniWriteValue to write a new value to a specific key in a section or use IniReadValue to read a value FROM a key in a specific Section. 

    That's all. It's very easy in C# to include API functions in your class(es). A
     */

    /// <summary>
    /// Create a New INI file to store or load data
    /// </summary>
    public class INIFile
    {
        public string path;

        [DllImport("kernel32")]
        private static extern long WritePrivateProfileString(string section,
            string key, string val, string filePath);
        [DllImport("kernel32")]
        private static extern int GetPrivateProfileString(string section,
                 string key, string def, StringBuilder retVal,
            int size, string filePath);

        /// <summary>
        /// INIFile Constructor.
        /// </summary>
        /// <PARAM name="INIPath"></PARAM>
        public INIFile(string INIPath)
        {
            path = INIPath;
        }
        /// <summary>
        /// Write Data to the INI File
        /// </summary>
        /// <PARAM name="Section"></PARAM>
        /// Section name
        /// <PARAM name="Key"></PARAM>
        /// Key Name
        /// <PARAM name="Value"></PARAM>
        /// Value Name
        public void IniWriteValue(string Section, string Key, string Value)
        {
            WritePrivateProfileString(Section, Key, Value, this.path);
        }

        /// <summary>
        /// Read Data Value From the Ini File
        /// </summary>
        /// <PARAM name="Section"></PARAM>
        /// <PARAM name="Key"></PARAM>
        /// <PARAM name="Path"></PARAM>
        /// <returns></returns>
        public string IniReadValue(string Section, string Key)
        {
            StringBuilder temp = new StringBuilder(255);
            int i = GetPrivateProfileString(Section, Key, "", temp,
                                            255, this.path);
            return temp.ToString();

        }
    }
}
