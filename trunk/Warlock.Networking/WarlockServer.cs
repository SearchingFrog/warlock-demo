using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Warlock.Networking.Game
{
	public class WarlockServer : IDisposable
	{
		private Server server;
		private bool isDisposed;
		private Game game;
		
		public WarlockServer(Server server)
		{
			this.server = server;
		}

		public void StartSending()
		{
			server.ClientConnected += (obj, eventArgs) =>
			{
				server.SendTo(eventArgs.Client.Id, "Hai from server");
				if (server.Clients.Count > 0)
				{
					int[] indices = server.Clients.Select(x => x.Key.Id).ToArray();
					this.game = new Game(indices);
					foreach (var player in server.Clients.Keys)
					{
						server.SendTo(player.Id, Notification.CreateGameStarted(player.Id, indices));
					}
					game.PlayerActed += (sdr, not) =>
					{
						foreach (var player in server.Clients.Keys)
						{
							server.SendTo(player.Id, not);
						}
					};
					game.PlatformCollapsed += (src, eventz) =>
					{
						foreach (var item in server.Clients)
						{
							server.SendTo(item.Key.Id, Notification.CreatePlatformCollapse());
						}
					};
				}
			};

			this.server.Start();

			Action<Task> action = null;
			action = (task) =>
			{
				Task.Run((Action)this.ReceiveDataAsync).ContinueWith(action);
			};
			action(Task.Delay(0));
		}

		private void ReceiveDataAsync()
		{
			Lazy<ConcurrentBag<int>> disconnected = new Lazy<ConcurrentBag<int>>(() => new ConcurrentBag<int>());
			Parallel.ForEach(this.server.Clients, (client) =>
			{
				NetworkStream stream = client.Value.GetStream();

				if (!client.Value.IsConnected())
				{
					disconnected.Value.Add(client.Key.Id);
					// Note: Return for parallel foreach. If you decide to use sequential foreach replace it with continue;
					return;
				}
				while (stream.DataAvailable)
				{
					Notification notification = ServiceManager.Networker.ReceiveFromStream(stream);
					string data = notification.ToString().Replace("\n", "");
					switch (notification.Type)
					{
						case NotificationType.Position:
							game.ClientActionReceived(notification);
							break;
					}
				}
			});

			// If none has disconnected skip the rest of the function (since otherwise the call to disconnected.Value will initialize the lazy)
			if (!disconnected.IsValueCreated)
				return;

			foreach (var client in disconnected.Value)
			{
				this.server.KickPlayer(client, "Disconnected");
			}
		}

		public void HandleInput(TextReader input, TextWriter output)
		{
			bool isRunning = true;
			IDictionary<string, Command> commands = new Dictionary<string, Command>(Command.Library);
			Command help = new Command(
				name: "help",
				annotation: "Lists all commands.",
				action: (srvr, args) =>
				{
					string intro = "*** The following are all available commands: ***";
					output.WriteLine(intro);
					foreach (var command in commands.Values)
					{
						output.WriteLine("*    - " + command.Name + "\n   " + command.Annotation);
					}
					output.WriteLine(new string('*', intro.Length));
				});
			commands.Add("help", help);

			char[] delimiter = new char[] { ' ' };
			while (isRunning)
			{
				string data = input.ReadLine();
				string[] split = data.Split(delimiter);
				string command = split[0].ToLowerInvariant();
				if (commands.ContainsKey(command))
				{
					commands[command].Action(this.server, split.Skip(1).ToArray());
				}
				else
				{
					output.WriteLine("No such command.");
				}
			}
		}

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		~WarlockServer()
		{
			this.Dispose(false);
		}

		private void Dispose(bool disposing)
		{
			if (this.isDisposed)
				return;

			if (disposing)
			{
				this.server.Dispose();
			}

			this.server = null;
			this.isDisposed = true;
		}
	}
}
