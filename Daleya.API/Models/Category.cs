using System.ComponentModel.DataAnnotations;

namespace Daleya.API.Models
{
    public class Category
    {
        [Key]
        public int CategoryId { get; set; }

        [Required]
        [MaxLength(50)]
        public required string Name { get; set; }
    }
}
