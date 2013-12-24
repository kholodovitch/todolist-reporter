using System;
using System.Xml;

namespace tool
{
	internal class TodoListTask
	{
		public static TodoListTask Parse(XmlNode node)
		{
			var attributes = node.Attributes;
			if (attributes == null)
				throw new NullReferenceException("attributes");

			return new TodoListTask
				{
					Id = int.Parse(attributes["ID"].Value),
					Title = attributes["TITLE"].Value,
					Node = node
				};
		}

		public int Id { get; set; }

		public string Title { get; set; }

		public XmlNode Node { get; set; }
	}
}