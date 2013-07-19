using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace Warlock.Networking
{
	public class DefaultSerializer : ISerializer
	{
		static JsonSerializer serializer = new JsonSerializer();
		static Encoding Encoding = Encoding.UTF8;

		public T Deserialize<T>(byte[] data)
		{
			return JsonConvert.DeserializeObject<T>(Encoding.GetString(data));
		}

		public byte[] Serialize<T>(T obj)
		{
			return Encoding.GetBytes(JsonConvert.SerializeObject(obj));
		}

	}
}
