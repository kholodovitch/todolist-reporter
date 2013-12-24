using System;

namespace tool
{
	internal class Interval
	{
		public Interval(DateTime start, DateTime end)
		{
			Start = start;
			End = end;
		}

		public DateTime Start { get; private set; }

		public DateTime End { get; private set; }

		public TimeSpan Duration
		{
			get { return End - Start; }
		}
	}
}