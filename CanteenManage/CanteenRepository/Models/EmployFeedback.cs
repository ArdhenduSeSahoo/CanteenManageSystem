using System.ComponentModel.DataAnnotations;

namespace CanteenManage.CanteenRepository.Models
{
    public class EmployFeedback
    {
        public int Id { get; set; }
        [Required]
        public string Message { get; set; }

        [Required]
        public string Name { get; set; }
        [Required, EmailAddress]
        public string Email { get; set; }

        public DateTime SubmittedAt { get; set; } = DateTime.Now;
        public int EmployeeId { get; set; }
        public Employee Employee { get; set; }
    }
}
