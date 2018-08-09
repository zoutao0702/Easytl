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
    public abstract class TCPClient
    {
        #region 公共属性

        /// <summary>
        /// 获取到远程主机的连接状态
        /// </summary>
        public bool Connected
        {
            get
            {
                return TCP_Client.Connected;
            }
        }

        #endregion

        #region 线程

        Task Recive_Task;

        #endregion

        #region 内部使用变量

        /// <summary>
        /// TCP连接
        /// </summary>
        TcpClient TCP_Client;

        IPEndPoint RemoteEP;

        /// <summary>
        /// 缓冲区字符串
        /// </summary>
        StringBuilder ReciveMessage = new StringBuilder();

        #endregion

        #region 接收到来自网络的数据时触发事件

        public delegate void Data_Recive_Delegate(string Recive_Data, IPEndPoint Local_EndPoint, IPEndPoint Net_EndPoint);
        /// <summary>
        /// 接收到来自网络的数据时触发事件
        /// </summary>
        public event Data_Recive_Delegate Data_Recive_Event;

        #endregion

        /// <summary>
        /// 连接到远程主机
        /// </summary>
        public void Connect(IPEndPoint remoteep)
        {
            TCP_Client = new TcpClient();

            ManualResetEvent timeout = new ManualResetEvent(true);
            timeout.Reset();

            TCP_Client.BeginConnect(remoteep.Address, remoteep.Port, new AsyncCallback((IAsyncResult ar) =>
            {
                TcpClient client = (TcpClient)ar.AsyncState;
                try
                {
                    client.EndConnect(ar);
                    RemoteEP = remoteep;

                    //开启接收数据线程
                    Recive_Task = new Task(Recive);
                    Recive_Task.Start();
                }
                catch (Exception e)
                {
                    throw e;
                }
                finally
                {
                    timeout.Set();
                }
            }), TCP_Client);
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
                    if (TCP_Client != null)
                    {
                        if (TCP_Client.Available > 0)
                        {
                            bs = new byte[TCP_Client.Available];
                            TCP_Client.Client.Receive(bs);
                            ReciveMessage.Append(BitConverter.ToString(bs).ToUpper().Trim().Replace("-", string.Empty));
                            ReciveMessage.Remove(0, AnalyCommand(ReciveMessage.ToString(), out Command));

                            if (Data_Recive_Event != null)
                                Data_Recive_Event(Command, (IPEndPoint)TCP_Client.Client.LocalEndPoint, (IPEndPoint)TCP_Client.Client.RemoteEndPoint);
                        }
                    }
                    else
                        break;
                }
                catch { }
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
        public virtual void Send(string Data)
        {
            try
            {
                if (string.IsNullOrEmpty(Data))
                    throw new Exception("数据为空");

                if (!TCP_Client.Connected)
                {
                    Close();
                    if (RemoteEP != null)
                        Connect(RemoteEP);
                    else
                        throw new Exception("未找到远程终端");
                }

                if (Data.Length % 2 != 0)
                    Data += "0";

                int ByteNum = Data.Length / 2;
                byte[] bs = new byte[ByteNum];
                for (int i = 0; i < ByteNum; i++)
                {
                    bs[i] = Convert.ToByte(Data.Substring(i * 2, 2), 16);
                }

                if (TCP_Client.Client.Send(bs, 0, bs.Length, SocketFlags.None) > 0)
                    Thread.Sleep(10);
                else
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
            if (TCP_Client != null)
            {
                TCP_Client.Close();
                TCP_Client = null;
            }

            if (Recive_Task != null)
                Recive_Task.Dispose();
        }
    }
}
