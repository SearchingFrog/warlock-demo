using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Warlock.Networking
{
	public struct Notification
	{
		public object Data;

		public Notification(string message)
		{
			this.Data = message;
		}

		public static implicit operator Notification(string text)
		{
			return new Notification() { Data = text };
		}

		public override string ToString()
		{
			return Data.ToString();
		}
	}
}
