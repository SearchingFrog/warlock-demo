using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Warlock.Networking.Game
{
	public class Hero
	{
		public Vector2 Position { get; set; }
		public int Health { get; set; }
		public Spell[] Spells { get; private set; }

		public float Speed { get; set; }

		public static readonly int MaxHealth = 100;

		public static Spell[] DefaultSpells = new Spell[] { Spell.Default };

		public bool IsAlive
		{
			get
			{
				return this.Health > 0;
			}
		}

		public Hero(Vector2 position)
			: this(position, DefaultSpells)
		{ }

		public Hero(Vector2 position, Spell[] spells)
		{
			Position = position;
			Health = MaxHealth;
			Spells = new Spell[spells.Length];
			spells.CopyTo(Spells, 0);
		}
	}

	public class Spell
	{
		public int Id { get; private set; }
		public int Cooldown { get; private set; }
		public DateTime LastUsage { get; set; }
		public Func<float, Vector2> UpdatePositionCallback { get; private set; }

		public Spell(int id) :
			this(id, 0)
		{

		}

		public Spell(int id, int cooldown)
		{
			Id = id;
			Cooldown = cooldown;
			LastUsage = DateTime.MinValue;
		}

		public bool CanBeCast
		{
			get 
			{ 
				return (DateTime.UtcNow - this.LastUsage).Milliseconds < Cooldown; 
			}
		}

		public static Spell Default
		{
			get
			{
				return new Spell(0, 0);
			}
		}
	}

	public struct Magic
	{
		private static int indexer = 0;

		public int SpellId { get; private set; }
		public int Id { get; private set; }
		public Vector2 Position;
		public Vector2 Direction;
		public float speed;

		public Func<Magic, Vector2> UpdatePositionCallback { get; private set; }


		public Magic(int id) :
			this(id, Vector2.Zero, (magic) => magic.Position)
		{
		}

		public Magic(int id, Vector2 position) :
			this(id, position, (magic) => magic.Position)
		{
		}

		public Magic(int id, Vector2 position, Func<Magic, Vector2> updatePositionCallback) :
			this()
		{
			SpellId = id;
			Position = position;
			UpdatePositionCallback = updatePositionCallback;
			Interlocked.Increment(ref indexer);
		}
	}
}
