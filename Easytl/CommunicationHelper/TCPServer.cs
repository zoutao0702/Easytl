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
    public abstract partial class TCPServer
    {
        #region 公共属性

        /// <summary>  
        /// 客户端列表  
        /// </summary>  
        public List<AsyncUserToken> ClientList { get; private set; }

        #endregion


        #region 协议参数

        /// <summary>
        /// 协议头
        /// </summary>
        protected virtual string CommandHead { get; } = string.Empty;

        #endregion


        #region 内部使用变量

        #endregion


        #region 接收协议时触发事件

        /// <summary>
        /// 接收到协议时触发事件
        /// </summary>
        public event EventHandler<ReciveCommandEventArgs> ReciveCommand;

        /// <summary>
        /// 接收协议异常时触发事件
        /// </summary>
        public event EventHandler<Exception> ReciveCommandException;

        #endregion


        /// <summary>
        /// 接收数据
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
        /// 获取协议内容长度
        /// </summary>
        /// <param name="Command">最小长度的协议</param>
        /// <returns>返回协议内容长度（16进制字符串总长度）</returns>
        protected abstract int GetCommandLength(string Command);

        /// <summary>
        /// 发送数据
        /// </summary>
        /// <param name="token">要发送的客户端</param>
        /// <param name="Data">要发送的数据</param>
        public virtual void Send(AsyncUserToken token, byte[] Data)
        {
            try
            {
                if (token.Socket.Send(Data) <= 0)
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
        /// <param name="token">要发送的客户端</param>
        /// <param name="Command">16进制协议字符串</param>
        public virtual void Send(AsyncUserToken token, string Command)
        {
            if (string.IsNullOrEmpty(Command) && (Command.Length % 2 != 0))
                throw new Exception("数据长度不正确");

            Send(token, Command.Str16_To_Bytes());
        }
    }

    public abstract partial class TCPServer
    {
        #region 公共属性

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

        #endregion


        #region 内部使用变量

        private int m_numConnections;   // the maximum number of connections the sample is designed to handle simultaneously 
        private int m_receiveBufferSize;// buffer size to use for each socket I/O operation 
        BufferManager m_bufferManager;  // represents a large reusable set of buffers for all socket operations
        const int opsToPreAlloc = 2;    // read, write (don't alloc buffer space for accepts)
        Socket listenSocket;            // the socket used to listen for incoming connection requests
                                        // pool of reusable SocketAsyncEventArgs objects for write, read and accept socket operations
        SocketAsyncEventArgsPool m_readPool;
        int m_totalBytesRead;           // counter of the total # bytes received by the server
        int m_numConnectedSockets;      // the total number of clients connected to the server 
        System.Threading.Semaphore m_maxNumberAcceptedClients;

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
        /// <param name="numConnections">最大连接数</param>
        /// <param name="receiveBufferSize"></param>
        public TCPServer(int numConnections, int receiveBufferSize = 1024)
        {
            m_totalBytesRead = 0;
            m_numConnectedSockets = 0;
            m_numConnections = numConnections;
            m_receiveBufferSize = receiveBufferSize;
            // allocate buffers such that the maximum number of sockets can have one outstanding read and 
            //write posted to the socket simultaneously  
            m_bufferManager = new BufferManager(receiveBufferSize * numConnections * opsToPreAlloc,
                receiveBufferSize);

            m_readPool = new SocketAsyncEventArgsPool(numConnections);
            m_maxNumberAcceptedClients = new System.Threading.Semaphore(numConnections, numConnections);

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

            // preallocate pool of SocketAsyncEventArgs objects
            SocketAsyncEventArgs readEventArg;

            for (int i = 0; i < m_numConnections; i++)
            {
                //Pre-allocate a set of reusable SocketAsyncEventArgs
                readEventArg = new SocketAsyncEventArgs();
                readEventArg.Completed += new EventHandler<SocketAsyncEventArgs>(IO_Completed);
                readEventArg.UserToken = new AsyncUserToken();

                // assign a byte buffer from the buffer pool to the SocketAsyncEventArg object
                m_bufferManager.SetBuffer(readEventArg);

                // add SocketAsyncEventArg to the pool
                m_readPool.Push(readEventArg);
            }

        }

        // Starts the server such that it is listening for 
        // incoming connection requests.    
        //
        // <param name="localEndPoint">The endpoint which the server will listening 
        // for connection requests on</param>
        /// <summary>
        /// 开始侦听
        /// </summary>
        /// <param name="ip">本地侦听IP</param>
        /// <param name="port">本地侦听端口</param>
        public virtual void Start(string ip, int port)
        {
            // create the socket which listens for incoming connections
            IPEndPoint localEndPoint = new IPEndPoint(IPAddress.Parse(ip), port);
            listenSocket = new Socket(localEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            listenSocket.Bind(localEndPoint);
            // start the server with a listen backlog of 100 connections
            listenSocket.Listen(100);

            // post accepts on the listening socket
            StartAccept(null);
        }


        // Begins an operation to accept a connection request from the client 
        //
        // <param name="acceptEventArg">The context object to use when issuing 
        // the accept operation on the server's listening socket</param>
        private void StartAccept(SocketAsyncEventArgs acceptEventArg)
        {
            if (acceptEventArg == null)
            {
                acceptEventArg = new SocketAsyncEventArgs();
                acceptEventArg.Completed += new EventHandler<SocketAsyncEventArgs>(AcceptEventArg_Completed);
            }
            else
            {
                // socket must be cleared since the context object is being reused
                acceptEventArg.AcceptSocket = null;
            }

            m_maxNumberAcceptedClients.WaitOne();
            bool willRaiseEvent = listenSocket.AcceptAsync(acceptEventArg);
            if (!willRaiseEvent)
                ProcessAccept(acceptEventArg);
        }

        // This method is the callback method associated with Socket.AcceptAsync 
        // operations and is invoked when an accept operation is complete
        //
        void AcceptEventArg_Completed(object sender, SocketAsyncEventArgs e)
        {
            ProcessAccept(e);
        }

        private void ProcessAccept(SocketAsyncEventArgs e)
        {
            Interlocked.Increment(ref m_numConnectedSockets);

            // Get the socket for the accepted client connection and put it into the 
            //ReadEventArg object user token
            SocketAsyncEventArgs readEventArgs = m_readPool.Pop();
            AsyncUserToken token = readEventArgs.UserToken as AsyncUserToken;
            token.Socket = e.AcceptSocket;
            token.RemoteEndPoint = (IPEndPoint)e.AcceptSocket.RemoteEndPoint;
            token.ConnectTime = DateTime.Now;

            if (!ClientList.Contains(token))
                ClientList.Add(token);

            if (ConnectStateChanged != null)
            {
                foreach (EventHandler<ConnectEventArgs> deleg in ConnectStateChanged.GetInvocationList())
                {
                    deleg.BeginInvoke(readEventArgs.UserToken, new ConnectEventArgs() { Connect = true }, null, null);
                }
            }

            // As soon as the client is connected, post a receive to the connection
            bool willRaiseEvent = e.AcceptSocket.ReceiveAsync(readEventArgs);
            if (!willRaiseEvent)
                ProcessReceive(readEventArgs);

            // Accept the next connection request
            StartAccept(e);
        }

        // This method is called whenever a receive or send operation is completed on a socket 
        //
        // <param name="e">SocketAsyncEventArg associated with the completed receive operation</param>
        void IO_Completed(object sender, SocketAsyncEventArgs e)
        {
            // determine which type of operation just completed and call the associated handler
            switch (e.LastOperation)
            {
                case SocketAsyncOperation.Receive:
                    ProcessReceive(e);
                    break;
                case SocketAsyncOperation.Send:
                    ProcessSend(e);
                    break;
                default:
                    throw new ArgumentException("The last operation completed on the socket was not a receive or send");
            }

        }

        // This method is invoked when an asynchronous receive operation completes. 
        // If the remote host closed the connection, then the socket is closed.  
        // If data was received then the data is echoed back to the client.
        //
        private void ProcessReceive(SocketAsyncEventArgs e)
        {
            // check if the remote host closed the connection
            AsyncUserToken token = (AsyncUserToken)e.UserToken;
            if (e.BytesTransferred > 0 && e.SocketError == SocketError.Success)
            {
                //increment the count of the total bytes receive by the server
                Interlocked.Add(ref m_totalBytesRead, e.BytesTransferred);

                ReciveEventArgs re = new ReciveEventArgs() { Data = new byte[e.BytesTransferred] };
                Array.Copy(e.Buffer, e.Offset, re.Data, 0, e.BytesTransferred);
                if (ReciveData != null)
                {
                    foreach (EventHandler<ReciveEventArgs> deleg in ReciveData.GetInvocationList())
                    {
                        deleg.BeginInvoke(token, re, null, null);
                    }
                }

                bool willRaiseEvent = token.Socket.ReceiveAsync(e);
                if (!willRaiseEvent)
                    ProcessReceive(e);
            }
            else
                CloseClientSocket(e);
        }

        // This method is invoked when an asynchronous send operation completes.  
        // The method issues another receive on the socket to read any additional 
        // data sent from the client
        //
        // <param name="e"></param>
        private void ProcessSend(SocketAsyncEventArgs e)
        {
            if (e.SocketError == SocketError.Success)
            {

            }
            else
                CloseClientSocket(e);
        }

        private void CloseClientSocket(SocketAsyncEventArgs e)
        {
            AsyncUserToken token = e.UserToken as AsyncUserToken;

            // close the socket associated with the client
            try
            {
                token.Socket.Shutdown(SocketShutdown.Send);
            }
            // throws if client process has already closed
            catch (Exception) { }
            token.Socket.Close();

            // decrement the counter keeping track of the total number of clients connected to the server
            Interlocked.Decrement(ref m_numConnectedSockets);

            // Free the SocketAsyncEventArg so they can be reused by another client
            m_readPool.Push(e);

            m_maxNumberAcceptedClients.Release();

            if (ClientList.Contains(token))
                ClientList.Remove(token);

            if (ConnectStateChanged != null)
            {
                foreach (EventHandler<ConnectEventArgs> deleg in ConnectStateChanged.GetInvocationList())
                {
                    deleg.BeginInvoke(token, new ConnectEventArgs() { Connect = false }, null, null);
                }
            }
        }

        /// <summary>
        /// 断开连接
        /// </summary>
        public virtual void DisConnect(AsyncUserToken token)
        {
            try
            {
                token.Socket.Disconnect(true);
            }
            catch (Exception) { }
        }

        /// <summary>
        /// 关闭所有连接并释放内存
        /// </summary>
        public virtual void Stop()
        {
            foreach (var item in ClientList)
            {
                DisConnect(item);
            }
            listenSocket.Close();
        }
    }
}
