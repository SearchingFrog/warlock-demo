using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Warlock.Networking
{
	[Serializable]
	public struct Vector2
	{
		public int X;
		public int Y;

		public override string ToString()
		{
			return string.Format("X: {0}, Y: {1}", this.X, this.Y);
		}
	}
}
