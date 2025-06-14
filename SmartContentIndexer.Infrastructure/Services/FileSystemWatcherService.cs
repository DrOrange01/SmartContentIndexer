using SmartContentIndexer.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartContentIndexer.Infrastructure.Services
{
    public class FileSystemWatcherService : IFileSystemWatcher, IDisposable
    {
        private readonly Dictionary<string, System.IO.FileSystemWatcher> _watchers = new();
        private readonly object _lock = new object();

        public event Action<string>? FileCreated;
        public event Action<string>? FileModified;
        public event Action<string>? FileDeleted;
        public event Action<string, string>? FileRenamed;

        public async Task StartWatchingAsync(string directoryPath, bool includeSubdirectories = true)
        {
            if (!Directory.Exists(directoryPath))
                throw new DirectoryNotFoundException($"Directory not found: {directoryPath}");

            lock (_lock)
            {
                if (_watchers.ContainsKey(directoryPath))
                    return; // Already watching

                var watcher = new System.IO.FileSystemWatcher(directoryPath)
                {
                    IncludeSubdirectories = includeSubdirectories,
                    EnableRaisingEvents = true,
                    NotifyFilter = NotifyFilters.CreationTime | NotifyFilters.LastWrite | NotifyFilters.FileName
                };

                watcher.Created += (sender, e) => FileCreated?.Invoke(e.FullPath);
                watcher.Changed += (sender, e) => FileModified?.Invoke(e.FullPath);
                watcher.Deleted += (sender, e) => FileDeleted?.Invoke(e.FullPath);
                watcher.Renamed += (sender, e) => FileRenamed?.Invoke(e.OldFullPath, e.FullPath);

                _watchers[directoryPath] = watcher;
            }
        }

        public async Task StopWatchingAsync(string directoryPath)
        {
            lock (_lock)
            {
                if (_watchers.TryGetValue(directoryPath, out var watcher))
                {
                    watcher.Dispose();
                    _watchers.Remove(directoryPath);
                }
            }
        }

        public async Task StopAllWatchingAsync()
        {
            lock (_lock)
            {
                foreach (var watcher in _watchers.Values)
                {
                    watcher.Dispose();
                }
                _watchers.Clear();
            }
        }

        public List<string> GetWatchedDirectories()
        {
            lock (_lock)
            {
                return _watchers.Keys.ToList();
            }
        }

        public void Dispose()
        {
            StopAllWatchingAsync().Wait();
        }
    }
}
