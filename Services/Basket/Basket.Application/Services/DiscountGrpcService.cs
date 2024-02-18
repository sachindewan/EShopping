using Discount.Grpc.Protos;
using Grpc.Net.Client;

namespace Basket.Application.Services
{
    public class DiscountGrpcService
    {
        private readonly DiscountProtoService.DiscountProtoServiceClient discountProtoServiceClient;

        public DiscountGrpcService(DiscountProtoService.DiscountProtoServiceClient discountProtoServiceClient,HttpClient httpClient)
        {
            this.discountProtoServiceClient = discountProtoServiceClient;;
            
        }

        public async Task<CouponModel> GetDiscountAsync(string productName) => await discountProtoServiceClient.GetDiscountAsync(new GetDiscountRequest { ProductName = productName });
    }
}
