using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace Warlock.Networking.Game
{
	public class GameData
	{
		private Dictionary<int, Hero> players;
		private ConcurrentBag<Magic> castSpells;
		private float platformSize;
		private float platformSizeOver2;

		public float PlatformSize
		{
			get { return this.platformSize; }
			set
			{
				this.platformSize = value;
				this.platformSizeOver2 = value / 2;
			}
		}

		public static readonly int StartRadius = 50;
		public static readonly int PlatformCollapseInterval = 15000;
		public static readonly int StartPlatformSize = 100;
		public static readonly float PlatformReduction = 0.5f;
		
		public GameData(int[] playerIndices)
		{
			this.players = new Dictionary<int,Hero>();
			// Place the players along a regular n-sided polygon with some radius 
			for (int i = 0; i < playerIndices.Length; i++)
			{
				float phi = (float) (i * 2 * Math.PI / playerIndices.Length);
				float x = (float) (StartRadius * Math.Cos(phi));
				float y = (float) (StartRadius * Math.Sin(phi));
				this.players[playerIndices[i]] = new Hero(new Vector2(x, y));
			}

			this.platformSize = StartPlatformSize;
		}

		public void SetPlayerPosition(int id, Vector2 position)
		{
			this.players[id].Position = position;
		}

		public Vector2 GetPlayerPosition(int id)
		{
			return this.players[id].Position;
		}

		public void CastSpell(int id, Spell spell)
		{
			this.players[id].Spells.First(index => spell.Id == index.Id).LastUsage = DateTime.UtcNow;
			this.castSpells.Add(new Magic(spell.Id, this.players[id].Position));
		}

		public void Update()
		{
			Action<Magic, Vector2> setPos = (spelly, pos) =>
				{
					spelly.Position = pos;
				};

			foreach (Magic magic in this.castSpells)
			{
				Vector2 position = magic.UpdatePositionCallback(magic);
				setPos(magic, position);
			}
		}

		public void CollapsePlatform()
		{
			this.PlatformSize *= GameData.PlatformReduction;
		}
	}
}
