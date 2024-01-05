using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ValidLoaderShared.Utilities
{
    // Updated class to handle port allocation with a minimum port option
    public static class PortAllocator
    {
        public static void FindAvailablePorts(out int httpPort, out int httpsPort, int minPort = 5000)
        {
            httpPort = FindAvailablePort(minPort);
            do
            {
                httpsPort = FindAvailablePort(httpPort + 1);
            } while (httpsPort == httpPort);
        }
        public static int FindAvailablePort(int minPort = 5000)
        {
            int port = minPort;
            while (true)
            {
                TcpListener l = null;
                try
                {
                    l = new TcpListener(IPAddress.Loopback, port);
                    l.Start();
                    return port; // Port is available
                }
                catch (SocketException)
                {
                    // Port is not available, try next one
                    port++;
                }
                finally
                {
                    if (l != null)
                    {
                        l.Stop();
                    }
                }
            }
        }
    }
}
