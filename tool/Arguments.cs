using ArgumentParser;

namespace tool
{
	public class Arguments
	{
		[Argument(ArgumentType.AtMostOnce, DefaultValue = 0)]
		public int Id;

		[Argument(ArgumentType.AtMostOnce, DefaultValue = @"todolist.tdl")]
		public string Path;

		[Argument(ArgumentType.AtMostOnce, DefaultValue = @"output.png")]
		public string OutputBitmap;

		[Argument(ArgumentType.AtMostOnce, DefaultValue = @"")]
		public string LogsDirectory;

		[Argument(ArgumentType.AtMostOnce, DefaultValue = @"{0}_Log.csv")]
		public string LogFilenamePattern;
	}
}