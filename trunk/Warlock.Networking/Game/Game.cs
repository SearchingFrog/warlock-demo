using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace Warlock.Networking.Game
{
	public class Game
	{
		private GameData data;
		private Timer platformTimer;
		private Timer gameTimer;

		public static readonly int UpdateTimerInterval = 1000 / 30;

		public event EventHandler PlatformCollapsed;
		public event EventHandler<NotificationEventArgs> PlayerActed;

		public Game(int[] playerIndices)
		{
			data = new GameData(playerIndices);

			this.platformTimer = new Timer(GameData.PlatformCollapseInterval);
			this.platformTimer.Elapsed += OnPlatformCollapse;

			this.gameTimer = new Timer(UpdateTimerInterval);
			this.gameTimer.Elapsed += OnUpdate;
		}

		private void OnUpdate(object sender, EventArgs args)
		{

		}

		private void OnPlayerAction(Notification notification)
		{
			if (this.PlayerActed != null)
				this.PlayerActed(this, notification);
		}

		private void OnPlatformCollapse(object sender, EventArgs args)
		{
			this.data.CollapsePlatform();
			if (this.PlatformCollapsed != null)
				this.PlatformCollapsed(this, EventArgs.Empty);
		}

		public void ClientActionReceived(Notification action)
		{
			switch (action.Type)
			{
				case NotificationType.Position:
					Vector2 currentPosition = (Vector2)action.Data;
					data.SetPlayerPosition(action.ClientId, currentPosition);
					//if (currentPosition.DistanceTo(player.Position) <= player.Speed)
					//{
					//	// All is fine
					//	OnPlayerAction(action);
					//	player.Position = currentPosition;
					//}
					//else
					//{
					//	// Speed hacking
					//}
					OnPlayerAction(action);
					break;
			};
		}
	}
}
