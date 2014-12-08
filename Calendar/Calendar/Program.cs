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
		enum TypeOfDate
		{
			day, week
		}
		struct Date
		{
			public int value;
			public TypeOfDate type;
			public bool current;
		}
		public struct Position
		{
			public int col;
			public int row;
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
				var row = 1;

				var height = (firstDay + days) / 7 * 80;
				var width = 540;
				var img = new Bitmap(width, height);
				var g = GraphicsInit(img);

				for (int i = 0; i < days; i++)
				{
					var toAdd = new Date();
					var position = new Position();
					if (i == 0 || (i + firstDay) % 7 == 0)
					{
						toAdd.type = TypeOfDate.week;
						toAdd.value = weekNumber % 52 + 1;
						toAdd.current = false;
						position.col = 1;
						position.row = row;
						AppendDay(g, toAdd, position);
						weekNumber++;
						row++;
					}
					toAdd.type = TypeOfDate.day;
					toAdd.value = i + 1;
					if (i == dateArr[0] - 1)
						toAdd.current = true;
					else
						toAdd.current = false;
					position.col = (i + firstDay) % 7 + 2;
					position.row = (i + firstDay) / 7 + 1;
					AppendDay(g, toAdd, position);
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
		static void AppendDay(Graphics g, Date date, Position pos)
		{
			var cellSize = 60;
			var font = new Font("Calibri", 22f);
			var color = Brushes.DarkGray;
			if (date.type == TypeOfDate.week)
			{
				font = new Font(font, FontStyle.Italic);
				color = Brushes.OrangeRed;
			}
			if (date.current)
			{
				var ellDia = cellSize / 1.4f;
				g.FillEllipse(Brushes.Red, pos.col * cellSize - ellDia/2, pos.row * cellSize - ellDia/2, ellDia, ellDia);
				color = Brushes.WhiteSmoke;
			}
			var s = date.value.ToString();
			var sz = g.MeasureString(s, font);
			g.DrawString(s, font, color, new PointF(cellSize * pos.col - sz.Width / 2, cellSize * pos.row - sz.Height / 2));
			g.Flush();
		}
	}
}
