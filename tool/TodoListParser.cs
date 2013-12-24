using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using nnDev.Components.Common;

namespace tool
{
	internal class TodoListParser
	{
		private const int Level = 3;

		private readonly Dictionary<string, TodoListTask> _days = new Dictionary<string, TodoListTask>();
		private readonly Arguments _args;
		private readonly LogParser _logParser;

		public TodoListParser(Arguments args)
		{
			_args = args;
			_logParser = new LogParser(_args);
		}

		public TimelineData Parse(int id)
		{
			var dayTask = GetDayTask(id);
			var dayTasksWithChild = dayTask
				.Node
				.GetChildsRecurse(y => y.ChildNodes.OfType<XmlNode>())
				.Where(y =>
					{
						int x = DistanceToParent(y, dayTask.Node);
						return x == Level || (x < Level && y.ChildNodes.Count == 0);
					})
				.Select(taskNode => new
					{
						Task = TodoListTask.Parse(taskNode),
						Childs = taskNode
					                    .GetChildsRecurse(y => y.ChildNodes.OfType<XmlNode>())
					                    .Union(new[] {taskNode})
					                    .Select(TodoListTask.Parse)
					})
				.ToList();

			List<TaskWithChildIntervals> intervalses = dayTasksWithChild
				.Select(x => new TaskWithChildIntervals
					{
						Task = x.Task,
						Intervals = x.Childs.ToDictionary(f => f, t => _logParser.Parse(t.Id))
					})
				.ToList();
			LogItemInterval workdayInterval = GetWorkdayInterval(intervalses);

			return new TimelineData
				{
					Intervals = intervalses,
					WorkdayInterval = workdayInterval,
				};
		}

		internal TodoListTask GetDayTask(int id)
		{
			string path = _args.Path;
			string dayKey = path + Path.GetInvalidPathChars()[0] + id;
			if (_days.ContainsKey(dayKey))
				return _days[dayKey];

			var mainDoc = new XmlDocument();
			using (var fileStram = new FileStream(path, FileMode.Open, FileAccess.Read))
				mainDoc.Load(fileStram);

			TodoListTask dayTask = mainDoc
				.GetChildsRecurse<XmlNode>(x => x.ChildNodes.OfType<XmlNode>())
				.Where(node => node.Name.Equals("TASK", StringComparison.CurrentCultureIgnoreCase))
				.Where(node => node.Attributes != null && int.Parse(node.Attributes["ID"].Value) == id)
				.Select(TodoListTask.Parse)
				.First();
			_days[dayKey] = dayTask;
			return dayTask;
		}

		private static LogItemInterval GetWorkdayInterval(List<TaskWithChildIntervals> allIntervals)
		{
			var start = new DateTime(allIntervals.Min(x => x.Intervals.Min(y => y.Value.Count() != 0 ? y.Value.Min(z => z.Start.Ticks) : long.MaxValue)));
			var end = new DateTime(allIntervals.Max(x => x.Intervals.Max(y => y.Value.Count() != 0 ? y.Value.Max(z => z.End.Ticks) : long.MinValue)));
			return new LogItemInterval { Start = start, End = end };
		}

		private static int DistanceToParent(XmlNode xmlNode, XmlNode node)
		{
			return xmlNode.ParentNode != node ? DistanceToParent(xmlNode, node, 0) : 0;
		}

		private static int DistanceToParent(XmlNode xmlNode, XmlNode node, int i)
		{
			if (xmlNode == null)
				return int.MaxValue;

			return xmlNode.ParentNode != node
				       ? DistanceToParent(xmlNode.ParentNode, node, ++i)
				       : i;
		}
	}
}