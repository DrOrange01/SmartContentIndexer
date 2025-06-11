using SmartContentIndexer.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartContentIndexer.Core.DTOs
{
    public class SearchResponse
    {
        public List<SearchResult> Results { get; set; } = new();
        public int TotalResults { get; set; }
        public float SearchDuration { get; set; } // in milliseconds
        public string ProcessedQuery { get; set; } = string.Empty;
        public List<string> Suggestions { get; set; } = new(); // Search suggestions
    }
}
