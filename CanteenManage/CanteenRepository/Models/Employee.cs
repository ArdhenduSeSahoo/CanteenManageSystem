using System.ComponentModel.DataAnnotations;

namespace CanteenManage.CanteenRepository.Models
{
    public class Employee
    {
        public int Id { get; set; }
        [StringLength(5000)]
        public required string Name { get; set; }
        public required string EmployID { get; set; }

        [StringLength(1000)]
        public required string Password { get; set; }
        [StringLength(1000)]
        public string? Email { get; set; }
        [StringLength(10)]
        [DataType(DataType.PhoneNumber)]
        public string? PhoneNumber { get; set; }
        public int EmployTypeId { get; set; }
        public EmployeeType EmployType { get; set; }
        public bool? IsActive { get; set; } = true;
        public bool? IsLogin { get; set; } = false;
    }
}
