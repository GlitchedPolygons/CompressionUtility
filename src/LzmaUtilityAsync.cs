using System;
using System.Text;
using System.Threading.Tasks;

namespace GlitchedPolygons.Services.CompressionUtility
{
    /// <summary>
    /// Compression utility class for LZMAing data using async/await. Uses <c>Task.Run</c> to run the synchronous variant:
    /// compression is usually a high-latency, CPU bound op; that's why this deserves a dedicated thread via <c>Task.Run</c>
    /// Implements the <see cref="GlitchedPolygons.Services.CompressionUtility.ICompressionUtilityAsync" /> interface.
    /// </summary>
    /// <seealso cref="GlitchedPolygons.Services.CompressionUtility.ICompressionUtilityAsync" />
    public class LzmaUtilityAsync : ICompressionUtilityAsync
    {
        /// <summary>
        /// Synchronous LZMA utility instance.
        /// </summary>
        static readonly LzmaUtility lzma = new LzmaUtility();

        /// <summary>
        /// The default <see cref="Encoding"/> to use for compressing/decompressing strings.
        /// </summary>
        static readonly Encoding DEFAULT_ENCODING = Encoding.UTF8;

        /// <summary>
        /// Default <see cref="CompressionSettings"/> to use for compressing/decompressing strings.
        /// </summary>
        static readonly CompressionSettings DEFAULT_COMPRESSION_SETTINGS = new CompressionSettings();

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
        /// Compresses the specified <c>string</c> using LZMA and default <see cref="CompressionSettings"/>.
        /// </summary>
        /// <param name="text">The <c>string</c> to compress.</param>
        /// <returns>The compressed <c>string</c>.</returns>
        public Task<string> Compress(string text)
        {
            return Task.Run(async () => Convert.ToBase64String(await Compress(DEFAULT_ENCODING.GetBytes(text), DEFAULT_COMPRESSION_SETTINGS)));
        }

        /// <summary>
        /// Decompresses the specified bytes using LZMA and the
        /// <see cref="CompressionSettings"/> that have been used to originally compress the bytes..
        /// </summary>
        /// <param name="compressedBytes">The compressed <c>byte[]</c> array that you want to decompress.</param>
        /// <param name="compressionSettings">The <see cref="CompressionSettings"/> that have been used to compress the bytes.</param>
        /// <returns>The decompressed <c>bytes[]</c>; <c>null</c> if decompression failed.</returns>
        public Task<byte[]> Decompress(byte[] compressedBytes, CompressionSettings compressionSettings)
        {
            return Task.Run(() => lzma.Decompress(compressedBytes, compressionSettings));
        }

        /// <summary>
        /// Decompresses the specified compressed <c>string</c> using the default <see cref="CompressionSettings"/>.
        /// </summary>
        /// <param name="compressedString">The compressed <c>string</c> to decompress.</param>
        /// <returns>The decompressed <c>string</c></returns>.
        public Task<string> Decompress(string compressedString)
        {
            return Task.Run(async () => DEFAULT_ENCODING.GetString(await Decompress(Convert.FromBase64String(compressedString), DEFAULT_COMPRESSION_SETTINGS)));
        }
    }
}
