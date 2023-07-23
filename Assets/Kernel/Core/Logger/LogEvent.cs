using System;

namespace Kernel.Core
{
	public struct LogEvent
	{
		public DateTime DateTime;
		public int Thread;
		public LogLevel Level;
		public string Message;
		public Exception Exception;
	    public int frame;

		public override string ToString()
		{
			return $"{DateTime:yyyy-MM-dd HH:mm:ss.fff} {Level,-5} {frame,10} [{Thread,3:000}] {Message}{(Exception == null ? "" : $" {Exception.GetType().FullName} {Exception.Message} {Exception.StackTrace}")}";
		}
	}
}