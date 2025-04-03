using LAB03.Models;
using LAB03.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using LAB03.Extensions;

namespace LAB03.Controllers
{
    public class ShoppingCartController : Controller
    {
        private readonly IProductRepository _productRepository;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public ShoppingCartController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IProductRepository productRepository)
        {
            _productRepository = productRepository;
            _context = context;
            _userManager = userManager;
        }
        

        // Phương thức thêm sản phẩm vào giỏ hàng
        public async Task<IActionResult> AddToCart(int productId, int quantity)
        {
            // Lấy thông tin sản phẩm từ cơ sở dữ liệu
            var product = await GetProductFromDatabase(productId);

            // Tạo đối tượng CartItem với thông tin sản phẩm
            var cartItem = new CartItem
            {
                ProductId = productId,
                Name = product.Name,
                Price = product.Price,
                Quantity = quantity
            };

            // Lấy giỏ hàng từ session, nếu không có thì tạo mới
            var cart = HttpContext.Session.GetObjectFromJson<ShoppingCart>("Cart") ?? new ShoppingCart();
            cart.AddItem(cartItem);  // Thêm sản phẩm vào giỏ hàng

            // Lưu giỏ hàng vào session
            HttpContext.Session.SetObjectAsJson("Cart", cart);

            // Điều hướng về trang Index
            return RedirectToAction("Index");
        }

        // Phương thức hiển thị giỏ hàng
        public IActionResult Index()
        {
            var cart = HttpContext.Session.GetObjectFromJson<ShoppingCart>("Cart") ?? new ShoppingCart();
            ViewBag.CartCount = cart.Items.Sum(i => i.Quantity); // Tổng số sản phẩm trong giỏ hàng
            return View(cart);
        }

        // Phương thức xóa sản phẩm khỏi giỏ hàng
        public IActionResult RemoveFromCart(int productId)
        {
            // Lấy giỏ hàng từ session
            var cart = HttpContext.Session.GetObjectFromJson<ShoppingCart>("Cart") ?? new ShoppingCart();

            if (cart != null)
            {
                cart.RemoveItem(productId);  // Xóa sản phẩm khỏi giỏ hàng
                HttpContext.Session.SetObjectAsJson("Cart", cart);  // Cập nhật lại giỏ hàng trong session
            }

            // Điều hướng về trang Index
            return RedirectToAction("Index");
        }

        // Phương thức lấy thông tin sản phẩm từ cơ sở dữ liệu
        private async Task<Product> GetProductFromDatabase(int productId)
        {
            // Truy vấn dữ liệu từ _productRepository để lấy thông tin sản phẩm
            var product = await _productRepository.GetByIdAsync(productId);
            return product;
        }
        public IActionResult Checkout()
        {
            return View(new Order());
        }

        [HttpPost]
        public async Task<IActionResult> Checkout(Order order)
        {
            var cart = HttpContext.Session.GetObjectFromJson<ShoppingCart>("Cart");

            if (cart == null || !cart.Items.Any())
            {
                // Nếu giỏ hàng trống hoặc không có sản phẩm, điều hướng về trang giỏ hàng
                return RedirectToAction("Index");
            }

            // Lấy thông tin người dùng hiện tại
            var user = await _userManager.GetUserAsync(User);

            order.UserId = user.Id;
            order.OrderDate = DateTime.UtcNow.ToLocalTime();

            order.TotalPrice = cart.Items.Sum(i => i.Price * i.Quantity);

            // Thêm chi tiết đơn hàng vào đơn hàng
            order.OrderDetails = cart.Items.Select(i => new OrderDetail
            {
                ProductId = i.ProductId,
                Quantity = i.Quantity,
                Price = i.Price
            }).ToList();

            // Thêm đơn hàng vào cơ sở dữ liệu
            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            // Xóa giỏ hàng sau khi hoàn tất đơn hàng
            HttpContext.Session.Remove("Cart");

            // Điều hướng đến trang xác nhận đơn hàng
            return View("OrderCompleted", order.Id);
        }

        [HttpPost]
        public async Task<IActionResult> AddToCartAjax(int productId, int quantity)
        {
            var product = await GetProductFromDatabase(productId);
            if (product == null)
            {
                return Json(new { success = false, message = "Sản phẩm không tồn tại." });
            }

            var cart = HttpContext.Session.GetObjectFromJson<ShoppingCart>("Cart") ?? new ShoppingCart();

            var existingItem = cart.Items.FirstOrDefault(item => item.ProductId == productId);
            if (existingItem != null)
            {
                existingItem.Quantity += quantity;
            }
            else
            {
                var cartItem = new CartItem
                {
                    ProductId = productId,
                    Name = product.Name,
                    Price = product.Price,
                    Quantity = quantity
                };
                cart.AddItem(cartItem);
            }

            // Lưu giỏ hàng vào session
            HttpContext.Session.SetObjectAsJson("Cart", cart);

            // Cập nhật lại tổng số lượng trong giỏ hàng
            var cartCount = cart.Items.Sum(i => i.Quantity);

            return Json(new { success = true, message = "Đã thêm vào giỏ hàng!", cartCount = cartCount });
        }


        //thêm số lượng



        [HttpPost]
        public IActionResult UpdateQuantity(int productId, int quantity)
        {
            var cart = HttpContext.Session.GetObjectFromJson<ShoppingCart>("Cart") ?? new ShoppingCart();

            var item = cart.Items.FirstOrDefault(i => i.ProductId == productId);
            if (item != null && quantity > 0)
            {
                // Cập nhật số lượng
                item.Quantity = quantity;
                HttpContext.Session.SetObjectAsJson("Cart", cart); // Lưu lại giỏ hàng vào session

                // Tính toán tổng tiền của tất cả sản phẩm trong giỏ
                var totalAmount = cart.Items.Sum(i => i.Price * i.Quantity);
                var itemTotal = item.Price * item.Quantity;
                var cartCount = cart.Items.Sum(i => i.Quantity);

                return Json(new { success = true, total = totalAmount, itemTotal = itemTotal, cartCount = cartCount });
            }

            return Json(new { success = false });
        }



        // Phương thức trả về số lượng sản phẩm trong giỏ hàng
        [HttpGet]
        public IActionResult GetCartCount()
        {
            var cart = HttpContext.Session.GetObjectFromJson<ShoppingCart>("Cart") ?? new ShoppingCart();
            var cartCount = cart.Items.Sum(i => i.Quantity);  // Tính tổng số lượng sản phẩm trong giỏ hàng
            return Json(new { cartCount });
        }
       
    }

}
