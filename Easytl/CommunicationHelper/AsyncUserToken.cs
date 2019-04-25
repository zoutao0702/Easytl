using System;
using System.Text;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

namespace Easytl.CommunicationHelper
{
    /// <summary>
    /// 异步客户端类
    /// </summary>
    public class AsyncUserToken
    {
        /// <summary>  
        /// 远程地址  
        /// </summary>  
        public IPEndPoint RemoteEndPoint { get; set; }

        /// <summary>  
        /// 通信SOKET  
        /// </summary>  
        public Socket Socket { get; set; }

        /// <summary>  
        /// 连接时间  
        /// </summary>  
        public DateTime ConnectTime { get; set; }


        /// <summary>  
        /// 协议缓存区  
        /// </summary>  
        internal StringBuilder CommandString;


        /// <summary>
        /// 实例化
        /// </summary>
        public AsyncUserToken()
        {
            this.CommandString = new StringBuilder();
        }
    }
}
