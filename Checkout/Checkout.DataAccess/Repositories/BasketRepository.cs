using Checkout.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Checkout.DataAccess.Repositories
{
    public class BasketRepository : IBasketRepository
    {
        private readonly DataContext context;

        public BasketRepository(DataContext context)
        {
            this.context = context;
        }

        public async Task<int> Add(Basket basket)
        {
            var addedBasket = await context.Baskets.AddAsync(basket);
            context.SaveChanges();

            return addedBasket.Entity.Id;
        }

        public async Task<Basket?> GetById(int id)
        {
            return await context.Baskets.SingleOrDefaultAsync(b => b.Id == id);
        }

        public async Task<Basket?> GetByIdIncludingItem(int id)
        {
            return await context.Baskets
                .Include(b => b.Items)
                .SingleOrDefaultAsync(b => b.Id == id);
        }

        public async Task<bool> Exists(int id)
        {
            return await context.Baskets.AnyAsync(b => b.Id == id);
        }

        public async Task UpdateStatus(int id, bool isClosed, bool isPayed)
        {
            Basket? basket = await GetById(id);
            if (basket == null) return;

            basket.IsClosed = isClosed;
            basket.IsPayed = isPayed;

            await context.SaveChangesAsync();
        }
    }
}
