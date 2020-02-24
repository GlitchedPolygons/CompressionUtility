using Brotli;
using System;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Threading.Tasks;

#if UNITY_EDITOR
using UnityEngine;
#endif

namespace GlitchedPolygons.Services.CompressionUtility
{
    /// <summary>
    /// Brotli implementation of the <see cref="ICompressionUtility"/> interface.<para> </para>
    /// Implements the <see cref="GlitchedPolygons.Services.CompressionUtility.ICompressionUtility" /> interface.
    /// <seealso cref="GlitchedPolygons.Services.CompressionUtility.ICompressionUtility" />
    /// </summary>
    public class BrotliUtilityAsync : ICompressionUtilityAsync
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
        /// Empty <c>byte[]</c> array for handling certain edge cases/failures
        /// (e.g. compressing an empty array will result in an empty array).
        /// </summary>
        static readonly byte[] EMPTY_BYTE_ARRAY = new byte[0];

        /// <summary>
        /// Compresses an array of bytes using Brotli.NET
        /// </summary>
        /// <param name="bytes">The bytes to compress.</param>
        /// <param name="compressionSettings">The <see cref="CompressionLevel"/> IS IGNORED, only the <see cref="CompressionSettings.BufferSize"/> is used!</param>
        /// <returns>The compressed bytes.</returns>
        public async Task<byte[]> Compress(byte[] bytes, CompressionSettings compressionSettings)
        {
            if (bytes == null)
            {
#if UNITY_EDITOR
                Debug.LogError($"{nameof(BrotliUtilityAsync)}: You tried to compress a null array; returning null...");
#endif
                return null;
            }

            if (bytes.Length == 0)
            {
#if UNITY_EDITOR
                Debug.LogWarning($"{nameof(BrotliUtilityAsync)}: You tried to compress an empty array; the resulting array will also be empty!");
#endif
                return EMPTY_BYTE_ARRAY;
            }

            BrotliStream brotliStream = null;
            var outputStream = new MemoryStream();
            var inputStream = new MemoryStream(bytes);
            
            try
            {
                brotliStream = new BrotliStream(outputStream, CompressionMode.Compress);
                brotliStream.SetQuality(11);
                brotliStream.SetWindow(22);
                await inputStream.CopyToAsync(brotliStream, compressionSettings?.BufferSize ?? DEFAULT_COMPRESSION_SETTINGS.BufferSize).ConfigureAwait(false);
                brotliStream.Close();
                return outputStream.ToArray();
            }
            catch (Exception e)
            {
#if UNITY_EDITOR
                    Debug.LogError($"{nameof(BrotliUtilityAsync)}: Compression failure; thrown error message: " + e.Message);
#endif
                throw e;
            }
            finally
            {
                inputStream.Dispose();
                outputStream.Dispose();
                brotliStream?.Dispose();
            }
        }

        /// <summary>
        /// Compresses the specified <c>string</c> using <see cref="BrotliStream"/> and default <see cref="CompressionSettings"/>.
        /// </summary>
        /// <param name="text">The <c>string</c> to compress.</param>
        /// <returns>The gzipped <c>string</c>.</returns>
        public async Task<string> Compress(string text)
        {
            return Convert.ToBase64String(await Compress(DEFAULT_ENCODING.GetBytes(text), DEFAULT_COMPRESSION_SETTINGS).ConfigureAwait(false));
        }

        /// <summary>
        /// Decompresses the specified bytes using a <see cref="BrotliStream"/>.
        /// </summary>
        /// <param name="compressedBytes">The "brotlified" <c>byte[]</c> array that you want to decompress (the returned value from <see cref="Compress(byte[], CompressionSettings)"/>).</param>
        /// <param name="compressionSettings">The <see cref="CompressionSettings"/> that have been used to compress the bytes.</param>
        /// <returns>The decompressed <c>bytes[]</c>.</returns>
        public async Task<byte[]> Decompress(byte[] compressedBytes, CompressionSettings compressionSettings)
        {
            if (ReferenceEquals(compressedBytes, null))
            {
#if UNITY_EDITOR
                Debug.LogError($"{nameof(BrotliUtilityAsync)}: You tried to decompress a null array; returning null...");
#endif
                return null;
            }

            if (compressedBytes.Length == 0)
            {
#if UNITY_EDITOR
                Debug.LogWarning($"{nameof(BrotliUtilityAsync)}: You tried to decompress an empty array; the resulting array will also be empty!");
#endif
                return EMPTY_BYTE_ARRAY;
            }

            BrotliStream brotliStream = null;
            var outputStream = new MemoryStream();
            var inputStream = new MemoryStream(compressedBytes);
            
            try
            {
                brotliStream = new BrotliStream(inputStream, CompressionMode.Decompress);
                await brotliStream.CopyToAsync(outputStream, compressionSettings?.BufferSize ?? DEFAULT_COMPRESSION_SETTINGS.BufferSize).ConfigureAwait(false);
                outputStream.Seek(0, SeekOrigin.Begin);
                return outputStream.ToArray();
            }
            catch (BrotliDecodeException e)
            {
                string msg = $"{nameof(BrotliUtilityAsync)}: Decompression failure due to invalid data stream (e.g. corrupt, wrong format?). Thrown exception message: {e.Message}";
#if UNITY_EDITOR
                Debug.LogError(msg);
#endif
                throw new InvalidDataException(msg);
            }
            finally
            {
                inputStream.Dispose();
                outputStream.Dispose();
                brotliStream?.Dispose();
            }
        }

        /// <summary>
        /// Decompresses the specified brotli <c>string</c> using the default <see cref="CompressionSettings"/>.
        /// </summary>
        /// <param name="compressedString">The compressed <c>string</c> to debrotlify.</param>
        /// <returns>The decompressed <c>string</c></returns>.
        public async Task<string> Decompress(string compressedString)
        {
            return DEFAULT_ENCODING.GetString(await Decompress(Convert.FromBase64String(compressedString), DEFAULT_COMPRESSION_SETTINGS).ConfigureAwait(false));
        }
    }
}
