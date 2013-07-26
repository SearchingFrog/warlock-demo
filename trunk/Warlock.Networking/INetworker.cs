using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Warlock.Networking
{
	public interface INetworker
	{
		void SendOverStream(Stream stream, Notification obj);
		Notification ReceiveFromStream(Stream stream);
	}
}
