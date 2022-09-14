using Checkout.DataAccess.Entities;

namespace Checkout.DataAccess.Repositories
{
    public interface IBasketRepository
    {
        Task<int> Add(Basket basket);
        Task<Basket?> GetById(int id);
        Task<Basket?> GetByIdIncludingItem(int id);
        Task<bool> Exists(int id);
        Task UpdateStatus(int id, bool isClosed, bool isPayed);
    }
}
