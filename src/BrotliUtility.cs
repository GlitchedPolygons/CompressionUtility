using System;
using System.IO;
using System.IO.Compression;
using System.Text;

namespace GlitchedPolygons.Services.CompressionUtility
{
    /// <summary>
    /// Brotli implementation of the <see cref="ICompressionUtility"/> interface.<para> </para>
    /// Implements the <see cref="GlitchedPolygons.Services.CompressionUtility.ICompressionUtility" /> interface.
    /// <seealso cref="GlitchedPolygons.Services.CompressionUtility.ICompressionUtility" />
    /// </summary>
    public class BrotliUtility : ICompressionUtility
    {
        /// <summary>
        /// Compresses the specified bytes using <see cref="BrotliStream"/> and the provided <see cref="CompressionSettings"/>.
        /// </summary>
        /// <returns>The compressed <c>byte[]</c> array; <c>null</c> if the input data was <c>null</c>; an empty array if the input array was also empty.</returns>
        /// <param name="bytes">The <c>byte[]</c> array to compress.</param>
        /// <param name="compressionSettings">The desired compression settings.</param>
        public byte[] Compress(byte[] bytes, CompressionSettings compressionSettings)
        {
            if (ReferenceEquals(bytes, null))
            {
                return null;
            }

            if (bytes.Length == 0)
            {
                return Array.Empty<byte>();
            }

            using MemoryStream input = new MemoryStream(bytes);
            using MemoryStream output = new MemoryStream(bytes.Length / 4 * 3);
            using BrotliStream compressionStream = new BrotliStream(output, compressionSettings.compressionLevel);
            
            input.CopyTo(compressionStream, Math.Max(4096, compressionSettings.bufferSize));
            compressionStream.Flush();
            
            return output.ToArray();
        }

        /// <summary>
        /// Compresses the specified <c>string</c> using <see cref="BrotliStream"/>.
        /// </summary>
        /// <param name="text">The <c>string</c> to compress.</param>
        /// <param name="encoding">The encoding to use. Can be <c>null</c>; UTF8 will be used in that case.</param>
        /// <returns>The compressed <c>string</c>.</returns>
        public string Compress(string text, Encoding encoding = null)
        {
            if (string.IsNullOrEmpty(text))
            {
                return null;
            }
            
            byte[] compressedBytes = Compress((encoding ?? Encoding.UTF8).GetBytes(text), CompressionSettings.Default);
            return Convert.ToBase64String(compressedBytes);
        }

        /// <summary>
        /// Decompresses the specified bytes using the
        /// <see cref="CompressionSettings"/> that have been used to originally compress the bytes.
        /// </summary>
        /// <param name="compressedBytes">The compressed <c>byte[]</c> array that you want to decompress.</param>
        /// <param name="compressionSettings">The <see cref="CompressionSettings"/> that have been used to compress the bytes.</param>
        /// <returns>The decompressed <c>bytes[]</c> - or <c>null</c> if the input array was also <c>null</c>; an empty array if the passed data was also empty.</returns>
        public byte[] Decompress(byte[] compressedBytes, CompressionSettings compressionSettings)
        {
            if (ReferenceEquals(compressedBytes, null))
            {
                return null;
            }

            if (compressedBytes.Length == 0)
            {
                return Array.Empty<byte>();
            }

            using MemoryStream input = new MemoryStream(compressedBytes);
            using MemoryStream output = new MemoryStream(compressedBytes.Length / 2 * 3);
            using BrotliStream decompressionStream = new BrotliStream(input, CompressionMode.Decompress);
            
            decompressionStream.CopyTo(output, Math.Max(4096, compressionSettings.bufferSize));
            decompressionStream.Flush();
            
            return output.ToArray();
        }

        /// <summary>
        /// Decompresses the specified compressed <c>string</c> with Brotli.
        /// </summary>
        /// <param name="compressedString">The compressed <c>string</c> to "debrotlify".</param>
        /// <param name="encoding">The encoding to use. Can be <c>null</c>; UTF8 will be used in that case.</param>
        /// <returns>The decompressed <c>string</c>.</returns>.
        public string Decompress(string compressedString, Encoding encoding = null)
        {
            byte[] decompressedBytes = Decompress(Convert.FromBase64String(compressedString), CompressionSettings.Default);
            return (encoding ?? Encoding.UTF8).GetString(decompressedBytes);
        }
    }
}
