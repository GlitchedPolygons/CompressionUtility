using System;
using System.IO;
using System.Text;
using System.IO.Compression;

namespace GlitchedPolygons.Services.CompressionUtility
{
    public class GZipUtility : ICompressionUtility
    {
        /// <summary>
        /// The default <see cref="Encoding"/> to use for compressing/decompressing strings.
        /// </summary>
        static readonly Encoding DEFAULT_ENCODING = Encoding.UTF8;

        /// <summary>
        /// Default <see cref="CompressionSettings"/> to use for compressing/decompressing strings.
        /// </summary>
        static readonly CompressionSettings DEFAULT_COMPRESSION_SETTINGS = new CompressionSettings();

        /// <summary>
        /// Empty byte array for handling certain edge cases/failures
        /// (e.g. compressing an empty array will result in an empty array).
        /// </summary>
        static readonly byte[] EMPTY_BYTE_ARRAY = new byte[0];

        /// <summary>
        /// Compresses the specified bytes using <see cref="GZipStream"/> and the provided <see cref="CompressionSettings"/>.
        /// </summary>
        /// <returns>The gzipped <c>byte[]</c> array.</returns>
        /// <param name="bytes">The <c>byte[]</c> array to compress.</param>
        /// <param name="compressionSettings">The desired <see cref="CompressionSettings"/>.</param>
        public byte[] Compress(byte[] bytes, CompressionSettings compressionSettings)
        {
            if (ReferenceEquals(bytes, null))
            {
#if UNITY_EDITOR
                Debug.LogError($"{nameof(GZip)}: You tried to compress a null array; returning null...");
#endif
                return null;
            }

            if (bytes.Length == 0)
            {
#if UNITY_EDITOR
                Debug.LogWarning($"{nameof(GZip)}: You tried to compress an empty array; the resulting array will also be empty!");
#endif
                return EMPTY_BYTE_ARRAY;
            }

            byte[] compressedBytes;

            using (var compressedStream = new MemoryStream())
            {
                using (var originalStream = new MemoryStream(bytes))
                {
                    using (var gzip = new GZipStream(compressedStream, compressionSettings.CompressionLevel))
                    {
                        originalStream.CopyTo(gzip, compressionSettings.BufferSize);
                    }
                }
                compressedBytes = compressedStream.ToArray();
            }

            return compressedBytes;
        }

        /// <summary>
        /// Compresses ("gee-zips") the specified <c>string</c> using <see cref="GZipStream"/> and default <see cref="CompressionSettings"/>.
        /// </summary>
        /// <param name="text">The <c>string</c> to compress.</param>
        /// <returns>The gzipped <c>string</c>.</returns>
        public string Compress(string text)
        {
            return Convert.ToBase64String(Compress(DEFAULT_ENCODING.GetBytes(text), DEFAULT_COMPRESSION_SETTINGS));
        }

        /// <summary>
        /// Decompresses the specified bytes using <see cref="GZipStream"/> and the
        /// <see cref="CompressionSettings"/> that have been used to originally compress the bytes..
        /// </summary>
        /// <param name="gzippedBytes">The gzipped <c>byte[]</c> array that you want to decompress.</param>
        /// <param name="compressionSettings">The <see cref="CompressionSettings"/> that have been used to compress the bytes.</param>
        /// <returns>The decompressed <c>bytes[]</c>.</returns>
        public byte[] Decompress(byte[] gzippedBytes, CompressionSettings compressionSettings)
        {
            if (ReferenceEquals(gzippedBytes, null))
            {
#if UNITY_EDITOR
                Debug.LogError($"{nameof(GZip)}: You tried to decompress a null array; returning null...");
#endif
                return null;
            }

            if (gzippedBytes.Length == 0)
            {
#if UNITY_EDITOR
                Debug.LogWarning($"{nameof(GZip)}: You tried to decompress an empty array; the resulting array will also be empty!");
#endif
                return EMPTY_BYTE_ARRAY;
            }

            using (MemoryStream decompressedStream = new MemoryStream())
            {
                using (MemoryStream compressedStream = new MemoryStream(gzippedBytes))
                {
                    using (GZipStream gzip = new GZipStream(compressedStream, CompressionMode.Decompress))
                    {
                        gzip.CopyTo(decompressedStream, compressionSettings.BufferSize);
                    }
                }

                return decompressedStream.ToArray();
            }
        }

        /// <summary>
        /// Decompresses the specified gzipped <c>string</c> using the default <see cref="CompressionSettings"/>.
        /// </summary>
        /// <param name="gzippedString">The compressed <c>string</c> to decompress.</param>
        /// <returns>The decompressed <c>string</c></returns>.
        public string Decompress(string gzippedString)
        {
            return DEFAULT_ENCODING.GetString(Decompress(Convert.FromBase64String(gzippedString), DEFAULT_COMPRESSION_SETTINGS));
        }
    }
}
