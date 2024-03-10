using MediatR;

namespace Order.Application.Commands
{
    public class DeleteOrderCommand : IRequest<Unit>
    {
        public int Id { get; set; }

    }
}
