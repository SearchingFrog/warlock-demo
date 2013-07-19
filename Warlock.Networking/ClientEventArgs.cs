using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Warlock.Networking
{
	public class ClientEventArgs : EventArgs
	{
		public ClientInfo Client { get; private set; }

		public ClientEventArgs(ClientInfo info)
		{
			this.Client = info;
		}
	}
}
