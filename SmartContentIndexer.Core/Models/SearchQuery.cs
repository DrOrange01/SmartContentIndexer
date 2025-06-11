using SmartContentIndexer.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartContentIndexer.Core.Models
{
    public class SearchQuery
    {
        public string Query { get; set; } = string.Empty;
        public SearchType Type { get; set; } = SearchType.Semantic;
        public List<FileType> FileTypes { get; set; } = new();
        public List<string> Categories { get; set; } = new();
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
        public int MaxResults { get; set; } = 50;
        public float MinimumScore { get; set; } = 0.5f;
    }
}
