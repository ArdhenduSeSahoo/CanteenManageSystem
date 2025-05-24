namespace CanteenManage.Utility
{
    public class ProjectFilePathConstants
    {
        public static string getImagePath()
        {

            string ProjectFolder = CustomDataConstants.ProjectFolder;

            string uploadsFolder = Path.Combine(ProjectFolder, "FoodImages");
            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }
            return uploadsFolder;
        }
    }
}
