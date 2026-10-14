using Microsoft.EntityFrameworkCore;
using MediatR;
using Stock_Exchange.Application.Common.Exceptions;
using Stock_Exchange.Application.Common.Interfaces;
using Stock_Exchange.Application.Common.Models;
using Stock_Exchange.Application.Localization;
using Stock_Exchange.Domain.Repositories.Interfaces;
using Stock_Exchange.Domain.Repositories.Interfaces.Base;

namespace Stock_Exchange.Application.Features.Experts.Commands.DeleteExpert
{
    public class DeleteExpertCommandHandler : IRequestHandler<DeleteExpertCommand, Result<bool>>
    {
        private readonly IExpertRepository _expertRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;

        public DeleteExpertCommandHandler(
            IExpertRepository expertRepository,
            IUnitOfWork unitOfWork,
            ICurrentUserService currentUserService)
        {
            _expertRepository = expertRepository;
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
        }

        public async Task<Result<bool>> Handle(DeleteExpertCommand request, CancellationToken cancellationToken)
        {
            if (!_currentUserService.IsAuthenticated || _currentUserService.UserId == Guid.Empty)
                throw new UnAuthorizedException(LocalizationKeys.ExceptionMessages.Unauthorized);

            var expert = await _expertRepository
                .GetAllWithIncluding(e => e.Id == request.Id, e => e.ConsultationAreas)
                .FirstOrDefaultAsync(cancellationToken);

            if (expert is null)
                throw new NotFoundException(LocalizationKeys.ExpertMessages.ExpertNotFound);

            foreach (var area in expert.ConsultationAreas.Where(c => !c.IsDeleted))
            {
                area.MarkAsDeleted(_currentUserService.UserId.ToString());
            }

            _expertRepository.Delete(expert);
            await _unitOfWork.SaveChangesAsync();

            return Result<bool>.Success(true);
        }
    }
}
