using System;
using System.Text;
using System.Threading.Tasks;

#if UNITY_EDITOR
using UnityEngine;
#endif

namespace GlitchedPolygons.Services.CompressionUtility
{
    /// <summary>
    /// Compression utility class for LZMAing data using async/await. Uses <c>Task.Run</c> to call the synchronous LZMA SDK:
    /// Not the best thing to do, I know... But the LZMA SDK doesn't have an async API yet, and compression is usually a high-latency,
    /// CPU bound op; that's why this deserves a dedicated thread via <c>Task.Run</c>
    /// Implements the <see cref="GlitchedPolygons.Services.CompressionUtility.ICompressionUtilityAsync" /> interface.
    /// </summary>
    /// <seealso cref="GlitchedPolygons.Services.CompressionUtility.ICompressionUtilityAsync" />
    public class LzmaUtilityAsync : ICompressionUtilityAsync
    {
        /// <summary>
        /// Synchronous LZMA utility instance.
        /// </summary>
        private readonly LzmaUtility lzma = new LzmaUtility();

        /// <summary>
        /// Compresses the specified bytes using LZMA.
        /// </summary>
        /// <returns>The compressed <c>byte[]</c> array.</returns>
        /// <param name="bytes">The <c>byte[]</c> array to compress.</param>
        /// <param name="compressionSettings">The desired <see cref="CompressionSettings"/>.</param>
        public Task<byte[]> Compress(byte[] bytes, CompressionSettings compressionSettings)
        {
            return Task.Run(() => lzma.Compress(bytes, compressionSettings));
        }

        /// <summary>
        /// Compresses the specified <c>string</c> using LZMA and <c>CompressionSettings.Default</c>.
        /// </summary>
        /// <param name="text">The <c>string</c> to compress.</param>
        /// <param name="encoding">The encoding to use. Can be <c>null</c>; UTF8 will be used in that case.</param>
        /// <returns>The compressed <c>string</c>.</returns>
        public async Task<string> Compress(string text, Encoding encoding = null)
        {
            byte[] compressedBytes = await Compress((encoding ?? Encoding.UTF8).GetBytes(text), CompressionSettings.Default).ConfigureAwait(false);
            return Convert.ToBase64String(compressedBytes);
        }

        /// <summary>
        /// Decompresses the specified bytes using LZMA and the
        /// <see cref="CompressionSettings"/> that have been used to originally compress the bytes..
        /// </summary>
        /// <param name="compressedBytes">The compressed <c>byte[]</c> array that you want to decompress.</param>
        /// <param name="compressionSettings">The <see cref="CompressionSettings"/> that have been used to compress the bytes.</param>
        /// <returns>The decompressed <c>bytes[]</c>.</returns>
        public Task<byte[]> Decompress(byte[] compressedBytes, CompressionSettings compressionSettings)
        {
            return Task.Run(() => lzma.Decompress(compressedBytes, compressionSettings));
        }

        /// <summary>
        /// Decompresses the specified compressed <c>string</c> using LZMA and <c>CompressionSettings.Default</c>.
        /// </summary>
        /// <param name="compressedString">The compressed <c>string</c> to decompress.</param>
        /// <param name="encoding">The encoding to use. Can be <c>null</c>; UTF8 will be used in that case.</param>
        /// <returns>The decompressed <c>string</c></returns>.
        public async Task<string> Decompress(string compressedString, Encoding encoding = null)
        {
            byte[] decompressedBytes = await Decompress(Convert.FromBase64String(compressedString), CompressionSettings.Default).ConfigureAwait(false);
            return (encoding ?? Encoding.UTF8).GetString(decompressedBytes);
        }
    }
}
