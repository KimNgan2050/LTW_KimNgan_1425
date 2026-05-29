namespace BTT3_KimNgan.Models
{
    public class Review
    {
        public int Id { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public int Rating { get; set; } // Số sao: 1, 2, 3, 4, 5
        public string Comment { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        // Khóa ngoại liên kết ngược lại với bảng Product
        public int ProductId { get; set; }
        public Product? Product { get; set; }
    }
}
