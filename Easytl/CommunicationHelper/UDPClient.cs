﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace Easytl.CommunicationHelper
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class UDPClient
    {
        #region 公共属性

        /// <summary>
        /// 获取本地UDP连接端口
        /// </summary>
        public int Port_Local { get; private set; }

        #endregion

        #region 协议参数

        /// <summary>
        /// 协议头
        /// </summary>
        public virtual string Command_Head { private get; set; }

        /// <summary>
        /// 协议最小长度
        /// </summary>
        public virtual int Command_MinLen { get; set; }

        #endregion

        #region 内部使用变量

        /// <summary>
        /// UDP连接
        /// </summary>
        UdpClient UDP_Client;

        /// <summary>
        /// 缓冲区字符串
        /// </summary>
        StringBuilder ReciveMessage = new StringBuilder();

        #endregion

        #region 接收到数据时触发事件

        public delegate void Data_Recive_Delegate(string Recive_Data, IPEndPoint Local_EndPoint, IPEndPoint Net_EndPoint);
        /// <summary>
        /// 接收到来自网络的数据时触发事件
        /// </summary>
        public event Data_Recive_Delegate Data_Recive_Event;

        #endregion

        #region 接收数据发生异常时触发事件

        public delegate void Data_ReciveException_Delegate(Exception e);

        public event Data_ReciveException_Delegate Data_ReciveException_Event;

        #endregion


        /// <summary>
        /// UDPClient实例化
        /// </summary>
        public UDPClient()
        { }


        /// <summary>
        /// UDPClient实例化
        /// </summary>
        /// <param name="_Port_Local">绑定的本地UDP通讯端口号</param>
        public UDPClient(int _Port_Local)
        {
            Start(_Port_Local);
        }


        /// <summary>
        /// 开始侦听端口并收发数据
        /// </summary>
        /// <param name="_Port_Local">绑定的本地UDP通讯端口号</param>
        public void Start(int _Port_Local)
        {
            Port_Local = _Port_Local;
            UDP_Client = new UdpClient(Port_Local);

            //开启接收数据线程
            new Task(Recive).Start();
        }


        /// <summary>
        /// 接收数据
        /// </summary>
        void Recive()
        {
            IPEndPoint Remote_IPEndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 0);
            string Command = null;
            while (true)
            {
                Thread.Sleep(10);

                try
                {
                    if (UDP_Client != null)
                    {
                        if (UDP_Client.Available > 0)
                        {
                            ReciveMessage.Append(BitConverter.ToString(UDP_Client.Receive(ref Remote_IPEndPoint)).ToUpper().Trim().Replace("-", string.Empty));

                            Command = ReciveMessage.ToString();
                            ReciveMessage.Remove(0, AnalyCommand(ref Command));

                            if (!string.IsNullOrEmpty(Command))
                                Data_Recive_Event?.Invoke(Command, (IPEndPoint)UDP_Client.Client.LocalEndPoint, Remote_IPEndPoint);
                        }
                    }
                    else
                        break;
                }
                catch (Exception e)
                {
                    Data_ReciveException_Event?.Invoke(e);
                }
            }
        }


        /// <summary>
        /// 获取协议内容长度（若重写了AnalyCommand方法，则该方法无效）
        /// </summary>
        /// <param name="Command_Min">最小长度的协议</param>
        /// <returns>返回协议内容长度</returns>
        public abstract int Body_Len(string Command_Min);

        /// <summary>
        /// 分析接收到的协议数据，并把它转为正确的一条协议输出，返回应清除的协议数据长度
        /// </summary>
        public virtual int AnalyCommand(ref string Command)
        {
            int Command_SIndex = 0, Command_BodySize = 0;
            if (Command_MinLen > 0)
            {
                if (!string.IsNullOrEmpty(Command_Head))
                    Command_SIndex = Command.IndexOf(Command_Head);
                if (Command.Length >= Command_SIndex + Command_MinLen)
                {
                    Command_BodySize = Body_Len(Command.Substring(Command_SIndex, Command_MinLen));
                    if (Command.Length >= Command_SIndex + Command_MinLen + Command_BodySize)
                        Command = Command.Substring(Command_SIndex, Command_MinLen + Command_BodySize);
                    else
                        Command = string.Empty;
                }
                else
                    Command = string.Empty;
            }

            return Command_SIndex + Command.Length;
        }


        /// <summary>
        /// 发送数据
        /// </summary>
        /// <param name="Data">字符串</param>
        /// <param name="Net_Address">要发送的远程端IP地址</param>
        /// <param name="Net_Port">要发送的远程端端口号</param>
        public virtual void Send(string Data, string Net_Address, int? Net_Port = null)
        {
            try
            {
                if (Data == string.Empty)
                    throw new Exception("数据为空");

                if (Data.Length % 2 != 0)
                    Data += "0";

                int ByteNum = Data.Length / 2;
                byte[] bs = new byte[ByteNum];
                for (int i = 0; i < ByteNum; i++)
                {
                    bs[i] = Convert.ToByte(Data.Substring(i * 2, 2), 16);
                }

                if (UDP_Client.Send(bs, bs.Length, Net_Address, ((Net_Port.HasValue) ? Net_Port.Value : Port_Local)) <= 0)
                    throw new Exception("发送失败");
            }
            catch (Exception e)
            {
                throw e;
            }
        }


        /// <summary>
        /// 关闭所有连接并释放内存
        /// </summary>
        public void Close()
        {
            ReciveMessage.Clear();

            try
            {
                if (UDP_Client != null)
                {
                    UDP_Client.Close();
                    UDP_Client = null;
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}
