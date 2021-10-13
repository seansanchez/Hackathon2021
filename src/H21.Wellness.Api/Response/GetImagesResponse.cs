using System.Collections.Generic;
using H21.Wellness.Models;

namespace H21.Wellness.Api.Response
{
    public class GetImagesResponse
    {
        public IEnumerable<ScavengerHuntItemModel> Images { get; set; }
    }
}