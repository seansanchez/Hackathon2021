using System;

namespace H21.Wellness.Api.Request
{
    public class PostValidateImageRequest
    {
        public Guid Id { get; set; }

        public string ImageDataUri { get; set; }
    }
}