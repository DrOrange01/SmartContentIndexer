using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartContentIndexer.Core.Interfaces
{
    public interface IImageAnalysisService
    {
        Task<List<string>> DetectObjectsAsync(string imagePath);
        Task<List<string>> ExtractTextFromImageAsync(string imagePath); // OCR
        Task<string> DescribeImageAsync(string imagePath);
        Task<List<string>> GetImageTagsAsync(string imagePath);
        Task<bool> IsImageSimilarAsync(string imagePath1, string imagePath2, float threshold = 0.8f);
        bool SupportsImageFormat(string extension);
    }
}
