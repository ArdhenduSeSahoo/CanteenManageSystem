namespace CanteenManage.Utility
{
    public class CustomDataConstants
    {
        public static int BreakfastTimeHour = 7;
        public static int LunchTimeHour = 10;
        public static int SnacksTimeHour = 15;

        public static int BreakfastTimeHourEnd = 11;
        public static int LunchTimeHourEnd = 15;
        public static int SnacksTimeHourEnd = 19;

        public static string ProjectFolder = Path.Combine(Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments), "CMS_Files");
        public static string CustomViewPath = "CustomView";

        public static string jwtTokencookieName = "iqweurhdsakfhabvdsreteh";
        public static string DefaultSecret = "asdwieurqwuerlkmcnzxmnvafjkasjdflkajs";
        public static string RoleAdmin = "Admin";
        public static string RoleCommitteeMemberOrEmployee = "CommitteeMember,Employee";
        public static string RoleCommitteeMember = "CommitteeMember";
        public static string RoleCanteenEmploy = "CanteenEmploy";
        public static string RoleEmployee = "Employee";
    }
}
