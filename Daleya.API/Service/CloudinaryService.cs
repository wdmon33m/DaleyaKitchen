using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Daleya.API.Models;
using Daleya.API.Service.IService;
using Daleya.API.Utility;

namespace Daleya.API.Service
{
    public class CloudinaryService : ICloudinaryService
    {
        private readonly Account _account;

        public CloudinaryService()
        {
            _account = new Account(SD.Cloudinary_CloudName, SD.Cloudinary_Apikey, SD.Cloudinary_Secretkey);
        }

        public ImageUploadResult UploadProductImage(Product obj)
        {
            Cloudinary cloudinary = new Cloudinary(_account);
            var uploadParams = new ImageUploadParams()
            {
                File = new FileDescription(obj.ImageLocalPath),
                PublicId = "ProductId_" + obj.ProductId
            };
            var uploadResult = cloudinary.Upload(uploadParams);
            return uploadResult;
        }
    }
}
