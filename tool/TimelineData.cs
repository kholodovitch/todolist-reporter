using System.Collections.Generic;

namespace tool
{
	internal class TimelineData
	{
		public List<TaskWithChildIntervals> Intervals { get; set; }

		public LogItemInterval WorkdayInterval { get; set; }
	}
}