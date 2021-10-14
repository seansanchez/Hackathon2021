namespace H21.Wellness.Api.Response
{
    public class DetectedTagResponse
    {
        /// <summary>
        /// Gets or sets name of the entity.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the level of confidence that the entity was observed.
        /// </summary>
        public double Confidence { get; set; }

        /// <summary>
        /// Gets or sets optional hint/details for this tag.
        /// </summary>
        public string Hint { get; set; }
    }
}
