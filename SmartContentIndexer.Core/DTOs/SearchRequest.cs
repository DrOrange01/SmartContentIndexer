using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartContentIndexer.Core.DTOs
{
    public class SearchRequest
    {
        public string Query { get; set; } = string.Empty;
        public string? FileType { get; set; }
        public string? Category { get; set; }
        public int MaxResults { get; set; } = 20;
    }
}
