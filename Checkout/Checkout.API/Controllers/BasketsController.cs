using Checkout.Business.Exceptions;
using Checkout.Business.Models;
using Checkout.Business.Services;
using Microsoft.AspNetCore.Mvc;

namespace Checkout.API.Controllers
{
    [Route("baskets")]
    [ApiController]
    public class BasketsController : ControllerBase
    {
        private readonly IBasketsService basketsService;

        public BasketsController(IBasketsService basketsService)
        {
            this.basketsService = basketsService;
        }

        [HttpPost]
        public async Task<IActionResult> Post(BasketCreateDto basket)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            int basketId = await basketsService.CreateBasket(basket);
            return Ok(basketId);
        }

        [HttpPut]
        [Route("{id}/article-line")]
        public async Task<IActionResult> Put(int id, ItemDto item)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await basketsService.AddItemToBasket(id, item);
            return Ok();
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            try
            {
                BasketDto basket = await basketsService.GetBasket(id);

                return Ok(basket);
            }
            catch (NotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpPatch]
        [Route("{id}")]
        public async Task<IActionResult> Patch(int id, BasketStatusDto basketCloseDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                await basketsService.UpdateBasketStatus(id, basketCloseDto);

                return Ok();
            }
            catch (NotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }
    }
}
