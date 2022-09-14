using Checkout.DataAccess.Entities;

namespace Checkout.DataAccess.Repositories
{
    public interface IBasketRepository
    {
        Task<int> Add(Basket basket);
    }
}
