using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Calendar
{
	class Program
	{
		struct Position
		{
			public int col;
			public int row;
		}
		struct GraphicSettings
		{
			public Graphics g;
			public int cellSize;
			public Brush weekColor;
			public Brush dayColor;
			public Brush weekdayColor;
			public Brush currentDayColor;
			public Brush currentDayBg;
			public Font weekFont;
			public Font dayFont;
			public Font weekdayFont;
		}

		static void Main(string[] args)
		{
			if (args.Length != 1)
			{
				Console.WriteLine("Using: {0} <dd.mm.yyyy>", AppDomain.CurrentDomain.FriendlyName);
				return;
			}
			try
			{
				var dateArr = args[0].Split('.').Select(x => int.Parse(x)).ToArray();
				var date = new DateTime(dateArr[2], dateArr[1], 1);
				var firstDay = (int)date.DayOfWeek == 0 ? 6 : (int)date.DayOfWeek - 1;
				var days = DateTime.DaysInMonth(dateArr[2], dateArr[1]);
				GregorianCalendar gc = new GregorianCalendar();
				var weekNumber = gc.GetWeekOfYear(date, CalendarWeekRule.FirstDay, DayOfWeek.Monday) - 1;

				var grSet = new GraphicSettings();
				grSet.cellSize = 60;
				var weeks = (firstDay + days + 6) / 7;
				var height = (weeks + 2) * grSet.cellSize;
				var width = grSet.cellSize * 9;
				var img = new Bitmap(width, height);
				grSet.g = GraphicsInit(img);
				grSet.weekColor = Brushes.RoyalBlue;
				grSet.dayColor = Brushes.Black;
				grSet.weekdayColor = Brushes.Gray;
				grSet.currentDayColor = Brushes.WhiteSmoke;
				grSet.currentDayBg = Brushes.Red;
				grSet.dayFont = new Font("Segoe UI", 22f);
				grSet.weekFont = new Font("Segoe UI Light", 22f);
				grSet.weekdayFont = new Font("Segoe UI Light", 18f);

				AddWeekNumbers(grSet, weekNumber, weeks);
				AddDayNames(grSet);

				for (int i = 0; i < days; i++)
				{
					var position = new Position();
					var value = i + 1;
					bool current = false;
					if (i == dateArr[0] - 1)
						current = true;
					position.col = (i + firstDay) % 7 + 2;
					position.row = (i + firstDay) / 7 + 1;
					AppendDay(grSet, value, position, current);
				}
				var fileName = "calendar.png";
				img.Save(fileName);

				Process.Start(new ProcessStartInfo(fileName) { WindowStyle = ProcessWindowStyle.Normal });
			}
			catch (Exception e)
			{
				Console.WriteLine("Program exception: {0}", e.Message);
			}
		}
		static Graphics GraphicsInit(Bitmap img)
		{
			var g = Graphics.FromImage(img);
			//g.InterpolationMode = InterpolationMode.HighQualityBicubic;
			g.SmoothingMode = SmoothingMode.HighQuality;
			//g.CompositingQuality = CompositingQuality.HighQuality;
			g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
			g.FillRectangle(Brushes.White, 0, 0, img.Width, img.Height);
			return g;
		}
		static void AppendDay(GraphicSettings gr, int value, Position pos, bool current)
		{
			var color = gr.dayColor;
			if (current)
			{
				var diam = gr.cellSize / 1.4f;
				gr.g.FillEllipse(Brushes.Red, pos.col * gr.cellSize - diam/2, (pos.row + 1) * gr.cellSize - diam/2, diam, diam);
				color = Brushes.WhiteSmoke;
			}
			if (pos.col == 8)
				color = Brushes.Red;
			var s = value.ToString();
			var sz = gr.g.MeasureString(s, gr.dayFont);
			gr.g.DrawString(s, gr.dayFont, color, new PointF(gr.cellSize * pos.col - sz.Width / 2, gr.cellSize * (pos.row + 1) - sz.Height / 2));
			gr.g.Flush();
		}
		static void AddWeekNumbers(GraphicSettings gr, int weekStart, int weekCount)
		{
			for (int i = 1; i <= weekCount; i++)
			{
				weekStart = weekStart % 52 + 1;
				var s = weekStart.ToString();
				var sz = gr.g.MeasureString(s, gr.weekFont);
				gr.g.DrawString(s, gr.weekFont, gr.weekColor, new PointF(gr.cellSize - sz.Width / 2, gr.cellSize * (i + 1) - sz.Height / 2));
				gr.g.Flush();
			}
		}
		static void AddDayNames(GraphicSettings gr)
		{
			var dayNames = Enum.GetValues(typeof(DayOfWeek)).OfType<DayOfWeek>().ToList().Skip(1).Select(x => x.ToString()).ToList();
			dayNames.Add(DayOfWeek.Sunday.ToString());

			foreach (var name in dayNames)
			{
				var s = name.Substring(0, 3);
				if (s == "Sun")
					gr.weekdayColor = Brushes.Red;
				var sz = gr.g.MeasureString(s, gr.weekdayFont);
				gr.g.DrawString(s, gr.weekdayFont, gr.weekdayColor, new PointF(gr.cellSize * (dayNames.IndexOf(name) + 2) - sz.Width / 2, gr.cellSize - sz.Height / 2));
			}
		}
	}
}
