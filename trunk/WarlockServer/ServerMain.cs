using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Warlock.Networking;

namespace WarlockServer
{
	class ServerMain
	{
		static void Main(string[] args)
		{

			using (Server server = new Server(IPAddress.Loopback, 8080, new Logger(Console.Out)))
			{
				server.ClientConnected += (obj, eventArgs) =>
					{
						server.SendTo(eventArgs.Client.Id, "Hai from server");
					};

				server.Start();

				server.HandleInput(Console.In, Console.Out);

				Console.ReadLine();
			}
		}

	}
}
