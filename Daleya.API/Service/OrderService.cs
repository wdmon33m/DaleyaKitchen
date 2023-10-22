using AutoMapper;
using Daleya.API.Models;
using Daleya.API.Models.Dto;
using Daleya.API.Models.Dto.Cart;
using Daleya.API.Models.Dto.Order;
using Daleya.API.Repository.IRepository;
using Daleya.API.Service.IService;
using Daleya.API.Utility;
using Stripe;
using Stripe.Checkout;
using System.Net;
using Coupon = Daleya.API.Models.Coupon;

namespace Daleya.API.Service
{
    public class OrderService : IOrderService
    {
        private readonly IOrderHeaderRepository _orderHeaderRepository;
        private readonly IOrderDetailsRepository _orderDetailsRepository;
        private readonly IProductRepository _productRepository;
        private readonly ICouponRepository _couponRepository;
        private readonly IMapper _mapper;
        private readonly ResponseDto _response;

        public OrderService(IOrderHeaderRepository orderHeaderRepository, 
            IOrderDetailsRepository orderDetailsRepository, 
            IProductRepository productRepository, 
            IMapper mapper, ICouponRepository couponRepository)
        {
            _orderHeaderRepository = orderHeaderRepository;
            _orderDetailsRepository = orderDetailsRepository;
            _productRepository = productRepository;
            _response = new();
            _mapper = mapper;
            _couponRepository = couponRepository;
        }

        public async Task<ResponseDto> CreateOrder(CartDto cartDto)
        {
            try
            {
                OrderHeaderDto orderHeaderDto = _mapper.Map<OrderHeaderDto>(cartDto.CartHeader);
                orderHeaderDto.OrderDate = DateOnly.FromDateTime(DateTime.Now);
                orderHeaderDto.OrderTime = TimeOnly.FromDateTime(DateTime.Now);
                orderHeaderDto.Status = SD.Status_Pending;


                orderHeaderDto.OrderDetails = _mapper.Map<IEnumerable<OrderDetailsDto>>(cartDto.CartDetails);

                OrderHeader orderCreated = _mapper.Map<OrderHeader>(orderHeaderDto);
                await _orderHeaderRepository.CreateAsync(orderCreated);
                orderHeaderDto.OrderId = orderCreated.OrderId;

                orderHeaderDto.OrderDetails = _mapper.Map<IEnumerable<OrderDetailsDto>>(orderCreated.OrderDetails);

                _response.Result = _mapper.Map<OrderHeaderDto>(orderHeaderDto);
                _response.StatusCode = HttpStatusCode.Created;
            }
            catch (Exception ex)
            {
                _response.InternalServerError(ex.Message);
            }
            return _response;
        }

        public async Task<ResponseDto> CreateStripeSession(StripeRequestDto stripeRequestDto)
        {
            try
            {
                var options = new SessionCreateOptions
                {
                    SuccessUrl = stripeRequestDto.ApprovedUrl,
                    CancelUrl = stripeRequestDto.CancelUrl,
                    LineItems = new List<SessionLineItemOptions>(),
                    Mode = "payment",
                };

                var discountObj = new List<SessionDiscountOptions>()
                {
                    new SessionDiscountOptions
                    {
                        Coupon = stripeRequestDto.OrderHeader.CouponCode
                    }
                };

                foreach (var item in stripeRequestDto.OrderHeader.OrderDetails)
                {
                    var seesionLineItem = new SessionLineItemOptions
                    {
                        PriceData = new SessionLineItemPriceDataOptions
                        {
                            UnitAmount = (long)(item.Price * 100),
                            Currency = "usd",
                            ProductData = new SessionLineItemPriceDataProductDataOptions
                            {
                                Name = item.Product.Name
                            }
                        },
                        Quantity = item.Quantity
                    };

                    options.LineItems.Add(seesionLineItem);
                }

                if (stripeRequestDto.OrderHeader.Discound > 0)
                {
                    options.Discounts = discountObj;
                }

                var service = new SessionService();
                Session session = service.Create(options);
                stripeRequestDto.StripeSessionUrl = session.Url;

                OrderHeader orderHeader = await _orderHeaderRepository.GetAsync(u => u.OrderId == stripeRequestDto.OrderHeader.OrderId);
                orderHeader.StripeSessionId = session.Id;
                await _orderHeaderRepository.UpdateAsync(orderHeader);

                _response.Result = stripeRequestDto;
                _response.StatusCode = HttpStatusCode.Created;
            }
            catch (Exception ex)
            {
                _response.InternalServerError(ex.Message);
            }
            return _response;
        }

