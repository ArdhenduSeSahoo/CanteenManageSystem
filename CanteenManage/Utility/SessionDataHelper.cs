using CanteenManage.Models;

namespace CanteenManage.Utility
{
    public class SessionDataHelper
    {
        public static int? getSessionUserId(ISession session)
        {
            return session.GetString(SessionConstants.UserId) == null ? null : int.Parse(session.GetString(SessionConstants.UserId));
        }
        public static SessionDataModel GetSessionDataModel(ISession session)
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
                DateCalculationService.DateTimeFromString(userSelectedDatetime_string)
                ;
            sessionDataModel.UserSelectedDateOrNow = string.IsNullOrEmpty(userSelectedDatetime_string) ? DateTime.Now :
    DateCalculationService.DateTimeFromString(userSelectedDatetime_string);
            return sessionDataModel;
        }
        public static string? getSessionUserName(ISession session)
        {
            return session.GetString(SessionConstants.UserName);
        }
        public static DateTime? getSelectedDateTimeFromSession(ISession session)
        {
            string? userSelectedDatetime_string = session.GetString(SessionConstants.UserSelectedDayFull);
            if (userSelectedDatetime_string == null)
            {
                return null;
            }
            return DateCalculationService.DateTimeFromString(userSelectedDatetime_string);
        }
    }
}
