using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace GlitchedPolygons.Services.CompressionUtility
{
    /// <summary>
    /// Useful utility interface for quickly and easily compress/decompress data using async/await.
    /// </summary>
    public interface ICompressionUtilityAsync
    {
        /// <summary>
        /// Compresses the specified bytes using the provided <see cref="CompressionSettings"/>.
        /// </summary>
        /// <returns>The compressed <c>byte[]</c> array.</returns>
        /// <param name="bytes">The <c>byte[]</c> array to compress.</param>
        /// <param name="compressionSettings">The desired compression settings.</param>
        Task<byte[]> Compress(byte[] bytes, CompressionSettings compressionSettings);

        /// <summary>
        /// Compresses the specified <c>string</c>.
        /// </summary>
        /// <param name="text">The <c>string</c> to compress.</param>
        /// <param name="encoding">The encoding to use. Can be <c>null</c>; UTF8 will be used in that case.</param>
        /// <returns>The compressed <c>string</c>.</returns>
        Task<string> Compress(string text, Encoding encoding = null);

        /// <summary>
        /// Decompresses the specified bytes using the
        /// <see cref="CompressionSettings"/> that have been used to originally compress the bytes..
        /// </summary>
        /// <param name="compressedBytes">The compressed <c>byte[]</c> array that you want to decompress.</param>
        /// <param name="compressionSettings">The <see cref="CompressionSettings"/> that have been used to compress the bytes.</param>
        /// <returns>The decompressed <c>bytes[]</c>.</returns>
        /// <exception cref="InvalidDataException">Thrown when the passed bytes are invalid in any way (e.g. corrupt/incomplete data, wrong format, etc...).</exception>
        Task<byte[]> Decompress(byte[] compressedBytes, CompressionSettings compressionSettings);

        /// <summary>
        /// Decompresses the specified compressed <c>string</c>.
        /// </summary>
        /// <param name="compressedString">The compressed <c>string</c> to decompress.</param>
        /// <param name="encoding">The encoding to use. Can be <c>null</c>; UTF8 will be used in that case.</param>
        /// <returns>The decompressed <c>string</c></returns>.
        Task<string> Decompress(string compressedString, Encoding encoding = null);
    }
}