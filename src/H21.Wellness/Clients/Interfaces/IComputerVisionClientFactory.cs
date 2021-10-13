using System.IO;
using System.Threading.Tasks;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;

namespace H21.Wellness.Clients
{
    public interface IComputerVisionClientFactory
    {
        Task<TagResult> GenerateTagsForUrlAsync(string imageUrl);

        Task<TagResult> GenerateTagsForStreamAsync(Stream imageStream);
    }
}
