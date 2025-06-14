using SmartContentIndexer.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace SmartContentIndexer.Infrastructure.Services
{
    public class ConfigurationService : IConfigurationService
    {
        private readonly Microsoft.Extensions.Configuration.IConfiguration _configuration;
        private readonly string _configPath;
        private readonly SemaphoreSlim _fileLock = new(1, 1);

        public ConfigurationService(Microsoft.Extensions.Configuration.IConfiguration configuration)
        {
            _configuration = configuration;
            _configPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                                      "SmartContentIndexer", "config.json");

            // Ensure the directory exists
            Directory.CreateDirectory(Path.GetDirectoryName(_configPath));
        }

        public List<string> GetWatchedDirectories()
        {
            var directories = _configuration.GetSection("WatchedDirectories").Get<List<string>>();
            return directories ?? new List<string> { Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) };
        }

        public async Task SetWatchedDirectoriesAsync(List<string> directories)
        {
            await UpdateConfigurationAsync("WatchedDirectories", directories);
        }

        public List<string> GetSupportedFileExtensions()
        {
            var extensions = _configuration.GetSection("SupportedExtensions").Get<List<string>>();
            return extensions ?? new List<string> { ".txt", ".pdf", ".docx", ".jpg", ".png", ".cs", ".js" };
        }

        public async Task SetSupportedFileExtensionsAsync(List<string> extensions)
        {
            await UpdateConfigurationAsync("SupportedExtensions", extensions);
        }

        public string GetOpenAIApiKey()
        {
            return _configuration["OpenAI:ApiKey"] ?? string.Empty;
        }

        public async Task SetOpenAIApiKeyAsync(string apiKey)
        {
            await UpdateConfigurationAsync("OpenAI:ApiKey", apiKey);
        }

        public string GetEmbeddingModel()
        {
            return _configuration["OpenAI:EmbeddingModel"] ?? "text-embedding-3-small";
        }

        public async Task SetEmbeddingModelAsync(string model)
        {
            await UpdateConfigurationAsync("OpenAI:EmbeddingModel", model);
        }

        public int GetMaxSearchResults()
        {
            return _configuration.GetValue<int>("Search:MaxResults", 50);
        }

        public async Task SetMaxSearchResultsAsync(int maxResults)
        {
            await UpdateConfigurationAsync("Search:MaxResults", maxResults);
        }

        public float GetMinimumSimilarityScore()
        {
            return _configuration.GetValue<float>("Search:MinimumScore", 0.5f);
        }

        public async Task SetMinimumSimilarityScoreAsync(float score)
        {
            await UpdateConfigurationAsync("Search:MinimumScore", score);
        }

        public int GetMaxConcurrentIndexingTasks()
        {
            return _configuration.GetValue<int>("Indexing:MaxConcurrentTasks", 3);
        }

        public async Task SetMaxConcurrentIndexingTasksAsync(int tasks)
        {
            await UpdateConfigurationAsync("Indexing:MaxConcurrentTasks", tasks);
        }

        public bool IsAutoIndexingEnabled()
        {
            return _configuration.GetValue<bool>("Indexing:AutoIndexing", true);
        }

        public async Task SetAutoIndexingEnabledAsync(bool enabled)
        {
            await UpdateConfigurationAsync("Indexing:AutoIndexing", enabled);
        }

        private async Task UpdateConfigurationAsync(string key, object value)
        {
            await _fileLock.WaitAsync();
            try
            {
                // Read existing configuration or create new one
                Dictionary<string, object> config;

                if (File.Exists(_configPath))
                {
                    var jsonContent = await File.ReadAllTextAsync(_configPath);
                    config = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonContent)
                            ?? new Dictionary<string, object>();
                }
                else
                {
                    config = new Dictionary<string, object>();
                }

                // Update the configuration value using nested key syntax
                SetNestedValue(config, key, value);

                // Write back to file
                var options = new JsonSerializerOptions
                {
                    WriteIndented = true,
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                };

                var updatedJson = JsonSerializer.Serialize(config, options);
                await File.WriteAllTextAsync(_configPath, updatedJson);
            }
            catch (Exception ex)
            {
                // Log the exception or handle it according to your logging strategy
                throw new InvalidOperationException($"Failed to update configuration for key '{key}'", ex);
            }
            finally
            {
                _fileLock.Release();
            }
        }

        private static void SetNestedValue(Dictionary<string, object> config, string key, object value)
        {
            var parts = key.Split(':');
            var current = config;

            // Navigate to the correct nested level
            for (int i = 0; i < parts.Length - 1; i++)
            {
                if (!current.ContainsKey(parts[i]))
                {
                    current[parts[i]] = new Dictionary<string, object>();
                }

                if (current[parts[i]] is JsonElement jsonElement)
                {
                    // Handle JsonElement from deserialization
                    current[parts[i]] = JsonElementToDictionary(jsonElement);
                }

                current = (Dictionary<string, object>)current[parts[i]];
            }

            // Set the final value
            current[parts[^1]] = value;
        }

        private static Dictionary<string, object> JsonElementToDictionary(JsonElement element)
        {
            var dict = new Dictionary<string, object>();

            if (element.ValueKind == JsonValueKind.Object)
            {
                foreach (var property in element.EnumerateObject())
                {
                    dict[property.Name] = JsonElementToObject(property.Value);
                }
            }

            return dict;
        }

        private static object JsonElementToObject(JsonElement element)
        {
            return element.ValueKind switch
            {
                JsonValueKind.String => element.GetString(),
                JsonValueKind.Number when element.TryGetInt32(out var intValue) => intValue,
                JsonValueKind.Number when element.TryGetSingle(out var floatValue) => floatValue,
                JsonValueKind.Number => element.GetDouble(),
                JsonValueKind.True => true,
                JsonValueKind.False => false,
                JsonValueKind.Array => element.EnumerateArray().Select(JsonElementToObject).ToList(),
                JsonValueKind.Object => JsonElementToDictionary(element),
                _ => null
            };
        }

        public void Dispose()
        {
            _fileLock?.Dispose();
        }
    }
}