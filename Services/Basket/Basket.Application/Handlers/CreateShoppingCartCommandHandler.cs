﻿using Basket.Application.Commands;
using Basket.Application.Mappers;
using Basket.Application.Responses;
using Basket.Core.Entities;
using Basket.Core.Repositories;
using MediatR;

namespace Basket.Application.Handlers
{
    public class CreateShoppingCartCommandHandler : IRequestHandler<CreateShoppingCartCommand, ShoppingCartResponse>
    {
        private readonly IBasketRepository _basketRepository;

        public CreateShoppingCartCommandHandler(IBasketRepository basketRepository)
        {
            _basketRepository = basketRepository;
        }

        public async Task<ShoppingCartResponse> Handle(CreateShoppingCartCommand request, CancellationToken cancellationToken)
        {
            var shoppingCart = await _basketRepository.UpdateBasket(new ShoppingCart(request.UserName,request.Items).ValidateItems());
            var shoppingCartResponse = BasketMapper.Mapper.Map<ShoppingCart, ShoppingCartResponse>(shoppingCart);
            return shoppingCartResponse;
        }
    }
}
