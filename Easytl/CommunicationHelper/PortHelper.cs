using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.NetworkInformation;
using System.Net;

namespace Easytl.CommunicationHelper
{
    /// <summary>
    /// 端口操作类
    /// </summary>
    public class PortHelper
    {
        /// <summary>
        /// 检测端口是否正在使用
        /// </summary>
        public static bool PortInUse(int port)
        {
            bool InUse = false;
            IPGlobalProperties IPProperties = IPGlobalProperties.GetIPGlobalProperties();

            //检测是否使用TCP占用该端口
            IPEndPoint[] IP_TCP_EndPoints = IPProperties.GetActiveTcpListeners();
            foreach (IPEndPoint EndPoint in IP_TCP_EndPoints)
            {
                if (EndPoint.Port == port)
                {
                    InUse = true;
                    break;
                }
            }

            //检测是否使用UDP占用该端口
            IPEndPoint[] IP_UDP_EndPoints = IPProperties.GetActiveUdpListeners();
            foreach (IPEndPoint EndPoint in IP_UDP_EndPoints)
            {
                if (EndPoint.Port == port)
                {
                    InUse = true;
                    break;
                }
            }

            return InUse;
        }


        /// <summary>
        /// 在范围内获取一个可使用的端口号
        /// </summary>
        public static int GetCanUsePort(int MinPort, int MaxPort)
        {
            List<int> PortList = new List<int>();
            IPGlobalProperties IPProperties = IPGlobalProperties.GetIPGlobalProperties();
            IPEndPoint[] IP_TCP_EndPoints = IPProperties.GetActiveTcpListeners();
            foreach (IPEndPoint EndPoint in IP_TCP_EndPoints)
            {
                PortList.Add(EndPoint.Port);
            }

            IPEndPoint[] IP_UDP_EndPoints = IPProperties.GetActiveUdpListeners();
            foreach (IPEndPoint EndPoint in IP_UDP_EndPoints)
            {
                PortList.Add(EndPoint.Port);
            }

            for (int i = MinPort; i <= MaxPort; i++)
            {
                if (!PortList.Contains(i))
                {
                    return i;
                }
            }

            return -1;
        }
    }
}
