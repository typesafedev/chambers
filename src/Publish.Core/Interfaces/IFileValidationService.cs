using Microsoft.AspNetCore.Http;
using Publish.Core.Entities.DocumentAggregate;

namespace Publish.Core.Interfaces
{
    public interface IFileValidationService
    {
        public IValidation Validate(IFormFile file);
    }
}
