using SmartContentIndexer.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartContentIndexer.Core.Interfaces
{
    public interface IIndexingJobRepository
    {
        Task<IndexingJob?> GetByIdAsync(Guid id);
        Task<List<IndexingJob>> GetAllAsync();
        Task<List<IndexingJob>> GetActiveJobsAsync();
        Task<List<IndexingJob>> GetCompletedJobsAsync(int limit = 50);

        Task<Guid> AddAsync(IndexingJob job);
        Task<bool> UpdateAsync(IndexingJob job);
        Task<bool> DeleteAsync(Guid id);
        Task<int> DeleteOldJobsAsync(DateTime olderThan);
    }
}
