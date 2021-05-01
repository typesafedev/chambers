using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Moq;
using Publish.Api.Models;
using Publish.Api.Services;
using System.Net.Mime;
using Xunit;

namespace Publish.Api.Tests.Services
{
    public class FileValidationServiceTests
    {
        [Fact]
        public void Unknown_File_Should_Err()
        {
            // Given
            var unknown = new Mock<IFormFile>();
            unknown.Setup(x => x.ContentType).Returns("application/unknown");
            unknown.Setup(x => x.Length).Returns(1000);
            var sut = new FileValidationService();

            // When
            var actual = sut.Validate(unknown.Object);

            // Then
            actual.As<ValidationError>().ErrorMessage.Should().Be("File is not Pdf");
        }

        [Fact]
        public void Pdf_File_Should_Err()
        {
            // Given
            var file = new Mock<IFormFile>();
            file.Setup(x => x.ContentType).Returns(MediaTypeNames.Application.Pdf);
            var sut = new FileValidationService();

            // When
            var actual = sut.Validate(file.Object);

            // Then
            actual.As<ValidationError>().ErrorMessage.Should().NotBe("File is not Pdf");
        }

        [Fact]
        public void Small_File_Should_Not_Err()
        {
            // Given
            var file = new Mock<IFormFile>();
            file.Setup(x => x.Length).Returns(1000);
            var sut = new FileValidationService();

            // When
            var actual = sut.Validate(file.Object);

            // Then
            actual.As<ValidationError>().ErrorMessage.Should().NotBe("File is too large");
        }

        [Fact]
        public void Large_File_Should_Err()
        {
            // Given
            const int MB = 1024 * 1024;
            var file = new Mock<IFormFile>();
            file.Setup(x => x.ContentType).Returns(MediaTypeNames.Application.Pdf);
            file.Setup(x => x.Length).Returns((5 * MB) + 1);
            var sut = new FileValidationService();

            // When
            var actual = sut.Validate(file.Object);

            // Then
            actual.As<ValidationError>().ErrorMessage.Should().Be("File is too large");
        }

        [Fact]
        public void Empty_File_Should_Err()
        {
            // Given
            var sut = new FileValidationService();

            // When
            var actual = sut.Validate(null);

            // Then
            actual.As<ValidationError>().ErrorMessage.Should().Be("File is empty");
        }
    }
}
