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

        bool? connected = null;
        /// <summary>
        /// 连接状态
        /// </summary>
        public bool Connected { get { return (connected.HasValue) ? connected.Value : false; } }

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

        ///// <summary>
        ///// 接收到完整协议时触发事件
        ///// </summary>
        //public event EventHandler<ReciveCommandEventArgs> ReciveCommand;

        ///// <summary>
        ///// 接收协议发生异常时触发事件
        ///// </summary>
        //public event EventHandler<Exception> ReciveCommandException;

        #endregion

        #region 协议参数

        ///// <summary>
        ///// 协议头
        ///// </summary>
        //protected virtual string CommandHead { get; set; } = string.Empty;

        /// <summary>
        /// 协议长度字段位置
        /// </summary>
        public virtual int CommandLenIndex { get; set; } = 0;

        /// <summary>
        /// 协议长度字段长度（字节数）
        /// </summary>
        public virtual int CommandLenLength { get; set; } = 4;

        #endregion

        #region 内部使用变量

        Socket tcpclient = null;

        //StringBuilder CommandString = new StringBuilder();

        bool re_connect = false;

        #endregion

        /// <summary>
        /// 连接远程端
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="port"></param>
        public virtual void Connect(string ip, int port)
        {
            IP = ip;
            Port = port;
            re_connect = true;
            Task.Run(() =>
            {
                Connect();
            });
        }

        private void Connect()
        {
            try
            {
                tcpclient = new Socket(SocketType.Stream, ProtocolType.Tcp);
                tcpclient.Connect(IP, Port);
                if (tcpclient.Connected)
                {
                    connected = true;
                    if (ConnectStateChanged != null)
                    {
                        foreach (EventHandler<ConnectEventArgs> deleg in ConnectStateChanged.GetInvocationList())
                        {
                            deleg.BeginInvoke(this, new ConnectEventArgs() { Connect = true }, null, null);
                        }
                    }

                    Recive();
                }
                else
                    throw new SocketException();
            }
            catch (SocketException)
            {
                ConnectStateClose();
            }
        }

        private void ConnectStateClose()
        {
            if (tcpclient.Connected)
                tcpclient.Shutdown(SocketShutdown.Both);
            tcpclient.Close();

            if (connected != false)
            {
                connected = false;
                if (ConnectStateChanged != null)
                {
                    foreach (EventHandler<ConnectEventArgs> deleg in ConnectStateChanged.GetInvocationList())
                    {
                        deleg.BeginInvoke(this, new ConnectEventArgs() { Connect = false }, null, null);
                    }
                }
            }

            if (re_connect)
            {
                Task.Run(() =>
                {
                    Thread.Sleep(4000);
                    Connect();
                });
            }
        }

        private void Recive()
        {
            try
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

                Recive();
            }
            catch (Exception ex)
            {
                if (ex is SocketException)
                    ConnectStateClose();
            }
        }

        ///// <summary>
        ///// 接收协议
        ///// </summary>
        //void reciveCommand(ReciveEventArgs e)
        //{
        //    try
        //    {
        //        CommandString.Append(BitConverter.ToString(e.Data).ToUpper().Trim().Replace("-", string.Empty));

        //        string Command;
        //        do
        //        {
        //            Command = CommandHepler.AnalyCommand(CommandHead, GetCommandLength, ref CommandString);

        //            if (!string.IsNullOrEmpty(Command) && (ReciveCommand != null))
        //            {
        //                foreach (EventHandler<ReciveCommandEventArgs> deleg in ReciveCommand.GetInvocationList())
        //                {
        //                    deleg.BeginInvoke(this, new ReciveCommandEventArgs() { Command = Command }, null, null);
        //                }
        //            }
        //        } while (!string.IsNullOrEmpty(Command));
        //    }
        //    catch (Exception ex)
        //    {
        //        if (ReciveCommandException != null)
        //        {
        //            foreach (EventHandler<Exception> deleg in ReciveCommandException.GetInvocationList())
        //            {
        //                deleg.BeginInvoke(this, ex, null, null);
        //            }
        //        }
        //    }
        //}

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
        public virtual void DisConnect(bool ReConnect = false)
        {
            try
            {
                if (!ReConnect)
                    re_connect = false;
                tcpclient.Disconnect(true);
            }
            catch (Exception) { }
        }
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
