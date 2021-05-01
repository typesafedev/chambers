using MediatR;
using Publish.Core.Entities.DocumentAggregate;
using Publish.Core.Interfaces;
using System.Threading;
using System.Threading.Tasks;

namespace Publish.Api.Commands
{
    public class DeleteDocumentCommand : IRequest<Unit>
    {
        public Document Document { get; set; }

        public static DeleteDocumentCommand Create(Document document) =>
            new DeleteDocumentCommand { Document = document };
    }

    public class DeleteDocumentCommandHandler : IRequestHandler<DeleteDocumentCommand, Unit>
    {
        private PublishContext PublishContext { get; }
        private IFileService FileService { get; }
        public DeleteDocumentCommandHandler(PublishContext publishContext, IFileService fileService)
        {
            PublishContext = publishContext;
            FileService = fileService;
        }

        public async Task<Unit> Handle(DeleteDocumentCommand request, CancellationToken cancellationToken)
        {
            PublishContext.Documents.Remove(request.Document);
            await PublishContext.SaveChangesAsync(cancellationToken);

            await FileService.DeleteFile(request.Document);
            return Unit.Value;
        }
    }
}
