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

		public List<Interval> Parse(int id)
		{
			Path.Combine(_args.LogsDirectory, _args.LogFilenamePattern);
			return new List<Interval>();
		}
	}
}
