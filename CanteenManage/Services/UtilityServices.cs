using CanteenManage.Models;
using System.Globalization;
using CanteenManage.Utility;

namespace CanteenManage.Services
{
    public class UtilityServices
    {
        private readonly OrderDataCaching orderDataCaching;

        public string FMT = "O";
        public UtilityServices(OrderDataCaching orderDataCaching)
        {
            this.orderDataCaching = orderDataCaching;
        }
        public List<DaysOfWeekModel> GetDaysOfWeek(int hourBeforeDisable)
        {

            var daysOfWeek = orderDataCaching.GetDayOfWeeks();
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

            var blockedDates = new List<DateTime>
              {
               new DateTime(2025, 6, 27),  // Rath yatra
               new DateTime(2025, 8, 15),  // Independence day
               new DateTime(2025, 9, 30),  // Maha Astami
               new DateTime(2025, 10, 2),  // Dushhera
               new DateTime(2025, 10, 21), //Diwali
              };
            //var testing = false;
            //if (!testing)
            {
                foreach (var item in daysOfWeek)
                {
                    item.IsSelected = false;
                    //var hourss = int.Parse(DateTime.Now.ToString("HH"));
                    if (blockedDates.Any(d => d.Date == item.DateTime.Date))
                    {
                        item.IsActiveDay = false;
                        continue;
                    }
                    if (item.DateTime.Date < DateTime.Now.Date) //(((int)item.DateTime.DayOfWeek) < ((int)DateTime.Now.DayOfWeek))
                    {
                        item.IsActiveDay = false;
                    }
                    else if (item.DateTime.Date == DateTime.Now.Date) //(((int)item.DateTime.DayOfWeek) == ((int)DateTime.Now.DayOfWeek))
                    {
                        if (int.Parse(DateTime.Now.ToString("HH")) < hourBeforeDisable)
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

            var firstActiveDay = daysOfWeekModels.Where(d => d.IsActiveDay).OrderBy(d => d.DateTime.Date).FirstOrDefault();
            return firstActiveDay;
        }

        public string DateTimeToString(DateTime dateTime)
        {
            return dateTime.ToString(FMT);
        }
        //public DateTime DateTimeFromString(string dateTime)
        //{
        //    return DateTime.ParseExact(dateTime, FMT, CultureInfo.InvariantCulture);
        //}

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

        public void SetDateTimeToSession(int DisableHour, ISession session, string? selectedDay, string? selectedDate)
        {
            //var selectedDate = formcollect["selecteddate"].ToString();
            if (string.IsNullOrEmpty(selectedDate))
            {
                //SetSessionData(session, DateTime.Now.Day.ToString(), DateTimeToString(DateTime.Now));
                throw new Exception("Selected date cannot be null or empty.");
            }
            else
            {
                var selectedDateObj = new DateCalculationHelper().DateTimeFromString(selectedDate);// Convert.ToInt32(selectedDay);
                if (selectedDateObj == null)
                {
                    var firstactivedate = getFirstActiveDate(GetDaysOfWeek(DisableHour));
                    if (firstactivedate != null)
                    {
                        SetSessionData(session, firstactivedate.DateShort, firstactivedate.DateFull);
                    }
                    return;
                }
                else if (selectedDateObj.Date < DateTime.Now.Date)
                {
                    var firstactivedate = getFirstActiveDate(GetDaysOfWeek(DisableHour));
                    if (firstactivedate != null)
                    {
                        SetSessionData(session, firstactivedate.DateShort, firstactivedate.DateFull);
                    }
                    return;
                }
                else if (selectedDateObj.Date == DateTime.Now.Date)
                {
                    if (int.Parse(DateTime.Now.ToString("HH")) < DisableHour)
                    {
                        SetSessionData(session, selectedDay, selectedDate);
                        return;

                    }
                    else
                    {
                        var firstactivedate = getFirstActiveDate(GetDaysOfWeek(DisableHour));
                        if (firstactivedate != null)
                        {
                            SetSessionData(session, firstactivedate.DateShort, firstactivedate.DateFull);
                        }
                        return;
                    }
                }
                else
                {
                    SetSessionData(session, selectedDay, selectedDate);
                }
            }
        }
        private void SetSessionData(ISession session, string? selectedDay, string SelectedDate)
        {
            session.SetString(SessionConstants.UserSelectedDay, selectedDay ?? "1");
            session.SetString(SessionConstants.UserSelectedDayOnSamePage, "1");
            session.SetString(SessionConstants.UserSelectedDayFull, SelectedDate);
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
            sessionDataModel.UserEmpIdOrNull = session.GetString(SessionConstants.UserEmpId);
            sessionDataModel.UserSelectedDate = string.IsNullOrEmpty(userSelectedDatetime_string) ? null :
                new DateCalculationHelper().DateTimeFromString(userSelectedDatetime_string)
                ;
            sessionDataModel.UserSelectedDateOrNow = string.IsNullOrEmpty(userSelectedDatetime_string) ? DateTime.Now :
    new DateCalculationHelper().DateTimeFromString(userSelectedDatetime_string);
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
            return new DateCalculationHelper().DateTimeFromString(userSelectedDatetime_string);
        }
    }
}
