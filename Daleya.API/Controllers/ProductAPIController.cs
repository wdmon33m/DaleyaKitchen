using AutoMapper;
using Daleya.API.Models;
using Daleya.API.Models.Dto;
using Daleya.API.Models.Dto.Create;
using Daleya.API.Repository.IRepository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Net;

namespace Daleya.API.Controllers
{
    [Route("api/product")]
    [ApiController]
    public class ProductAPIController : ControllerBase
    {
        private readonly IProductRepository _dbProduct;
        private readonly ICategoryRepository _dbCategory;
        private readonly ResponseDto _response;
        private IMapper _mapper;
        public ProductAPIController(IProductRepository db, IMapper mapper, ICategoryRepository dbCategory)
        {
            _dbProduct = db;
            _mapper = mapper;
            _response = new();
            _dbCategory = dbCategory;
        }

        [HttpGet]
        [MapToApiVersion("1.0")]
        [ResponseCache(CacheProfileName = "Default30")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ResponseDto> Get()
        {
            try
            {
                IEnumerable<Product> list = await _dbProduct.GetAllAsync();

                if (list is null || list.Count() == 0)
                {
                    return _response.NotFound("Product table is empty!");
                }
                _response.StatusCode = HttpStatusCode.OK;
                _response.Result = _mapper.Map<IEnumerable<ProductDto>>(list);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessage = ex.Message;
            }
            return _response;
        }
        [HttpGet("{id:int}")]
        [MapToApiVersion("1.0")]
        [ResponseCache(CacheProfileName = "Default30")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ResponseDto> Get(int id)
        {
            try
            {
                if (id == 0)
                {
                    return _response.BadRequest("Id 0 is not correct!");
                }

                var obj = await _dbProduct.GetAsync(v => v.ProductId == id);

                if (obj == null)
                {
                    return _response.NotFound("Product with id: " + id + " not found");
                }

                _response.Result = _mapper.Map<ProductDto>(obj);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessage = ex.Message;
            }
            return _response;
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ResponseDto> Post([FromBody] CreateProductDto createDto)
        {
            try
            {
                if (await _dbCategory.GetAsync(u => u.CategoryId == createDto.CategoryId) == null)
                {
                    return _response.BadRequest("Category ID is not correct!");
                }
                if (await _dbProduct.GetAsync(u => u.Name.ToLower() == createDto.Name.ToLower()) != null)
                {
                    return _response.BadRequest("Product is Already Exists!");
                }
                if (createDto.Name.IsNullOrEmpty())
                {
                    return _response.NotFound("You must enter product name");
                }

                Product obj = _mapper.Map<Product>(createDto);

                if (obj.ImageUrl.IsNullOrEmpty())
                {
                    obj.ImageUrl = "https://placehold.co/600x400/";
                }
                await _dbProduct.CreateAsync(obj);

                _response.Result = _mapper.Map<ProductDto>(obj);
                _response.StatusCode = HttpStatusCode.Created;
            }
            catch (Exception ex)
            {
                _response.InternalServerError(ex.Message);
            }
            return _response;
        }


        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ResponseDto> Delete(int id)
        {
            try
            {
                var obj = await _dbProduct.GetAsync(u => u.ProductId == id);
                if (obj == null)
                {
                    return _response.NotFound("Product is not Exists!");
                }

                await _dbProduct.RemoveAsync(obj);
                _response.Result = "Product has been deleted successfully.";
                _response.StatusCode = HttpStatusCode.OK;
            }
            catch (Exception ex)
            {
                _response.InternalServerError(ex.Message);
            }
            return _response;
        }
    }
}
