using System.Xml;
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

			TodoListParser todoListParser = new TodoListParser(parsedArgs);
			TimelineData timelineDate = todoListParser.Parse(parsedArgs.Id);
			XmlNode xmlNode = todoListParser.GetDayTask(parsedArgs.Id).Node;
			
			var renderer = new TimelineRenderer(parsedArgs);
			renderer.Render(timelineDate, xmlNode);
		}
	}
}
