using Daleya.WEB.Models.Dto;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Daleya.WEB.Models.ViewModel
{
    public class ProductViewModel
    {
        public ProductDto? Product { get; set; }
        [ValidateNever]
        public IEnumerable<SelectListItem>? CategoryList{ get; set; }
    }
}
