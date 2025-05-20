namespace CanteenManage.Models
{
    public class SessionDataModel
    {
        public int? UserId { get; set; }
        public int UserIdOrZero { get; set; }
        public DateTime? UserSelectedDate { get; set; }
        public string? UserSelectedDay { get; set; }

        public DateTime UserSelectedDateOrNow { get; set; }
        public string? UserName { get; set; }


    }
}
