using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArgumentParser;

namespace tool
{
	class Program
	{
		static void Main(string[] args)
		{
			var parsedArgs = new Arguments();

			if (!Parser.ParseArgumentsWithUsage(args, parsedArgs))
				return;

			var logParser = new LogParser(parsedArgs);
			List<LogItemInterval> f = logParser.Parse(parsedArgs.Id);
		}
	}
}
