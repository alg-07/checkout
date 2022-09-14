using Checkout.Business.Models;

namespace Checkout.Business.Services
{
    public interface IBasketsService
    {
        Task<int> CreateBasket(BasketCreateDto basketCreateDto);
        Task AddItemToBasket(int basketId, ItemDto itemDto);
        Task<BasketDto> GetBasket(int basketId);
        Task UpdateBasketStatus(int basketId, BasketStatusDto basketStatusDto);
    }
}
