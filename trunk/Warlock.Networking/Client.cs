using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace Warlock.Networking
{
	public class Client : IDisposable
	{
		private TcpClient client;
		private NetworkStream networkStream;
		private bool isDisposed;
		private Timer timer;

		public bool IsConnected { get; private set; }

		public Client()
		{
			this.client = new TcpClient();

			// TODO: Magic numbers for the win.
			timer = new Timer(1000);
			timer.AutoReset = true;
			timer.Elapsed += (time, args) =>
				{
					if (!this.client.IsConnected())
					{
						this.IsConnected = false;
						timer.Stop();
					}
				};
		}

		public async Task ConnectAsync(IPAddress address, ushort port)
		{
			await this.client.ConnectAsync(address, port);
			this.IsConnected = true;
			this.timer.Start();
			this.networkStream = this.client.GetStream();
		}

		public void Send(Notification value)
		{
			ServiceManager.Networker.SendOverStream(this.networkStream, value);
		}

		public IEnumerable<Notification> Receive()
		{
			while (this.networkStream.DataAvailable)
			{
				yield return ServiceManager.Networker.ReceiveFromStream<Notification>(this.networkStream);
			}
		}

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		~Client()
		{
			this.Dispose(false);
		}

		private void Dispose(bool disposing)
		{
			if (this.isDisposed)
				return;

			if (disposing)
			{
				this.networkStream.Dispose();
				this.client.Close();
				this.timer.Dispose();
			}
			this.networkStream = null;
			this.client = null;

			this.isDisposed = true;
		}
	}
}
