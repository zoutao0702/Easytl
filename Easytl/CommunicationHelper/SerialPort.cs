using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO.Ports;
using System.Threading;
using System.Threading.Tasks;

namespace Easytl.CommunicationHelper
{
    /// <summary>
    /// 串口操作类
    /// </summary>
    public abstract class SerialPort
    {
        #region 公共属性

        /// <summary>
        /// 获取COM串口号
        /// </summary>
        public int COM { get; set; }

        /// <summary>
        /// 获取波特率
        /// </summary>
        public int BaudRate { get; set; }

        /// <summary>
        /// 串口是否已打开
        /// </summary>
        public bool IsOpen
        {
            get
            {
                if (_SerialPort != null)
                    return _SerialPort.IsOpen;
                return false;
            }
        }

        /// <summary>
        /// 是否开启重连
        /// </summary>
        public bool ReConnection { get; set; }

        #endregion

        #region 协议参数

        /// <summary>
        /// 协议头
        /// </summary>
        public virtual string Command_Head { private get; set; }

        /// <summary>
        /// 协议最小长度
        /// </summary>
        public virtual int Command_MinLen { get; set; }

        #endregion

        #region 内部参数

        /// <summary>
        /// COM连接
        /// </summary>
        System.IO.Ports.SerialPort _SerialPort;

        /// <summary>
        /// 缓冲区字符串
        /// </summary>
        StringBuilder ReciveMessage = new StringBuilder();

        #endregion

        #region 接收到数据时触发事件

        public delegate void Data_Recive_Delegate(string Recive_Data, int COM_Id);

        public event Data_Recive_Delegate Data_Recive_Event;

        #endregion

        #region 发生异常时触发事件

        public delegate void Exception_Delegate(Exception e);

        public event Exception_Delegate Exception_Event;

        #endregion

        #region 串口打开时触发事件

        public delegate void Open_Delegate();

        public event Open_Delegate Open_Event;

        #endregion

        #region 串口关闭时触发事件

        public delegate void Close_Delegate();

        public event Close_Delegate Close_Event;

        #endregion


        /// <summary>
        /// SerialPort实例化
        /// </summary>
        public SerialPort()
        {
            COM = 1;
            BaudRate = 57600;
        }


        /// <summary>
        /// SerialPort实例化
        /// </summary>
        /// <param name="com">绑定的本机接收串口号</param>
        /// <param name="baudRate">波特率</param>
        public SerialPort(int com, int baudRate)
        {
            COM = com;
            BaudRate = baudRate;
        }


        /// <summary>
        /// 打开串口
        /// </summary>
        public void Open()
        {
            try
            {
                if ((_SerialPort == null) || (!_SerialPort.IsOpen))
                {
                    if (_SerialPort == null)
                        _SerialPort = new System.IO.Ports.SerialPort("COM" + COM.ToString(), BaudRate);
                    else
                    {
                        _SerialPort.PortName = "COM" + COM.ToString();
                        _SerialPort.BaudRate = BaudRate;
                    }
                    _SerialPort.Open();
                    if (_SerialPort.IsOpen)
                    {
                        Open_Event?.Invoke();

                        //开启接收数据线程
                        new Task(Recive).Start();
                    }
                    else
                        throw new Exception("串口打开失败");
                }
                else
                    throw new Exception("串口已打开");
            }
            catch (Exception e)
            {
                Exception_Event?.Invoke(e);
            }
        }


        /// <summary>
        /// 串口重连
        /// </summary>
        private void ReConn()
        {
            while (true)
            {
                Thread.Sleep(500);
                if (ReConnection)
                {
                    Open();
                    if (_SerialPort.IsOpen)
                        break;
                }
                else
                    break;
            }
        }


        /// <summary>
        /// 接收数据
        /// </summary>
        private void Recive()
        {
            string Command = null;
            byte[] bs = null;
            while (true)
            {
                Thread.Sleep(10);

                try
                {
                    if (_SerialPort.IsOpen)
                    {
                        if (_SerialPort.BytesToRead > 0)
                        {
                            bs = new byte[_SerialPort.BytesToRead];
                            _SerialPort.Read(bs, 0, bs.Length);
                            ReciveMessage.Append(BitConverter.ToString(bs).ToUpper().Trim().Replace("-", string.Empty));

                            Command = ReciveMessage.ToString();
                            ReciveMessage.Remove(0, AnalyCommand(ref Command));

                            if (!string.IsNullOrEmpty(Command))
                                Data_Recive_Event?.Invoke(Command, COM);
                        }
                    }
                    else
                    {
                        Close_Event?.Invoke();
                        if (ReConnection)
                            new Task(ReConn).Start();
                        break;
                    }
                }
                catch (Exception e)
                {
                    Exception_Event?.Invoke(e);
                }
            }
        }


        /// <summary>
        /// 获取协议内容长度（若重写了AnalyCommand方法，则该方法无效）
        /// </summary>
        /// <param name="Command_Min">最小长度的协议</param>
        /// <returns>返回协议内容长度</returns>
        public abstract int Body_Len(string Command_Min);

        /// <summary>
        /// 分析接收到的协议数据，并把它转为正确的一条协议输出，返回应清除的协议数据长度
        /// </summary>
        public virtual int AnalyCommand(ref string Command)
        {
            int Command_SIndex = 0, Command_BodySize = 0;
            if (Command_MinLen > 0)
            {
                if (!string.IsNullOrEmpty(Command_Head))
                    Command_SIndex = Command.IndexOf(Command_Head);
                if (Command.Length >= Command_SIndex + Command_MinLen)
                {
                    Command_BodySize = Body_Len(Command.Substring(Command_SIndex, Command_MinLen));
                    if (Command.Length >= Command_SIndex + Command_MinLen + Command_BodySize)
                        Command = Command.Substring(Command_SIndex, Command_MinLen + Command_BodySize);
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

                if (Data.Length % 2 != 0)
                    Data += "0";

                int ByteNum = Data.Length / 2;
                byte[] bs = new byte[ByteNum];
                for (int i = 0; i < ByteNum; i++)
                {
                    bs[i] = Convert.ToByte(Data.Substring(i * 2, 2), 16);
                }

                _SerialPort.Write(bs, 0, bs.Length);
            }
            catch (Exception e)
            {
                throw e;
            }
        }


        /// <summary>
        /// 关闭串口
        /// </summary>
        public void Close()
        {
            try
            {
                ReciveMessage.Clear();
                ReConnection = false;
                if (_SerialPort.IsOpen)
                    _SerialPort.Close();
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}
