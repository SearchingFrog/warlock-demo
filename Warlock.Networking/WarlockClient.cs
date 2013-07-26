using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Warlock.Networking.Game;

namespace Warlock.Networking
{
	public class WarlockClient : IDisposable
	{
		private Client client;
		private bool isDisposed;

		public bool IsGameStarted { get; private set; }
		public GameData Data { get; private set; }
		public int Id { get; private set; }

		public WarlockClient(IPAddress address, ushort port)
		{
			client = new Client();
			client.ConnectAsync(address, port).ContinueWith((t) => this.StartReceiving()); ;
		}

		private void StartReceiving()
		{
			Action action = () =>
			{
				if (this.client.IsConnected)
				{
					foreach (var item in this.client.Receive())
					{
						if (item.Type == NotificationType.Handshake)
						{
							Id = item.ClientId;
							Console.WriteLine("Handshake completed");
						}

						if (item.Type == NotificationType.GameStarting)
						{
							Console.WriteLine("Game starting.");
							this.IsGameStarted = true;
							this.Data = new GameData((int[])item.Data);
						}
						if (item.Type == NotificationType.PlainText)
						{
							Console.WriteLine(item.Data);
						}
						if (IsGameStarted)
						{
							switch (item.Type)
							{
								case NotificationType.Position:
									Vector2 position = (Vector2)item.Data;
									Data.SetPlayerPosition(item.ClientId, position);
									break;

								case NotificationType.Collapse:
									Console.WriteLine("Collapse at: {0}", DateTime.UtcNow);
									Data.CollapsePlatform();
									break;
							};
						}
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
				Task.Run(action).ContinueWith(repeatStuff);
			};
			repeatStuff(Task.Delay(0));
		}

		public void SendNotification(Notification notification)
		{
			this.client.Send(notification);
		}

		public void HandleInput(TextReader reader)
		{
			string input = reader.ReadLine().ToLower();
			switch (input)
			{
				case "up":
					client.Send(Notification.CreatePosition(this.Id, this.Data.GetPlayerPosition(this.Id) + Vector2.Up));
					break;
			};
		}

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		~WarlockClient()
		{
			this.Dispose(false);
		}

		private void Dispose(bool disposing)
		{
			if (this.isDisposed)
				return;

			if (disposing)
			{
				this.client.Dispose();
			}
			this.client = null;
			this.isDisposed = true;
		}

	}
}
