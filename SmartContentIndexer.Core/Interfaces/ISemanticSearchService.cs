using SmartContentIndexer.Core.DTOs;
using SmartContentIndexer.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartContentIndexer.Core.Interfaces
{
    public interface ISemanticSearchService
    {
        Task<SearchResponse> SearchAsync(SearchRequest request);
        Task<float[]> GenerateTextEmbeddingAsync(string text);
        Task<float[]> GenerateImageEmbeddingAsync(string imagePath);
        Task<float> CalculateSimilarityAsync(float[] embedding1, float[] embedding2);
        Task<List<string>> GetSearchSuggestionsAsync(string partialQuery);
        Task<string> ExplainSearchResultAsync(SearchResult result, string originalQuery);
    }
}