        public async Task<ResponseDto> GetAllAsync(string? userId = "")
        {
            try
            {
                OrderHeader orderHeader = new();
                if (string.IsNullOrEmpty(userId))
                {
                    orderHeader = await _orderHeaderRepository.GetAsync();
                }
                else
                {
                    orderHeader = await _orderHeaderRepository.GetAsync(e => e.UserId == userId);
                }

                if (orderHeader == null)
                {
                    return _response.BadRequest("Order table is empty!");
                }

                OrderDto order = new()
                {
                    OrderHeader = _mapper.Map<OrderHeaderDto>(orderHeader),
                };

                
                var OrderDetails = await _orderDetailsRepository.GetAllAsync(u => u.OrderId == order.OrderHeader.OrderId);
                order.OrderDetails = _mapper.Map<IEnumerable<OrderDetailsDto>>(OrderDetails);

                var productDtos = _mapper.Map<IEnumerable<ProductDto>>(await _productRepository.GetAllAsync());

                foreach (var item in order.OrderDetails)
                {
                    item.Product =  productDtos.FirstOrDefault(u => u.ProductId == item.ProductId);
                    order.OrderHeader.TotalAmount += item.Quantity * item.Product.Price;
                }

                //apply coupon if any
                if (!string.IsNullOrEmpty(order.OrderHeader.CouponCode))
                {
                    Coupon coupon = await _couponRepository.GetAsync(c => c.CouponCode == order.OrderHeader.CouponCode);

                    if (coupon != null && order.OrderHeader.TotalAmount >= coupon.MinAmount)
                    {
                        order.OrderHeader.TotalAmount -= coupon.DiscountAmount;
                        order.OrderHeader.Discound = coupon.DiscountAmount;
                    }
                }
                _response.Result = order;
                _response.StatusCode = HttpStatusCode.OK;
            }
            catch (Exception ex)
            {
                _response.InternalServerError(ex.Message);
            }
            return _response;
        }

        public async Task<ResponseDto> GetByOrderIdAsync(int orderHeaderID)
        {
            try
            {
                OrderHeader orderHeader = new();
                orderHeader = await _orderHeaderRepository.GetAsync(e => e.OrderId == orderHeaderID);
                if (orderHeader == null)
                {
                    return _response.BadRequest("Order table is empty!");
                }

                OrderDto order = new()
                {
                    OrderHeader = _mapper.Map<OrderHeaderDto>(orderHeader),
                };

                var OrderDetails = await _orderDetailsRepository.GetAllAsync(u => u.OrderId == order.OrderHeader.OrderId);
                order.OrderDetails = _mapper.Map<IEnumerable<OrderDetailsDto>>(OrderDetails);

                var productDtos = _mapper.Map<IEnumerable<ProductDto>>(await _productRepository.GetAllAsync());

                foreach (var item in order.OrderDetails)
                {
                    item.Product = productDtos.FirstOrDefault(u => u.ProductId == item.ProductId);
                    order.OrderHeader.TotalAmount += item.Quantity * item.Product.Price;
                }

                //apply coupon if any
                if (!string.IsNullOrEmpty(order.OrderHeader.CouponCode))
                {
                    Coupon coupon = await _couponRepository.GetAsync(c => c.CouponCode == order.OrderHeader.CouponCode);

                    if (coupon != null && order.OrderHeader.TotalAmount >= coupon.MinAmount)
                    {
                        order.OrderHeader.TotalAmount -= coupon.DiscountAmount;
                        order.OrderHeader.Discound = coupon.DiscountAmount;
                    }
                }
                _response.Result = order;
                _response.StatusCode = HttpStatusCode.OK;
            }
            catch (Exception ex)
            {
                _response.InternalServerError(ex.Message);
            }
            return _response;
        }

        public async Task<ResponseDto> UpdateOrderStatus(int orderId, string newStatus)
        {
            try
            {
                OrderHeader orderHeader = new();
                orderHeader = await _orderHeaderRepository.GetAsync(e => e.OrderId == orderId);
                if (orderHeader == null)
                {
                    return _response.BadRequest("Cart is empty");
                }

                orderHeader.Status = newStatus;
                _orderHeaderRepository.UpdateAsync(orderHeader);

                _response.Result = _mapper.Map<OrderHeaderDto>(orderHeader);
                _response.StatusCode = HttpStatusCode.OK;
            }
            catch (Exception ex)
            {
                _response.InternalServerError(ex.Message);
            }
            return _response;
        }

        public async Task<ResponseDto> ValidateStripeSession(int orderHeaderId)
        {
            try
            {
                OrderHeader orderHeader = await _orderHeaderRepository.GetAsync(u => u.OrderId == orderHeaderId);

                var service = new SessionService();
                Session session = service.Get(orderHeader.StripeSessionId);

                var paymentIntentService = new PaymentIntentService();
                PaymentIntent paymentIntent = paymentIntentService.Get(session.PaymentIntentId);

                if (paymentIntent.Status == "succeeded")
                {
                    //then payment was successfull
                    orderHeader.PaymentIntenId = paymentIntent.Id;
                    orderHeader.Status = SD.Status_Approved;
                    await _orderHeaderRepository.UpdateAsync(orderHeader);

                    _response.Result = _mapper.Map<OrderHeaderDto>(orderHeader);
                    _response.StatusCode = HttpStatusCode.OK;
                }
            }
            catch (Exception ex)
            {
                _response.InternalServerError(ex.Message);
            }
            return _response;
        }
    }
}
