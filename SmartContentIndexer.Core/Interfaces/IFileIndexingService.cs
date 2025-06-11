using SmartContentIndexer.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartContentIndexer.Core.Interfaces
{
    public interface IFileIndexingService
    {
        Task<IndexingJob> StartIndexingAsync(string directoryPath, bool includeSubdirectories = true);
        Task<IndexingJob> StartIndexingAsync(List<string> filePaths);
        Task<IndexingJob> GetJobStatusAsync(Guid jobId);
        Task<bool> CancelJobAsync(Guid jobId);
        Task<FileItem> IndexSingleFileAsync(string filePath);
        Task<bool> ReindexFileAsync(string filePath);
        Task<List<IndexingJob>> GetActiveJobsAsync();

        // Events
        event Action<IndexingJob> JobStarted;
        event Action<IndexingJob> JobCompleted;
        event Action<FileItem> FileIndexed;
        event Action<IndexingJob, Exception> JobFailed;
    }
}
