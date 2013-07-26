using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Warlock.Networking
{
	public interface ISerializer
	{
		byte[] Serialize(Notification notification);
		Notification Deserialize(byte[] data);
	}
}
