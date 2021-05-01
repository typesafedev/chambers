using Microsoft.AspNetCore.Http;
using Publish.Core.Entities.DocumentAggregate;

namespace Publish.Api.Dtos
{
    public class DocumentDto
    {
        public string Name { get; set; }
        public string Location { get; set; }
        public int FileSize { get; set; }

        public static DocumentDto Create(Document document, IHttpContextAccessor httpContextAccessor)
        {
            return new DocumentDto
            {
                Name = document.Name,
                Location = MakeLocation(document, httpContextAccessor),
                FileSize = document.FileSize
            };
        }

        private static string MakeLocation(Document document, IHttpContextAccessor httpContextAccessor) =>
            $"https://{httpContextAccessor.HttpContext.Request.Host.Value}/Document/{document.Id}";
    }
}
