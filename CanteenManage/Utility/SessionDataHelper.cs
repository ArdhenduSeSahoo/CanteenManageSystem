namespace CanteenManage.Utility
{
    public class SessionDataHelper
    {
        public static int? getSessionUserId(ISession session)
        {
            return session.GetString(SessionConstants.UserId)==null?null: int.Parse(session.GetString(SessionConstants.UserId));
        }
        public static string? getSessionUserName(ISession session)
        {
            return session.GetString(SessionConstants.UserName);
        }
        public static DateTime? getDateTimeFromSession(ISession session)
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
