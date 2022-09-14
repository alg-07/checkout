using System;
using System.Collections.Generic;

namespace Checkout.DataAccess.Entities
{
    public partial class Basket
    {
        public Basket()
        {
            Items = new HashSet<Item>();
        }

        public int Id { get; set; }
        public string Customer { get; set; } = null!;
        public bool PaysVat { get; set; }
        public bool IsClosed { get; set; }
        public bool IsPayed { get; set; }

        public virtual ICollection<Item> Items { get; set; }
    }
}
