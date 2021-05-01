using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Publish.Api.Commands;
using Publish.Api.Models;
using Publish.Api.Queries;
using Publish.Core.Interfaces;
using System;
using System.Linq;
using System.Net.Mime;
using System.Threading;
using System.Threading.Tasks;
using static Microsoft.AspNetCore.Http.StatusCodes;
namespace Publish.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DocumentController : ControllerBase
    {
        private ILogger<DocumentController> Logger { get; }
        private IMediator Mediator { get; }
        private IFileValidationService FileValidationService { get; }
        private IFileService FileService { get; }

        public DocumentController(
            ILogger<DocumentController> logger,
            IMediator mediator,
            IFileValidationService fileValidationService,
            IFileService fileService)
        {
            Logger = logger;
            Mediator = mediator;
            FileValidationService = fileValidationService;
            FileService = fileService;
        }

        [HttpGet]
        [ProducesResponseType(Status200OK)]
        public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
        {
            Logger.LogInformation("Get all documents");
            var query = new AllDocumentsQuery();
            var docs = await Mediator.Send(query, cancellationToken);
            return Ok(docs);
        }

        [HttpGet("Ordered")]
        [ProducesResponseType(Status200OK)]
        public async Task<IActionResult> GetAllOrdered(CancellationToken cancellationToken)
        {
            Logger.LogInformation("Get all documents");
            var query = new AllDocumentsQuery();
            var docs = await Mediator.Send(query, cancellationToken);
            return Ok(docs.OrderBy(x => x.Name));
        }

        [HttpGet("{id}")]
        [ProducesResponseType(Status200OK)]
        [ProducesResponseType(Status404NotFound)]
        public async Task<IActionResult> Get(Guid id, CancellationToken cancellationToken)
        {
            Logger.LogInformation("Get document with id {DocumentId}", id);
            var query = new DocumentQuery(id);
            var doc = await Mediator.Send(query, cancellationToken);
            if (doc == null)
            {
                return NotFound("Document could not be found in system");
            }

            var fileContents = await FileService.ReadFile(doc);
            if (fileContents == null)
            {
                return NotFound("Document could not be found in storage");
            }

            return File(fileContents, MediaTypeNames.Application.Pdf, doc.Name);
        }

        [HttpPost, DisableRequestSizeLimit]
        [ProducesResponseType(Status204NoContent)]
        [ProducesResponseType(Status400BadRequest)]
        public async Task<IActionResult> Post(IFormFile file, CancellationToken cancellationToken)
        {
            if (FileValidationService.Validate(file) is ValidationError validation)
            {
                return BadRequest(validation.ErrorMessage);
            }

            var newDocumentCommand = await NewDocumentCommand.Create(file);
            await Mediator.Send(newDocumentCommand, cancellationToken);
            return NoContent();
        }

        [HttpDelete]
        [ProducesResponseType(Status204NoContent)]
        [ProducesResponseType(Status404NotFound)]
        public async Task<IActionResult> Delete(Guid documentId, CancellationToken cancellationToken)
        {
            var documentQuery = new DocumentQuery(documentId);
            var document = await Mediator.Send(documentQuery, cancellationToken);
            if (document == null)
            {
                return NotFound("Document not found");
            }

            var deleteDocumentCommand = DeleteDocumentCommand.Create(document);
            await Mediator.Send(deleteDocumentCommand, cancellationToken);
            return NoContent();
        }
    }
}
