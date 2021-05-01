using Microsoft.Extensions.Hosting;
using Publish.Core.Entities.DocumentAggregate;
using Publish.Core.Interfaces;
using System.IO;
using System.IO.Abstractions;
using System.Threading.Tasks;

namespace Publish.Api.Services
{
    public class FileService : IFileService
    {
        private IHostEnvironment Environment { get; }
        private IFileSystem FileSystem { get; }

        public FileService(IHostEnvironment env, IFileSystem fileSystem)
        {
            Environment = env;
            FileSystem = fileSystem;
        }

        // Should persist in az blob storage
        public async Task<byte[]> ReadFile(Document document)
        {
            var path = Environment.ContentRootPath;
            var filePath = Path.Combine(path, "Documents", $"{document.Id}_{document.Name}");
            return await FileSystem.File.ReadAllBytesAsync(filePath);
        }

        // Should persist in az blob storage
        public async Task WriteFile(Document document, byte[] contents)
        {
            var path = Environment.ContentRootPath;
            var filePath = Path.Combine(path, "Documents", $"{document.Id}_{document.Name}");
            await FileSystem.File.WriteAllBytesAsync(filePath, contents);
        }

        public Task DeleteFile(Document document)
        {
            var path = Environment.ContentRootPath;
            var filePath = Path.Combine(path, "Documents", $"{document.Id}_{document.Name}");
            FileSystem.File.Delete(filePath);
            return Task.CompletedTask;
        }
    }
}
