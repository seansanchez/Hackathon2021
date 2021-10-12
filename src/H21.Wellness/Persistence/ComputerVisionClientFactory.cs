using H21.Wellness.Persistence.Interfaces;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace H21.Wellness.Persistence
{
    public class ComputerVisionClientFactory : IComputerVisionClientFactory
    {   /// <summary>
        /// Sends a URL to Cognitive Services and generates tags for it.
        /// </summary>
        /// <param name="imageUrl">The URL of the image for which to generate tags.</param>
        /// <returns>Awaitable tagging result.</returns>
        private async Task<TagResult> GenerateTagsForUrlAsync(string imageUrl)
        {
            // -----------------------------------------------------------------------
            // KEY SAMPLE CODE STARTS HERE
            // -----------------------------------------------------------------------

            //
            // Create Cognitive Services Vision API Service client.
            //
            using (var client = new ComputerVisionClient(Credentials) { Endpoint = Endpoint })
            {
                
                TagResult analysisResult = await client.TagImageAsync(imageUrl, "EN");
                return analysisResult;
            }

            // -----------------------------------------------------------------------
            // KEY SAMPLE CODE ENDS HERE
            // -----------------------------------------------------------------------
        }
    }
}
