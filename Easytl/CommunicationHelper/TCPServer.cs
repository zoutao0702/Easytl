using System;
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
    public abstract class TCPServer
    {
        #region 公共属性

        /// <summary>
        /// 获取本地TCP侦听端口
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
        /// TCP侦听类
        /// </summary>
        TcpListener TCP_Listener;

        /// <summary>
        /// TCP连接列表(键：IP地址)
        /// </summary>
        Dictionary<string, TcpClient> TCP_Client_List = new Dictionary<string, TcpClient>();

        /// <summary>
        /// 缓冲区字符串
        /// </summary>
        StringBuilder ReciveMessage = new StringBuilder();

        #endregion

        #region 接收到连接请求时触发事件

        public delegate void Connect_Delegate(IPEndPoint Local_EndPoint, IPEndPoint Net_EndPoint);
        /// <summary>
        /// 接收到来自网络的连接请求时触发事件
        /// </summary>
        public event Connect_Delegate Connect_Event;

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

        #region 连接断开时触发事件

        public delegate void Close_Delegate(IPEndPoint Local_EndPoint, IPEndPoint Net_EndPoint);

        public event Close_Delegate Close_Event;

        #endregion


        /// <summary>
        /// TCP_Touch实例化
        /// </summary>
        public TCPServer()
        { }


        /// <summary>
        /// TCP_Touch实例化
        /// </summary>
        /// <param name="_Port_Local">绑定的本地TCP通讯端口号</param>
        public TCPServer(int _Port_Local)
        {
            Start(_Port_Local);
        }

        /// <summary>
        /// 开始侦听端口并收发数据
        /// </summary>
        /// <param name="_Port_Local">绑定的本地TCP通讯端口号</param>
        public void Start(int _Port_Local)
        {
            try
            {
                Port_Local = _Port_Local;
                TCP_Listener = new TcpListener(IPAddress.Any, _Port_Local);

                //开启TCP侦听连接线程
                new Task(Listen).Start();

                //开启TCP接收数据线程
                new Task(Recive).Start();
            }
            catch (Exception e)
            {
                throw e;
            }
        }


        /// <summary>
        /// 侦听端口
        /// </summary>
        void Listen()
        {
            TCP_Listener.Start();
            
            string Net_Address;

            while (true)
            {
                try
                {
                    if (TCP_Listener != null)
                    {
                        TcpClient TCP_Custom = TCP_Listener.AcceptTcpClient();
                        Net_Address = (TCP_Custom.Client.RemoteEndPoint as IPEndPoint).Address.ToString();
                        if (TCP_Client_List.ContainsKey(Net_Address))
                        {
                            TCP_Client_List[Net_Address].Close();
                            TCP_Client_List[Net_Address] = TCP_Custom;
                        }
                        else
                            TCP_Client_List.Add(Net_Address, TCP_Custom);

                        Connect_Event((IPEndPoint)TCP_Custom.Client.LocalEndPoint, (IPEndPoint)TCP_Custom.Client.RemoteEndPoint);
                    }
                    else
                        break;
                }
                catch 
                { }
            }
        }

        /// <summary>
        /// 接收数据
        /// </summary>
        void Recive()
        {
            string Command = null;
            byte[] bs = null;
            string[] Keys = null;
            while (true)
            {
                Thread.Sleep(10);

                try
                {
                    if (TCP_Listener != null)
                    {
                        TCP_Client_List.Keys.CopyTo(Keys, 0);
                        foreach (string Address in Keys)
                        {
                            Thread.Sleep(1);

                            TcpClient item = TCP_Client_List[Address];
                            
                            if (item.Connected)
                            {
                                if (item.Available > 0)
                                {
                                    bs = new byte[item.Available];
                                    item.Client.Receive(bs);
                                    ReciveMessage.Append(BitConverter.ToString(bs).ToUpper().Trim().Replace("-", string.Empty));

                                    Command = ReciveMessage.ToString();
                                    ReciveMessage.Remove(0, AnalyCommand(ref Command));

                                    if (!string.IsNullOrEmpty(Command))
                                        Data_Recive_Event?.Invoke(Command, (IPEndPoint)item.Client.LocalEndPoint, (IPEndPoint)item.Client.RemoteEndPoint);
                                }
                            }
                            else
                            {
                                Close_Event?.Invoke((IPEndPoint)item.Client.LocalEndPoint, (IPEndPoint)item.Client.RemoteEndPoint);
                                TCP_Client_List.Remove(Address);
                            }
                        }
                        //foreach (string Address in TCP_Client_List.Select(x => x.Key))
                        //{

                        //}
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
        public void Send(string Data, string Net_Address)
        {
            try
            {
                if (string.IsNullOrEmpty(Data))
                    throw new Exception("数据为空");

                if (TCP_Client_List.ContainsKey(Net_Address))
                {
                    if (TCP_Client_List[Net_Address].Connected)
                    {
                        if (Data.Length % 2 != 0)
                            Data += "0";

                        int ByteNum = Data.Length / 2;
                        byte[] bs = new byte[ByteNum];
                        for (int i = 0; i < ByteNum; i++)
                        {
                            bs[i] = Convert.ToByte(Data.Substring(i * 2, 2), 16);
                        }

                        if (TCP_Client_List[Net_Address].Client.Send(bs, 0, bs.Length, SocketFlags.None) > 0)
                            Thread.Sleep(10);
                        else
                            throw new Exception("发送失败");
                    }
                }
                else
                    throw new Exception("连接断开");
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
            TCP_Listener.Stop();
            TCP_Listener = null;
            foreach (TcpClient item in TCP_Client_List.Values)
            {
                item.Close();
            }
            TCP_Client_List.Clear();
        }
    }
}
