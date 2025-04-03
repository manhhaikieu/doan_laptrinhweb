using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations.Schema;

namespace LAB03.Models
{
    public class Order
    {
        public int Id { get; set; }

        // Thông tin người dùng
        public string UserId { get; set; }
        public string CustomerName { get; set; } // Thêm tên khách hàng
        public string PhoneNumber { get; set; }  // Thêm số điện thoại
        public string Email { get; set; }        // Thêm email của khách hàng

        // Thông tin đơn hàng
        public DateTime OrderDate { get; set; }
        public decimal TotalPrice { get; set; }

        // Địa chỉ giao hàng
        public string ShippingAddress { get; set; }

        // Các trường mới thêm vào
        public string City { get; set; }    // Tỉnh thành
        public string District { get; set; } // Quận huyện

        // Các yêu cầu đặc biệt khác
        public string Notes { get; set; }
        public string PaymentMethod { get; set; }

        [ForeignKey("UserId")]
        [ValidateNever]
        public ApplicationUser User { get; set; }

        // Liên kết với các chi tiết đơn hàng
        public List<OrderDetail> OrderDetails { get; set; }
    }
}
