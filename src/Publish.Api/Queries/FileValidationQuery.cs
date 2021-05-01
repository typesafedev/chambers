using MediatR;
using Microsoft.AspNetCore.Http;
using Publish.Api.Models;
using Publish.Core.Interfaces;
using System.Net.Mime;
using System.Threading;
using System.Threading.Tasks;

namespace Publish.Api.Queries
{
    public record FileValidationQuery(IFormFile File) : IRequest<IValidation> { }

    public class FileValidationQueryHandler : IRequestHandler<FileValidationQuery, IValidation>
    {
        public async Task<IValidation> Handle(FileValidationQuery request, CancellationToken cancellationToken)
        {
            var file = request.File;
            if (IsNullOrEmpty(file))
            {
                return new ValidationError("File is empty");
            }

            if (IsPdf(file) == false)
            {
                return new ValidationError("File is not Pdf");
            }

            if (IsTooLarge(file))
            {
                return new ValidationError("File is too large");
            }

            return new ValidationSuccess();
        }

        const int MB = 1024 * 1024;
        private bool IsPdf(IFormFile file) => file.ContentType == MediaTypeNames.Application.Pdf;
        private bool IsTooLarge(IFormFile file) => file.Length > 5 * MB;
        private bool IsNullOrEmpty(IFormFile file) => file == null || file.Length == 0;
    }
}
