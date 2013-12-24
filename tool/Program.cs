using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Xml;
using ArgumentParser;

namespace tool
{
	class Program
	{
		private static readonly Font Font = new Font(new FontFamily("Verdana"), 6);
		private static readonly Font LegendFont = new Font(new FontFamily("Verdana"), 10);
		private static TodoListParser todoListParser;

		static void Main(string[] args)
		{
			var parsedArgs = new Arguments();

			if (!Parser.ParseArgumentsWithUsage(args, parsedArgs))
				return;

			todoListParser = new TodoListParser(parsedArgs);
			List<TaskWithChildIntervals> f = todoListParser.Parse(parsedArgs.Id);
			LogItemInterval z = GetWorkdayInterval(f);

			GetValue(z, f, parsedArgs);
		}

		private static void GetValue(LogItemInterval z, List<TaskWithChildIntervals> f, Arguments parsedArgs)
		{
			var start = z.Start - new TimeSpan(0, z.Start.Minute, z.Start.Second);
			var end = z.End + new TimeSpan(0, 60 - z.End.Minute - 1, 60 - z.End.Second);
			var barRect = new Rectangle(0, 0, (int) (end - start).TotalMinutes, 32);
			var currentDate = start.Date;
			int topOffset = 32;
			int bottomOffset = 24;
			int sideOffset = 24;

			int count = f.Count;
			int legendRow = 24;
			int legendHeight = count*legendRow;

			using (var bmp = new Bitmap(barRect.Width + 2*sideOffset, barRect.Height + topOffset + bottomOffset + legendHeight))
			{
				using (var gr = Graphics.FromImage(bmp))
				{
					gr.Clear(Color.White);
					const int greyLevel = 240;
					gr.TranslateTransform(sideOffset, topOffset);
					gr.FillRectangle(new SolidBrush(Color.FromArgb(greyLevel, greyLevel, greyLevel)), barRect);
					for (int i = 0; i <= barRect.Width; i += 30)
					{
						gr.DrawLine(Pens.Black, i, barRect.Height, i, barRect.Height + 6);
						DateTime dateTime = start.AddMinutes(i);
						if (dateTime.Minute != 0)
							continue;

						if (dateTime.Date != currentDate)
						{
							DrawDayMark(start, gr, dateTime);
							currentDate = dateTime.Date;
						}

						string str = dateTime.ToShortTimeString();
						var ff = gr.MeasureString(str, Font);
						gr.DrawString(str, Font, Brushes.Black, i - ff.Width/2, barRect.Height + 6);
					}

					DrawDayMark(start, gr, start);
					DrawDayMark(start, gr, end);

					var taskBrushes = new Dictionary<int, Brush>();
					for (int i = 0; i < count; i++)
					{
						TaskWithChildIntervals taskWithChildIntervals = f[i];
						taskBrushes.Add(taskWithChildIntervals.Task.Id, GetBrush(i, 6));
					}

					foreach (TaskWithChildIntervals at0 in f)
					{
						foreach (var interval1 in at0.Intervals.SelectMany(interval => interval.Value))
						{
							gr.FillRectangle(taskBrushes[at0.Task.Id], (int) (interval1.Start - start).TotalMinutes, 0, (int) interval1.Duration.TotalMinutes, barRect.Height);
						}
					}
					gr.DrawRectangle(Pens.Black, barRect);

					gr.TranslateTransform(0, barRect.Height + bottomOffset);
					for (int i = 0; i < count; i++)
					{
						TaskWithChildIntervals a = f[i];
						string[] values = GetParents(a.Task.Node, u => u == todoListParser.GetDayTask(parsedArgs.Id).Node)
							.Select(TodoListTask.Parse)
							.Select(y => y.Title)
							.ToArray();
						string title = string.Join(" / ", values);
						int legendRectSize = 12;
						int x = (legendRow - legendRectSize)/2;
						var rect = new Rectangle(x, x + i*legendRow, legendRectSize, legendRectSize);
						gr.FillRectangle(taskBrushes[a.Task.Id], rect);
						gr.DrawRectangle(Pens.Black, rect);
						var t = gr.MeasureString(title, LegendFont);
						gr.DrawString(title, LegendFont, Brushes.Black, legendRow, i*legendRow + (legendRow - t.Height)/2.0f + 1);
					}
					gr.ResetTransform();
				}
				bmp.Save(parsedArgs.OutputBitmap);
			}
		}

