using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartContentIndexer.Core.Enums
{
    public enum IndexStatus
    {
        Pending,
        Processing,
        Completed,
        Failed,
        Outdated      // File changed since last indexing
    }
}
