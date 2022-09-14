using Checkout.Business.Configuration;
using Checkout.Business.Models;
using Checkout.Business.Services;
using Checkout.DataAccess.Entities;
using Checkout.DataAccess.Repositories;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using NSubstitute.Extensions;

namespace Checkout.Business.UnitTests
{
    [TestClass]
    public class BasketsServiceTests
    {
        private IBasketsService basketsService = null!;
        private IBasketRepository substituteBasketRepository = null!;
        private IItemRepository substituteItemRepository = null!;
        private IOptions<CheckoutOptions> checkoutOptions = null!;

        [TestInitialize]
        public void TestInitialize()
        {
            substituteBasketRepository = Substitute.For<IBasketRepository>();
            substituteItemRepository = Substitute.For<IItemRepository>();
            checkoutOptions = Options.Create<CheckoutOptions>(new CheckoutOptions { VatValue = 10 });

            basketsService = new BasketsService(substituteBasketRepository, substituteItemRepository, checkoutOptions);
        }

        [TestMethod]
        public async Task CreateBasketTest()
        {
            int expectedBasketId = 771;
            substituteBasketRepository.Configure().Add(Arg.Any<Basket>()).Returns(expectedBasketId);
            BasketCreateDto basketCreateDto = new BasketCreateDto
            {
                Customer = "Andrei",
                PaysVat = true
            };

            int createdBasketId = await basketsService.CreateBasket(basketCreateDto);

            await substituteBasketRepository.Received().Add(Arg.Is<Basket>(b => b.Customer == basketCreateDto.Customer && b.PaysVat == basketCreateDto.PaysVat));
            Assert.AreEqual(expectedBasketId, createdBasketId);
        }

        [TestMethod]
        public async Task AddItemToBasketTest()
        {
            substituteBasketRepository.Configure().Exists(Arg.Any<int>()).Returns(true);
            int basketId = 771;
            ItemDto itemDto = new ItemDto
            {
                Item = "tomato",
                Price = 20
            };

            await basketsService.AddItemToBasket(basketId, itemDto);

            await substituteBasketRepository.Received().Exists(basketId);
            await substituteItemRepository.Received().Add(Arg.Is<Item>(item => item.BasketId == basketId && item.Name == itemDto.Item && item.Price == itemDto.Price));
        }

        [TestMethod]
        public async Task GetBasketTest()
        {
            int basketId = 772;
            Basket basket = new Basket
            {
                Id = basketId,
                Customer = "Andrei",
                PaysVat = true,
                Items = new List<Item>
                {
                    new Item
                    {
                        Name = "tomato",
                        Price = 8
                    },
                    new Item
                    {
                        Name = "juice",
                        Price = 7
                    }
                }
            };

            substituteBasketRepository.Configure().GetByIdIncludingItem(basketId).Returns(basket);

            BasketDto basketDto = await basketsService.GetBasket(basketId);

            Assert.AreEqual(basketId, basketDto.Id);
            Assert.AreEqual(basket.Customer, basketDto.Customer);
            Assert.AreEqual(basket.PaysVat, basketDto.PaysVat);
            Assert.AreEqual(basket.Items.Count, basketDto.Items?.Count());

            foreach(var item in basket.Items)
            {
                ItemDto? itemDto = basketDto.Items?.FirstOrDefault(i => i.Item == item.Name);
                Assert.IsNotNull(itemDto);
                Assert.AreEqual(item.Price, itemDto.Price);
            }

            decimal totalNet = basket.Items.Sum(item => item.Price);
            Assert.AreEqual(totalNet, basketDto.TotalNet);
            decimal totalGross = totalNet * (1 + (decimal)checkoutOptions.Value.VatValue / 100);
            Assert.AreEqual(totalGross, basketDto.TotalGross);
        }

        [TestMethod]
        public async Task UpdateBasketStatusTest()
        {
            int basketId = 773;
            BasketStatusDto basketStatusDto = new BasketStatusDto()
            {
                Closed = false,
                Payed = true
            };
            substituteBasketRepository.Configure().Exists(basketId).Returns(true);

            await basketsService.UpdateBasketStatus(basketId, basketStatusDto);

            await substituteBasketRepository.Received().Exists(basketId);
            await substituteBasketRepository.Received().UpdateStatus(basketId, basketStatusDto.Closed.Value, basketStatusDto.Payed.Value);
        }
    }
}