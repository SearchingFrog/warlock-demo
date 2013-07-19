using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Warlock.Networking
{
	public class Logger : IDisposable
	{
		private TextWriter writer;
		private bool isDisposed;

		public Logger(TextWriter writer)
		{
			this.writer = writer;
		}
		
		private DateTime TimeStamp
		{
			get
			{
				return DateTime.UtcNow;
			}
		}

		public void WriteClientConnected(int id)
		{
			this.writer.WriteLine("[{0}]: Client {1} connected.", this.TimeStamp, id);
		}

		public void WriteClientDisconnected(int id, string reason)
		{
			this.writer.WriteLine("[{0}]: Client {1} disconnected. Reason: {2}", this.TimeStamp, id, reason);
		}

		public void WriteClientDisconnected(int id)
		{
			this.writer.WriteLine("[{0}]: Client {1} disconnected", this.TimeStamp, id);
		}

		public void WriteDataReceived(int id, string data)
		{
			this.writer.WriteLine("[{0}]: Received data from client {1}. Contents: {2}", this.TimeStamp, id, data);
		}

		public void WriteDataReceived(int id)
		{
			this.writer.WriteLine("[{0}]: Received data from {1}", this.TimeStamp, id);
		}

		public void WriteDataSent(int id, string data)
		{
			this.writer.WriteLine("[{0}]: Sent data to {1}. Contents: {2}", this.TimeStamp, id, data);
		}

		public void WriteServerStarted()
		{
			this.writer.WriteLine("[{0}]: Server started.", this.TimeStamp);
		}

		~Logger()
		{
			this.Dispose(false);
		}

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		private void Dispose(bool disposing)
		{
			if (!this.isDisposed)
			{
				if (disposing)
				{
					this.writer.Dispose();
				}
				this.isDisposed = true;
			}

		}
	}
}
