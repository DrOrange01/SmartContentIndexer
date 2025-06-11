using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartContentIndexer.Core.Enums;

namespace SmartContentIndexer.Core.Models
{
    public class FileItem
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string FilePath { get; set; } = string.Empty;
        public string FileName { get; set; } = string.Empty;
        public string FileExtension { get; set; } = string.Empty;
        public long FileSize { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
        public DateTime LastIndexed { get; set; }
        public FileType FileType { get; set; }
        public IndexStatus Status { get; set; }

        // Content analysis results
        public string ExtractedText { get; set; } = string.Empty;
        public List<string> Keywords { get; set; } = new();
        public List<string> Tags { get; set; } = new();
        public string Summary { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;

        // AI embeddings for semantic search
        public float[]? TextEmbedding { get; set; }
        public float[]? ImageEmbedding { get; set; }

        // Metadata
        public Dictionary<string, object> Metadata { get; set; } = new();
    }
}
