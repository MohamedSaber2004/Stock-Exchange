using MediatR;
using Stock_Exchange.Application.Common.Models;
using Stock_Exchange.Application.Features.HelpCenter.DTOs;

namespace Stock_Exchange.Application.Features.HelpCenter.Queries.GetHelpCenterById
{
    public class GetHelpCenterByIdQuery : IRequest<Result<HelpCenterDto>>
    {
        public Guid Id { get; set; }

        public GetHelpCenterByIdQuery()
        {
        }

        public GetHelpCenterByIdQuery(Guid id)
        {
            Id = id;
        }
    }
}
