using System.ComponentModel.DataAnnotations;

namespace Checkout.Business.Models
{
    public class ItemDto
    {
        [Required(AllowEmptyStrings = false)]
        public string? Item { get; set; }
        [Required]
        public decimal? Price { get; set; }
    }
}
