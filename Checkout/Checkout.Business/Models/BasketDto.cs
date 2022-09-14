namespace Checkout.Business.Models
{
    public class BasketDto
    {
        public int Id { get; set; }
        public IEnumerable<ItemDto>? Items { get; set; }
        public decimal TotalNet { get; set; }
        public decimal TotalGross { get; set; }
        public string Customer { get; set; } = string.Empty;
        public bool PaysVat { get; set; } = false;
    }
}
