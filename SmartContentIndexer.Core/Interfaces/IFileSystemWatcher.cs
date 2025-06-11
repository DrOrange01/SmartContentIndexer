using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartContentIndexer.Core.Interfaces
{
    public interface IFileSystemWatcher
    {
        Task StartWatchingAsync(string directoryPath, bool includeSubdirectories = true);
        Task StopWatchingAsync(string directoryPath);
        Task StopAllWatchingAsync();
        List<string> GetWatchedDirectories();

        // Events
        event Action<string> FileCreated;
        event Action<string> FileModified;
        event Action<string> FileDeleted;
        event Action<string, string> FileRenamed; // oldPath, newPath
    }
}