		private static IEnumerable<XmlNode> GetParents(XmlNode node, Func<XmlNode, bool> breakFunc)
		{
			if (breakFunc(node))
				yield break;

			if (node.ParentNode != null)
				foreach (var parents in GetParents(node.ParentNode, breakFunc))
					yield return parents;
			yield return node;
		}

		private static Brush GetBrush(int index, int count)
		{
			Color color = GetColor(index, count);
			if (index < count * 3)
				return new SolidBrush(color);

			int i = index - count * 3;
			return new HatchBrush(HatchStyle.ForwardDiagonal, i < count ? Color.Black : Color.White, GetColor(i, count));
		}

		private static Color GetColor(int index, int count)
		{
			double u = (double)index / count;
			return ToColor(((double)(index % count) / count) * 360, Math.Truncate(u) <= 1 ? 100 : 30, Math.Truncate(u) == 1 ? 60 : 100);
		}

		public static Color ToColor(double Hue, double Saturation, double Value)
		{
			// VPHsvColor contains values scaled as in the color wheel:

			double r = 0;
			double g = 0;
			double b = 0;

			// Scale Hue to be between 0 and 360. Saturation
			// and value scale to be between 0 and 1.
			var h = Hue % 360;
			var s = Saturation / 100;
			var v = Value / 100;

			if (s == 0)
			{
				// If s is 0, all colors are the same.
				// This is some flavor of gray.
				r = v;
				g = v;
				b = v;
			}
			else
			{
				// The color wheel consists of 6 sectors.
				// Figure out which sector you're in.
				var sectorPos = h / 60;
				var sectorNumber = (int)(Math.Floor(sectorPos));

				// get the fractional part of the sector.
				// That is, how many degrees into the sector
				// are you?
				var fractionalSector = sectorPos - sectorNumber;

				// Calculate values for the three axes
				// of the color. 
				var p = v * (1 - s);
				var q = v * (1 - (s * fractionalSector));
				var t = v * (1 - (s * (1 - fractionalSector)));

				// Assign the fractional colors to r, g, and b
				// based on the sector the angle is in.
				switch (sectorNumber)
				{
					case 0:
						r = v;
						g = t;
						b = p;
						break;

					case 1:
						r = q;
						g = v;
						b = p;
						break;

					case 2:
						r = p;
						g = v;
						b = t;
						break;

					case 3:
						r = p;
						g = q;
						b = v;
						break;

					case 4:
						r = t;
						g = p;
						b = v;
						break;

					case 5:
						r = v;
						g = p;
						b = q;
						break;
				}
			}

			try
			{
				// return an RgbColor structure, with values scaled
				// to be between 0 and 255.
				return Color.FromArgb(Normilize(r * 255), Normilize(g * 255), Normilize(b * 255));
			}
			catch (Exception ex)
			{
				return Color.Empty;
			}
		}

		private static int Normilize(double colorComponent)
		{
			return (int)Math.Max(Math.Min(colorComponent, 255), 0);
		}

		private static void DrawDayMark(DateTime start, Graphics gr, DateTime dateTime)
		{
			var offset = (float)(dateTime - start).TotalMinutes;
			gr.DrawLine(Pens.Black, offset, 0, offset, -6);
			string str1 = dateTime.ToShortTimeString();
			var ff1 = gr.MeasureString(str1, Font);
			gr.DrawString(str1, Font, Brushes.Black, -ff1.Width / 2 + offset, -ff1.Height - 6);

			str1 = dateTime.ToShortDateString();
			var ff2 = gr.MeasureString(str1, Font);
			gr.DrawString(str1, Font, Brushes.Black, -ff2.Width / 2 + offset, -ff1.Height - ff2.Height - 6);
		}

		public static LogItemInterval GetWorkdayInterval(IEnumerable<TaskWithChildIntervals> f)
		{
			var start = new DateTime(f.Min(x => x.Intervals.Min(y => y.Value.Count() != 0 ? y.Value.Min(z => z.Start.Ticks) : long.MaxValue)));
			var end = new DateTime(f.Max(x => x.Intervals.Max(y => y.Value.Count() != 0 ? y.Value.Max(z => z.End.Ticks) : long.MinValue)));
			return new LogItemInterval {Start = start, End = end};
		}
	}
}
