using MediatR;
using Microsoft.AspNetCore.Http;
using Publish.Core.Entities.DocumentAggregate;
using Publish.Core.Interfaces;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Publish.Api.Commands
{
    public class NewDocumentCommand : IRequest<Unit>
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string FileType { get; set; }
        public int FileSize { get; set; }
        public string Path { get; set; }
        public byte[] Content { get; set; }

        public static async Task<NewDocumentCommand> Create(IFormFile file)
        {
            var id = Guid.NewGuid();

            using var fileStream = file.OpenReadStream();
            byte[] content = new byte[(int)file.Length];
            if (fileStream != Stream.Null)
            {
                await fileStream.ReadAsync(content, 0, (int)file.Length);
            }
            return new() 
            { 
                Id = id,
                Name = file.FileName,
                FileType = file.ContentType,
                FileSize = (int)file.Length,
                Path = $"{id}_{file.Name}",
                Content = content
            };
        }

        public Document ToDocument()
        {
            return new()
            {
                Id = Id,
                Name = Name,
                FileType = FileType,
                FileSize = FileSize,
                Path = Path
            };
        }
    }

    public class NewDocumentCommandHandler : IRequestHandler<NewDocumentCommand, Unit>
    {
        private PublishContext PublishContext { get; }
        private IFileService FileService { get; }
        public NewDocumentCommandHandler(PublishContext publishContext, IFileService fileService)
        {
            PublishContext = publishContext;
            FileService = fileService;
        }

        public async Task<Unit> Handle(NewDocumentCommand request, CancellationToken cancellationToken)
        {
            await PublishContext.AddAsync(request.ToDocument(), cancellationToken);
            await PublishContext.SaveChangesAsync(cancellationToken);

            await FileService.WriteFile(request.ToDocument(), request.Content);
            return Unit.Value;
        }
    }
}
