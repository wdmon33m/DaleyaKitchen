using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Daleya.API.Models.Dto;

namespace Daleya.API.Models
{
    public class OrderDetails
    {
        [Key]
        public int OrderDetailId { get; set; }

        public int Quantity { get; set; }

        public int ProductId { get; set; }
        [NotMapped]
        public ProductDto Product { get; set; }

        [ForeignKey("OrderHeader")]
        public int OrderId { get; set; }
        public OrderHeader OrderHeader { get; set; }
        public decimal Price { get; set; }
    }
}
