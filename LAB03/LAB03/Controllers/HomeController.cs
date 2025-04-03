using LAB03.Models;
using LAB03.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


public class HomeController : Controller
{

    private readonly ApplicationDbContext _context;  // Inject DbContext
    private readonly IProductRepository _productRepository;

    // Inject DbContext và IProductRepository vào controller
    public HomeController(ApplicationDbContext context, IProductRepository productRepository)
    {
        _context = context;
        _productRepository = productRepository;
    }

    public async Task<IActionResult> Index(int? categoryId, int page = 1, int pageSize = 6, decimal minPrice = 0, decimal maxPrice = 10000000)
    {
        // Kiểm tra nếu _context bị null
        if (_context == null)
        {
            Console.WriteLine("❌ Lỗi: Database Context chưa được khởi tạo trong HomeController!");
            return View(new List<Product>());
        }

        // Lấy danh mục từ database
        var categories = await _context.Categories.ToListAsync();
        ViewBag.Categories = categories;

        // Lọc sản phẩm theo danh mục
        var products = _context.Products.Include(p => p.Category).AsQueryable();

        // Nếu có categoryId thì lọc theo categoryId
        if (categoryId.HasValue)
        {
            products = products.Where(p => p.CategoryId == categoryId.Value);
            ViewBag.CurrentCategoryId = categoryId.Value;  // Lưu lại categoryId vào ViewBag
        }

        // Lọc theo giá
        products = products.Where(p => p.Price >= minPrice && p.Price <= maxPrice);

        // Tính tổng số sản phẩm và tổng trang
        var totalItems = await products.CountAsync();
        var totalPages = (int)Math.Ceiling((double)totalItems / pageSize);

        // Kiểm tra xem page có hợp lệ hay không
        if (page < 1 || page > totalPages)
        {
            page = 1; // Nếu page không hợp lệ, mặc định trở về trang 1
        }

        // Lấy sản phẩm trong phạm vi trang hiện tại
        var result = await products
            .Skip((page - 1) * pageSize) // Bỏ qua sản phẩm từ các trang trước
            .Take(pageSize) // Lấy số sản phẩm theo pageSize
            .ToListAsync();

        // Trả về các sản phẩm và thông tin phân trang
        ViewBag.CurrentPage = page;
        ViewBag.PageSize = pageSize;
        ViewBag.TotalPages = totalPages;
        ViewBag.MinPrice = minPrice;
        ViewBag.MaxPrice = maxPrice;

        return View(result);
    }




    public IActionResult Search(string query)
    {
        if (string.IsNullOrEmpty(query))
        {
            return RedirectToAction("Index"); // Nếu từ khóa tìm kiếm trống, trả về trang Index
        }

        // Lọc sản phẩm theo từ khóa tìm kiếm
        var products = _context.Products
            .Where(p => p.Name.Contains(query))
            .Include(p => p.Category)
            .ToList();

        // Trả về danh mục và từ khóa tìm kiếm để hiển thị lại trong view
        ViewBag.Categories = _context.Categories.ToList();
        ViewBag.SearchQuery = query;

        // Trả về kết quả tìm kiếm
        return View("Index", products);
    }



    [HttpGet]
    public IActionResult GetAllProducts()
    {
        var products = _context.Products.ToList();
        return Json(products); // Return the data as JSON
    }
    public async Task<IActionResult> Details(int id)
    {
        var product = await _productRepository.GetByIdAsync(id);
        if (product == null) return NotFound();
        return View(product);
    }

}
