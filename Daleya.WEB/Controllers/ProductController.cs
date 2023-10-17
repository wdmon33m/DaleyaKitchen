using Daleya.WEB.Models;
using Daleya.WEB.Models.Dto;
using Daleya.WEB.Models.ViewModel;
using Daleya.WEB.Service.IService;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;

namespace Daleya.WEB.Controllers
{
    public class ProductController : Controller
    {
        private readonly IProductService _productService;
        private readonly ICategoryService _categoryService;
        public ProductController(IProductService productService, ICategoryService categoryService)
        {
            _productService = productService;
            _categoryService = categoryService;
        }
        public async Task<IActionResult> Index()
        {
            List<ProductDto>? list = new();
            ResponseDto? response = await _productService.GetAllAsync();

            if (response != null && response.IsSuccess)
            {
                list = JsonConvert.DeserializeObject<List<ProductDto>>(Convert.ToString(response.Result));
            }
            else
            {
                TempData["error"] = response?.ErrorMessage;
            }

            return View(list);
        }
        
        public async Task<IActionResult> Create()
        {
            ProductViewModel productVM = new()
            {
                CategoryList = _categoryService.GetAllDeserialize().Select(x => new SelectListItem
                {
                    Text = x.Name,
                    Value = x.CategoryId.ToString()
                })
            };
            return View(productVM);
        }
        [HttpPost]
        public async Task<IActionResult> Create(ProductViewModel obj)
        {
            if (ModelState.IsValid)
            {
                ResponseDto? response = await _productService.CreateAsync(obj.Product);
                if (response != null && response.IsSuccess )
                {
                    TempData["success"] = "Product created successfully";
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    TempData["error"] = response?.ErrorMessage;
                } 
            }

            ProductViewModel productVM = new()
            {
                CategoryList = _categoryService.GetAllDeserialize().Select(x => new SelectListItem
                {
                    Text = x.Name,
                    Value = x.CategoryId.ToString()
                }),
                Product = obj.Product
            };
            return View(productVM);
        }

        public async Task<IActionResult> Update(int productId)
        {
            ResponseDto? response = await _productService.GetAsync(productId);

            if (response != null && response.IsSuccess)
            {
                ProductDto? productDto = JsonConvert.DeserializeObject<ProductDto>(Convert.ToString(response.Result));

                ProductViewModel productVM = new()
                {
                    Product = productDto,
                    CategoryList = _categoryService.GetAllDeserialize().Select(x => new SelectListItem
                    {
                        Text = x.Name,
                        Value = x.CategoryId.ToString()
                    })
                };
                return View(productVM);
            }
            else
            {
                TempData["error"] = response?.ErrorMessage;
            }
            return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> Update(ProductViewModel obj)
        {
            if (ModelState.IsValid)
            {
                ResponseDto? response = await _productService.UpdateAsync(obj.Product);
                if (response != null && response.IsSuccess)
                {
                    TempData["success"] = "Product updated successfully";
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    TempData["error"] = response?.ErrorMessage;
                }
            }
            return View(obj);
        }

        public async Task<IActionResult> Delete(int productId)
        {
            ResponseDto? response = await _productService.GetAsync(productId);

            if (response != null && response.IsSuccess)
            {
                ProductDto? productDto = JsonConvert.DeserializeObject<ProductDto>(Convert.ToString(response.Result));

                ProductViewModel productVM = new()
                {
                    Product = productDto,
                    CategoryList = _categoryService.GetAllDeserialize().Select(x => new SelectListItem
                    {
                        Text = x.Name,
                        Value = x.CategoryId.ToString()
                    })
                };
                return View(productVM);
            }
            else
            {
                TempData["error"] = response?.ErrorMessage;
            }
            return NotFound();
        }
        [HttpPost]
        public async Task<IActionResult> Delete(ProductViewModel obj)
        {
            ResponseDto? response = await _productService.DeleteAsync(obj.Product.ProductId);

            if (response != null && response.IsSuccess)
            {
                TempData["success"] = "Product deleted successfully";
                return RedirectToAction(nameof(Index));
            }
            else
            {
                TempData["error"] = response?.ErrorMessage;
                return RedirectToAction(nameof(Index));
            }
        }
    }
}
