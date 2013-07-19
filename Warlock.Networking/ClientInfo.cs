using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Warlock.Networking
{
	public struct ClientInfo
	{
		public int Id { get; private set; }

		public ClientInfo(int id) : this()
		{
			this.Id = id;
		}

		public override string ToString()
		{
			return this.Id.ToString();
		}
	}
}
