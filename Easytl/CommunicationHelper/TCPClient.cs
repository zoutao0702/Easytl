using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace Easytl.CommunicationHelper
{
    /// <summary>
    /// TCP客户端操作类
    /// </summary>
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
                if (TCP_Client != null)
                    return TCP_Client.Connected;
                return false;
            }
        }

        /// <summary>
        /// 连接超时时长（秒）
        /// </summary>
        public int ConnectTimeOut { get; set; } = 10;

        #endregion

        #region 协议参数

        /// <summary>
        /// 协议头
        /// </summary>
        public virtual string Command_Head { get; }

        /// <summary>
        /// 协议最短字节数
        /// </summary>
        public virtual int Command_MinByteCount { get; } = 0;

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

        #region 接收到数据时触发事件

        public delegate void Data_Recive_Delegate(string Recive_Data, IPEndPoint Local_EndPoint, IPEndPoint Net_EndPoint);
        /// <summary>
        /// 接收到数据时触发事件
        /// </summary>
        public event Data_Recive_Delegate Data_Recive_Event;

        #endregion

        #region 接收数据发生异常时触发事件

        public delegate void Data_ReciveException_Delegate(Exception e);

        public event Data_ReciveException_Delegate Data_ReciveException_Event;

        #endregion

        #region 连接断开时触发事件

        public delegate void Close_Delegate();

        public event Close_Delegate Close_Event;

        #endregion

        /// <summary>
        /// 连接到远程主机
        /// </summary>
        public void Connect(IPEndPoint remoteep)
        {
            TCP_Client = new TcpClient();

            Semaphore s = new Semaphore(0, 1);
            Exception ex = null;
            TCP_Client.BeginConnect(remoteep.Address, remoteep.Port, new AsyncCallback((IAsyncResult ar) =>
            {
                TcpClient client = (TcpClient)ar.AsyncState;
                try
                {
                    client.EndConnect(ar);
                    RemoteEP = remoteep;

                    //开启接收数据线程
                    new Task(Recive).Start();
                }
                catch (Exception e)
                {
                    ex = e;
                }
                finally
                {
                    s.Release();
                }
            }), TCP_Client);
            if (!s.WaitOne(ConnectTimeOut * 1000))
                ex = new Exception("连接超时");
            s.Close();
            s.Dispose();

            if (ex != null)
                throw ex;
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
                    if (TCP_Client.Connected)
                    {
                        if (TCP_Client.Available > 0)
                        {
                            bs = new byte[TCP_Client.Available];
                            TCP_Client.GetStream().Read(bs, 0, bs.Length);
                            ReciveMessage.Append(BitConverter.ToString(bs).ToUpper().Trim().Replace("-", string.Empty));

                            Command = ReciveMessage.ToString();
                            ReciveMessage.Remove(0, AnalyCommand(ref Command));

                            if (!string.IsNullOrEmpty(Command))
                                Task.Run(()=> { Data_Recive_Event?.Invoke(Command, (IPEndPoint)TCP_Client.Client.LocalEndPoint, (IPEndPoint)TCP_Client.Client.RemoteEndPoint); });
                        }
                    }
                    else
                    {
                        Close_Event?.Invoke();
                        break;
                    }
                }
                catch (Exception e)
                {
                    Data_ReciveException_Event?.Invoke(e);
                }
            }
        }

        /// <summary>
        /// 获取协议内容字节数（若重写了AnalyCommand方法，则该方法无效）
        /// </summary>
        /// <param name="Command_Min">最小长度的协议</param>
        /// <returns>返回协议内容长度</returns>
        public abstract int Body_ByteCount(string Command_Min);

        /// <summary>
        /// 分析接收到的协议数据，并把它转为正确的一条协议输出，返回应清除的协议数据长度
        /// </summary>
        public virtual int AnalyCommand(ref string Command)
        {
            int Command_SIndex = 0, Command_BodyByteCount = 0;
            if (Command_MinByteCount > 0)
            {
                if (!string.IsNullOrEmpty(Command_Head))
                    Command_SIndex = Command.IndexOf(Command_Head);
                if (Command.Length >= Command_SIndex + (Command_MinByteCount * 2))
                {
                    Command_BodyByteCount = Body_ByteCount(Command.Substring(Command_SIndex, Command_MinByteCount * 2));
                    if (Command.Length >= Command_SIndex + (Command_MinByteCount * 2) + (Command_BodyByteCount * 2))
                        Command = Command.Substring(Command_SIndex, (Command_MinByteCount * 2) + (Command_BodyByteCount * 2));
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
        /// <param name="Data">16进制字符串</param>
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

                TCP_Client.GetStream().Write(bs, 0, bs.Length);
                Thread.Sleep(10);
            }
            catch (Exception e)
            {
                throw e;
            }
        }


        /// <summary>
        /// 关闭所有连接
        /// </summary>
        public void Close()
        {
            try
            {
                ReciveMessage.Clear();

                if (TCP_Client.Connected)
                    TCP_Client.Close();
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}
