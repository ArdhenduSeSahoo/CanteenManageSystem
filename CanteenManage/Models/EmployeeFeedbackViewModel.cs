using CanteenManage.CanteenRepository.Models;
using System.ComponentModel.DataAnnotations;

namespace CanteenManage.Models
{
    public class EmployeeFeedbackViewModel : LayoutViewDataModel
    {
        public List<EmployeeFeedbacks> EmployeeFeedbacks { get; set; }
        public EmployeeFeedbacks EmployeeFeedback { get; set; } = new EmployeeFeedbacks();
        public bool FeedbackSubmitted { get; set; } = false;
        public string EmployeeNameId { get; set; }
    }
    public class EmployeeFeedbacks
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
