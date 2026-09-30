using MediatR;
using Stock_Exchange.Application.Common.Exceptions;
using Stock_Exchange.Application.Common.Interfaces;
using Stock_Exchange.Application.Common.Models;
using Stock_Exchange.Application.Localization;
using Stock_Exchange.Domain.Repositories.Interfaces;

namespace Stock_Exchange.Application.Features.Videos.Commands.DeleteVideo
{
    public class DeleteVideoCommandHandler : IRequestHandler<DeleteVideoCommand, Result<bool>>
    {
        private readonly IVideoRepository _videoRepository;
        private readonly ICurrentUserService _currentUserService;

        public DeleteVideoCommandHandler(
            IVideoRepository videoRepository,
            ICurrentUserService currentUserService)
        {
            _videoRepository = videoRepository;
            _currentUserService = currentUserService;
        }

        public async Task<Result<bool>> Handle(DeleteVideoCommand request, CancellationToken cancellationToken)
        {
            if (!_currentUserService.IsAuthenticated || _currentUserService.UserId == Guid.Empty)
                throw new UnAuthorizedException(LocalizationKeys.ExceptionMessages.Unauthorized);

            var affected = await _videoRepository.HardDeleteByIdAsync(request.Id, cancellationToken);

            if (affected == 0)
                throw new NotFoundException(LocalizationKeys.VideoMessages.VideoNotFound);

            return Result<bool>.Success(true);
        }
    }
}
