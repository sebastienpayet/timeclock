﻿using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace TimeClock.infrastructure.util
{

    internal struct LASTINPUTINFO
    {
        public uint cbSize;

        public uint dwTime;
    }

    public class SystemUtils
    {

        [DllImport("User32.dll")]
        private static extern bool GetLastInputInfo(ref LASTINPUTINFO plii);

        [DllImport("Kernel32.dll")]
        private static extern uint GetLastError();

        public static uint GetIdleTime()
        {
            LASTINPUTINFO lastInPut = new LASTINPUTINFO();
            lastInPut.cbSize = (uint)Marshal.SizeOf(lastInPut);
            GetLastInputInfo(ref lastInPut);

            return (uint)((((Environment.TickCount & int.MaxValue) - (lastInPut.dwTime & int.MaxValue)) & int.MaxValue) / 1000);
        }

        internal static void ConfigureAutoStart()
        {
            // auto start configuration
            RegistryKey rkApp = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
            rkApp.SetValue("TimeClockStatupWithWindows", System.Reflection.Assembly.GetExecutingAssembly().Location);
        }

        public static void OpenExplorerOnFolder(string folderPath)
        {
            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                Arguments = folderPath,
                FileName = "explorer.exe"
            };

            _ = Process.Start(startInfo);
        }
    }
}
