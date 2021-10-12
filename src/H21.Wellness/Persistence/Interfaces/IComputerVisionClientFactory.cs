using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;
using System.IO;
using System.Threading.Tasks;

namespace H21.Wellness.Persistence.Interfaces
{
    public interface IComputerVisionClientFactory
    {
        Task<TagResult> GenerateTagsForUrlAsync(string imageUrl);

        Task<TagResult> GenerateTagsForStreamAsync(Stream imageStream);
    }
}
