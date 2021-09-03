using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Net.Sockets;

namespace Easytl.CommunicationHelper
{
    /// <summary>
    /// TCPClientET
    /// </summary>
    public class TCPClientET
    {
        #region 公共属性

        /// <summary>
        /// 服务端IP地址
        /// </summary>
        public string IP { get; private set; }

        /// <summary>
        /// 服务端端口号
        /// </summary>
        public int Port { get; private set; }

        /// <summary>
        /// 连接状态
        /// </summary>
        public bool Connected { get; private set; }

        #endregion

        #region 公共事件

        /// <summary>
        /// 连接状态改变事件
        /// </summary>
        public event EventHandler<ConnectEventArgs> ConnectStateChanged;

        /// <summary>
        /// 接收到数据事件
        /// </summary>
        public event EventHandler<ReciveEventArgs> ReciveData;

        /// <summary>
        /// 接收到数据事件
        /// </summary>
        public event EventHandler<ReciveHexEventArgs> ReciveDataHex;

        #endregion

        #region 协议参数

        /// <summary>
        /// 协议长度字段位置
        /// </summary>
        public virtual int CommandLenIndex { get; set; }

        /// <summary>
        /// 协议长度字段长度（字节数）
        /// </summary>
        public virtual int CommandLenLength { get; set; }

        /// <summary>
        /// 协议长度最大字段长度（字节数）
        /// </summary>
        public virtual int CommandLenMaxLength { get; set; } = 1024;

        #endregion

        #region 内部使用变量

        Socket tcpclient = null;

        Thread thread_ReConnect = null;
        Thread thread_Recive = null;

        #endregion

        /// <summary>
        /// 初始化连接类
        /// </summary>
        public void Start()
        {
            if (thread_ReConnect != null)
                thread_ReConnect.Abort();
            thread_ReConnect = new Thread(new ThreadStart(ReConnect));
            thread_ReConnect.IsBackground = true;
            thread_ReConnect.Start();

            if (thread_Recive != null)
                thread_Recive.Abort();
            thread_Recive = new Thread(new ThreadStart(Recive));
            thread_Recive.IsBackground = true;
            thread_Recive.Start();
        }

        object lock_Connect = new object();
        /// <summary>
        /// 连接远程端
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="port"></param>
        public virtual void Connect(string ip, int port)
        {
            lock (lock_Connect)
            {
                if (tcpclient != null && tcpclient.Connected)
                    throw new Exception("必须先断开连接后才能重连");
                IP = ip;
                Port = port;
            }
        }

        private void ReConnect()
        {
            while (true)
            {
                try
                {
                    if (!string.IsNullOrEmpty(IP) && Port > 0)
                    {
                        if (!Connected)
                        {
                            lock (lock_Connect)
                            {
                                if (tcpclient != null)
                                {
                                    if (tcpclient.Connected)
                                        tcpclient.Shutdown(SocketShutdown.Both);
                                    tcpclient.Close();

                                    Connected = false;
                                    ConnectStateChange();
                                }
                                tcpclient = new Socket(SocketType.Stream, ProtocolType.Tcp);
                                tcpclient.Connect(IP, Port);
                            }
                            if (tcpclient.Connected)
                            {
                                Connected = true;
                                ConnectStateChange();
                            }
                            Thread.Sleep(3000);
                        }
                        else
                            Thread.Sleep(3000);
                    }
                    else
                        Thread.Sleep(1000);
                }
                catch (Exception ex)
                {
                    Thread.Sleep(3000);
                }
            }
        }

        private void ConnectStateChange()
        {
            if (ConnectStateChanged != null)
            {
                foreach (EventHandler<ConnectEventArgs> deleg in ConnectStateChanged.GetInvocationList())
                {
                    deleg.BeginInvoke(this, new ConnectEventArgs() { Connect = Connected }, null, null);
                }
            }
        }

