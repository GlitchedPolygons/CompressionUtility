using System;
using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;
using System.Text;

namespace GlitchedPolygons.Services.CompressionUtility
{
    /// <summary>
    /// Compression utility class for gzipping data asynchronously.
    /// Implements the <see cref="GlitchedPolygons.Services.CompressionUtility.ICompressionUtility" /> interface.
    /// </summary>
    /// <seealso cref="GlitchedPolygons.Services.CompressionUtility.ICompressionUtility" />
    public class GZipUtilityAsync : ICompressionUtilityAsync
    {
        /// <summary>
        /// Compresses the specified bytes using <see cref="GZipStream"/> and the provided <see cref="CompressionSettings"/>.
        /// </summary>
        /// <returns>The gzipped <c>byte[]</c> array.</returns>
        /// <param name="bytes">The <c>byte[]</c> array to compress.</param>
        /// <param name="compressionSettings">The desired <see cref="CompressionSettings"/>.</param>
        public async Task<byte[]> Compress(byte[] bytes, CompressionSettings compressionSettings)
        {
            if (ReferenceEquals(bytes, null))
            {
                return null;
            }

            if (bytes.Length == 0)
            {
                return Array.Empty<byte>();
            }

            await using MemoryStream input = new MemoryStream(bytes);
            await using MemoryStream output = new MemoryStream(bytes.Length / 4 * 3);
            await using GZipStream compressionStream = new GZipStream(output, compressionSettings.compressionLevel);
            
            await input.CopyToAsync(compressionStream, Math.Max(4096, compressionSettings.bufferSize)).ConfigureAwait(false);
            await compressionStream.FlushAsync().ConfigureAwait(false);
            
            return output.ToArray();
        }

        /// <summary>
        /// Compresses ("gee-zips") the specified <c>string</c> using <see cref="GZipStream"/> and default <see cref="CompressionSettings"/>.
        /// </summary>
        /// <param name="text">The <c>string</c> to compress.</param>
        /// <param name="encoding">The encoding to use. Can be <c>null</c>; UTF8 will be used in that case.</param>
        /// <returns>The gzipped <c>string</c>.</returns>
        public async Task<string> Compress(string text, Encoding encoding = null)
        {
            if (string.IsNullOrEmpty(text))
            {
                return null;
            }
            byte[] compressedBytes = await Compress((encoding ?? Encoding.UTF8).GetBytes(text), CompressionSettings.Default).ConfigureAwait(false);
            return Convert.ToBase64String(compressedBytes);
        }

        /// <summary>
        /// Decompresses the specified bytes using <see cref="GZipStream"/> and the
        /// <see cref="CompressionSettings"/> that have been used to originally compress the bytes.
        /// </summary>
        /// <param name="gzippedBytes">The gzipped <c>byte[]</c> array that you want to decompress.</param>
        /// <param name="compressionSettings">The <see cref="CompressionSettings"/> that have been used to compress the bytes.</param>
        /// <returns>The decompressed <c>bytes[]</c>.</returns>
        public async Task<byte[]> Decompress(byte[] gzippedBytes, CompressionSettings compressionSettings)
        {
            if (ReferenceEquals(gzippedBytes, null))
            {
                return null;
            }

            if (gzippedBytes.Length == 0)
            {
                return Array.Empty<byte>();
            }

            await using MemoryStream input = new MemoryStream(gzippedBytes);
            await using MemoryStream output = new MemoryStream(gzippedBytes.Length / 2 * 3);
            await using GZipStream decompressionStream = new GZipStream(input, CompressionMode.Decompress);
            
            await decompressionStream.CopyToAsync(output, Math.Max(4096, compressionSettings.bufferSize)).ConfigureAwait(false);
            await decompressionStream.FlushAsync().ConfigureAwait(false);
            
            return output.ToArray();
        }

        /// <summary>
        /// Decompresses the specified gzipped <c>string</c> using the default <see cref="CompressionSettings"/>.
        /// </summary>
        /// <param name="gzippedString">The compressed <c>string</c> to decompress.</param>
        /// <param name="encoding">The encoding to use. Can be <c>null</c>; UTF8 will be used in that case.</param>
        /// <returns>The decompressed <c>string</c></returns>.
        public async Task<string> Decompress(string gzippedString, Encoding encoding = null)
        {
            byte[] decompressedBytes = await Decompress(Convert.FromBase64String(gzippedString), CompressionSettings.Default).ConfigureAwait(false);
            return (encoding ?? Encoding.UTF8).GetString(decompressedBytes);
        }
    }
}
