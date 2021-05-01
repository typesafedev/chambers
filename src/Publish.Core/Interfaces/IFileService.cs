using Publish.Core.Entities.DocumentAggregate;
using System.Threading.Tasks;

namespace Publish.Core.Interfaces
{
    public interface IFileService
    {
        public Task<byte[]> ReadFile(Document document);
        public Task WriteFile(Document document, byte[] contents);
        public Task DeleteFile(Document document);
    }
}
