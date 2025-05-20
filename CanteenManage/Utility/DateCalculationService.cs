using System.Globalization;
using CanteenManage.Models;
using Mono.TextTemplating;

namespace CanteenManage.Utility
{
    public class DateCalculationService
    {

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
