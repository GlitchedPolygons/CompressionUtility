using System;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Threading.Tasks;

namespace GlitchedPolygons.Services.CompressionUtility
{
    /// <summary>
    /// Brotli implementation of the <see cref="ICompressionUtilityAsync"/> interface.<para> </para>
    /// Implements the <see cref="GlitchedPolygons.Services.CompressionUtility.ICompressionUtilityAsync" /> interface.
    /// <seealso cref="GlitchedPolygons.Services.CompressionUtility.ICompressionUtilityAsync" />
    /// </summary>
    public class BrotliUtilityAsync : ICompressionUtilityAsync
    {
        /// <summary>
        /// Asynchronously compresses the specified bytes using <see cref="BrotliStream"/> and the provided <see cref="CompressionSettings"/>.
        /// </summary>
        /// <returns>The compressed <c>byte[]</c> array; <c>null</c> if the input data was <c>null</c>; an empty array if the input array was also empty.</returns>
        /// <param name="bytes">The <c>byte[]</c> array to compress.</param>
        /// <param name="compressionSettings">The desired compression settings.</param>
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
            await using BrotliStream compressionStream = new BrotliStream(output, compressionSettings.compressionLevel);
            
            await input.CopyToAsync(compressionStream, Math.Max(4096, compressionSettings.bufferSize)).ConfigureAwait(false);
            await compressionStream.FlushAsync().ConfigureAwait(false);
            
            return output.ToArray();
        }

        /// <summary>
        /// Asynchronously compresses the specified <c>string</c> using Brotli.
        /// </summary>
        /// <param name="text">The <c>string</c> to compress.</param>
        /// <param name="encoding">The encoding to use. Can be <c>null</c>; UTF8 will be used in that case.</param>
        /// <returns>The compressed <c>string</c>.</returns>
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
        /// Asynchronously decompresses the specified bytes with Brotli using the
        /// <see cref="CompressionSettings"/> that have been used to originally compress the bytes.
        /// </summary>
        /// <param name="compressedBytes">The compressed <c>byte[]</c> array that you want to decompress.</param>
        /// <param name="compressionSettings">The <see cref="CompressionSettings"/> that have been used to compress the bytes.</param>
        /// <returns>The decompressed <c>bytes[]</c>.</returns>
        public async Task<byte[]> Decompress(byte[] compressedBytes, CompressionSettings compressionSettings)
        {
            if (ReferenceEquals(compressedBytes, null))
            {
                return null;
            }

            if (compressedBytes.Length == 0)
            {
                return Array.Empty<byte>();
            }

            await using MemoryStream input = new MemoryStream(compressedBytes);
            await using MemoryStream output = new MemoryStream(compressedBytes.Length / 2 * 3);
            await using BrotliStream decompressionStream = new BrotliStream(input, CompressionMode.Decompress);
            
            await decompressionStream.CopyToAsync(output, Math.Max(4096, compressionSettings.bufferSize)).ConfigureAwait(false);
            await decompressionStream.FlushAsync().ConfigureAwait(false);
            
            return output.ToArray();
        }

        /// <summary>
        /// Asynchronously decompresses the specified compressed <c>string</c> using Brotli.
        /// </summary>
        /// <param name="compressedString">The compressed <c>string</c> to decompress.</param>
        /// <param name="encoding">The encoding to use. Can be <c>null</c>; UTF8 will be used in that case.</param>
        /// <returns>The decompressed <c>string</c>.</returns>.
        public async Task<string> Decompress(string compressedString, Encoding encoding = null)
        {
            byte[] decompressedBytes = await Decompress(Convert.FromBase64String(compressedString), CompressionSettings.Default).ConfigureAwait(false);
            return (encoding ?? Encoding.UTF8).GetString(decompressedBytes);
        }
    }
}
