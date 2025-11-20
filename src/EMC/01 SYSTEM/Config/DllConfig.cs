
using System;
using System.Runtime.InteropServices;

namespace EMC
{
    public static class DllConfig
    {
        [DllImport("kernel32", SetLastError = true)]
        private static extern bool SetDllDirectory(string lpPathName);

        public static void AddLibPath(string folderName)
        {
            string libPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, folderName);
            SetDllDirectory(libPath);
        }

    }
}
