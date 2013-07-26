using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Warlock.Networking;
using System.Net;
using Warlock.Networking.Game;
using System.IO;

namespace Warlock.ConsoleApp
{
	class Program
	{
		static void Main(string[] args)
		{
			using (WarlockClient client = new WarlockClient(IPAddress.Loopback, 8080))
			{
				while (true)
				{
					client.HandleInput(Console.In);
				}
			}
		}

	}
}
