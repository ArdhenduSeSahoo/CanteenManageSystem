namespace CanteenManage.Repo.Models
{
    public class EmployType
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public string Description { get; set; }
        public ICollection<Employe> Employes { get; set; } = new List<Employe>();
    }
}
