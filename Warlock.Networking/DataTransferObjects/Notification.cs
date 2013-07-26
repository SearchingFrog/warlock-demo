using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Warlock.Networking.Game;

namespace Warlock.Networking
{
	public struct Notification
	{
		public int ClientId;
		public NotificationType Type;
		public object Data;

		private Notification(int id, object data, NotificationType type)
		{
			this.ClientId = id;
			this.Data = data;
			this.Type = type;
		}

		public static Notification CreatePlainMessage(int id, string message)
		{
			return new Notification(id, message, NotificationType.PlainText);
		}

		public static Notification CreatePosition(int id, Vector2 position)
		{
			return new Notification(id, position, NotificationType.Position);
		}

		public static Notification CreateHandshake(int id)
		{
			return new Notification(id, null, NotificationType.Handshake);
		}

		public static Notification CreateGameStarted(int id, int[] indices)
		{
			return new Notification(id, indices, NotificationType.GameStarting);
		}

		public static Notification CreateDisconnected(int id, string reason)
		{
			return new Notification(id, reason, NotificationType.Disconnected);
		}

		public static Notification CreatePlatformCollapse()
		{
			return new Notification(-1, null, NotificationType.Collapse);
		}

		public static Notification CreateSpellCast(int id, Spell spell)
		{
			return new Notification(id, spell, NotificationType.SpellCast);
		}

		internal static Notification CreateFromTextType(int id, string text, NotificationType type)
		{
			bool isText =
				type == NotificationType.PlainText || type == NotificationType.Handshake || 
				type == NotificationType.Disconnected || type == NotificationType.Collapse;

			if (!isText)
				throw new InvalidOperationException("Not a text type");

			return new Notification(id, text, type);
		}

		public static implicit operator Notification(string text)
		{
			return new Notification() { Data = text };
		}

		public override string ToString()
		{
			if (this.Type != NotificationType.GameStarting)
				return string.Format("Type: {0}, Id: {1}, Data: {2}", this.Type, this.ClientId, this.Data);
			else
			{
				StringBuilder builder = new StringBuilder();
				builder.AppendFormat("Type: {0}, Id: {1}, Data: ", this.Type, this.ClientId);
				int[] indices = (int[])this.Data;
				for (int i = 0; i < indices.Length; i++)
				{
					builder.AppendFormat("{0} ", indices[i]);
				}
				return builder.ToString();
			}
		}
	}

	public enum NotificationType
	{
		PlainText,
		Handshake,
		GameStarting,
		Disconnected,
		Collapse,
		Position,
		SpellCast
	}
}
