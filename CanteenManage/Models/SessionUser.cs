namespace CanteenManage.Models
{
    public class SessionUser
    {
        public string UserEmpID { get; set; }
        public string Portal_Token { get; set; }
        public bool isLogin { get; set; }
        public int UserType { get; set; }
    }
}
