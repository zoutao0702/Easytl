using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Management;
using System.Diagnostics;
using System.IO;

namespace Easytl.FileHelper
{
    /// <summary>
    /// 各种操作文件夹的方法
    /// </summary>
    public class FolderHelper
    {
        /// <summary>
        /// 创建文件夹
        /// </summary>
        /// <param name="DirectoryPath">文件夹路径</param>
        public static void CreateFolder(string DirectoryPath)
        {
            string[] FolderNames = DirectoryPath.Split('\\');
            if (FolderNames.Length > 0)
            {
                string FolderUrl = string.Empty;
                for (int i = 0; i < FolderNames.Length; i++)
                {
                    FolderUrl += FolderNames[i];
                    if (!FolderNames[i].Contains(':'))
                    {
                        if (!Directory.Exists(FolderUrl))
                        {
                            Directory.CreateDirectory(FolderUrl);
                        }
                    }

                    if (i < FolderNames.Length - 1)
                    {
                        FolderUrl += @"\";
                    }
                }
            }

            DirectoryInfo FolderDirectory = new DirectoryInfo(DirectoryPath);
            FolderDirectory.Refresh();
        }

        /// <summary>
        /// 设置文件夹共享
        /// </summary>
        /// <param name="FolderPath">文件夹路径</param>
        /// <param name="ShareName">共享名</param>
        /// <param name="Description">共享注释</param>
        /// <param name="NetHide">是否在网络中隐藏</param>
        public static bool ShareNetFolder(string FolderPath, string ShareName, string Description, bool CanWrite)
        {
            try
            {
                //创建文件夹
                if (!Directory.Exists(FolderPath))
                {
                    CreateFolder(FolderPath);
                }

                //设置文件夹共享
                ManagementClass managementClass = new ManagementClass("Win32_Share");
                ManagementBaseObject inParams = managementClass.GetMethodParameters("Create");

                inParams["Description"] = Description;
                inParams["Name"] = ShareName;
                inParams["Path"] = FolderPath;
                inParams["Type"] = 0x0;
                //if (CanWrite)
                //{
                //    inParams["Access"] = 0x01;
                //}
                ManagementBaseObject outParams = managementClass.InvokeMethod("Create", inParams, null);
                managementClass.Dispose();

                int ReturnValue = Convert.ToInt32(outParams.Properties["ReturnValue"].Value);
                if ((ReturnValue == 0) || (ReturnValue == 22))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }

        /// <summary>
        /// 打开通道连接共享文件夹
        /// </summary>
        public static bool OpenShareNetFolder(string FolderPath, string UserName, string Password, out string ErrorMsg)
        {
            bool Flag = false;
            Process proc = new Process();
            try
            {
                proc.StartInfo.FileName = "cmd.exe";
                proc.StartInfo.UseShellExecute = false;
                proc.StartInfo.RedirectStandardInput = true;
                proc.StartInfo.RedirectStandardOutput = true;
                proc.StartInfo.RedirectStandardError = true;
                proc.StartInfo.CreateNoWindow = true;
                proc.Start();
                string dosLine = @"net use " + FolderPath + " /User:" + UserName + " " + Password + " /PERSISTENT:YES";
                proc.StandardInput.WriteLine(dosLine);
                proc.StandardInput.WriteLine("exit");
                while (!proc.HasExited)
                {
                    proc.WaitForExit(1000);
                }
                ErrorMsg = proc.StandardError.ReadToEnd();
                proc.StandardError.Close();
                if (string.IsNullOrEmpty(ErrorMsg))
                {
                    Flag = true;
                }
                else
                {
                    Flag = false;
                }
            }
            catch (Exception exception)
            {
                throw exception;
            }
            finally
            {
                proc.Close();
                proc.Dispose();
            }
            return Flag;
        }

        /// <summary>
        /// 关闭与共享文件夹的通道
        /// </summary>
        public static bool CloseShareNetFolder(string FolderPath, out string ErrorMsg)
        {
            bool Flag = false;
            Process proc = new Process();
            try
            {
                proc.StartInfo.FileName = "cmd.exe";
                proc.StartInfo.UseShellExecute = false;
                proc.StartInfo.RedirectStandardInput = true;
                proc.StartInfo.RedirectStandardOutput = true;
                proc.StartInfo.RedirectStandardError = true;
                proc.StartInfo.CreateNoWindow = true;
                proc.Start();
                string dosLine = @"net use " + FolderPath + " /delete";
                proc.StandardInput.WriteLine(dosLine);
                proc.StandardInput.WriteLine("exit");
                while (!proc.HasExited)
                {
                    proc.WaitForExit(1000);
                }
                ErrorMsg = proc.StandardError.ReadToEnd();
                proc.StandardError.Close();
                if (string.IsNullOrEmpty(ErrorMsg))
                {
                    Flag = true;
                }
                else
                {
                    Flag = false;
                }
            }
            catch (Exception exception)
            {
                throw exception;
            }
            finally
            {
                proc.Close();
                proc.Dispose();
            }
            return Flag;
        }
    }
}
