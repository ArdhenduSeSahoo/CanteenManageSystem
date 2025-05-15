namespace CanteenManage.Utility
{
    public class ProjectFilePathConstants
    {
        public static string getImagePath()
        {
            System.Environment.SpecialFolder folder = System.Environment.SpecialFolder.MyDocuments;
            string path_document = Environment.GetFolderPath(folder);

            string uploadsFolder = Path.Combine(path_document + "\\CanteenManagementSystem", "FoodImages");
            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }
            return uploadsFolder;
        }
    }
}
