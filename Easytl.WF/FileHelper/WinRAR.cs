using System;
using System.Diagnostics;
using Microsoft.Win32;
using System.IO;

namespace Easytl.WF.FileHelper
{
    public class WinRar
    {
        /// <summary>
        /// 是否安装WinRAR
        /// </summary>
        /// <param name="WinRARPath">WinRAR路径</param>
        /// <returns></returns>
        public static bool Exists()
        {
            string WinRARPath;
            return Exists(out WinRARPath);
        }

        /// <summary>
        /// 是否安装WinRAR
        /// </summary>
        /// <param name="WinRARPath">WinRAR路径</param>
        /// <returns></returns>
        public static bool Exists(out string WinRARPath)
        {
            bool Exist = false;
            WinRARPath = string.Empty;

            string key = @"SOFTWARE\Microsoft\Windows\CurrentVersion\App Paths\WinRAR.exe";
            RegistryKey registryKey = Registry.LocalMachine.OpenSubKey(key);
            if (registryKey != null)
            {
                WinRARPath = registryKey.GetValue("").ToString();
                Exist = true;
            }

            registryKey.Close();

            return Exist;
        }

        /// <summary>
        /// 将格式为RAR的压缩文件解压到指定的目录（利用系统自带WinRAR解压工具）
        /// </summary>
        /// <param name="RARFileName">要解压RAR文件的路径</param>
        /// <param name="SaveDir">解压后要保存到的目录</param>
        public static void DeCompressRAR(string RARFileName, string SaveDir)
        {
            string WinRARPath;
            if (Exists(out WinRARPath))
            {
                string rarPath = Path.Combine(Path.GetDirectoryName(WinRARPath), "Rar.exe");

                DeCompressRAR(rarPath, RARFileName, SaveDir);
            }
        }

        /// <summary>
        /// 将格式为RAR的压缩文件解压到指定的目录
        /// </summary>
        /// <param name="WinRARPath">解压缩工具的路径</param>
        /// <param name="RARFileName">要解压RAR文件的路径</param>
        /// <param name="SaveDir">解压后要保存到的目录</param>
        public static void DeCompressRAR(string WinRARPath, string RARFileName, string SaveDir)
        {
            if (File.Exists(WinRARPath))
            {
                String commandOptions = string.Format("x \"{0}\" \"{1}\" -y", RARFileName, SaveDir);

                ProcessStartInfo processStartInfo = new ProcessStartInfo();
                processStartInfo.FileName = WinRARPath;
                processStartInfo.Arguments = commandOptions;
                processStartInfo.WindowStyle = ProcessWindowStyle.Hidden;

                Process process = new Process();
                process.StartInfo = processStartInfo;
                process.Start();
                process.WaitForExit();
                process.Close();
            }
        }
    }
}
