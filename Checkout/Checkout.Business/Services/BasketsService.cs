using Checkout.Business.Configuration;
using Checkout.Business.Exceptions;
using Checkout.Business.Models;
using Checkout.DataAccess.Entities;
using Checkout.DataAccess.Repositories;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Checkout.Business.Services
{
    public class BasketsService : IBasketsService
    {
        private readonly IBasketRepository basketRepository;
        private readonly IItemRepository itemRepository;
        private readonly CheckoutOptions options;
        private readonly ILogger logger;

        public BasketsService(IBasketRepository basketRepository, IItemRepository itemRepository, IOptions<CheckoutOptions> options, ILogger<BasketsService> logger)
        {
            this.basketRepository = basketRepository;
            this.itemRepository = itemRepository;
            this.options = options.Value;
            this.logger = logger;
        }

        public async Task<int> CreateBasket(BasketCreateDto basketCreateDto)
        {
            logger.LogInformation("Entering - CreateBasket: customer = '{Customer}', paysVat = '{PaysVat}'", basketCreateDto.Customer, basketCreateDto.PaysVat);

            Basket basket = new Basket
            {
                Customer = basketCreateDto.Customer ?? string.Empty,
                PaysVat = basketCreateDto.PaysVat ?? false
            };

            int basketId = await basketRepository.Add(basket);

            logger.LogInformation("Leaving - CreateBasket returns {BasketIt}", basketId);
            return basketId;
        }

        public async Task AddItemToBasket(int basketId, ItemDto itemDto)
        {
            logger.LogInformation("Entering - AddItemToBasket: basketId = {BasketId}, item: {Item}, price: {Price}", basketId, itemDto.Item, itemDto.Price);
            await CheckBasketExists(basketId);

            Item item = new Item
            {
                Name = itemDto.Item ?? string.Empty,
                Price = itemDto.Price ?? 0,
                BasketId = basketId
            };
            await itemRepository.Add(item);

            logger.LogInformation("Leaving - AddItemToBasket");
        }

        public async Task<BasketDto> GetBasket(int basketId)
        {
            logger.LogInformation("Entering - GetBasket: basketId = {BasketId}", basketId);

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

            logger.LogInformation("Leaving - GetBasket");
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
            logger.LogInformation("Entering - UpdateBasketStatus: basketId = {BasketId}, closed: {Closed}, payed: {Payed}", basketId, basketStatusDto.Closed, basketStatusDto.Payed);
            
            await CheckBasketExists(basketId);

            await basketRepository.UpdateStatus(basketId, basketStatusDto.Closed ?? false, basketStatusDto.Payed ?? false);

            logger.LogInformation("Leaving - UpdateBasketStatus");
        }

        private async Task CheckBasketExists(int basketId)
        {
            bool basketExists = await basketRepository.Exists(basketId);
            if (!basketExists)
            {
                throw new NotFoundException($"Cannot find basket with Id {basketId}");
            }
        }
    }
}
