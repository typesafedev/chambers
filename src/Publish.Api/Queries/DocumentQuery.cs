using MediatR;
using Publish.Core.Entities.DocumentAggregate;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Publish.Api.Queries
{
    public record DocumentQuery(Guid DocumentId) : IRequest<Document> { }

    public class DocumentQueryHandler : IRequestHandler<DocumentQuery, Document>
    {
        private PublishContext PublishContext { get; }

        public DocumentQueryHandler(PublishContext publishContext)
        {
            PublishContext = publishContext;
        }

        public async Task<Document> Handle(DocumentQuery request, CancellationToken cancellationToken)
        {
            var document = await PublishContext.Documents.FindAsync(request.DocumentId);
            return document;
        }
    }
}
