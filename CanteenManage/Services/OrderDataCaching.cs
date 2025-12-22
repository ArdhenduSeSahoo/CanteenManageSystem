using System.Collections.Concurrent;
using System.Globalization;
using CanteenManage.Models;
using CanteenManage.Utility;

namespace CanteenManage.Services
{
    public class OrderDataCaching
    {

        public ConcurrentDictionary<string, EmployeeFoodOrdersTableDataModel> OrderCacheDataDictionary;
        List<DaysOfWeekModel> DaysOfWeek;
        public string FMT = "O";
        public OrderDataCaching()
        {

            OrderCacheDataDictionary = new ConcurrentDictionary<string, EmployeeFoodOrdersTableDataModel>();
            DaysOfWeek = new List<DaysOfWeekModel>();
        }
        public List<DaysOfWeekModel> GetDayOfWeeks()
        {
            //if (DateTime.Now.DayOfWeek != DayOfWeek.Sunday && this.DaysOfWeek.Count > 0)
            //{
            //    return DaysOfWeek;
            //}
            var daysOfWeek = new List<DaysOfWeekModel>();

            List<DateTime> TwoWeekdates = new List<DateTime>();
            DateTime today = DateTime.Now;
            DayOfWeek firstDayOfWeek = CultureInfo.CurrentCulture.DateTimeFormat.FirstDayOfWeek;

            firstDayOfWeek = DayOfWeek.Monday;

            int diff = (7 + (today.DayOfWeek - firstDayOfWeek)) % 7;
            DateTime startOfCurrentWeek = today.AddDays(-1 * diff);


            List<DateTime> currentWeekDates = new List<DateTime>();
            List<DateTime> nextWeekDates = new List<DateTime>();

            //list range is 0 to 7 but
            // Fill lists start from 1 is monday end with 6 is fryday
            for (int i = 0; i < 5; i++)
            {
                currentWeekDates.Add(startOfCurrentWeek.AddDays(i));
                nextWeekDates.Add(startOfCurrentWeek.AddDays(i + 7));
            }

            TwoWeekdates.AddRange(currentWeekDates);
            TwoWeekdates.AddRange(nextWeekDates);

            DateCalculationHelper dateCalculationHelper = new DateCalculationHelper();

            foreach (var day in TwoWeekdates)
            {
                daysOfWeek.Add(new DaysOfWeekModel
                {
                    DaysOfWeek = (int)day.DayOfWeek,
                    DateShort = day.ToString("dd"),
                    DateFull = dateCalculationHelper.DateTimeToString(day),
                    DateTime = day,
                    DaysOfWeekName = day.ToString("ddd"),//.DayOfWeek.ToString(),
                    IsSelected = false,//DateTime.Now.DayOfWeek == day.DayOfWeek,
                    IsActiveDay = false//((int)day.DayOfWeek) >= ((int)DateTime.Now.DayOfWeek)
                });
            }
            this.DaysOfWeek.Clear();
            this.DaysOfWeek = daysOfWeek;
            return daysOfWeek;
        }
    }
}
