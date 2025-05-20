using CanteenManage.Utility;

namespace CanteenManage.Services
{
    public class UtilityServices
    {
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

        public void SetDateTimeToSession(ISession session, string? selectedDay, string? selectedDate)
        {
            //var selectedDate = formcollect["selecteddate"].ToString();
            if (string.IsNullOrEmpty(selectedDate))
            {
                session.SetString(SessionConstants.UserSelectedDay, DateTime.Now.Day.ToString());
                session.SetString(SessionConstants.UserSelectedDayOnSamePage, "1");
                session.SetString(SessionConstants.UserSelectedDayFull, DateTime.Now.ToString());
            }
            else
            {
                var selectedDateObj = Convert.ToInt32(selectedDay);
                if (selectedDateObj < DateTime.Now.Day)
                {
                    session.SetString(SessionConstants.UserSelectedDay, DateTime.Now.Day.ToString());
                    session.SetString(SessionConstants.UserSelectedDayOnSamePage, "1");
                    session.SetString(SessionConstants.UserSelectedDayFull, DateTime.Now.ToString());
                }
                else
                {
                    session.SetString(SessionConstants.UserSelectedDay, selectedDay ?? "1");
                    session.SetString(SessionConstants.UserSelectedDayOnSamePage, "1");
                    session.SetString(SessionConstants.UserSelectedDayFull, selectedDate);
                }
            }
        }
    }
}
