using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Publish.Api.Dtos;
using Publish.Core.Entities.DocumentAggregate;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Publish.Api.Queries
{
    public class AllDocumentsQuery : IRequest<IEnumerable<DocumentDto>> { }

    public class AllDocumentsQueryHandler : IRequestHandler<AllDocumentsQuery, IEnumerable<DocumentDto>>
    {
        private PublishContext PublishContext { get; }
        private IHttpContextAccessor HttpContextAccessor { get; }

        public AllDocumentsQueryHandler(PublishContext publishContext, IHttpContextAccessor httpContextAccessor)
        {
            PublishContext = publishContext;
            HttpContextAccessor = httpContextAccessor;
        }

        public async Task<IEnumerable<DocumentDto>> Handle(AllDocumentsQuery request, CancellationToken cancellationToken)
        {
            var all = await PublishContext
                .Set<Document>()
                .ToListAsync(cancellationToken);

            return all.Select(x => DocumentDto.Create(x, HttpContextAccessor)); ;
        }
    }
}
