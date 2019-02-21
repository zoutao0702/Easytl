using System;
using System.Text;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace Easytl.CommunicationHelper
{
    /// <summary>
    /// TCP客户端操作类
    /// </summary>
    public abstract partial class TCPClient
    {
        #region 公共属性

        /// <summary>
        /// 获取到远程主机的连接状态
        /// </summary>
        public bool Connected
        {
            get
            {
                if (localSocket != null)
                {
                    try
                    {
                        return localSocket.Connected;
                    }
                    catch { }
                }
                return false;
            }
        }

        #endregion

        #region 协议参数

        /// <summary>
        /// 协议头
        /// </summary>
        public virtual string Command_Head { get; }

        /// <summary>
        /// 协议最短字节数
        /// </summary>
        public virtual int Command_MinByteCount { get; }

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

                string Command = token.CommandString.ToString();
                token.CommandString.Remove(0, CommandHepler.AnalyCommand(Command_MinByteCount, Command_Head, Body_ByteCount, ref Command));

                if (!string.IsNullOrEmpty(Command) && (ReciveCommand != null))
                {
                    foreach (EventHandler<ReciveCommandEventArgs> deleg in ReciveCommand.GetInvocationList())
                    {
                        deleg.BeginInvoke(sender, new ReciveCommandEventArgs() { Command = Command }, null, null);
                    }
                }
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
        /// 获取协议内容字节数
        /// </summary>
        /// <param name="Command_Min">最小长度的协议</param>
        /// <returns>返回协议内容长度</returns>
        public abstract int Body_ByteCount(string Command_Min);

        /// <summary>
        /// 发送数据
        /// </summary>
        /// <param name="Data">要发送的数据</param>
        public void Send(byte[] Data)
        {
            try
            {
                if (localSocket.Send(Data) <= 0)
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
        /// <param name="Command">16进制协议字符串</param>
        public virtual void Send(string Command)
        {
            if (string.IsNullOrEmpty(Command) && (Command.Length % 2 != 0))
                throw new Exception("数据长度不正确");

            Send(Command.Str16_To_Bytes());
        }
    }

    public abstract partial class TCPClient
    {
        #region 公共属性

        /// <summary>
        /// 远程地址
        /// </summary>
        public string IP { get; set; }

        /// <summary>
        /// 远程端口号
        /// </summary>
        public int Port { get; set; }

        /// <summary>
        /// 重连时间间隔（秒）
        /// </summary>
        public int ReConnectInterval { get; set; } = 3;

        /// <summary>
        /// 连接超时时长（秒）
        /// </summary>
        public int ConnectTimeOut { get; set; } = 5;

        #endregion


        #region 公共事件

        /// <summary>
        /// 连接状态改变事件
        /// </summary>
        public event EventHandler<ConnectEventArgs> ConnectStateChanged;

        /// <summary>
        /// 连接异常事件
        /// </summary>
        public event EventHandler<Exception> ConnectException;

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
        bool m_reconnect;

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
        /// <param name="ip">远程地址</param>
        /// <param name="port">远程端口</param>
        /// <param name="receiveBufferSize">缓冲区大小</param>
        public TCPClient(string ip, int port, int receiveBufferSize = 1024)
        {
            IP = ip;
            Port = port;

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
        /// 连接
        /// </summary>
        public void Connect()
        {
            m_reconnect = true;

            // create the socket which listens for incoming connections
            localSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            SocketAsyncEventArgs acceptEventArg = new SocketAsyncEventArgs();
            acceptEventArg.RemoteEndPoint = new IPEndPoint(IPAddress.Parse(IP), Port);
            acceptEventArg.Completed += new EventHandler<SocketAsyncEventArgs>((object sender, SocketAsyncEventArgs e) => { ProcessConnect(e); });

            bool willRaiseEvent = localSocket.ConnectAsync(acceptEventArg);
            if (!willRaiseEvent)
                ProcessConnect(acceptEventArg);
        }

        private void ProcessConnect(SocketAsyncEventArgs e)
        {
            if (e.ConnectSocket != null)
            {
                // Get the socket for the accepted client connection and put it into the 
                //ReadEventArg object user token
                AsyncUserToken token = readEventArg.UserToken as AsyncUserToken;
                token.Socket = e.ConnectSocket;
                token.RemoteEndPoint = (IPEndPoint)e.ConnectSocket.RemoteEndPoint;
                token.ConnectTime = DateTime.Now;

                if (ConnectStateChanged != null)
                {
                    foreach (EventHandler<ConnectEventArgs> deleg in ConnectStateChanged.GetInvocationList())
                    {
                        deleg.BeginInvoke(readEventArg.UserToken, new ConnectEventArgs() { Connect = true }, null, null);
                    }
                }

                // As soon as the client is connected, post a receive to the connection
                bool willRaiseEvent = e.ConnectSocket.ReceiveAsync(readEventArg);
                if (!willRaiseEvent)
                    ProcessReceive(readEventArg);
            }
            else
            {
                e.Dispose();
                if (ConnectException != null)
                {
                    foreach (EventHandler<Exception> deleg in ConnectException.GetInvocationList())
                    {
                        deleg.BeginInvoke(readEventArg.UserToken, new Exception("连接失败"), null, null);
                    }
                }

                if (m_reconnect)
                {
                    Thread.Sleep(ReConnectInterval * 1000);
                    Connect();
                }
            }
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
                CloseSocket(e);
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
                CloseSocket(e);
        }

        private void CloseSocket(SocketAsyncEventArgs e)
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

            if (ConnectStateChanged != null)
            {
                foreach (EventHandler<ConnectEventArgs> deleg in ConnectStateChanged.GetInvocationList())
                {
                    deleg.BeginInvoke(readEventArg.UserToken, new ConnectEventArgs() { Connect = false }, null, null);
                }
            }

            if (m_reconnect)
            {
                Thread.Sleep(ReConnectInterval * 1000);
                Connect();
            }
        }

        /// <summary>
        /// 断开连接
        /// </summary>
        public void DisConnect()
        {
            m_reconnect = false;
            (readEventArg.UserToken as AsyncUserToken).Socket.Disconnect(true);
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
    public class ReciveCommandEventArgs : EventArgs
    {
        /// <summary>
        /// 协议
        /// </summary>
        public string Command { get; internal set; }
    }


    class BufferManager
    {
        int m_numBytes;                 // the total number of bytes controlled by the buffer pool
        byte[] m_buffer;                // the underlying byte array maintained by the Buffer Manager
        Stack<int> m_freeIndexPool;     // 
        int m_currentIndex;
        int m_bufferSize;

        public BufferManager(int totalBytes, int bufferSize)
        {
            m_numBytes = totalBytes;
            m_currentIndex = 0;
            m_bufferSize = bufferSize;
            m_freeIndexPool = new Stack<int>();
        }

        // Allocates buffer space used by the buffer pool
        public void InitBuffer()
        {
            // create one big large buffer and divide that 
            // out to each SocketAsyncEventArg object
            m_buffer = new byte[m_numBytes];
        }

        // Assigns a buffer from the buffer pool to the 
        // specified SocketAsyncEventArgs object
        //
        // <returns>true if the buffer was successfully set, else false</returns>
        public bool SetBuffer(SocketAsyncEventArgs args)
        {

            if (m_freeIndexPool.Count > 0)
            {
                args.SetBuffer(m_buffer, m_freeIndexPool.Pop(), m_bufferSize);
            }
            else
            {
                if ((m_numBytes - m_bufferSize) < m_currentIndex)
                {
                    return false;
                }
                args.SetBuffer(m_buffer, m_currentIndex, m_bufferSize);
                m_currentIndex += m_bufferSize;
            }
            return true;
        }

        // Removes the buffer from a SocketAsyncEventArg object.  
        // This frees the buffer back to the buffer pool
        public void FreeBuffer(SocketAsyncEventArgs args)
        {
            m_freeIndexPool.Push(args.Offset);
            args.SetBuffer(null, 0, 0);
        }

    }


    // Represents a collection of reusable SocketAsyncEventArgs objects.  
    class SocketAsyncEventArgsPool
    {
        Stack<SocketAsyncEventArgs> m_pool;

        // Initializes the object pool to the specified size
        //
        // The "capacity" parameter is the maximum number of 
        // SocketAsyncEventArgs objects the pool can hold
        public SocketAsyncEventArgsPool(int capacity)
        {
            m_pool = new Stack<SocketAsyncEventArgs>(capacity);
        }

        // Add a SocketAsyncEventArg instance to the pool
        //
        //The "item" parameter is the SocketAsyncEventArgs instance 
        // to add to the pool
        public void Push(SocketAsyncEventArgs item)
        {
            if (item == null) { throw new ArgumentNullException("Items added to a SocketAsyncEventArgsPool cannot be null"); }
            lock (m_pool)
            {
                m_pool.Push(item);
            }
        }

        // Removes a SocketAsyncEventArgs instance from the pool
        // and returns the object removed from the pool
        public SocketAsyncEventArgs Pop()
        {
            lock (m_pool)
            {
                return m_pool.Pop();
            }
        }

        // The number of SocketAsyncEventArgs instances in the pool
        public int Count
        {
            get { return m_pool.Count; }
        }

    }

    class CommandHepler
    {
        /// <summary>
        /// 分析接收到的协议数据，并把它转为正确的一条协议输出，返回应清除的协议数据长度
        /// </summary>
        public static int AnalyCommand(int Command_MinByteCount, string Command_Head, Func<string, int> Body_ByteCount, ref string Command)
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
    }
}
