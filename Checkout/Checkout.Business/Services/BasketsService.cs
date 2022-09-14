using Checkout.Business.Configuration;
using Checkout.Business.Exceptions;
using Checkout.Business.Models;
using Checkout.DataAccess.Entities;
using Checkout.DataAccess.Repositories;
using Microsoft.Extensions.Options;

namespace Checkout.Business.Services
{
    public class BasketsService : IBasketsService
    {
        private readonly IBasketRepository basketRepository;
        private readonly IItemRepository itemRepository;
        private readonly CheckoutOptions options;

        public BasketsService(IBasketRepository basketRepository, IItemRepository itemRepository, IOptions<CheckoutOptions> options)
        {
            this.basketRepository = basketRepository;
            this.itemRepository = itemRepository;
            this.options = options.Value;
        }

        public async Task<int> CreateBasket(BasketCreateDto basketCreateDto)
        {
            Basket basket = new Basket
            {
                Customer = basketCreateDto.Customer ?? string.Empty,
                PaysVat = basketCreateDto.PaysVat ?? false
            };

            int basketId = await basketRepository.Add(basket);
            return basketId;
        }

        public async Task AddItemToBasket(int basketId, ItemDto itemDto)
        {
            Basket? basket = await basketRepository.GetById(basketId);
            if (basket == null)
            {
                throw new NotFoundException($"Cannot find basket with Id {basketId}");
            }

            Item item = new Item
            {
                Name = itemDto.Item ?? string.Empty,
                Price = itemDto.Price ?? 0,
                BasketId = basketId
            };
            await itemRepository.Add(item);
        }

        public async Task<BasketDto> GetBasket(int basketId)
        {
            Basket? basket = await basketRepository.GetByIdIncludingItem(basketId);
            if (basket == null)
            {
                throw new NotFoundException($"Cannot find basket with Id {basketId}");
            }

            BasketDto basketDto = BasketToBasketDto(basket);

            basketDto.TotalNet = basketDto.Items!.Sum(item => item.Price) ?? 0;
            basketDto.TotalGross = basketDto.PaysVat ?
                basketDto.TotalNet * (1 + (decimal)options.VatValue / 100) :
                basketDto.TotalNet;

            return basketDto;
        }

        private BasketDto BasketToBasketDto(Basket basket)
            => new BasketDto
            {
                Id = basket.Id,
                Customer = basket.Customer,
                PaysVat = basket.PaysVat,
                Items = basket.Items.Select(item => ItemtoItemDto(item)).ToArray()
            };

        private ItemDto ItemtoItemDto(Item item)
            => new ItemDto
            {
                Item = item.Name,
                Price = item.Price
            };

        public async Task UpdateBasketStatus(int basketId, BasketStatusDto basketStatusDto)
        {
            bool basketExists = await basketRepository.Exists(basketId);
            if (!basketExists)
            {
                throw new NotFoundException($"Cannot find basket with Id {basketId}");
            }

            await basketRepository.UpdateStatus(basketId, basketStatusDto.Closed ?? false, basketStatusDto.Payed ?? false);
        }
    }
}
