using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace LAB03.Models
{
    public class Product
    {
        public int Id { get; set; }
        [Required, StringLength(100)]
        public string Name { get; set; }
        [Range(0.01, 1000000.00)]
        public decimal Price { get; set; }
        public string Description { get; set; }
        public string? ImageUrl { get; set; }
        public List<ProductImage>? Images { get; set; }
        public int CategoryId { get; set; }

        [JsonIgnore]  // Tránh chu kỳ
        public Category? Category { get; set; }
        public bool IsFeatured { get; set; } // Nổi bật
        public bool IsBestSeller { get; set; } // Best Seller
        public bool IsNew { get; set; } // Sản phẩm mới
    }
}
