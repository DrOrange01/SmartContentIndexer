using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartContentIndexer.Core.Enums
{
    public enum JobStatus
    {
        Queued,
        Running,
        Completed,
        Failed,
        Cancelled
    }
}
