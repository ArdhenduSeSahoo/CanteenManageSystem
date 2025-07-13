using System.Globalization;
using CanteenManage.Models;
using Mono.TextTemplating;

namespace CanteenManage.Utility
{
    public class DateCalculationHelper
    {

        public string FMT = "O";

        public string DateTimeToString(DateTime dateTime)
        {
            return dateTime.ToString(FMT);
        }
        public DateTime DateTimeFromString(string dateTime)
        {
            return DateTime.ParseExact(dateTime, FMT, CultureInfo.InvariantCulture);
        }

        public TimeSpan GetSpecificTimeSpan(FoodTypeEnum foodTypeEnum)
        {
            TimeSpan ts = new TimeSpan();
            if (foodTypeEnum == FoodTypeEnum.Breakfast)
            {
                ts = new TimeSpan(CustomDataConstants.BreakfastTimeHour, 00, 0);
            }
            else if (foodTypeEnum == FoodTypeEnum.Lunch)
            {
                ts = new TimeSpan(CustomDataConstants.LunchTimeHour, 00, 0);
            }
            else if (foodTypeEnum == FoodTypeEnum.Snacks)
            {
                ts = new TimeSpan(CustomDataConstants.SnacksTimeHour, 00, 0);
            }
            else if (foodTypeEnum == FoodTypeEnum.QuickFood)
            {
                ts = new TimeSpan();
            }
            else
            {
                ts = new TimeSpan();
            }
            return ts;
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
