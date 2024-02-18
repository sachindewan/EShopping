using Basket.Application.Commands;
using Basket.Application.Mappers;
using Basket.Application.Responses;
using Basket.Core.Entities;
using Basket.Core.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Basket.Application.Handlers
{
    public class CreateShoppingCartCommandHandler : IRequestHandler<CreateShoppingCartCommand, ShoppingCartResponse>
    {
        private readonly IBasketRepository _basketRepository;
        private readonly ILogger<CreateShoppingCartCommandHandler> logger;

        public CreateShoppingCartCommandHandler(IBasketRepository basketRepository,ILogger<CreateShoppingCartCommandHandler> logger)
        {
            _basketRepository = basketRepository;
            this.logger = logger;
        }

        public async Task<ShoppingCartResponse> Handle(CreateShoppingCartCommand request, CancellationToken cancellationToken)
        {
            logger.LogInformation("Cart is processing to be added");
            var shoppingCart = await _basketRepository.UpdateBasket(new ShoppingCart(request.UserName,request.Items).ValidateItems());
            var shoppingCartResponse = BasketMapper.Mapper.Map<ShoppingCart, ShoppingCartResponse>(shoppingCart);
            logger.LogInformation("Cart has been added with the following items : {items}",request.Items);
            return shoppingCartResponse;
        }
    }
}
