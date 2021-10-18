using System;

namespace H21.Wellness.Api.Request
{
    public class PostScavengerHuntScoreRequest
    {
        public Guid Id { get; set; }

        public uint CompleteCount { get; set; }

        public uint CompletedTimeInSeconds { get; set; }
    }
}