using System.IO;
using System.Threading.Tasks;

namespace H21.Wellness.Extensions
{
    public static class StreamExtensions
    {
        public static async Task<byte[]> ToBytesAsync(this Stream source)
        {
            source.ThrowIfNull(nameof(source));

            await using var stream = new MemoryStream();
            await source.CopyToAsync(stream).ConfigureAwait(false);

            return stream.ToArray();
        }
    }
}