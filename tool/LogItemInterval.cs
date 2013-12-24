using System;

namespace tool
{
	internal class LogItemInterval
	{
		public DateTime Start { get; set; }

		public DateTime End { get; set; }

		public double TrachedHours { get; set; }

		public TimeSpan Duration
		{
			get { return End - Start; }
		}
	}
}