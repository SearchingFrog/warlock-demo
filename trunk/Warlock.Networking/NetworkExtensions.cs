using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Warlock.Networking
{
	static class NetworkExtensions
	{
		public static bool IsConnected(this TcpClient client)
		{
			try
			{
				bool connected = !(client.Client.Poll(1, SelectMode.SelectRead) && client.Client.Available == 0);

				return connected;
			}
			catch
			{
				return false;
			}
		}
	}
}
