using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace H21.Wellness.Persistence.Interfaces
{
    public interface IComputerVisionClientFactory
    {
        Task<TagResult> GenerateTagsForUrlAsync(string imageUrl);
    }
}
