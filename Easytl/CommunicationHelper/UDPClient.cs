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
    public abstract partial class UDPClient
    {
        #region 公共属性

        /// <summary>
        /// 本地UDP端口
        /// </summary>
        public int Port { get; private set; }

        #endregion

        #region 协议参数

        /// <summary>
        /// 协议头
        /// </summary>
        public virtual string CommandHead { get; } = string.Empty;

        #endregion

        #region 内部使用变量

        #endregion

        #region 接收协议时触发事件

        /// <summary>
        /// 接收到完整协议时触发事件
        /// </summary>
        public event EventHandler<ReciveCommandEventArgs> ReciveCommand;

        /// <summary>
        /// 接收协议发生异常时触发事件
        /// </summary>
        public event EventHandler<Exception> ReciveCommandException;

        #endregion


        /// <summary>
        /// 接收协议
        /// </summary>
        void reciveCommand(object sender, ReciveEventArgs e)
        {
            try
            {
                AsyncUserToken token = sender as AsyncUserToken;
                token.CommandString.Append(BitConverter.ToString(e.Data).ToUpper().Trim().Replace("-", string.Empty));

                string Command;
                do
                {
                    Command = CommandHepler.AnalyCommand(CommandHead, GetCommandLength, ref token.CommandString);

                    if (!string.IsNullOrEmpty(Command) && (ReciveCommand != null))
                    {
                        foreach (EventHandler<ReciveCommandEventArgs> deleg in ReciveCommand.GetInvocationList())
                        {
                            deleg.BeginInvoke(sender, new ReciveCommandEventArgs() { Command = Command }, null, null);
                        }
                    }
                } while (!string.IsNullOrEmpty(Command));
            }
            catch (Exception ex)
            {
                if (ReciveCommandException != null)
                {
                    foreach (EventHandler<Exception> deleg in ReciveCommandException.GetInvocationList())
                    {
                        deleg.BeginInvoke(sender, ex, null, null);
                    }
                }
            }
        }

        /// <summary>
        /// 获取协议长度
        /// </summary>
        /// <param name="Command">协议</param>
        /// <returns>返回协议长度</returns>
        public abstract int GetCommandLength(string Command);

        /// <summary>
        /// 发送数据
        /// </summary>
        /// <param name="remote">要发送的客户端</param>
        /// <param name="Data">要发送的数据</param>
        public void Send(IPEndPoint remote, byte[] Data)
        {
            try
            {
                if (localSocket.SendTo(Data, remote) <= 0)
                    throw new Exception("发送失败");
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        /// <summary>
        /// 发送协议
        /// </summary>
        /// <param name="remote">要发送的客户端</param>
        /// <param name="Command">16进制协议字符串</param>
        public virtual void Send(IPEndPoint remote, string Command)
        {
            if (string.IsNullOrEmpty(Command) && (Command.Length % 2 != 0))
                throw new Exception("数据长度不正确");

            Send(remote, Command.Str16_To_Bytes());
        }
    }

    public abstract partial class UDPClient
    {
        #region 公共属性

        #endregion


        #region 公共事件

        /// <summary>
        /// 接收到数据事件
        /// </summary>
        public event EventHandler<ReciveEventArgs> ReciveData;

        #endregion


        #region 内部使用变量

        private int m_receiveBufferSize;// buffer size to use for each socket I/O operation 
        BufferManager m_bufferManager;  // represents a large reusable set of buffers for all socket operations
        const int opsToPreAlloc = 2;    // read, write (don't alloc buffer space for accepts)
        Socket localSocket;            // the socket used to listen for incoming connection requests
                                       // pool of reusable SocketAsyncEventArgs objects for write, read and accept socket operations
        int m_totalBytesRead;           // counter of the total # bytes received by the server
        SocketAsyncEventArgs readEventArg;

        #endregion

        // Create an uninitialized server instance.  
        // To start the server listening for connection requests
        // call the Init method followed by Start method 
        //
        // <param name="numConnections">the maximum number of connections the sample is designed to handle simultaneously</param>
        // <param name="receiveBufferSize">buffer size to use for each socket I/O operation</param>
        /// <summary>
        /// 实例化
        /// </summary>
        /// <param name="receiveBufferSize">数据缓冲区</param>
        public UDPClient(int receiveBufferSize = 1024)
        {
            m_totalBytesRead = 0;
            m_receiveBufferSize = receiveBufferSize;
            // allocate buffers such that the maximum number of sockets can have one outstanding read and 
            //write posted to the socket simultaneously  
            m_bufferManager = new BufferManager(receiveBufferSize * opsToPreAlloc,
                receiveBufferSize);

            Init();

            ReciveData += reciveCommand;
        }

        // Initializes the server by preallocating reusable buffers and 
        // context objects.  These objects do not need to be preallocated 
        // or reused, but it is done this way to illustrate how the API can 
        // easily be used to create reusable objects to increase server performance.
        //
        private void Init()
        {
            // Allocates one large byte buffer which all I/O operations use a piece of.  This gaurds 
            // against memory fragmentation
            m_bufferManager.InitBuffer();

            //Pre-allocate a set of reusable SocketAsyncEventArgs
            readEventArg = new SocketAsyncEventArgs();
            readEventArg.Completed += new EventHandler<SocketAsyncEventArgs>(IO_Completed);
            readEventArg.UserToken = new AsyncUserToken();

            // assign a byte buffer from the buffer pool to the SocketAsyncEventArg object
            m_bufferManager.SetBuffer(readEventArg);
        }

        // Starts the server such that it is listening for 
        // incoming connection requests.    
        //
        // <param name="localEndPoint">The endpoint which the server will listening 
        // for connection requests on</param>
        /// <summary>
        /// 开始侦听
        /// </summary>
        /// <param name="port">端口号</param>
        public void Start(int port)
        {
            Port = port;

            // create the socket which listens for incoming connections
            IPEndPoint localEndPoint = new IPEndPoint(IPAddress.Any, port);
            localSocket = new Socket(localEndPoint.AddressFamily, SocketType.Dgram, ProtocolType.Udp);
            localSocket.Bind(localEndPoint);

            (readEventArg.UserToken as AsyncUserToken).Socket = localSocket;
            readEventArg.RemoteEndPoint = new IPEndPoint(IPAddress.None, 0);

            bool willRaiseEvent = localSocket.ReceiveFromAsync(readEventArg);
            if (!willRaiseEvent)
                ProcessReceiveFrom(readEventArg);
        }

        // This method is called whenever a receive or send operation is completed on a socket 
        //
        // <param name="e">SocketAsyncEventArg associated with the completed receive operation</param>
        void IO_Completed(object sender, SocketAsyncEventArgs e)
        {
            // determine which type of operation just completed and call the associated handler
            switch (e.LastOperation)
            {
                case SocketAsyncOperation.ReceiveFrom:
                    ProcessReceiveFrom(e);
                    break;
                case SocketAsyncOperation.SendTo:
                    ProcessSendTo(e);
                    break;
                default:
                    throw new ArgumentException("The last operation completed on the socket was not a receive or send");
            }

        }

        // This method is invoked when an asynchronous receive operation completes. 
        // If the remote host closed the connection, then the socket is closed.  
        // If data was received then the data is echoed back to the client.
        //
        private void ProcessReceiveFrom(SocketAsyncEventArgs e)
        {
            // check if the remote host closed the connection
            AsyncUserToken token = (AsyncUserToken)e.UserToken;
            if (e.BytesTransferred > 0 && e.SocketError == SocketError.Success)
            {
                //increment the count of the total bytes receive by the server
                Interlocked.Add(ref m_totalBytesRead, e.BytesTransferred);

                token.RemoteEndPoint = (IPEndPoint)e.RemoteEndPoint;

                ReciveEventArgs re = new ReciveEventArgs() { Data = new byte[e.BytesTransferred] };
                Array.Copy(e.Buffer, e.Offset, re.Data, 0, e.BytesTransferred);
                if (ReciveData != null)
                {
                    foreach (EventHandler<ReciveEventArgs> deleg in ReciveData.GetInvocationList())
                    {
                        deleg.BeginInvoke(token, re, null, null);
                    }
                }

                bool willRaiseEvent = token.Socket.ReceiveFromAsync(e);
                if (!willRaiseEvent)
                    ProcessReceiveFrom(e);
            }
        }

        // This method is invoked when an asynchronous send operation completes.  
        // The method issues another receive on the socket to read any additional 
        // data sent from the client
        //
        // <param name="e"></param>
        private void ProcessSendTo(SocketAsyncEventArgs e)
        {
            if (e.SocketError == SocketError.Success)
            {

            }
        }
    }
}
