using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Warlock.Networking
{
	public interface INetworker
	{
		void SendOverStream<T>(Stream stream, T obj);
		T ReceiveFromStream<T>(Stream stream);
	}
}
