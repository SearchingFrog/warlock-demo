using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Warlock.Networking
{
	static class ServiceManager
	{
		public static INetworker Networker { get; private set; }

		public static ISerializer Serializer { get; private set; }

		static ServiceManager()
		{
			Networker = new DefaultNetworker();
			Serializer = new DefaultSerializer();
		}

		public static void Register(Func<INetworker> valueFactory)
		{
			Networker = valueFactory();
		}

		public static void Register(Func<ISerializer> valueFactory)
		{
			Serializer = valueFactory();
		}
	}
}
