
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Warlock.Networking
{
	public class NotificationEventArgs : EventArgs
	{
		public Notification Notification { get; private set; }

		public NotificationEventArgs(Notification notification)
		{
			this.Notification = notification;
		}
	}
}
