namespace CanteenManage.CanteenRepository.Models
{
    public class EmployeeType
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public string Description { get; set; }
        public ICollection<Employee> Employees { get; set; } = new List<Employee>();
    }
}
