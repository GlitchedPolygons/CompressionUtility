﻿using System;
using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;
using System.Text;

#if UNITY_EDITOR
using UnityEngine;
#endif

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
#if UNITY_EDITOR
                Debug.LogError($"{nameof(GZipUtilityAsync)}: You tried to compress a null array; returning null...");
#endif
                return null;
            }

            if (bytes.Length == 0)
            {
#if UNITY_EDITOR
                Debug.LogWarning($"{nameof(GZipUtilityAsync)}: You tried to compress an empty array; the resulting array will also be empty!");
#endif
                return Array.Empty<byte>();
            }

            byte[] compressedBytes;

            using (var compressedStream = new MemoryStream())
            {
                using (var originalStream = new MemoryStream(bytes))
                {
                    using (var gzip = new GZipStream(compressedStream, compressionSettings.compressionLevel))
                    {
                        await originalStream.CopyToAsync(gzip, Math.Max(4096, compressionSettings.bufferSize)).ConfigureAwait(false);
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
        /// <param name="encoding">The encoding to use. Can be <c>null</c>; UTF8 will be used in that case.</param>
        /// <returns>The gzipped <c>string</c>.</returns>
        public async Task<string> Compress(string text, Encoding encoding = null)
        {
            byte[] compressedBytes = await Compress((encoding ?? Encoding.UTF8).GetBytes(text), CompressionSettings.Default).ConfigureAwait(false);
            return Convert.ToBase64String(compressedBytes);
        }

        /// <summary>
        /// Decompresses the specified bytes using <see cref="GZipStream"/> and the
        /// <see cref="CompressionSettings"/> that have been used to originally compress the bytes..
        /// </summary>
        /// <param name="gzippedBytes">The gzipped <c>byte[]</c> array that you want to decompress.</param>
        /// <param name="compressionSettings">The <see cref="CompressionSettings"/> that have been used to compress the bytes.</param>
        /// <returns>The decompressed <c>bytes[]</c>.</returns>
        public async Task<byte[]> Decompress(byte[] gzippedBytes, CompressionSettings compressionSettings)
        {
            if (ReferenceEquals(gzippedBytes, null))
            {
#if UNITY_EDITOR
                Debug.LogError($"{nameof(GZipUtilityAsync)}: You tried to decompress a null array; returning null...");
#endif
                return null;
            }

            if (gzippedBytes.Length == 0)
            {
#if UNITY_EDITOR
                Debug.LogWarning($"{nameof(GZipUtilityAsync)}: You tried to decompress an empty array; the resulting array will also be empty!");
#endif
                return Array.Empty<byte>();
            }

            using (MemoryStream decompressedStream = new MemoryStream())
            {
                using (MemoryStream compressedStream = new MemoryStream(gzippedBytes))
                {
                    using (GZipStream gzip = new GZipStream(compressedStream, CompressionMode.Decompress))
                    {
                        await gzip.CopyToAsync(decompressedStream, Math.Max(4096, compressionSettings.bufferSize)).ConfigureAwait(false);
                    }
                }

                return decompressedStream.ToArray();
            }
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
