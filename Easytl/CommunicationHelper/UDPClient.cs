using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace Easytl.CommunicationHelper
{
    public abstract class UDPClient
    {
        #region 公共属性

        /// <summary>
        /// 获取本地UDP连接端口
        /// </summary>
        public int Port_Local { get; private set; }

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


        #region 接收到来自网络的数据时触发事件

        public delegate void Data_Recive_Delegate(string Recive_Data, IPEndPoint Local_EndPoint, IPEndPoint Net_EndPoint);
        /// <summary>
        /// 接收到来自网络的数据时触发事件
        /// </summary>
        public event Data_Recive_Delegate Data_Recive_Event;

        #endregion


        /// <summary>
        /// UDPClient实例化
        /// </summary>
        /// <param name="_Port_Local">绑定的本机接收端口号</param>
        public UDPClient(int _Port_Local)
        {
            Port_Local = _Port_Local;
            UDP_Client = new UdpClient(Port_Local);

            //开启接收数据线程
            ThreadPool.QueueUserWorkItem(new WaitCallback(Recive));
        }


        /// <summary>
        /// 接收数据
        /// </summary>
        void Recive(object obj)
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
                            ReciveMessage.Remove(0, AnalyCommand(ReciveMessage.ToString(), out Command));

                            if (Data_Recive_Event != null)
                                Data_Recive_Event(Command, (IPEndPoint)UDP_Client.Client.LocalEndPoint, Remote_IPEndPoint);
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
            if (UDP_Client != null)
            {
                UDP_Client.Close();
                UDP_Client = null;
            }
        }
    }
}
