using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Warlock.Networking.Game;

namespace Warlock.Networking
{
	public class NotificationSerializer : ISerializer
	{
		private static Encoding Encoding = UTF8Encoding.UTF8;

		public byte[] Serialize(Notification notification)
		{
			string asText = string.Format("{0} {1} ", (int)notification.Type, notification.ClientId);
			switch (notification.Type)
			{
				case NotificationType.Position :
					Vector2 position = (Vector2) notification.Data;
					asText += string.Format("{0} {1}", position.X, position.Y);
					break;

				case NotificationType.SpellCast :
					Spell spell = (Spell)notification.Data;
					asText += string.Format("{0}", spell.Id);
					break;

				case NotificationType.GameStarting:
					int[] indices = (int[])notification.Data;
					for (int i = 0; i < indices.Length; i++)
					{
						asText += string.Format("{0} ", indices[i]);
					}
					break;

				default:
					asText += notification.Data;
					break;
			};

			byte[] data = Encoding.GetBytes(asText);
			return data;
		}

		static char[] delimiters = new char[] { ' ' };
		public Notification Deserialize(byte[] data)
		{
			string asText = Encoding.GetString(data);
			string[] input = asText.Split(delimiters, StringSplitOptions.RemoveEmptyEntries);
			NotificationType type = (NotificationType)int.Parse(input[0]);
			int id = int.Parse(input[1]);

			switch (type)
			{
				case NotificationType.Position :
					Vector2 position = new Vector2(Single.Parse(input[2]), Single.Parse(input[3]));
					return Notification.CreatePosition(id, position);

				case NotificationType.SpellCast :
					Spell spell = new Spell(int.Parse(input[2]));
					return Notification.CreateSpellCast(id, spell);

				case NotificationType.GameStarting:
					int[] indices = new int[input.Length - 2];
					for (int i = 0; i < indices.Length; i++)
					{
						indices[i] = int.Parse(input[i + 2]);
					}
					return Notification.CreateGameStarted(id, indices);

				default:
					string text = input.Skip(2).Aggregate(string.Empty, (x, y) => x + " " + y);
					return Notification.CreateFromTextType(id, text, type);

			};
		}
	}
}
