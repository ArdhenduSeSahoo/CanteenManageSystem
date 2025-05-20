using CanteenManage.Models;
using System.Globalization;
using CanteenManage.Utility;

namespace CanteenManage.Services
{
    public class UtilityServices
    {

        public string FMT = "O";
        public List<DaysOfWeekModel> GetDaysOfWeek(int? hourBeforeDisable = null)
        {

            var daysOfWeek = new List<DaysOfWeekModel>();

            List<DateTime> dates = new List<DateTime>();
            DateTime myDate = DateTime.ParseExact("2025-05-18 05:40:52,531", "yyyy-MM-dd HH:mm:ss,fff",
                                       System.Globalization.CultureInfo.InvariantCulture);
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
            //var testing = false;
            //if (!testing)
            {
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

            }

            return daysOfWeek;
        }

        public DaysOfWeekModel? getFirstActiveDate(List<DaysOfWeekModel> daysOfWeekModels)
        {

            var firstActiveDay = daysOfWeekModels.Where(d => d.IsActiveDay).OrderBy(d => d.DateShort).FirstOrDefault();
            return firstActiveDay;
        }

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

        public int? getSessionUserId(ISession session)
        {
            return session.GetString(SessionConstants.UserId) == null ? null : int.Parse(session.GetString(SessionConstants.UserId));
        }
        public SessionDataModel GetSessionDataModel(ISession session)
        {
            string? userSelectedDatetime_string = session.GetString(SessionConstants.UserSelectedDayFull);

            SessionDataModel sessionDataModel = new SessionDataModel();
            sessionDataModel.UserId = session.GetString(SessionConstants.UserId) == null ?
                null : int.Parse(session.GetString(SessionConstants.UserId) ?? "0");
            sessionDataModel.UserIdOrZero = session.GetString(SessionConstants.UserId) == null ?
                0 : int.Parse(session.GetString(SessionConstants.UserId) ?? "0");
            sessionDataModel.UserName = session.GetString(SessionConstants.UserName);
            sessionDataModel.UserSelectedDay = session.GetString(SessionConstants.UserSelectedDay);

            sessionDataModel.UserSelectedDate = string.IsNullOrEmpty(userSelectedDatetime_string) ? null :
                DateTimeFromString(userSelectedDatetime_string)
                ;
            sessionDataModel.UserSelectedDateOrNow = string.IsNullOrEmpty(userSelectedDatetime_string) ? DateTime.Now :
    DateTimeFromString(userSelectedDatetime_string);
            return sessionDataModel;
        }
        public static string? getSessionUserName(ISession session)
        {
            return session.GetString(SessionConstants.UserName);
        }
        public DateTime? getSelectedDateTimeFromSession(ISession session)
        {
            string? userSelectedDatetime_string = session.GetString(SessionConstants.UserSelectedDayFull);
            if (userSelectedDatetime_string == null)
            {
                return null;
            }
            return DateTimeFromString(userSelectedDatetime_string);
        }
    }
}
