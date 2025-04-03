using LAB03.Models;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace LAB03.Controllers
{
    public class CategoryController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CategoryController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Hiển thị danh mục
        public IActionResult Index()
        {
            

            return View();
        }





        // Hiển thị các sản phẩm của một danh mục
        
    }
}
