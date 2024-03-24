using Basket.Application.Commands;
using Basket.Application.Queries;
using Basket.Application.Responses;
using Basket.Application.Services;
using Discount.Grpc.Protos;
using Espire.Common.Logging.Correlation;
using Google.Protobuf;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Writers;
using Newtonsoft.Json;
using Polly;
using System.Net;
using System.Text;

namespace Basket.API.Controllers
{
    public class BasketController : ApiController
    {
        private readonly DiscountGrpcService discountGrpcService;
        private readonly ICorrelationIdGenerator correlationIdGenerator;
        private readonly ILogger<BasketController> logger;
   
        public BasketController(DiscountGrpcService discountGrpcService,ICorrelationIdGenerator correlationIdGenerator,ILogger<BasketController> logger)
        {
            this.discountGrpcService = discountGrpcService;
            this.correlationIdGenerator = correlationIdGenerator;
            this.logger = logger;
        }

        [HttpGet]
        [Route("[action]/{userName}", Name = "GetBasketByUserName")]
        [ProducesResponseType(typeof(ShoppingCartResponse), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<ShoppingCartResponse>> GetBasket(string userName)
        {
            var query = new GetBasketByUserNameQuery(userName);
            var basket = await Mediator.Send(query);
            return Ok(basket);
        }

        [HttpPost("CreateBasket")]
        [ProducesResponseType(typeof(ShoppingCartResponse), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<ShoppingCartResponse>> UpdateBasket([FromBody] CreateShoppingCartCommand createShoppingCartCommand)
        {

            foreach (var item in createShoppingCartCommand.Items)
            {
                var coupon = await discountGrpcService.GetDiscountAsync(item.ProductName);
                item.Price -= coupon.Amount;
            }
            var basket = await Mediator.Send(createShoppingCartCommand);
            return Ok(basket);
        }

        [HttpDelete]
        [Route("[action]/{userName}", Name = "DeleteBasketByUserName")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<ActionResult<ShoppingCartResponse>> DeleteBasket(string userName)
        {
            var query = new DeleteBasketByUserNameQuery(userName);
            logger.LogInformation("basket deleted successfully.");
            return Ok(await Mediator.Send(query));
        }
    }
}
