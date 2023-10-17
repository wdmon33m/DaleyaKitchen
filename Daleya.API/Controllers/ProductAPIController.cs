using AutoMapper;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Daleya.API.Models;
using Daleya.API.Models.Dto;
using Daleya.API.Repository.IRepository;
using Daleya.API.Service.IService;
using Daleya.API.Utility;
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
        private readonly ICloudinaryService _cloudinaryService;
        private readonly ResponseDto _response;
        private IMapper _mapper;
        public ProductAPIController(IProductRepository db, IMapper mapper, ICategoryRepository dbCategory, ICloudinaryService cloudinaryService)
        {
            _dbProduct = db;
            _mapper = mapper;
            _response = new();
            _dbCategory = dbCategory;
            _cloudinaryService = cloudinaryService;
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
                IEnumerable<Product> list = await _dbProduct.GetAllAsync(includeProperties: "Category");

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
		public async Task<ResponseDto> Post([FromForm] ProductDto createDto)
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
				await _dbProduct.CreateAsync(obj);

                if (createDto.Image != null)
                {
                    string fileName = obj.ProductId + Path.GetExtension(createDto.Image.FileName);
                    string filePath = @"wwwroot\ProductImages\" + fileName;
                    var filePathDirectory = Path.Combine(Directory.GetCurrentDirectory(), filePath);
                    using (var fileStream = new FileStream(filePathDirectory, FileMode.Create))
                    {
                        createDto.Image.CopyTo(fileStream);
                    }
                    var baseUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host.Value}{HttpContext.Request.PathBase.Value}";
                    //obj.ImageUrl = baseUrl + "/ProductImages/" + fileName;
                    obj.ImageLocalPath = filePath;

                    ImageUploadResult uploadResult = _cloudinaryService.UploadProductImage(obj);

                    if (uploadResult.Error == null)
                    {
                        obj.ImageUrl = uploadResult.Url.ToString();
                    }

                }
                else
                {
                    obj.ImageUrl = "https://placehold.co/600x400";
                }

                await _dbProduct.UpdateAsync(obj);
                
                _response.Result = _mapper.Map<ProductDto>(obj);
				_response.StatusCode = HttpStatusCode.Created;
			}
			catch (Exception ex)
			{
				_response.InternalServerError(ex.Message);
			}
			return _response;
		}

        [HttpPut]
		[ProducesResponseType(StatusCodes.Status201Created)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<ResponseDto> Put([FromForm] ProductDto updateDto)
		{
			try
			{
				if (await _dbCategory.GetAsync(u => u.CategoryId == updateDto.CategoryId) == null)
				{
					return _response.BadRequest("Category ID is not correct!");
				}
				if (updateDto.Name.IsNullOrEmpty())
				{
					return _response.NotFound("You must enter product name");
				}

				Product obj = _mapper.Map<Product>(updateDto);
                await _dbProduct.UpdateAsync(obj);

                if (updateDto.Image != null)
                {
                    if (!string.IsNullOrEmpty(obj.ImageLocalPath))
                    {
                        var oldFilePathDirectory = Path.Combine(Directory.GetCurrentDirectory(), obj.ImageLocalPath);
                        FileInfo file = new FileInfo(oldFilePathDirectory);
                        if (file.Exists)
                        {
                            file.Delete();
                        }
                    }

                    string fileName = obj.ProductId + Path.GetExtension(updateDto.Image.FileName);
                    string filePath = @"wwwroot\ProductImages\" + fileName;
                    var filePathDirectory = Path.Combine(Directory.GetCurrentDirectory(), filePath);
                    using (var fileStream = new FileStream(filePathDirectory, FileMode.Create))
                    {
                        updateDto.Image.CopyTo(fileStream);
                    }
                    var baseUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host.Value}{HttpContext.Request.PathBase.Value}";
                    //obj.ImageUrl = baseUrl + "/ProductImages/" + fileName;
                    obj.ImageLocalPath = filePath;

                    ImageUploadResult uploadResult = _cloudinaryService.UploadProductImage(obj);

                    if (uploadResult.Error == null)
                    {
                        obj.ImageUrl = uploadResult.Url.ToString();
                    }
                }

                if(string.IsNullOrEmpty(obj.ImageUrl))
                {
                    obj.ImageUrl = "https://placehold.co/600x400";
                }

                await _dbProduct.UpdateAsync(obj);

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
