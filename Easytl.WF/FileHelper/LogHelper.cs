﻿using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Windows.Forms;

namespace Easytl.WF.FileHelper
{
    /// <summary>
    /// 各种操作Log文件的方法
    /// </summary>
    public class LogHelper
    {
        /// <summary>
        /// 写入日志文件（适用于winform程序）
        /// </summary>
        /// <param name="Message">日志记录</param>
        /// <param name="RecordType">日志类型</param>
        public static void WriteLogHasDate_WinForm(string Message, string RecordType)
        {
            StringBuilder logpath = new StringBuilder();
            logpath.Append(string.Format(@"{0}\log", Application.StartupPath));
            if (!Directory.Exists(logpath.ToString()))
                Directory.CreateDirectory(logpath.ToString());
            logpath.Append(string.Format(@"\{0}", RecordType));
            if (!Directory.Exists(logpath.ToString()))
                Directory.CreateDirectory(logpath.ToString());
            logpath.Append(string.Format(@"\{0}.log", DateTime.Today.ToString("yyyy-MM-dd")));
            FileStream fs = new FileStream(logpath.ToString(), FileMode.Append, FileAccess.Write, FileShare.ReadWrite);
            StreamWriter sw = new StreamWriter(fs, Encoding.GetEncoding("GB2312"));
            sw.Write(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + "  " + Message);
            sw.WriteLine();
            sw.Flush();
            sw.Close();
            fs.Close();
        }

        /// <summary>
        /// 写入日志文件（适用于winform程序）
        /// </summary>
        /// <param name="Message">日志记录</param>
        /// <param name="RecordStartupPath">程序内日志路径，主目录为exe程序文件夹</param>
        /// <param name="ShowTime">是否在记录前显示时间</param>
        public static void WriteLog_WinForm(string Message, string RecordStartupPath, bool ShowTime)
        {
            if (RecordStartupPath != string.Empty)
            {
                string[] FileNames = RecordStartupPath.Split('\\');
                if (FileNames.Length > 0)
                {
                    string FileUrl = System.Windows.Forms.Application.StartupPath + @"\";
                    for (int i = 0; i < FileNames.Length - 1; i++)
                    {
                        FileUrl += FileNames[i];
                        if (!Directory.Exists(FileUrl))
                            Directory.CreateDirectory(FileUrl);
                        FileUrl += @"\";
                    }
                    FileUrl += FileNames[FileNames.Length - 1] + ".log";
                    FileStream fs = new FileStream(FileUrl, FileMode.Append, FileAccess.Write, FileShare.ReadWrite);
                    StreamWriter sw = new StreamWriter(fs, Encoding.GetEncoding("GB2312"));
                    if (ShowTime)
                        Message = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + "  " + Message;
                    sw.Write(Message);
                    sw.WriteLine();
                    sw.Flush();
                    sw.Close();
                    fs.Close();
                }
            }
        }

        /// <summary>
        /// 写入日志文件
        /// </summary>
        public static void WriteLog(string Message, string FileUrl, bool ShowTime)
        {
            FileStream fs = new FileStream(FileUrl, FileMode.Append, FileAccess.Write, FileShare.ReadWrite);
            StreamWriter sw = new StreamWriter(fs, Encoding.GetEncoding("GB2312"));
            if (ShowTime)
                Message = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + "  " + Message;
            sw.Write(Message);
            sw.WriteLine();
            sw.Flush();
            sw.Close();
            fs.Close();
        }
    }
}
