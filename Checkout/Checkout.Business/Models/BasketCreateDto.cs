using System.ComponentModel.DataAnnotations;

namespace Checkout.Business.Models
{
    public class BasketCreateDto
    {
        [Required(AllowEmptyStrings = false)]
        public string? Customer { get; set; }
        [Required]
        public bool? PaysVat { get; set; }
    }
}
