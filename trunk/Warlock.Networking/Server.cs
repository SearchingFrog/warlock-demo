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
				var info = new ClientInfo(this.counter);;
				if (this.Clients.TryAdd(info, client))
				{
					Interlocked.Increment(ref this.counter);
					this.OnClientConnected(new ClientEventArgs(info));
					this.log.WriteClientConnected(info.Id);
					this.SendTo(info.Id, Notification.CreateHandshake(info.Id));
				}
			}
		}


		public void SendTo(int clientId, string message)
		{
			ClientInfo info = new ClientInfo(clientId);
			TcpClient client;
			if (this.Clients.TryGetValue(info, out client))
			{
				ServiceManager.Networker.SendOverStream(client.GetStream(), Notification.CreatePlainMessage(clientId, message));
				this.log.WriteDataSent(clientId, message);
			}
		}


		public void SendTo(int clientId, Notification message)
		{
			ClientInfo info = new ClientInfo(clientId);
			TcpClient client;
			if (this.Clients.TryGetValue(info, out client))
			{
				ServiceManager.Networker.SendOverStream(client.GetStream(), message);
				this.log.WriteDataSent(clientId, message.ToString());
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
