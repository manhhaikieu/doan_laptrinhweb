using LAB03.Repositories;
using Microsoft.AspNetCore.Mvc;

public class ProductController : Controller
{
    private readonly IProductRepository _productRepository;
    private readonly ICategoryRepository _categoryRepository;

    public ProductController(IProductRepository productRepository, ICategoryRepository categoryRepository)
    {
        _productRepository = productRepository;
        _categoryRepository = categoryRepository;
    }


    // Hiển thị danh sách sản phẩm (có tìm kiếm)
    public async Task<IActionResult> Index(string searchTerm)
    {
        var products = await _productRepository.GetAllAsync();

        if (!string.IsNullOrEmpty(searchTerm))
        {
            searchTerm = searchTerm.Trim().ToLower();
            products = products.Where(p => p.Name.ToLower().Contains(searchTerm)).ToList();
        }

        ViewBag.SearchTerm = searchTerm;
        return View(products);
    }



    
}
