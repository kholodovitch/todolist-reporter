using System.Collections.Generic;

namespace tool
{
	internal class TaskWithChildIntervals
	{
		public TodoListTask Task { get; set; }

		public Dictionary<TodoListTask, List<LogItemInterval>> Intervals { get; set; }
	}
}