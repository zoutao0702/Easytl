using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO.Ports;
using System.Threading;

namespace Easytl.CommunicationHelper
{
    public class COMHelper
    {
        #region 操作参数

        /// <summary>
        /// 该类是否已被释放
        /// </summary>
        bool isdispose = false;
        /// <summary>
        /// 该类是否已被释放
        /// </summary>
        public bool IsDispose
        {
            get { return isdispose; }
        }

        /// <summary>
        /// COM串口号
        /// </summary>
        int _COM_Bind_Id = 1;
        /// <summary>
        /// 获取COM串口号
        /// </summary>
        public int COM_Bind_Id
        {
            set { _COM_Bind_Id = value; }
            get { return _COM_Bind_Id; }
        }

        /// <summary>
        /// COM波特率
        /// </summary>
        int _COM_Bind_BaudRate = 57600;
        /// <summary>
        /// 获取COM波特率
        /// </summary>
        public int COM_Bind_BaudRate
        {
            set { _COM_Bind_BaudRate = value; }
            get { return _COM_Bind_BaudRate; }
        }

        /// <summary>
        /// COM连接
        /// </summary>
        SerialPort COM_Client;

        #region 接收到来自串口的数据时触发事件

        public delegate void COM_Data_Recive_Delegate(string Recive_Data, int COM_Id);

        public event COM_Data_Recive_Delegate Data_Recive_Event;
        /// <summary>
        /// 接收到来自串口的数据时触发事件
        /// </summary>
        /// <param name="Recive_Data"></param>
        /// <param name="COM_Id"></param>
        void Data_Recive_Fun(string Recive_Data, int COM_Id)
        {
            if (Data_Recive_Event != null)
            {
                Data_Recive_Event(Recive_Data, COM_Id);
            }
        }

        #endregion

        #endregion


        /// <summary>
        /// COM_Touch实例化
        /// </summary>
        /// <param name="Port_Bind">绑定的本机接收串口号</param>
        public COMHelper(int COM_Bind, int baudRate)
        {
            try
            {
                COM_Client = new SerialPort("COM" + COM_Bind.ToString(), baudRate);
                _COM_Bind_Id = COM_Bind;
                _COM_Bind_BaudRate = baudRate;

                //开启接收数据线程
                ThreadPool.QueueUserWorkItem(new WaitCallback(Recive));
            }
            catch(Exception e)
            {
                throw e;
            }
        }


        /// <summary>
        /// 打开串口
        /// </summary>
        public void COM_Open()
        {
            try
            {
                if (!isdispose)
                {
                    if (COM_Client.IsOpen)
                    {
                        COM_Client.Close();
                    }
                    COM_Client.PortName = "COM" + COM_Bind_Id.ToString();
                    COM_Client.BaudRate = COM_Bind_BaudRate;
                    COM_Client.Open();
                }
                else
                {
                    throw new Exception("该类已被释放");
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }


        /// <summary>
        /// 关闭串口
        /// </summary>
        public void COM_Close()
        {
            try
            {
                if (!isdispose)
                {
                    if (COM_Client.IsOpen)
                    {
                        COM_Client.Close();
                    }
                }
                else
                {
                    throw new Exception("该类已被释放");
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }


        /// <summary>
        /// 接收数据
        /// </summary>
        private void Recive(object obj)
        {
            while (true)
            {
                Thread.Sleep(10);

                try
                {
                    if (!isdispose)
                    {
                        if (COM_Client != null)
                        {
                            if (COM_Client.IsOpen)
                            {
                                if (COM_Client.BytesToRead > 0)
                                {
                                    byte[] bs = new byte[COM_Client.BytesToRead];
                                    COM_Client.Read(bs, 0, bs.Length);

                                    Data_Recive_Fun(BitConverter.ToString(bs).ToUpper().Trim().Replace("-", string.Empty), COM_Bind_Id);
                                }
                            }
                        }
                    }
                    else
                    {
                        if (COM_Client != null)
                        {
                            COM_Client.Close();
                            COM_Client = null;
                        }
                        return;
                    }
                }
                catch (Exception e)
                {
                    throw e;
                }
            }
        }


        /// <summary>
        /// 发送数据
        /// </summary>
        /// <param name="Data_Base16">16进制字符串</param>
        /// <param name="COM_Bind">要发送的COM地址</param>
        /// <returns>0：发送失败，1：发送成功，-99：数据为空</returns>
        public int Send(string Data_Base16, int COM_Bind)
        {
            try
            {
                if (!isdispose)
                {
                    if (Data_Base16 == string.Empty)
                    {
                        return -99;
                    }
                    if (Data_Base16.Length % 2 != 0)
                    {
                        Data_Base16 += "0";
                    }
                    int ByteNum = Data_Base16.Length / 2;
                    byte[] bs = new byte[ByteNum];
                    for (int i = 0; i < ByteNum; i++)
                    {
                        bs[i] = Convert.ToByte(Data_Base16.Substring(i * 2, 2), 16);
                    }

                    COM_Client.Write(bs, 0, bs.Length);
                    return 1;
                }
                else
                {
                    throw new Exception("该类已被释放");
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }


        /// <summary>
        /// 关闭所有连接并释放内存
        /// </summary>
        public void Dispose()
        {
            isdispose = true;
        }
    }
}
