namespace CanteenManage.CanteenRepository.Models
{
    public class EmployeeCart
    {
        public int Id { get; set; }
        public int EmployeeId { get; set; }
        public Employee Employee { get; set; }
        public DateTime OrderDate { get; set; }
        public Food Food { get; set; }
        public int FoodId { get; set; }
        public int Quantity { get; set; }
        public decimal TotalPrice { get; set; }
        public int OutDateStatus { get; set; } // 0: not deleted, 1: it will deleted

    }
}
