using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Warlock.Networking
{
	public interface ISerializer
	{
		byte[] Serialize<T>(T obj);
		T Deserialize<T>(byte[] data);
	}
}
