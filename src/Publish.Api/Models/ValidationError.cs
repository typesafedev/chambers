using Publish.Core.Interfaces;

namespace Publish.Api.Models
{
    public record ValidationError(string ErrorMessage) : IValidation
    {
    }
}
