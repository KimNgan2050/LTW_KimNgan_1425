using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace BTT3_KimNgan.Models
{
    public class Order
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public DateTime OrderDate { get; set; }

        // Bổ sung Data Annotation cấu hình kiểu dữ liệu decimal cho SQL Server
        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalPrice { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập địa chỉ nhận hàng.")]
        public string ShippingAddress { get; set; }

        public string? Notes { get; set; }

        [ForeignKey("UserId")]
        [ValidateNever]
        public ApplicationUser ApplicationUser { get; set; }

        public List<OrderDetail> OrderDetails { get; set; }
    }
}