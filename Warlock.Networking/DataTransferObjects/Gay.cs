using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Warlock.Networking
{
	public struct Gay
	{
		public Vector2 Position;
		public HeroState State;
		public Guid Id;

		public static Gay Produce()
		{
			var hero = new Gay();

			var rng = new Random();
			hero.Position = new Vector2() { X = rng.Next(), Y = rng.Next() };
			hero.State = HeroState.Resting;
			hero.Id = Guid.NewGuid();

			return hero;
		}

		public override string ToString()
		{
			return string.Format("Name: {0}, Pos: {1}, State: {2}", this.Id, this.Position, this.State);
		}
	}
}
