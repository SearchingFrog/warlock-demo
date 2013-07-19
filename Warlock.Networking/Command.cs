using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Warlock.Networking
{
	public struct Command
	{	
		public string Name { get; private set; }
		public string Annotation { get; private set; }
		public Action<Server, string[]> Action { get; private set; }

		private static Lazy<ReadOnlyDictionary<string, Command>> library =
			new Lazy<ReadOnlyDictionary<string, Command>>(() =>
				{
					var dict = new Dictionary<string, Command>();
					dict.Add(Command.Send.Name, Command.Send);
					dict.Add(Command.Kick.Name, Command.Kick);
					dict.Add(Command.Shutdown.Name, Command.Shutdown);
					var readable = new ReadOnlyDictionary<string, Command>(dict);
					return readable;
				});

		public Command(string name, string annotation, Action<Server, string[]> action)
			: this()
		{
			this.Name = name;
			this.Annotation = annotation;
			this.Action = action;
		}

		public static ReadOnlyDictionary<string, Command> Library
		{
			get
			{
				return library.Value;
			}
		}

		private static Command Kick
		{
			get
			{
				return new Command()
				{
					Name = "kick",
					Annotation = "Kicks the player with the given id and the specified reason. Structure: kick <id> [reason]",
					Action = (server, args) =>
						{
							int clientId;
							if (int.TryParse(args[0], out clientId))
							{
								string reason = string.Empty;
								if (args.Length > 1)
									reason = args.Skip(1).Aggregate((current, next) => current + " " + next);

								server.KickPlayer(clientId, reason);
							}
							else
							{
								Console.WriteLine("Error: Invalid id!");
							}
						},
				};
			}
		}

		private static Command Send
		{
			get
			{
				return new Command()
				{
					Name = "send",
					Annotation = "Send the player with the given id the specified message. Structure: send <id> <message>",
					Action = (server, args) =>
						{
							int clientId;
							if (int.TryParse(args[0], out clientId))
							{
								string message = string.Empty;
								if (args.Length > 1)
									message = args.Skip(1).Aggregate((current, next) => current + " " + next);

								server.SendTo(clientId, message);
							}
							else
							{
								Console.WriteLine("Error: Invalid id!");
							}
						},
				};
			}
		}

		private static Command Shutdown
		{
			get
			{
				return new Command()
				{
					Name = "shutdown",
					Annotation = "Shuts down the server. Structure: shutdown [time]",
					Action = (server, args) =>
						{
							int time = 0;
							if (args.Length > 0)
							{
								if (!int.TryParse(args[0], out time))
								{
									Console.WriteLine("Error: Invalid time. Must be in ms");
									return;
								}
							}
							Task.Delay(time).ContinueWith((t) => server.Shutdown());
						},
				};
			}
		}
	}
	
}
