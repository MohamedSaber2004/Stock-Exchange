using MediatR;
using Stock_Exchange.Application.Common.Exceptions;
using Stock_Exchange.Application.Common.Interfaces;
using Stock_Exchange.Application.Common.Models;
using Stock_Exchange.Application.Localization;
using Stock_Exchange.Domain.Repositories.Interfaces;

namespace Stock_Exchange.Application.Features.Articles.Commands.DeleteArticle
{
    public class DeleteArticleCommandHandler : IRequestHandler<DeleteArticleCommand, Result<bool>>
    {
        private readonly IArticleRepository _articleRepository;
        private readonly ICurrentUserService _currentUserService;

        public DeleteArticleCommandHandler(
            IArticleRepository articleRepository,
            ICurrentUserService currentUserService)
        {
            _articleRepository = articleRepository;
            _currentUserService = currentUserService;
        }

        public async Task<Result<bool>> Handle(DeleteArticleCommand request, CancellationToken cancellationToken)
        {
            if (!_currentUserService.IsAuthenticated || _currentUserService.UserId == Guid.Empty)
                throw new UnAuthorizedException(LocalizationKeys.ExceptionMessages.Unauthorized);

            var affected = await _articleRepository.HardDeleteByIdAsync(request.Id, cancellationToken);

            if (affected == 0)
                throw new NotFoundException(LocalizationKeys.ArticleMessages.ArticleNotFound);

            return Result<bool>.Success(true);
        }
    }
}
