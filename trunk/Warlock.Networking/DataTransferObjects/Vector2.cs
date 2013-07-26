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
		public float X;
		public float Y;

		public Vector2(float x, float y)
		{
			this.X = x;
			this.Y = y;
		}

		public override string ToString()
		{
			return string.Format("X: {0}, Y: {1}", this.X, this.Y);
		}

		public static Vector2 Zero
		{
			get
			{
				return new Vector2(0, 0);
			}
		}

		public float DistanceTo(Vector2 vector)
		{
			float dx = (this.X - vector.X);
			float dy = (this.Y - vector.Y);
			return (float)Math.Sqrt(dx * dx + dy * dy);
		}

		public static Vector2 Up
		{
			get
			{
				return new Vector2(0, 1);
			}
		}

		public static Vector2 operator +(Vector2 first, Vector2 second)
		{
			return new Vector2(first.X + second.X, first.Y + second.Y);
		}
	}
}
