using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartContentIndexer.Core.Enums
{
    public enum FileType
    {
        Unknown,
        Text,
        Document,      // PDF, Word, PowerPoint
        Image,         // JPG, PNG, etc.
        Video,         // MP4, AVI, etc.
        Audio,         // MP3, WAV, etc.
        Archive,       // ZIP, RAR, etc.
        Code,          // CS, JS, PY, etc.
        Spreadsheet,   // Excel, CSV
        Email          // MSG, EML
    }
}
