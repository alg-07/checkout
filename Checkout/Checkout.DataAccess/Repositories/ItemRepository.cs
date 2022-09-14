using Checkout.DataAccess.Entities;

namespace Checkout.DataAccess.Repositories
{
    public class ItemRepository : IItemRepository
    {
        private readonly DataContext context;

        public ItemRepository(DataContext context)
        {
            this.context = context;
        }

        public async Task<int> Add(Item item)
        {
            var addedItem = await context.Items.AddAsync(item);
            context.SaveChanges();

            return addedItem.Entity.Id;
        }
    }
}
