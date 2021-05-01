using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Moq;
using Publish.Api.Commands;
using Publish.Api.Controllers;
using Publish.Api.Dtos;
using Publish.Api.Models;
using Publish.Api.Queries;
using Publish.Core.Entities.DocumentAggregate;
using Publish.Core.Interfaces;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Publish.Api.Tests.Controllers
{
    public class DocumentControllerTests
    {
        [Fact]
        public async Task UploadPdfToApiShouldSucceed()
        {
            // Given
            var fixture = new Fixture().Customize(new AutoMoqCustomization());
            fixture.Customize<BindingInfo>(c => c.OmitAutoProperties());
            var mediator = fixture.Freeze<Mock<IMediator>>();
            var file = fixture.Freeze<Mock<IFormFile>>();
            file.Setup(x => x.Length).Returns(1);
            file.Setup(x => x.OpenReadStream()).Returns(Stream.Null);
            var success = fixture.Create<ValidationSuccess>();
            mediator.Setup(x => x.Send(It.IsAny<FileValidationQuery>(), CancellationToken.None))
                .ReturnsAsync(success);
            var sut = fixture.Create<DocumentController>();

            // When
            var actual = await sut.Post(file.Object, CancellationToken.None);

            // Then
            actual.Should().BeOfType<NoContentResult>();
        }

        [Fact]
        public async Task UploadNonPdfToApiShouldFailWithBadRequest()
        {
            // Given
            var fixture = new Fixture().Customize(new AutoMoqCustomization());
            fixture.Customize<BindingInfo>(c => c.OmitAutoProperties());
            var mediator = fixture.Freeze<Mock<IMediator>>();
            var file = fixture.Freeze<Mock<IFormFile>>();
            file.Setup(x => x.Length).Returns(1);
            var error = new ValidationError("File is not Pdf");
            mediator.Setup(x => x.Send(It.IsAny<FileValidationQuery>(), CancellationToken.None))
                .ReturnsAsync(error);

            var sut = fixture.Create<DocumentController>();

            // When
            var actual = await sut.Post(file.Object, CancellationToken.None);

            // Then
            actual.Should().BeOfType<BadRequestObjectResult>();
            actual.As<BadRequestObjectResult>().Value.As<string>().Should().Be("File is not Pdf");
        }

        [Fact]
        public async Task UploadBigPdfToApiShouldFailWithBadRequest()
        {
            // Given
            var fixture = new Fixture().Customize(new AutoMoqCustomization());
            fixture.Customize<BindingInfo>(c => c.OmitAutoProperties());
            var mediator = fixture.Freeze<Mock<IMediator>>();
            var file = fixture.Freeze<Mock<IFormFile>>();
            file.Setup(x => x.Length).Returns(1);
            var error = new ValidationError("File is too large");
            mediator.Setup(x => x.Send(It.IsAny<FileValidationQuery>(), CancellationToken.None))
                .ReturnsAsync(error);
            var sut = fixture.Create<DocumentController>();

            // When
            var actual = await sut.Post(file.Object, CancellationToken.None);

            // Then
            actual.Should().BeOfType<BadRequestObjectResult>();
            actual.As<BadRequestObjectResult>().Value.As<string>().Should().Be("File is too large");
        }

        [Fact]
        public async Task UploadEmptyFileToApiShouldFailWithBadRequest()
        {
            // Given
            var fixture = new Fixture().Customize(new AutoMoqCustomization());
            fixture.Customize<BindingInfo>(c => c.OmitAutoProperties());
            var mediator = fixture.Freeze<Mock<IMediator>>();
            var file = fixture.Freeze<Mock<IFormFile>>(); 
            var error = new ValidationError("File is empty");
            mediator.Setup(x => x.Send(It.IsAny<FileValidationQuery>(), CancellationToken.None))
                .ReturnsAsync(error);
            var sut = fixture.Create<DocumentController>();

            // When
            var actual = await sut.Post(file.Object, CancellationToken.None);

            // Then
            actual.Should().BeOfType<BadRequestObjectResult>();
            actual.As<BadRequestObjectResult>().Value.As<string>().Should().Be("File is empty");
        }

        [Fact]
        public async Task GetAllDocumentsShouldReturnOKWithCorrectProperties()
        {
            // Given
            var fixture = new Fixture().Customize(new AutoMoqCustomization());
            fixture.Customize<BindingInfo>(c => c.OmitAutoProperties());
            var mediator = fixture.Freeze<Mock<IMediator>>();
            var dtos = fixture.CreateMany<DocumentDto>();
            mediator.Setup(x => x.Send(It.IsAny<AllDocumentsQuery>(), CancellationToken.None)).ReturnsAsync(dtos);
            var sut = fixture.Create<DocumentController>();

            // When
            var actual = await sut.GetAll(CancellationToken.None);

            // Then
            actual.Should().BeOfType<OkObjectResult>();
            actual.As<OkObjectResult>().Value.Should().BeAssignableTo<IEnumerable<DocumentDto>>();
            actual.As<OkObjectResult>().Value.Should().BeEquivalentTo(dtos);
        }

        [Fact]
        public async Task GetAllDocumentsOrderedShouldReturnOKWithDtosSortedByName()
        {
            // Given
            var fixture = new Fixture().Customize(new AutoMoqCustomization());
            fixture.Customize<BindingInfo>(c => c.OmitAutoProperties());
            var mediator = fixture.Freeze<Mock<IMediator>>();
            var dtos = fixture.CreateMany<DocumentDto>().OrderByDescending(x => x.Name);
            mediator.Setup(x => x.Send(It.IsAny<AllDocumentsQuery>(), CancellationToken.None))
                .ReturnsAsync(dtos);
            var sut = fixture.Create<DocumentController>();

            // When
            var actual = await sut.GetAllOrdered(CancellationToken.None);

            // Then
            actual.Should().BeOfType<OkObjectResult>();
            actual.As<OkObjectResult>().Value.Should().BeAssignableTo<IOrderedEnumerable<DocumentDto>>();
            actual.As<OkObjectResult>().Value.Should().BeEquivalentTo(dtos.OrderBy(x => x.Name), options => options.WithStrictOrdering());
            actual.As<OkObjectResult>().Value.Should().NotBeEquivalentTo(dtos, options => options.WithStrictOrdering());
        }

        [Fact]
        public async Task GetExistingPdfDownloadsPdf()
        {
            // Given
            var fixture = new Fixture().Customize(new AutoMoqCustomization());
            fixture.Customize<BindingInfo>(c => c.OmitAutoProperties());
            var mediator = fixture.Freeze<Mock<IMediator>>();
            var document = fixture.Create<Document>();
            mediator.Setup(x => x.Send(It.IsAny<DocumentQuery>(), CancellationToken.None)).ReturnsAsync(document);
            var sut = fixture.Create<DocumentController>();

            // When
            var actual = await sut.Get(document.Id, CancellationToken.None);

            // Then
            actual.Should().BeOfType<FileContentResult>();
            actual.As<FileContentResult>().FileDownloadName.Should().NotBeNull();
        }

        [Fact]
        public async Task DeleteExistingDocumentDeletesDoc()
        {
            // Given
            var fixture = new Fixture().Customize(new AutoMoqCustomization());
            fixture.Customize<BindingInfo>(c => c.OmitAutoProperties());
            var mediator = fixture.Freeze<Mock<IMediator>>();
            var document = fixture.Create<Document>();
            mediator.Setup(x => x.Send(It.IsAny<DocumentQuery>(), CancellationToken.None))
                .ReturnsAsync(document);
            mediator.Setup(x => x.Send(It.IsAny<DeleteDocumentCommand>(), CancellationToken.None))
                .ReturnsAsync(Unit.Value);
            var fileService = fixture.Freeze<Mock<IFileService>>();
            var sut = fixture.Create<DocumentController>();

            // When
            var actual = await sut.Delete(document.Id, CancellationToken.None);

            // Then
            actual.Should().BeOfType<NoContentResult>();
            mediator.Verify(x => x.Send(It.IsAny<DeleteDocumentCommand>(), It.IsAny<CancellationToken>()), Times.Once());
        }

        [Fact]
        public async Task DeleteNonExistingDocumentReturnsNotFound()
        {
            // Given
            var fixture = new Fixture().Customize(new AutoMoqCustomization());
            fixture.Customize<BindingInfo>(c => c.OmitAutoProperties());
            var mediator = fixture.Freeze<Mock<IMediator>>();
            var document = fixture.Create<Document>();
            mediator.Setup(x => x.Send(It.IsAny<DocumentQuery>(), CancellationToken.None)).ReturnsAsync((Document)null);
            var fileService = fixture.Freeze<Mock<IFileService>>();
            var sut = fixture.Create<DocumentController>();

            // When
            var actual = await sut.Delete(document.Id, CancellationToken.None);

            // Then
            actual.Should().BeOfType<NotFoundObjectResult>();
            actual.As<NotFoundObjectResult>().Value.Should().Be("Document not found");
            fileService.Verify(x => x.DeleteFile(document), Times.Never);
        }
    }
}