        private void Recive()
        {
            while (true)
            {
                try
                {
                    if (Connected && tcpclient != null)
                    {
                        if (CommandLenLength > 0)
                        {
                            byte[] datalen = new byte[CommandLenIndex + CommandLenLength];
                            int rnum = tcpclient.Receive(datalen, datalen.Length, SocketFlags.None);
                            if (rnum == datalen.Length)
                            {
                                int dataalllen = 0;
                                try
                                {
                                    dataalllen = GetDataAllLength(datalen.Skip(CommandLenIndex).ToArray());
                                }
                                catch
                                {
                                    dataalllen = 0;
                                }

                                if (dataalllen > 0)
                                {
                                    byte[] data = new byte[dataalllen];
                                    int datarevcount = rnum;
                                    datalen.CopyTo(data, datarevcount - datalen.Length);
                                    do
                                    {
                                        datarevcount += tcpclient.Receive(data, datarevcount, dataalllen - datarevcount, SocketFlags.None);
                                    } while (datarevcount < dataalllen);

                                    if (ReciveData != null)
                                    {
                                        foreach (EventHandler<ReciveEventArgs> deleg in ReciveData.GetInvocationList())
                                        {
                                            deleg.BeginInvoke(this, new ReciveEventArgs() { Data = data }, null, null);
                                        }
                                    }

                                    if (ReciveDataHex != null)
                                    {
                                        foreach (EventHandler<ReciveHexEventArgs> deleg in ReciveDataHex.GetInvocationList())
                                        {
                                            deleg.BeginInvoke(this, new ReciveHexEventArgs() { DataHex = BitConverter.ToString(data).ToUpper().Trim().Replace("-", string.Empty) }, null, null);
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            byte[] datalen = new byte[CommandLenMaxLength];
                            int rnum = tcpclient.Receive(datalen, datalen.Length, SocketFlags.None);
                            if (rnum > 0)
                            {
                                byte[] data = new byte[rnum];
                                Array.Copy(datalen, 0, data, 0, data.Length);

                                if (ReciveData != null)
                                {
                                    foreach (EventHandler<ReciveEventArgs> deleg in ReciveData.GetInvocationList())
                                    {
                                        deleg.BeginInvoke(this, new ReciveEventArgs() { Data = data }, null, null);
                                    }
                                }

                                if (ReciveDataHex != null)
                                {
                                    foreach (EventHandler<ReciveHexEventArgs> deleg in ReciveDataHex.GetInvocationList())
                                    {
                                        deleg.BeginInvoke(this, new ReciveHexEventArgs() { DataHex = BitConverter.ToString(data).ToUpper().Trim().Replace("-", string.Empty) }, null, null);
                                    }
                                }
                            }
                        }
                    }
                    else
                        Thread.Sleep(2000);
                }
                catch (Exception ex)
                {
                    if (ex is SocketException)
                    {
                        if (Connected)
                        {
                            if (tcpclient != null && tcpclient.Connected)
                            {
                                lock (lock_Connect)
                                {
                                    tcpclient.Shutdown(SocketShutdown.Both);
                                    tcpclient.Close();
                                    tcpclient = null;
                                }
                            }
                            Connected = false;
                            ConnectStateChange();
                        }
                    }
                    Thread.Sleep(2000);
                }
            }
        }

        /// <summary>
        /// 获取协议总长度
        /// </summary>
        /// <param name="DataLen">根据协议长度索引和字节数截取到的表示协议内部长度的字节数组</param>
        /// <returns>返回协议总长度</returns>
        protected virtual int GetDataAllLength(byte[] DataLen)
        {
            return Convert.ToInt32(DataLen);
        }

        /// <summary>
        /// 发送数据
        /// </summary>
        /// <param name="Data">要发送的数据</param>
        public virtual void Send(byte[] Data)
        {
            tcpclient.Send(Data);
        }

        /// <summary>
        /// 发送协议
        /// </summary>
        /// <param name="Command">16进制协议字符串</param>
        public virtual void Send(string Command)
        {
            if (string.IsNullOrEmpty(Command) && (Command.Length % 2 != 0))
                throw new Exception("数据长度不正确");

            Send(Command.Str16_To_Bytes());
        }

        /// <summary>
        /// 断开连接
        /// </summary>
        public void DisConnect(bool ReConnect = false)
        {
            if (tcpclient != null)
            {
                lock (lock_Connect)
                {
                    if (tcpclient.Connected)
                        tcpclient.Shutdown(SocketShutdown.Both);
                    tcpclient.Close();
                    tcpclient = null;
                    if (!ReConnect)
                    {
                        IP = string.Empty;
                        Port = 0;
                    }
                }

                Connected = false;
                ConnectStateChange();
            }
        }

        /// <summary>
        /// 卸载连接类
        /// </summary>
        public void Close()
        {
            if (thread_ReConnect != null)
            {
                thread_ReConnect.Abort();
                thread_ReConnect = null;
            }

            if (thread_Recive != null)
            {
                thread_Recive.Abort();
                thread_Recive = null;
            }

            if (tcpclient != null)
            {
                lock (lock_Connect)
                {
                    if (tcpclient.Connected)
                        tcpclient.Shutdown(SocketShutdown.Both);
                    tcpclient.Close();
                    tcpclient = null;
                    IP = string.Empty;
                    Port = 0;
                }
            }

            Connected = false;
            ConnectStateChange();
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class ConnectEventArgs : EventArgs
    {
        /// <summary>
        /// 连接状态
        /// </summary>
        public bool Connect { get; internal set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class ReciveEventArgs : EventArgs
    {
        /// <summary>
        /// 数据
        /// </summary>
        public byte[] Data { get; internal set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class ReciveHexEventArgs : EventArgs
    {
        /// <summary>
        /// hex
        /// </summary>
        public string DataHex { get; internal set; }
    }
}
