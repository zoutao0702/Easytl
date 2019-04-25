using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Runtime.InteropServices;

namespace Easytl.WF.FileHelper
{
    /// <summary>
    /// 各种操作Ini文件的方法
    /// </summary>
    public class IniHelper
    {
        /// <summary>
        /// 写入ini文件
        /// </summary>
        /// <param name="section"></param>
        /// <param name="key"></param>
        /// <param name="val"></param>
        /// <param name="filePath"></param>
        /// <returns></returns>
        [DllImport("kernel32")]
        private static extern long WritePrivateProfileString(string section, string key, string val, string filePath);

        /// <summary>
        /// 写入ini文件
        /// </summary>
        /// <param name="section"></param>
        /// <param name="str"></param>
        /// <param name="filePath"></param>
        /// <returns></returns>
        [DllImport("kernel32")]
        private static extern long WritePrivateProfileSection(string section, string str, string filePath);

        /// <summary>
        /// 读取ini文件
        /// </summary>
        /// <param name="section"></param>
        /// <param name="key"></param>
        /// <param name="def"></param>
        /// <param name="retVal"></param>
        /// <param name="size"></param>
        /// <param name="filePath"></param>
        /// <returns></returns>
        [DllImport("kernel32")]
        private static extern int GetPrivateProfileString(string section, string key, string def, byte[] retVal, int size, string filePath);

        /// <summary>
        /// 读取ini文件
        /// </summary>
        /// <param name="section"></param>
        /// <param name="retVal"></param>
        /// <param name="size"></param>
        /// <param name="filePath"></param>
        /// <returns></returns>
        [DllImport("kernel32")]
        private static extern int GetPrivateProfileSection(string section, byte[] retVal, int size, string filePath);

        /// <summary>
        /// 写入ini文件
        /// </summary>
        /// <param name="FileUrl">ini文件路径</param>
        /// <param name="Section">目录</param>
        /// <param name="ParaName">属性名称</param>
        /// <param name="ParaValue">属性值</param>
        /// <returns>1：成功，-1：ini文件不存在，其他：失败</returns>
        public static void WriteIni(string FileUrl, string Section, string ParaName, string ParaValue)
        {
            if (File.Exists(FileUrl))
                WritePrivateProfileString(Section, ParaName, ParaValue, FileUrl);
        }

        /// <summary>
        /// 写入ini文件
        /// </summary>
        /// <param name="FileUrl">ini文件路径</param>
        /// <param name="Section">目录</param>
        /// <param name="ParaName">属性名称</param>
        /// <param name="ParaValue">属性值</param>
        /// <returns>1：成功，-1：ini文件不存在，其他：失败</returns>
        public static void WriteIni(string FileUrl, string Section, string ParaName)
        {
            if (File.Exists(FileUrl))
                WritePrivateProfileSection(Section, ParaName, FileUrl);
        }

        /// <summary>
        /// 读取ini文件
        /// </summary>
        /// <param name="FileUrl">ini文件路径</param>
        /// <param name="Section">目录</param>
        /// <param name="ParaName">属性名称</param>
        /// <returns>1：成功，-1：ini文件不存在，其他：失败</returns>
        public static string ReadIni(string FileUrl, string Section, string ParaName)
        {
            if (File.Exists(FileUrl))
            {
                StringBuilder temp = new StringBuilder();
                byte[] bs = new byte[32767];
                int len = GetPrivateProfileString(Section, ParaName, "", bs, 32767, FileUrl);
                temp.Append(Encoding.Default.GetString(bs, 0, len));
                return temp.ToString();
            }
            else
                return string.Empty;
        }

        /// <summary>
        /// 读取ini文件
        /// </summary>
        /// <param name="FileUrl">ini文件路径</param>
        /// <param name="Section">目录</param>
        /// <param name="ParaName">属性名称</param>
        /// <returns>1：成功，-1：ini文件不存在，其他：失败</returns>
        public static string ReadIni(string FileUrl, string Section)
        {
            if (File.Exists(FileUrl))
            {
                StringBuilder temp = new StringBuilder();
                byte[] bs = new byte[32767];
                int len = GetPrivateProfileSection(Section, bs, 32767, FileUrl);
                temp.Append(Encoding.Default.GetString(bs, 0, len));
                return temp.ToString();
            }
            else
                return string.Empty;
        }
    }
}
