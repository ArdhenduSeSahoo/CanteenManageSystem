using System.Globalization;
using CanteenManage.Models;
using Mono.TextTemplating;

namespace CanteenManage.Utility
{
    public class DateCalculationService
    {
        public static string FMT = "O";
        public static List<DaysOfWeekModel> GetDaysOfWeek(int? hourBeforeDisable = null)
        {

            var daysOfWeek = new List<DaysOfWeekModel>();

            List<DateTime> dates = new List<DateTime>();
            DateTime currentDatetime = DateTime.Now;
            if (((int)DateTime.Now.DayOfWeek) == 6)
            {
                currentDatetime = DateTime.Now.AddDays(2);
            }
            else if (((int)DateTime.Now.DayOfWeek) == 0)
            {
                currentDatetime = DateTime.Now.AddDays(1);
            }
            DayOfWeek currentDay = currentDatetime.DayOfWeek;
            int daysTillCurrentDay = currentDay - DayOfWeek.Monday;
            DateTime currentWeekMonday = currentDatetime.AddDays(-daysTillCurrentDay);
            DateTime currentWeekTuesday = currentWeekMonday.AddDays(1);
            DateTime currentWeekwednesday = currentWeekMonday.AddDays(2);
            DateTime currentWeekThursday = currentWeekMonday.AddDays(3);
            DateTime currentWeekFriday = currentWeekMonday.AddDays(4);
            dates.Add(currentWeekMonday);
            dates.Add(currentWeekTuesday);
            dates.Add(currentWeekwednesday);
            dates.Add(currentWeekThursday);
            dates.Add(currentWeekFriday);

            //daysOfWeek.Add(new DaysOfWeekModel
            //{
            //    DaysOfWeek = (int)currentWeekMonday.DayOfWeek,
            //    DaysOfWeekName = currentWeekMonday.DayOfWeek.ToString(),
            //    IsSelected = DateTime.Now.DayOfWeek == currentWeekMonday.DayOfWeek,
            //    IsActiveDay = currentWeekMonday.DayOfWeek>= DateTime.Now.DayOfWeek 
            //});
            foreach (var day in dates)
            {
                daysOfWeek.Add(new DaysOfWeekModel
                {
                    DaysOfWeek = (int)day.DayOfWeek,
                    DateShort = day.ToString("dd"),
                    DateFull = DateTimeToString(day),
                    DateTime = day,
                    DaysOfWeekName = day.ToString("ddd"),//.DayOfWeek.ToString(),
                    IsSelected = DateTime.Now.DayOfWeek == day.DayOfWeek,
                    IsActiveDay = ((int)day.DayOfWeek) >= ((int)DateTime.Now.DayOfWeek)
                });
            }
            //if(daysOfWeek.Where(x => x.IsActiveDay).Count() <= 0)
            //{
            //    daysOfWeek[0].IsActiveDay = true;
            //    daysOfWeek[0].IsSelected = true;
            //}
            var firstActiveDay = daysOfWeek.Where(d => d.IsActiveDay).OrderBy(d => d.DateShort).FirstOrDefault();
            if (firstActiveDay != null)
            {
                firstActiveDay.IsSelected = true;
            }
            foreach (var item in daysOfWeek)
            {
                item.IsSelected = false;
                var hourss = int.Parse(DateTime.Now.ToString("HH"));
                if (((int)item.DateTime.DayOfWeek) < ((int)DateTime.Now.DayOfWeek))
                {
                    item.IsActiveDay = false;
                }
                else if (((int)item.DateTime.DayOfWeek) == ((int)DateTime.Now.DayOfWeek))
                {
                    if (hourBeforeDisable != null && int.Parse(DateTime.Now.ToString("HH")) < hourBeforeDisable)
                    {
                        item.IsActiveDay = true;
                    }
                    else
                    {
                        item.IsActiveDay = false;
                    }
                }
                else if (((int)item.DateTime.DayOfWeek) > ((int)DateTime.Now.DayOfWeek))
                {
                    item.IsActiveDay = true;
                }
            }



            return daysOfWeek;
        }

        public static string DateTimeToString(DateTime dateTime)
        {
            return dateTime.ToString(FMT);
        }
        public static DateTime DateTimeFromString(string dateTime)
        {
            return DateTime.ParseExact(dateTime, FMT, CultureInfo.InvariantCulture);
        }
    }

    public static class DateTimeExtensions
    {
        public static DateTime StartOfWeek(this DateTime dt, DayOfWeek startOfWeek)
        {
            int diff = (7 + (dt.DayOfWeek - startOfWeek)) % 7;
            return dt.AddDays(-1 * diff).Date;
        }
    }
}
