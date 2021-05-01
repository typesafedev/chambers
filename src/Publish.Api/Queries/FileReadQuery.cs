using MediatR;
using Microsoft.AspNetCore.Http;
using Publish.Api.Models;
using Publish.Core.Entities.DocumentAggregate;
using Publish.Core.Interfaces;
using System.Net.Mime;
using System.Threading;
using System.Threading.Tasks;

namespace Publish.Api.Queries
{
    public record FileReadQuery(Document Document) : IRequest<byte[]> { }

    public class FileReadQueryHandler : IRequestHandler<FileReadQuery, byte[]>
    {
        private IFileService FileService { get; }
        public FileReadQueryHandler(IFileService fileService) =>
            FileService = fileService;

        public async Task<byte[]> Handle(FileReadQuery request, CancellationToken cancellationToken) =>
            await FileService.ReadFile(request.Document);
    }
}
