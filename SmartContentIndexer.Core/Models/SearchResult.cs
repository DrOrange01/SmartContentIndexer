using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartContentIndexer.Core.Models
{
    public class SearchResult
    {
        public FileItem File { get; set; } = new();
        public float RelevanceScore { get; set; }
        public List<string> MatchedSnippets { get; set; } = new();
        public string Explanation { get; set; } = string.Empty; // Why this file matched
        public Dictionary<string, float> FeatureScores { get; set; } = new(); // Content, filename, metadata scores
    }
}
