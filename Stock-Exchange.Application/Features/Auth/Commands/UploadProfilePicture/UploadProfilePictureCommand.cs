using MediatR;
using Microsoft.AspNetCore.Http;
using Stock_Exchange.Domain.Enums;

namespace Stock_Exchange.Application.Features.Auth.Commands.UploadProfilePicture
{
    public class UploadProfilePictureCommand : IRequest<string>
    {
        public IFormFile? File { get; set; }
        public int Place { get; set; } = 0;
        public MediaType MediaType { get; set; } = MediaType.Image;

        public UploadProfilePictureCommand()
        {
        }

        public UploadProfilePictureCommand(IFormFile? file, int place = 0, MediaType mediaType = MediaType.Image)
        {
            File = file;
            Place = place;
            MediaType = mediaType;
        }
    }
}
