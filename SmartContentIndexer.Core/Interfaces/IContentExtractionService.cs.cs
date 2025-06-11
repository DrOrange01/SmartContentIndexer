using SmartContentIndexer.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartContentIndexer.Core.Interfaces
{
    public interface IContentExtractionService
    {
        Task<string> ExtractTextAsync(string filePath, FileType fileType);
        Task<Dictionary<string, object>> ExtractMetadataAsync(string filePath);
        Task<List<string>> ExtractKeywordsAsync(string content);
        Task<string> GenerateSummaryAsync(string content, int maxLength = 200);
        Task<string> CategorizeContentAsync(string content);
        bool SupportsFileType(FileType fileType);
    }
}
