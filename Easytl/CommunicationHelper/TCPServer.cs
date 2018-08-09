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
    public abstract class TCPServer
    {
        #region 线程

        Task Listen_Task;
        Task Recive_Task;

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


        #region 接收到来自网络的连接请求时触发事件

        public delegate void Connect_Delegate(IPEndPoint Local_EndPoint, IPEndPoint Net_EndPoint);
        /// <summary>
        /// 接收到来自网络的连接请求时触发事件
        /// </summary>
        public event Connect_Delegate Connect_Event;

        #endregion


        #region 接收到来自网络的数据时触发事件

        public delegate void Data_Recive_Delegate(string Recive_Data, IPEndPoint Local_EndPoint, IPEndPoint Net_EndPoint);
        /// <summary>
        /// 接收到来自网络的数据时触发事件
        /// </summary>
        public event Data_Recive_Delegate Data_Recive_Event;

        #endregion


        /// <summary>
        /// TCP_Touch实例化
        /// </summary>
        /// <param name="_Port_Local">绑定的本地TCP通讯端口号</param>
        public TCPServer(int _Port_Local)
        {
            try
            {
                TCP_Listener = new TcpListener(IPAddress.Any, _Port_Local);

                //开启TCP侦听连接线程
                Listen_Task = new Task(Listen);
                Listen_Task.Start();

                //开启TCP接收数据线程
                Recive_Task = new Task(Recive);
                Recive_Task.Start();
            }
            catch(Exception e)
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
            while (true)
            {
                Thread.Sleep(10);

                try
                {
                    foreach (TcpClient item in TCP_Client_List.Values)
                    {
                        Thread.Sleep(1);
                        if (item.Available > 0)
                        {
                            bs = new byte[item.Available];
                            item.Client.Receive(bs);
                            ReciveMessage.Append(BitConverter.ToString(bs).ToUpper().Trim().Replace("-", string.Empty));
                            ReciveMessage.Remove(0, AnalyCommand(ReciveMessage.ToString(), out Command));

                            if (Data_Recive_Event != null)
                                Data_Recive_Event(Command, (IPEndPoint)item.Client.LocalEndPoint, (IPEndPoint)item.Client.RemoteEndPoint);
                        }
                    }
                    foreach (string Address in TCP_Client_List.Select(x => x.Key))
                    {
                        
                    }
                }
                catch
                { }
            }
        }

        /// <summary>
        /// 分析协议，必须重写此方法
        /// </summary>
        /// <returns>返回协议长度（若协议解析失败则返回失败部分长度，若协议不完整则返回0）</returns>
        public abstract int AnalyCommand(string recivemessage, out string command);


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
        public void Dispose()
        {
            TCP_Listener.Stop();
            foreach (TcpClient item in TCP_Client_List.Values)
            {
                item.Close();
            }

            if (Listen_Task != null)
                Listen_Task.Dispose();
            if (Recive_Task != null)
                Recive_Task.Dispose();
        }
    }
}
