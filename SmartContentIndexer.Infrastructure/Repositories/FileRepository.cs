using SmartContentIndexer.Core.Enums;
using SmartContentIndexer.Core.Interfaces;
using SmartContentIndexer.Core.Models;
using SmartContentIndexer.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartContentIndexer.Infrastructure.Repositories
{
    public class FileRepository : IFileRepository
    {
        private readonly SmartContentDbContext _context;

        public FileRepository(SmartContentDbContext context)
        {
            _context = context;
        }

        public async Task<FileItem?> GetByIdAsync(Guid id)
        {
            return await _context.Files.FindAsync(id);
        }

        public async Task<FileItem?> GetByPathAsync(string filePath)
        {
            return await _context.Files.FirstOrDefaultAsync(f => f.FilePath == filePath);
        }

        public async Task<List<FileItem>> GetAllAsync()
        {
            return await _context.Files.ToListAsync();
        }

        public async Task<List<FileItem>> GetByDirectoryAsync(string directoryPath, bool includeSubdirectories = false)
        {
            var query = _context.Files.AsQueryable();

            if (includeSubdirectories)
            {
                query = query.Where(f => f.FilePath.StartsWith(directoryPath));
            }
            else
            {
                query = query.Where(f => Path.GetDirectoryName(f.FilePath) == directoryPath);
            }

            return await query.ToListAsync();
        }

        public async Task<List<FileItem>> GetByFileTypeAsync(FileType fileType)
        {
            return await _context.Files.Where(f => f.FileType == fileType).ToListAsync();
        }

        public async Task<List<FileItem>> GetByCategoryAsync(string category)
        {
            return await _context.Files.Where(f => f.Category == category).ToListAsync();
        }

        public async Task<List<FileItem>> GetModifiedSinceAsync(DateTime date)
        {
            return await _context.Files.Where(f => f.ModifiedDate > date).ToListAsync();
        }

        public async Task<List<FileItem>> GetOutdatedFilesAsync()
        {
            return await _context.Files.Where(f => f.ModifiedDate > f.LastIndexed).ToListAsync();
        }

        public async Task<Guid> AddAsync(FileItem fileItem)
        {
            _context.Files.Add(fileItem);
            await _context.SaveChangesAsync();
            return fileItem.Id;
        }

        public async Task<bool> UpdateAsync(FileItem fileItem)
        {
            _context.Files.Update(fileItem);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var file = await _context.Files.FindAsync(id);
            if (file == null) return false;

            _context.Files.Remove(file);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeleteByPathAsync(string filePath)
        {
            var file = await GetByPathAsync(filePath);
            if (file == null) return false;

            return await DeleteAsync(file.Id);
        }

        public async Task<int> DeleteMissingFilesAsync()
        {
            var files = await _context.Files.ToListAsync();
            var missingFiles = files.Where(f => !File.Exists(f.FilePath)).ToList();

            _context.Files.RemoveRange(missingFiles);
            await _context.SaveChangesAsync();

            return missingFiles.Count;
        }

        public async Task<List<FileItem>> SearchByKeywordsAsync(List<string> keywords)
        {
            // This is a simplified implementation - in production you'd use full-text search
            var query = _context.Files.AsQueryable();

            foreach (var keyword in keywords)
            {
                var lowerKeyword = keyword.ToLower();
                query = query.Where(f =>
                    f.FileName.ToLower().Contains(lowerKeyword) ||
                    f.ExtractedText.ToLower().Contains(lowerKeyword) ||
                    f.Keywords.Any(k => k.ToLower().Contains(lowerKeyword)));
            }

            return await query.ToListAsync();
        }

        public async Task<List<FileItem>> SearchByTextAsync(string text)
        {
            var lowerText = text.ToLower();
            return await _context.Files
                .Where(f => f.ExtractedText.ToLower().Contains(lowerText) ||
                           f.FileName.ToLower().Contains(lowerText))
                .ToListAsync();
        }

        public async Task<List<FileItem>> GetSimilarFilesAsync(float[] embedding, float threshold = 0.7f, int maxResults = 10)
        {
            // Note: This is a simplified implementation
            // In production, you'd use vector database or more efficient similarity search
            var filesWithEmbeddings = await _context.Files
                .Where(f => f.TextEmbedding != null)
                .ToListAsync();

            var similarFiles = new List<(FileItem file, float similarity)>();

            foreach (var file in filesWithEmbeddings)
            {
                if (file.TextEmbedding != null)
                {
                    var similarity = CalculateCosineSimilarity(embedding, file.TextEmbedding);
                    if (similarity >= threshold)
                    {
                        similarFiles.Add((file, similarity));
                    }
                }
            }

            return similarFiles
                .OrderByDescending(x => x.similarity)
                .Take(maxResults)
                .Select(x => x.file)
                .ToList();
        }

        private float CalculateCosineSimilarity(float[] a, float[] b)
        {
            if (a.Length != b.Length) return 0f;

            float dotProduct = 0f;
            float normA = 0f;
            float normB = 0f;

            for (int i = 0; i < a.Length; i++)
            {
                dotProduct += a[i] * b[i];
                normA += a[i] * a[i];
                normB += b[i] * b[i];
            }

            if (normA == 0f || normB == 0f) return 0f;

            return dotProduct / (MathF.Sqrt(normA) * MathF.Sqrt(normB));
        }
    }
}
