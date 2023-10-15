using AutoMapper;

namespace Restaurant.Services.CouponAPI
{
    public class MappingConfig
    {
        public static MapperConfiguration RegisterMaps()
        {
            var mappingConfig = new MapperConfiguration(config =>
            {
                //config.CreateMap<CouponDto, Coupon>();
            });
            return mappingConfig;
        }
    }
}
