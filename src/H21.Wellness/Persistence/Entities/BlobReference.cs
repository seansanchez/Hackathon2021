namespace H21.Wellness.Persistence.Entities
{
    /// <summary>
    ///     Represents the values required to locate a blob.
    /// </summary>
    public class BlobReference
    {
        /// <summary>
        ///     Gets or sets the blob container name.
        /// </summary>
        public string BlobContainerName { get; set; }

        /// <summary>
        ///     Gets or sets the blob name.
        /// </summary>
        public string BlobName { get; set; }
    }
}