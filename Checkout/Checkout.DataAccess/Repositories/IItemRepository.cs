using Checkout.DataAccess.Entities;

namespace Checkout.DataAccess.Repositories
{
    public interface IItemRepository
    {
        Task<int> Add(Item item);
    }
}
