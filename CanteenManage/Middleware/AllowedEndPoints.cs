namespace CanteenManage.Middleware
{
    public class AllowedEndPoints
    {
        public static List<string> AllowedURL_List = new List<string>() {
        "/login/".ToLower(),
            "/login/index".ToLower(),
        "/error".ToLower(),
        "/error/".ToLower(),
        "/Login/LoginUser".ToLower(),
        "/Login/PortalLogin".ToLower(),
        "/Login/PortalLogOut".ToLower()
        };
    }
}
