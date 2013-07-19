using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Warlock.Networking;
using System.Net;

namespace Warlock.ConsoleApp
{
	class Program
	{
		static void Main(string[] args)
		{
			using (Client client = new Client())
			{
				client.ConnectAsync(IPAddress.Loopback, 8080).ContinueWith((task) =>
				{
					client.Send("HAAAAAAAAAAAAAAAAI, SUP?");

					System.Console.WriteLine(client.Receive());
					Action action = () =>
					{
						if (client.IsConnected)
						{
							Console.WriteLine("Sending data now - {0}", DateTime.Now);
							client.Send(new Notification() { Data = "Another batch" });

							foreach (var item in client.Receive())
							{								
								Console.WriteLine("Received data: {0}", item);
							}
						}
						else
						{
							Console.WriteLine("I got disconnected");
						}
					};

					Action<Task> repeatStuff = null;
					repeatStuff = (t) =>
						{
							Task.Run(action).ContinueWith(k => Task.Delay(5000).ContinueWith(repeatStuff));
						};
					repeatStuff(Task.Delay(0));
				});
				Console.ReadLine();
			}
		}
	}
}
