namespace CanteenManage.CanteenRepository.Models
{
    public class EmployType
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public string Description { get; set; }
        public ICollection<Employee> Employes { get; set; } = new List<Employee>();
    }
}
