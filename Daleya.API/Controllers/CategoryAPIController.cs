using AutoMapper;
using Daleya.API.Models;
using Daleya.API.Models.Dto;
using Daleya.API.Repository.IRepository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Net;

namespace Daleya.API.Controllers
{
    [Route("api/category")]
    [ApiController]
    public class CategoryAPIController : ControllerBase
    {
        private readonly ICategoryRepository _dbCategory;
        private readonly IProductRepository _productRepository;
        private readonly ResponseDto _response;
        private IMapper _mapper;
        public CategoryAPIController(ICategoryRepository db, IMapper mapper, IProductRepository productRepository)
        {
            _dbCategory = db;
            _mapper = mapper;
            _response = new();
            _productRepository = productRepository;
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
                IEnumerable<Category> list = await _dbCategory.GetAllAsync();

                if (list is null || list.Count() == 0)
                {
                    return _response.NotFound("Category table is empty!");
                }
                _response.StatusCode = HttpStatusCode.OK;
                _response.Result = _mapper.Map<IEnumerable<CategoryDto>>(list);
            }
            catch (Exception ex)
            {
                _response.InternalServerError(ex.Message);
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

                var obj = await _dbCategory.GetAsync(v => v.CategoryId == id);

                if (obj == null)
                {
                    return _response.NotFound("Category with id: " + id + " not found");
                }

                _response.Result = _mapper.Map<CategoryDto>(obj);
            }
            catch (Exception ex)
            {
                _response.InternalServerError(ex.Message);
            }
            return _response;
        }

        [HttpPost("{CategoryName}")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ResponseDto> Post(string CategoryName)
        {
            try
            {
                if (await _dbCategory.GetAsync(u => u.Name.ToLower() == CategoryName.ToLower()) != null)
                {
                    return _response.BadRequest("Category is Already Exists!");
                }
                if (CategoryName.IsNullOrEmpty())
                {
                    return _response.NotFound("You must enter category name");
                }

                Category obj = new() { Name = CategoryName };
                
                await _dbCategory.CreateAsync(obj);
                
                _response.Result = _mapper.Map<CategoryDto>(obj);
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
                var obj = await _dbCategory.GetAsync(u => u.CategoryId == id);
                if (obj == null)
                {
                    return _response.NotFound("Category is not Exists!");
                }

                var product = await _productRepository.GetAsync(p => p.CategoryId == id);
                if (product != null)
                {
                    return _response.BadRequest("Cannot delete the category because it has associated products.");
                }

                await _dbCategory.RemoveAsync(obj);
                _response.Result = "Category has been deleted successfully.";
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
