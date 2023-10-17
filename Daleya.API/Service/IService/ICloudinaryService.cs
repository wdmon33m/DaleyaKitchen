using CloudinaryDotNet.Actions;
using Daleya.API.Models;

namespace Daleya.API.Service.IService
{
    public interface ICloudinaryService
    {
        public ImageUploadResult UploadProductImage(Product obj);
    }
}
