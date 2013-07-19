using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using System.IO;
using System.Threading;

namespace Warlock.Networking
{
	/// <summary>
	/// A server that takes care of connecting clients, receiving and validating data and updating values on the client.
	/// </summary>
	public class Server : IDisposable
	{
		private TcpListener listener;
		private bool isDisposed = false;
		private int counter = 0;
		private Logger log;

		public ConcurrentDictionary<ClientInfo, TcpClient> Clients { get; private set; }

		public event EventHandler<ClientEventArgs> ClientConnected;

		public Server(IPAddress address, ushort port, Logger log)
		{
			IPEndPoint endPoint = new IPEndPoint(address, port);
			this.listener = new TcpListener(endPoint);
			this.Clients = new ConcurrentDictionary<ClientInfo, TcpClient>();
			this.log = log;
		}

		public void Start()
		{
			this.listener.Start();
			this.AcceptClientAsync();
			Action<Task> action = null;
			action = (task) =>
				{
					Task.Run((Action)this.ReceiveDataAsync).ContinueWith(action);
				};
			action(Task.Delay(0));

			this.log.WriteServerStarted();
		}

		public void Shutdown()
		{
			foreach (var client in this.Clients.Keys)
			{
				this.KickPlayer(client);
			}
			this.Dispose();
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
				string data = Console.ReadLine();
				string[] split = data.Split(delimiter);
				string command = split[0].ToLowerInvariant();
				if (commands.ContainsKey(command))
				{
					commands[command].Action(this, split.Skip(1).ToArray());
				}
				else
				{
					output.WriteLine("No such command.");
				}
			}
		}

		private void OnClientConnected(ClientEventArgs args)
		{
			if (ClientConnected != null)
				ClientConnected(this, args);
		}

		private async Task AcceptClientAsync()
		{
			while (!this.isDisposed)
			{
				var client = await listener.AcceptTcpClientAsync();
				var info = new ClientInfo(Interlocked.Increment(ref this.counter));;
				if (this.Clients.TryAdd(info, client))
				{
					this.OnClientConnected(new ClientEventArgs(info));
					this.log.WriteClientConnected(info.Id);
				}
			}
		}

		public void ReceiveDataAsync()
		{
			Lazy<ConcurrentBag<ClientInfo>> disconnected = new Lazy<ConcurrentBag<ClientInfo>>(() => new ConcurrentBag<ClientInfo>());
			Parallel.ForEach(this.Clients, (client) =>
			{
				NetworkStream stream = client.Value.GetStream();

				if (!client.Value.IsConnected())
				{
					disconnected.Value.Add(client.Key);
					// Note: Return for parallel foreach. If you decide to use sequential foreach replace it with continue;
					return;
				}
				while (stream.DataAvailable)
				{
					string data = ServiceManager.Networker.ReceiveFromStream<Notification>(stream).Data.ToString().Replace("\n", "");
					// Trim the data
					if (data.Length >= 15)
						data = data.Substring(0, 15) + "...";
					log.WriteDataReceived(client.Key.Id, data);
				}
			});

			// If none has disconnected skip the rest of the function (since otherwise the call to disconnected.Value will initialize the lazy)
			if (!disconnected.IsValueCreated)
				return;

			foreach (var client in disconnected.Value)
			{
				this.KickPlayer(client);			
			}
		}

		public void SendTo(int clientId, string message)
		{
			ClientInfo info = new ClientInfo(clientId);
			TcpClient client;
			if (this.Clients.TryGetValue(info, out client))
			{
				ServiceManager.Networker.SendOverStream(client.GetStream(), new Notification(message));
				this.log.WriteDataSent(clientId, message);
			}
		}

		private void KickPlayer(ClientInfo client)
		{
			if (this.TryKickPlayer(client))
			{
				log.WriteClientDisconnected(client.Id);
			}
		}

		public void KickPlayer(int id, string reason)
		{
			ClientInfo client = new ClientInfo(id);
			if (this.TryKickPlayer(client))
			{
				this.log.WriteClientDisconnected(client.Id, reason);
			}
		}

		private bool TryKickPlayer(ClientInfo client)
		{
			TcpClient connection;
			if (this.Clients.TryRemove(client, out connection))
			{
				connection.Close();
				return true;
			}
			return false;
		}

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		~Server()
		{
			this.Dispose(false);
		}

		private void Dispose(bool disposing)
		{
			if (this.isDisposed)
				return;

			if (disposing)
			{
				this.listener.Stop();
				this.listener.Server.Dispose();
				foreach (var client in this.Clients)
				{
					client.Value.Close();
				}
				this.log.Dispose();
			}

			this.listener = null;
			this.log = null;
			this.Clients.Clear();
			this.Clients = null;
			this.isDisposed = true;
		}

	}
}
