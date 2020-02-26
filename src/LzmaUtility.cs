using System;
using System.IO;
using System.Text;
using System.IO.Compression;

using SevenZip;

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
                return null;
            }

            if (bytes.Length == 0)
            {
                return Array.Empty<byte>();
            }

            byte[] compressedBytes;
            
            var encoder = new Encoder();
            encoder.SetCoderProperties(propIDs, compressionSettings.compressionLevel == CompressionLevel.Optimal ? propertiesOptimal : propertiesNormal);

            MemoryStream input = new MemoryStream(bytes);
            MemoryStream output = new MemoryStream(bytes.Length / 4 * 3);

            try
            {
                encoder.WriteCoderProperties(output);

                long size = input.Length;
                for (int i = 0; i < 8; i++)
                {
                    output.WriteByte((byte)(size >> (8 * i)));
                }

                encoder.Code(input, output, -1, -1, null);
                compressedBytes = output.ToArray();
            }
            catch
            {
                compressedBytes = null;
            }
            finally
            {
                input.Dispose();
                output.Dispose();
            }

            return compressedBytes;
        }

        /// <summary>
        /// Compresses the specified <c>string</c> using LZMA and <see cref="CompressionSettings.Default"/>.
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
        /// Decompresses the specified bytes using LZMA and the
        /// <see cref="CompressionSettings"/> that have been used to originally compress the bytes.
        /// </summary>
        /// <param name="compressedBytes">The compressed <c>byte[]</c> array that you want to decompress.</param>
        /// <param name="compressionSettings">The <see cref="CompressionSettings"/> that have been used to compress the bytes.</param>
        /// <returns>The decompressed <c>bytes[]</c>.</returns>
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
            
            byte[] decompressedBytes;
            var decoder = new Decoder();
            
            MemoryStream input = new MemoryStream(compressedBytes);
            MemoryStream output = new MemoryStream(compressedBytes.Length / 2 * 3);

            try
            {
                input.Seek(0, 0);

                byte[] props = new byte[5];
                if (input.Read(props, 0, 5) != 5)
                {
                    throw new InvalidDataException($"{nameof(LzmaUtility)}: input LZMA data is too short. Returning null...");
                }

                long outSize = 0;
                for (int i = 0; i < 8; i++)
                {
                    int v = input.ReadByte();
                    if (v < 0)
                    {
                        throw new InvalidDataException($"{nameof(LzmaUtility)}: Can't read byte at {nameof(input)} position \"{input.Position}\"...");
                    }

                    outSize |= ((long)(byte)v) << (8 * i);
                }

                decoder.SetDecoderProperties(props);

                long compressedSize = input.Length - input.Position;
                decoder.Code(input, output, compressedSize, outSize, null);

                decompressedBytes = output.ToArray();
            }
            catch (Exception e)
            {
                string msg = $"{nameof(LzmaUtility)}: Decompression failed; thrown exception: {e.ToString()}";
                throw new InvalidDataException(msg);
            }
            finally
            {
                input.Dispose();
                output.Dispose();
            }

            return decompressedBytes;
        }

        /// <summary>
        /// Decompresses the specified compressed <c>string</c> using LZMA and <c>CompressionSettings.Default</c>.
        /// </summary>
        /// <param name="compressedString">The compressed <c>string</c> to decompress.</param>
        /// <param name="encoding">The encoding to use. Can be <c>null</c>; UTF8 will be used in that case.</param>
        /// <returns>The decompressed <c>string</c>.</returns>.
        public string Decompress(string compressedString, Encoding encoding = null)
        {
            byte[] decompressedBytes = Decompress(Convert.FromBase64String(compressedString), CompressionSettings.Default);
            return (encoding ?? Encoding.UTF8).GetString(decompressedBytes);
        }
    }
}
