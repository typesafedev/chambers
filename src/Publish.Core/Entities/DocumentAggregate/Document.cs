using System;

namespace Publish.Core.Entities.DocumentAggregate
{
    public class Document
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string FileType { get; set; }
        public int FileSize { get; set; }
        public string Path { get; set; }
    }
}
