using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartContentIndexer.Core.Enums;

namespace SmartContentIndexer.Core.Models
{
    public class IndexingJob
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string DirectoryPath { get; set; } = string.Empty;
        public List<string> FilePatterns { get; set; } = new(); // *.pdf, *.docx, etc.
        public bool IncludeSubdirectories { get; set; } = true;
        public DateTime Started { get; set; }
        public DateTime? Completed { get; set; }
        public JobStatus Status { get; set; }
        public int TotalFiles { get; set; }
        public int ProcessedFiles { get; set; }
        public int FailedFiles { get; set; }
        public List<string> Errors { get; set; } = new();
    }
}
