using System.ComponentModel.DataAnnotations;

namespace Checkout.Business.Models
{
    public class BasketStatusDto
    {
        [Required]
        public bool? Closed { get; set; }
        [Required]
        public bool? Payed { get; set; }
    }
}
