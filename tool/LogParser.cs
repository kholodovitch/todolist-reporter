using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tool
{
	class LogParser
	{
		private readonly Arguments _args;

		public LogParser(Arguments args)
		{
			_args = args;
		}

		public List<LogItemInterval> Parse(int id)
		{
			string logName = string.Format(_args.LogFilenamePattern, id);
			string logPath = Path.Combine(_args.LogsDirectory, logName);
			string[] lines = File.ReadAllLines(logPath);
			if (lines.Length == 0)
				throw new Exception("Empty log file");

			if (!string.Equals(lines[0],"TODOTIMELOG VERSION 1"))
				throw new NotImplementedException("Supported only one released version of log-files");

			return lines
				.Skip(2)
				.AsParallel()
				.Select(x => x.Split('\t'))
				.Select(x => new
					{
						Id = int.Parse(x[0]),
						Values = x
					})
				.Where(x => x.Id == id)
				.Select(x => new LogItemInterval
					{
						Start = DateTime.Parse(x.Values[3] + " " + x.Values[4]),
						End = DateTime.Parse(x.Values[5] + " " + x.Values[6]),
						TrachedHours = double.Parse(x.Values[7])
					})
				.OrderBy(x => x.Start)
				.ToList();
		}
	}
}
