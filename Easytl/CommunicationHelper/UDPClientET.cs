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
    public class UDPClientET
    {
        #region 公共属性

        /// <summary>
        /// 本地UDP端口
        /// </summary>
        public int Port { get; private set; }

        #endregion

        #region 公共事件

        /// <summary>
        /// 接收到数据事件
        /// </summary>
        public event EventHandler<ReciveClientEventArgs> ReciveData;

        /// <summary>
        /// 接收到数据事件
        /// </summary>
        public event EventHandler<ReciveClientHexEventArgs> ReciveDataHex;

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

        Socket udpclient = null;

        Thread thread_Recive = null;

        #endregion

        /// <summary>
        /// 初始化连接类
        /// </summary>
        public void Start(int port)
        {
            Port = port;
            udpclient = new Socket(SocketType.Dgram, ProtocolType.Udp);
            udpclient.Bind(new IPEndPoint(IPAddress.Any, Port));

            if (thread_Recive != null)
                thread_Recive.Abort();
            thread_Recive = new Thread(new ThreadStart(Recive));
            thread_Recive.IsBackground = true;
            thread_Recive.Start();
        }

        private void Recive()
        {
            while (true)
            {
                try
                {
                    if (udpclient != null)
                    {
                        EndPoint RemoteEndPoint = new IPEndPoint(IPAddress.Any, 0);
                        if (CommandLenLength > 0)
                        {
                            byte[] datalen = new byte[CommandLenIndex + CommandLenLength];
                            int rnum = udpclient.ReceiveFrom(datalen, datalen.Length, SocketFlags.None, ref RemoteEndPoint);
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
                                        datarevcount += udpclient.ReceiveFrom(data, datarevcount, dataalllen - datarevcount, SocketFlags.None, ref RemoteEndPoint);
                                    } while (datarevcount < dataalllen);

                                    if (ReciveData != null)
                                    {
                                        foreach (EventHandler<ReciveClientEventArgs> deleg in ReciveData.GetInvocationList())
                                        {
                                            deleg.BeginInvoke(this, new ReciveClientEventArgs() { RemoteEndPoint = RemoteEndPoint, Data = data }, null, null);
                                        }
                                    }

                                    if (ReciveDataHex != null)
                                    {
                                        foreach (EventHandler<ReciveClientHexEventArgs> deleg in ReciveDataHex.GetInvocationList())
                                        {
                                            deleg.BeginInvoke(this, new ReciveClientHexEventArgs() { RemoteEndPoint = RemoteEndPoint, DataHex = BitConverter.ToString(data).ToUpper().Trim().Replace("-", string.Empty) }, null, null);
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            byte[] datalen = new byte[CommandLenMaxLength];
                            int rnum = udpclient.ReceiveFrom(datalen, datalen.Length, SocketFlags.None, ref RemoteEndPoint);
                            if (rnum > 0)
                            {
                                byte[] data = new byte[rnum];
                                Array.Copy(datalen, 0, data, 0, data.Length);

                                if (ReciveData != null)
                                {
                                    foreach (EventHandler<ReciveClientEventArgs> deleg in ReciveData.GetInvocationList())
                                    {
                                        deleg.BeginInvoke(this, new ReciveClientEventArgs() { RemoteEndPoint = RemoteEndPoint, Data = data }, null, null);
                                    }
                                }

                                if (ReciveDataHex != null)
                                {
                                    foreach (EventHandler<ReciveClientHexEventArgs> deleg in ReciveDataHex.GetInvocationList())
                                    {
                                        deleg.BeginInvoke(this, new ReciveClientHexEventArgs() { RemoteEndPoint = RemoteEndPoint, DataHex = BitConverter.ToString(data).ToUpper().Trim().Replace("-", string.Empty) }, null, null);
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
        /// <param name="ipEndPoint">远程终结点</param>
        /// <param name="Data">要发送的数据</param>
        public virtual void Send(EndPoint ipEndPoint, byte[] Data)
        {
            udpclient.SendTo(Data, ipEndPoint);
        }

        /// <summary>
        /// 发送协议
        /// </summary>
        /// <param name="ipEndPoint">远程终结点</param>
        /// <param name="Command">16进制协议字符串</param>
        public virtual void Send(EndPoint ipEndPoint, string Command)
        {
            if (string.IsNullOrEmpty(Command) && (Command.Length % 2 != 0))
                throw new Exception("数据长度不正确");

            Send(ipEndPoint, Command.Str16_To_Bytes());
        }

        /// <summary>
        /// 卸载连接类
        /// </summary>
        public void Close()
        {
            if (thread_Recive != null)
            {
                thread_Recive.Abort();
                thread_Recive = null;
            }

            if (udpclient != null)
            {
                udpclient.Close();
                udpclient = null;
                Port = 0;
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class ReciveClientEventArgs : ReciveEventArgs
    {
        /// <summary>
        /// 远程终结点
        /// </summary>
        public EndPoint RemoteEndPoint { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class ReciveClientHexEventArgs : ReciveHexEventArgs
    {
        /// <summary>
        /// 远程终结点
        /// </summary>
        public EndPoint RemoteEndPoint { get; set; }
    }
}
