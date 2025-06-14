using SmartContentIndexer.Core.Enums;
using SmartContentIndexer.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SmartContentIndexer.Infrastructure.Services
{
    public class ContentExtractionService : IContentExtractionService
    {
        private readonly Dictionary<FileType, List<string>> _supportedExtensions = new()
        {
            { FileType.Text, new List<string> { ".txt", ".md", ".log", ".csv" } },
            { FileType.Document, new List<string> { ".pdf", ".docx", ".doc", ".pptx", ".ppt" } },
            { FileType.Code, new List<string> { ".cs", ".js", ".py", ".java", ".cpp", ".html", ".css", ".xml", ".json" } },
            { FileType.Spreadsheet, new List<string> { ".xlsx", ".xls" } }
        };

        public async Task<string> ExtractTextAsync(string filePath, FileType fileType)
        {
            try
            {
                switch (fileType)
                {
                    case FileType.Text:
                    case FileType.Code:
                        return await File.ReadAllTextAsync(filePath, Encoding.UTF8);

                    case FileType.Document:
                        return await ExtractFromDocumentAsync(filePath);

                    case FileType.Spreadsheet:
                        return await ExtractFromSpreadsheetAsync(filePath);

                    default:
                        return string.Empty;
                }
            }
            catch (Exception ex)
            {
                // Log error
                return $"Error extracting text: {ex.Message}";
            }
        }

        public async Task<Dictionary<string, object>> ExtractMetadataAsync(string filePath)
        {
            var metadata = new Dictionary<string, object>();

            try
            {
                var fileInfo = new FileInfo(filePath);
                metadata["Size"] = fileInfo.Length;
                metadata["Created"] = fileInfo.CreationTime;
                metadata["Modified"] = fileInfo.LastWriteTime;
                metadata["Extension"] = fileInfo.Extension;
                metadata["Directory"] = fileInfo.DirectoryName ?? "";

                // Add more metadata extraction based on file type
                // This is where you'd use libraries like ExifLib for images, etc.

                return metadata;
            }
            catch
            {
                return metadata;
            }
        }

        public async Task<List<string>> ExtractKeywordsAsync(string content)
        {
            if (string.IsNullOrWhiteSpace(content))
                return new List<string>();

            // Simple keyword extraction - in production you'd use NLP libraries
            var words = Regex.Split(content.ToLower(), @"\W+")
                .Where(w => w.Length > 3 && !IsStopWord(w))
                .GroupBy(w => w)
                .OrderByDescending(g => g.Count())
                .Take(20)
                .Select(g => g.Key)
                .ToList();

            return words;
        }

        public async Task<string> GenerateSummaryAsync(string content, int maxLength = 200)
        {
            if (string.IsNullOrWhiteSpace(content))
                return string.Empty;

            // Simple summarization - first few sentences
            var sentences = content.Split(new[] { '.', '!', '?' }, StringSplitOptions.RemoveEmptyEntries)
                .Take(3)
                .Select(s => s.Trim());

            var summary = string.Join(". ", sentences);

            if (summary.Length > maxLength)
            {
                summary = summary.Substring(0, maxLength) + "...";
            }

            return summary;
        }

        public async Task<string> CategorizeContentAsync(string content)
        {
            // Simple categorization based on keywords
            var lowerContent = content.ToLower();

            var categories = new Dictionary<string, List<string>>
            {
                { "Programming", new List<string> { "code", "function", "class", "variable", "programming", "software", "development" } },
                { "Business", new List<string> { "meeting", "project", "client", "business", "company", "revenue", "strategy" } },
                { "Education", new List<string> { "university", "course", "exam", "study", "research", "paper", "thesis" } },
                { "Personal", new List<string> { "vacation", "travel", "family", "friend", "personal", "diary", "blog" } },
                { "Finance", new List<string> { "money", "budget", "investment", "finance", "bank", "expense", "income" } }
            };

            var categoryScores = new Dictionary<string, int>();

            foreach (var category in categories)
            {
                var score = category.Value.Count(keyword => lowerContent.Contains(keyword));
                if (score > 0)
                {
                    categoryScores[category.Key] = score;
                }
            }

            return categoryScores.OrderByDescending(kvp => kvp.Value).FirstOrDefault().Key ?? "General";
        }

        public bool SupportsFileType(FileType fileType)
        {
            return _supportedExtensions.ContainsKey(fileType);
        }

        private async Task<string> ExtractFromDocumentAsync(string filePath)
        {
            var extension = Path.GetExtension(filePath).ToLower();

            // TODO: Implement actual document extraction
            // For PDF: use iTextSharp or PdfPig
            // For Word: use DocumentFormat.OpenXml
            // For PowerPoint: use DocumentFormat.OpenXml

            return $"[Document content extraction not implemented for {extension}]";
        }

        private async Task<string> ExtractFromSpreadsheetAsync(string filePath)
        {
            // TODO: Implement spreadsheet extraction
            // Use EPPlus or ClosedXML for Excel files

            return "[Spreadsheet content extraction not implemented]";
        }

        private bool IsStopWord(string word)
        {
            var stopWords = new HashSet<string>
            {
                "the", "and", "or", "but", "in", "on", "at", "to", "for", "of", "with", "by", "from", "about",
                "into", "through", "during", "before", "after", "above", "below", "up", "down", "out", "off",
                "over", "under", "again", "further", "then", "once", "here", "there", "when", "where", "why",
                "how", "all", "any", "both", "each", "few", "more", "most", "other", "some", "such", "no",
                "nor", "not", "only", "own", "same", "so", "than", "too", "very", "can", "will", "just",
                "should", "now", "one", "two", "three", "four", "five", "six", "seven", "eight", "nine", "ten"
            };

            return stopWords.Contains(word);
        }
    }
}
