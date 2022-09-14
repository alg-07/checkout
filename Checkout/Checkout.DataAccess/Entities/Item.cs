using System;
using System.Collections.Generic;

namespace Checkout.DataAccess.Entities
{
    public partial class Item
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public decimal Price { get; set; }
        public int BasketId { get; set; }

        public virtual Basket Basket { get; set; } = null!;
    }
}
