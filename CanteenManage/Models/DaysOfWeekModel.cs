namespace CanteenManage.Models
{
    public class DaysOfWeekModel
    {
        public int DaysOfWeek { get; set; }
        public string DateShort { get; set; }

        public string DateFull { get; set; }
        public DateTime DateTime { get; set; }
        public string DaysOfWeekName { get; set; }
        public bool IsSelected { get; set; }
        public bool IsActiveDay { get; set; }
    }
}
