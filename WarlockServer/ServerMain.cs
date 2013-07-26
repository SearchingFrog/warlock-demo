using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Warlock.Networking;
using Warlock.Networking.Game;

namespace Warlock.ServerApp
{
	class ServerMain
	{
		static void Main(string[] args)
		{

			using (WarlockServer server = new WarlockServer(new Server(IPAddress.Loopback, 8080, new Logger(Console.Out))))
			{

				server.StartSending();

				server.HandleInput(Console.In, Console.Out);

				Console.ReadLine();
			}
		}

	}
}
