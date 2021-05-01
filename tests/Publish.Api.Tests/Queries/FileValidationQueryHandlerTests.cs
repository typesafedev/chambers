using Microsoft.AspNetCore.Http;
using FluentAssertions;
using Moq;
using Publish.Api.Models;
using Publish.Api.Queries;
using System.Net.Mime;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Publish.Api.Tests.Queries
{
    public class FileValidationQueryHandlerTests
    {
        [Fact]
        public async Task Unknown_File_Should_Err()
        {
            // Given
            var unknown = new Mock<IFormFile>();
            unknown.Setup(x => x.ContentType).Returns("application/unknown");
            unknown.Setup(x => x.Length).Returns(1000);
            var validationQuery = new FileValidationQuery(unknown.Object);
            var sut = new FileValidationQueryHandler();

            // When
            var actual = await sut.Handle(validationQuery, CancellationToken.None);

            // Then
            actual.As<ValidationError>().ErrorMessage.Should().Be("File is not Pdf");
        }

        [Fact]
        public async Task Pdf_File_Should_Err()
        {
            // Given
            var file = new Mock<IFormFile>();
            file.Setup(x => x.ContentType).Returns(MediaTypeNames.Application.Pdf);
            var validationQuery = new FileValidationQuery(file.Object);
            var sut = new FileValidationQueryHandler();

            // When
            var actual = await sut.Handle(validationQuery, CancellationToken.None);

            // Then
            actual.As<ValidationError>().ErrorMessage.Should().NotBe("File is not Pdf");
        }

        [Fact]
        public async Task Small_File_Should_Not_Err()
        {
            // Given
            var file = new Mock<IFormFile>();
            file.Setup(x => x.Length).Returns(1000);
            var validationQuery = new FileValidationQuery(file.Object);
            var sut = new FileValidationQueryHandler();

            // When
            var actual = await sut.Handle(validationQuery, CancellationToken.None);

            // Then
            actual.As<ValidationError>().ErrorMessage.Should().NotBe("File is too large");
        }

        [Fact]
        public async Task Large_File_Should_Err()
        {
            // Given
            const int MB = 1024 * 1024;
            var file = new Mock<IFormFile>();
            file.Setup(x => x.ContentType).Returns(MediaTypeNames.Application.Pdf);
            file.Setup(x => x.Length).Returns((5 * MB) + 1);
            var validationQuery = new FileValidationQuery(file.Object);
            var sut = new FileValidationQueryHandler();

            // When
            var actual = await sut.Handle(validationQuery, CancellationToken.None);

            // Then
            actual.As<ValidationError>().ErrorMessage.Should().Be("File is too large");
        }

        [Fact]
        public async Task Empty_File_Should_Err()
        {
            // Given
            var validationQuery = new FileValidationQuery(null);
            var sut = new FileValidationQueryHandler();

            // When
            var actual = await sut.Handle(validationQuery, CancellationToken.None);

            // Then
            actual.As<ValidationError>().ErrorMessage.Should().Be("File is empty");
        }
    }
}
