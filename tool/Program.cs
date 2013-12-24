using System.Xml;
using ArgumentParser;

namespace tool
{
	class Program
	{
		private static TodoListParser todoListParser;

		static void Main(string[] args)
		{
			var parsedArgs = new Arguments();

			if (!Parser.ParseArgumentsWithUsage(args, parsedArgs))
				return;

			todoListParser = new TodoListParser(parsedArgs);
			TimelineData timelineDate = todoListParser.Parse(parsedArgs.Id);

			var renderer = new TimelineRenderer(parsedArgs);
			XmlNode xmlNode = todoListParser.GetDayTask(parsedArgs.Id).Node;
			renderer.Render(timelineDate, xmlNode);
		}
	}
}
