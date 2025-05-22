namespace CanteenManage.Models
{
    public abstract class LayoutViewDataModel
    {
        public int? UserId { get; set; } = 0;
        public string? UserName { get; set; } = "";
        public string? UserEmpId { get; set; } = "";
        public string? UserEmail { get; set; } = "";
        public int? CartItemCount { get; set; } = 0;
    }
    public class LayoutViewDataModelDefault : LayoutViewDataModel
    {

    }
}
