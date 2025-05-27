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

            List<DateTime> TwoWeekdates = new List<DateTime>();


            DateTime today = DateTime.Today;


            DayOfWeek firstDayOfWeek = CultureInfo.CurrentCulture.DateTimeFormat.FirstDayOfWeek;
            //if(firstDayOfWeek==DayOfWeek.Sunday)
            //{
            //    firstDayOfWeek = DayOfWeek.Monday; // Adjust to Monday if Sunday is the first day of the week
            //}
            firstDayOfWeek = DayOfWeek.Monday;

            int diff = (7 + (today.DayOfWeek - firstDayOfWeek)) % 7;
            DateTime startOfCurrentWeek = today.AddDays(-1 * diff).Date;


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


            foreach (var day in TwoWeekdates)
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

            //var firstActiveDay = daysOfWeek.Where(d => d.IsActiveDay).OrderBy(d => d.DateShort).FirstOrDefault();
            //if (firstActiveDay != null)
            //{
            //    firstActiveDay.IsSelected = true;
            //}

            //var testing = false;
            //if (!testing)
            {
                foreach (var item in daysOfWeek)
                {
                    item.IsSelected = false;
                    var hourss = int.Parse(DateTime.Now.ToString("HH"));
                    if (item.DateTime.Date < DateTime.Now.Date) //(((int)item.DateTime.DayOfWeek) < ((int)DateTime.Now.DayOfWeek))
                    {
                        item.IsActiveDay = false;
                    }
                    else if (item.DateTime.Date == DateTime.Now.Date) //(((int)item.DateTime.DayOfWeek) == ((int)DateTime.Now.DayOfWeek))
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
                    else if (item.DateTime.Date > DateTime.Now.Date) //(((int)item.DateTime.DayOfWeek) > ((int)DateTime.Now.DayOfWeek))
                    {
                        item.IsActiveDay = true;
                    }
                }
            }

            return daysOfWeek;
        }

        public DaysOfWeekModel? getFirstActiveDate(List<DaysOfWeekModel> daysOfWeekModels)
        {

            var firstActiveDay = daysOfWeekModels.Where(d => d.IsActiveDay).OrderBy(d => d.DateTime).FirstOrDefault();
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
                var selectedDateObj = DateTimeFromString(selectedDate);// Convert.ToInt32(selectedDay);
                if (selectedDateObj < DateTime.Now)
                {
                    session.SetString(SessionConstants.UserSelectedDay, DateTime.Now.Day.ToString());
                    session.SetString(SessionConstants.UserSelectedDayOnSamePage, "1");
                    session.SetString(SessionConstants.UserSelectedDayFull, DateTimeToString(DateTime.Now));
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
