using Checkout.DataAccess.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Checkout.DataAccess.Repositories
{
    public class BasketRepository : IBasketRepository
    {
        private readonly checkoutContext context;

        public BasketRepository(checkoutContext context)
        {
            this.context = context;
        }

        public async Task<int> Add(Basket basket)
        {
            var addedBasket = await context.Baskets.AddAsync(basket);
            context.SaveChanges();

            return addedBasket.Entity.Id;
        }
    }
}
