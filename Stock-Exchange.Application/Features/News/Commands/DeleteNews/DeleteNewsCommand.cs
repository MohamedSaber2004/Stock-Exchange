using MediatR;
using Stock_Exchange.Application.Common.Models;

namespace Stock_Exchange.Application.Features.News.Commands.DeleteNews
{
    public record DeleteNewsCommand(Guid Id) : IRequest<Result<bool>>;
}
