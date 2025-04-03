using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LAB03.Models;

namespace LAB03.Controllers
{
    public class OrderManagementController : Controller
    {
        private readonly ApplicationDbContext _context;

        public OrderManagementController(ApplicationDbContext context)
        {
            _context = context;
        }

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



        // Hiển thị chi tiết đơn hàng
        public IActionResult Details(int orderId)
        {
            var order = _context.Orders
                .Include(o => o.OrderDetails) // Bao gồm chi tiết đơn hàng
                .ThenInclude(od => od.Product) // Bao gồm thông tin sản phẩm trong OrderDetail
                .FirstOrDefault(o => o.Id == orderId);

            if (order == null)
            {
                return NotFound();
            }

            // Tính tổng tiền của tất cả sản phẩm trong đơn hàng
            decimal totalAmount = order.OrderDetails.Sum(od => od.Price * od.Quantity);

            ViewBag.TotalAmount = totalAmount; // Truyền tổng tiền vào View

            return View(order);
        }



        //public IActionResult Edit(int orderId)
        //{
        //    var order = _context.Orders
        //        .Include(o => o.OrderDetails)
        //        .FirstOrDefault(o => o.Id == orderId);

        //    if (order == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(order);
        //}

        //[HttpPost]
        //public IActionResult UpdateOrder(Order updatedOrder)
        //{
        //    var order = _context.Orders.FirstOrDefault(o => o.Id == updatedOrder.Id);
        //    if (order != null)
        //    {
        //        order.CustomerName = updatedOrder.CustomerName;
        //        order.ShippingAddress = updatedOrder.ShippingAddress;
        //        // Cập nhật các thông tin khác nếu cần
        //        _context.SaveChanges();
        //    }

        //    return RedirectToAction("Details", new { orderId = updatedOrder.Id });
        //}

        public IActionResult Delete(int orderId)
        {
            var order = _context.Orders.Find(orderId);
            if (order != null)  
            {
                _context.Orders.Remove(order);
                _context.SaveChanges();
            }

            return RedirectToAction("Index");
        }

    }
}
