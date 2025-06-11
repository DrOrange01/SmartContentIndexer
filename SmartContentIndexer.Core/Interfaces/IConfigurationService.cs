using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartContentIndexer.Core.Interfaces
{
    public interface IConfigurationService
    {
        // Indexing settings
        List<string> GetWatchedDirectories();
        Task SetWatchedDirectoriesAsync(List<string> directories);
        List<string> GetSupportedFileExtensions();
        Task SetSupportedFileExtensionsAsync(List<string> extensions);

        // AI settings
        string GetOpenAIApiKey();
        Task SetOpenAIApiKeyAsync(string apiKey);
        string GetEmbeddingModel();
        Task SetEmbeddingModelAsync(string model);

        // Search settings
        int GetMaxSearchResults();
        Task SetMaxSearchResultsAsync(int maxResults);
        float GetMinimumSimilarityScore();
        Task SetMinimumSimilarityScoreAsync(float score);

        // Performance settings
        int GetMaxConcurrentIndexingTasks();
        Task SetMaxConcurrentIndexingTasksAsync(int tasks);
        bool IsAutoIndexingEnabled();
        Task SetAutoIndexingEnabledAsync(bool enabled);
    }
}
