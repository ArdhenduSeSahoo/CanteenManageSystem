namespace CanteenManage.Repo.Models
{
    public class Employe
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public required string EmployID { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public int EmployTypeId { get; set; }
        public EmployType EmployType { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
