using System;
using System.IO;
using System.Text;
using System.IO.Compression;

using SevenZip;

#if UNITY_EDITOR
using UnityEngine;
#endif

namespace GlitchedPolygons.Services.CompressionUtility
{
    using Encoder = SevenZip.Compression.LZMA.Encoder;
    using Decoder = SevenZip.Compression.LZMA.Decoder;

    /// <summary>
    /// Compression utility class for LZMAing data.
    /// Implements the <see cref="GlitchedPolygons.Services.CompressionUtility.ICompressionUtility" /> interface.
    /// </summary>
    /// <seealso cref="GlitchedPolygons.Services.CompressionUtility.ICompressionUtility" />
    public class LzmaUtility : ICompressionUtility
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

        static CoderPropID[] propIDs =
        {
            CoderPropID.DictionarySize,
            CoderPropID.PosStateBits,
            CoderPropID.LitContextBits,
            CoderPropID.LitPosBits,
            CoderPropID.Algorithm,
            CoderPropID.NumFastBytes,
            CoderPropID.MatchFinder,
            CoderPropID.EndMarker
        };

        static object[] propertiesNormal =
        {
            (System.Int32)1 << 24, // 16MB
            (System.Int32)(2),
            (System.Int32)(3),
            (System.Int32)(0),
            (System.Int32)(2),
            (System.Int32)(32),
            "bt4",
            false
        };

        static object[] propertiesOptimal =
        {
            (System.Int32)1 << 25, // 32MB
            (System.Int32)(2),
            (System.Int32)(3),
            (System.Int32)(0),
            (System.Int32)(2),
            (System.Int32)(64),
            "bt4",
            false
        };

        /// <summary>
        /// Compresses the specified bytes using LZMA.
        /// </summary>
        /// <returns>The compressed <c>byte[]</c> array.</returns>
        /// <param name="bytes">The <c>byte[]</c> array to compress.</param>
        /// <param name="compressionSettings">The desired <see cref="CompressionSettings"/>.</param>
        public byte[] Compress(byte[] bytes, CompressionSettings compressionSettings)
        {
            if (ReferenceEquals(bytes, null))
            {
#if UNITY_EDITOR
                Debug.LogError($"{nameof(LzmaUtility)}: You tried to compress a null array; returning null...");
#endif
                return null;
            }

            if (bytes.Length == 0)
            {
#if UNITY_EDITOR
                Debug.LogWarning($"{nameof(LzmaUtility)}: You tried to compress an empty array; the resulting array will also be empty!");
#endif
                return EMPTY_BYTE_ARRAY;
            }

            byte[] compressedBytes;
            
            var encoder = new Encoder();
            encoder.SetCoderProperties(propIDs, compressionSettings.CompressionLevel == CompressionLevel.Optimal ? propertiesOptimal : propertiesNormal);

            MemoryStream inBytes = new MemoryStream(bytes);
            MemoryStream outBytes = new MemoryStream();

            try
            {
                encoder.WriteCoderProperties(outBytes);

                long size = inBytes.Length;
                for (int i = 0; i < 8; i++)
                {
                    outBytes.WriteByte((byte)(size >> (8 * i)));
                }

                encoder.Code(inBytes, outBytes, -1, -1, null);
                compressedBytes = outBytes.ToArray();
            }
            catch
            {
#if UNITY_EDITOR
                Debug.LogWarning($"{nameof(LzmaUtility)}: Compression failed; thrown exception: {e.ToString()}");
#endif
                compressedBytes = null;
            }
            finally
            {
                inBytes.Dispose();
                outBytes.Dispose();
            }

            return compressedBytes;
        }

        /// <summary>
        /// Compresses the specified <c>string</c> using LZMA and default <see cref="CompressionSettings"/>.
        /// </summary>
        /// <param name="text">The <c>string</c> to compress.</param>
        /// <returns>The compressed <c>string</c>.</returns>
        public string Compress(string text)
        {
            return Convert.ToBase64String(Compress(DEFAULT_ENCODING.GetBytes(text), DEFAULT_COMPRESSION_SETTINGS));
        }

        /// <summary>
        /// Decompresses the specified bytes using LZMA and the
        /// <see cref="CompressionSettings"/> that have been used to originally compress the bytes..
        /// </summary>
        /// <param name="compressedBytes">The compressed <c>byte[]</c> array that you want to decompress.</param>
        /// <param name="compressionSettings">The <see cref="CompressionSettings"/> that have been used to compress the bytes.</param>
        /// <returns>The decompressed <c>bytes[]</c>.</returns>
        public byte[] Decompress(byte[] compressedBytes, CompressionSettings compressionSettings)
        {
            if (ReferenceEquals(compressedBytes, null))
            {
#if UNITY_EDITOR
                Debug.LogError($"{nameof(LzmaUtility)}: You tried to decompress a null array; returning null...");
#endif
                return null;
            }

            if (compressedBytes.Length == 0)
            {
#if UNITY_EDITOR
                Debug.LogWarning($"{nameof(LzmaUtility)}: You tried to decompress an empty array; the resulting array will also be empty!");
#endif
                return EMPTY_BYTE_ARRAY;
            }
            
            byte[] decompressedBytes;
            var decoder = new Decoder();
            
            MemoryStream inBytes = new MemoryStream(compressedBytes);
            MemoryStream outBytes = new MemoryStream();

            try
            {
                inBytes.Seek(0, 0);

                byte[] props = new byte[5];
                if (inBytes.Read(props, 0, 5) != 5)
                {
                    throw new InvalidDataException($"{nameof(LzmaUtility)}: input LZMA data is too short. Returning null...");
                }

                long outSize = 0;
                for (int i = 0; i < 8; i++)
                {
                    int v = inBytes.ReadByte();
                    if (v < 0)
                    {
                        throw new InvalidDataException($"{nameof(LzmaUtility)}: Can't read byte at {nameof(inBytes)} position \"{inBytes.Position}\"...");
                    }

                    outSize |= ((long)(byte)v) << (8 * i);
                }

                decoder.SetDecoderProperties(props);

                long compressedSize = inBytes.Length - inBytes.Position;
                decoder.Code(inBytes, outBytes, compressedSize, outSize, null);

                decompressedBytes = outBytes.ToArray();
            }
            catch (Exception e)
            {
                string msg = $"{nameof(LzmaUtility)}: Decompression failed; thrown exception: {e.ToString()}";
#if UNITY_EDITOR
                Debug.LogError(msg);
#endif
                throw new InvalidDataException(msg);
            }
            finally
            {
                inBytes.Dispose();
                outBytes.Dispose();
            }

            return decompressedBytes;
        }

        /// <summary>
        /// Decompresses the specified compressed <c>string</c> using the default <see cref="CompressionSettings"/>.
        /// </summary>
        /// <param name="compressedString">The compressed <c>string</c> to decompress.</param>
        /// <returns>The decompressed <c>string</c></returns>.
        public string Decompress(string compressedString)
        {
            return DEFAULT_ENCODING.GetString(Decompress(Convert.FromBase64String(compressedString), DEFAULT_COMPRESSION_SETTINGS));
        }
    }
}
