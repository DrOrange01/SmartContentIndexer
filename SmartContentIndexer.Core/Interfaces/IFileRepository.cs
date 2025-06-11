using SmartContentIndexer.Core.Enums;
using SmartContentIndexer.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartContentIndexer.Core.Interfaces
{
    public interface IFileRepository
    {
        Task<FileItem?> GetByIdAsync(Guid id);
        Task<FileItem?> GetByPathAsync(string filePath);
        Task<List<FileItem>> GetAllAsync();
        Task<List<FileItem>> GetByDirectoryAsync(string directoryPath, bool includeSubdirectories = false);
        Task<List<FileItem>> GetByFileTypeAsync(FileType fileType);
        Task<List<FileItem>> GetByCategoryAsync(string category);
        Task<List<FileItem>> GetModifiedSinceAsync(DateTime date);
        Task<List<FileItem>> GetOutdatedFilesAsync(); // Files that changed since last indexing

        Task<Guid> AddAsync(FileItem fileItem);
        Task<bool> UpdateAsync(FileItem fileItem);
        Task<bool> DeleteAsync(Guid id);
        Task<bool> DeleteByPathAsync(string filePath);
        Task<int> DeleteMissingFilesAsync(); // Remove files that no longer exist

        // Search helpers
        Task<List<FileItem>> SearchByKeywordsAsync(List<string> keywords);
        Task<List<FileItem>> SearchByTextAsync(string text);
        Task<List<FileItem>> GetSimilarFilesAsync(float[] embedding, float threshold = 0.7f, int maxResults = 10);
    }
}
