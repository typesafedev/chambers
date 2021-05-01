using Microsoft.AspNetCore.Http;
using Publish.Api.Models;
using Publish.Core.Entities.DocumentAggregate;
using Publish.Core.Interfaces;
using System.Net.Mime;

namespace Publish.Api.Services
{
    public class FileValidationService : IFileValidationService
    {
        const int MB = 1024 * 1024;
        private bool IsPdf(IFormFile file) => file.ContentType == MediaTypeNames.Application.Pdf;
        private bool IsTooLarge(IFormFile file) => file.Length > 5 * MB;
        private bool IsNullOrEmpty(IFormFile file) => file == null || file.Length == 0;

        public IValidation Validate(IFormFile file)
        {
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
    }
}
