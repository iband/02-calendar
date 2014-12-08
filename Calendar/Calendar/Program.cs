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
				var firstDay = (int)date.DayOfWeek - 1;
				var days = DateTime.DaysInMonth(dateArr[2], dateArr[1]);
				GregorianCalendar gc = new GregorianCalendar();
				var weekNumber = gc.GetWeekOfYear(date, CalendarWeekRule.FirstDay, DayOfWeek.Monday);

				for (int i = 0; i < days; i++)
				{
					if (i == 0 || (i + firstDay) % 7 == 0)
					{
						// append week number
						// increase week number
					}
					if (i == dateArr[0] - 1)
					{
						// append circle
					}
					// append day
				}
			}
			catch (Exception e)
			{
				Console.WriteLine("Program exception: {0}", e.Message);
			}
		}
	}
}
