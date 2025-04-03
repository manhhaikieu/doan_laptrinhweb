using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LAB03.Areas.Admin.Controllers
{
    public class OrderManagementControlller : Controller
    {
        private readonly ApplicationDbContext _context;

       

        // Hiển thị danh sách các đơn hàng đã giao
        public IActionResult Index()
        {
            var orders = _context.Orders
                .Include(o => o.OrderDetails)  // Bao gồm chi tiết đơn hàng
                .Where(o => o.OrderDate != null)  // Lọc các đơn hàng đã có ngày giao
                .OrderByDescending(o => o.OrderDate)  // Sắp xếp theo ngày đặt
                .ToList();

            return View(orders);
        }

    }
}
