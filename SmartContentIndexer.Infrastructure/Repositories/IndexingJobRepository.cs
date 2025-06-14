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
    public class IndexingJobRepository : IIndexingJobRepository
    {
        private readonly SmartContentDbContext _context;

        public IndexingJobRepository(SmartContentDbContext context)
        {
            _context = context;
        }

        public async Task<IndexingJob?> GetByIdAsync(Guid id)
        {
            return await _context.IndexingJobs.FindAsync(id);
        }

        public async Task<List<IndexingJob>> GetAllAsync()
        {
            return await _context.IndexingJobs.OrderByDescending(j => j.Started).ToListAsync();
        }

        public async Task<List<IndexingJob>> GetActiveJobsAsync()
        {
            return await _context.IndexingJobs
                .Where(j => j.Status == JobStatus.Queued || j.Status == JobStatus.Running)
                .ToListAsync();
        }

        public async Task<List<IndexingJob>> GetCompletedJobsAsync(int limit = 50)
        {
            return await _context.IndexingJobs
                .Where(j => j.Status == JobStatus.Completed || j.Status == JobStatus.Failed)
                .OrderByDescending(j => j.Started)
                .Take(limit)
                .ToListAsync();
        }

        public async Task<Guid> AddAsync(IndexingJob job)
        {
            _context.IndexingJobs.Add(job);
            await _context.SaveChangesAsync();
            return job.Id;
        }

        public async Task<bool> UpdateAsync(IndexingJob job)
        {
            _context.IndexingJobs.Update(job);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var job = await _context.IndexingJobs.FindAsync(id);
            if (job == null) return false;

            _context.IndexingJobs.Remove(job);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<int> DeleteOldJobsAsync(DateTime olderThan)
        {
            var oldJobs = await _context.IndexingJobs
                .Where(j => j.Started < olderThan &&
                           (j.Status == JobStatus.Completed || j.Status == JobStatus.Failed))
                .ToListAsync();

            _context.IndexingJobs.RemoveRange(oldJobs);
            await _context.SaveChangesAsync();

            return oldJobs.Count;
        }
    }
}
