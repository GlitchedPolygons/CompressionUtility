using System;
using System.IO;
using Xunit;

namespace GlitchedPolygons.Services.CompressionUtility.UnitTests
{
    public class GZipUtilityTests
    {
        ICompressionUtility GZip { get; }
        CompressionSettings CompressionSettings { get; }

        public GZipUtilityTests()
        {
            GZip = new GZipUtility();
            CompressionSettings = new CompressionSettings();
        }

        [Fact]
        public void GZip_CompressEmptyArray_ReturnsEmptyArray()
        {
            byte[] empty = new byte[0];
            byte[] result = GZip.Compress(empty, CompressionSettings);
            Assert.Empty(result);
        }

        [Fact]
        public void GZip_DecompressEmptyArray_ReturnsEmptyArray()
        {
            byte[] empty = new byte[0];
            byte[] result = GZip.Decompress(empty, CompressionSettings);
            Assert.Empty(result);
        }

        [Fact]
        public void GZip_CompressNullArray_ReturnsNull()
        {
            byte[] result = GZip.Compress(null, CompressionSettings);
            Assert.Null(result);
        }

        [Fact]
        public void GZip_DecompressNullArray_ReturnsNull()
        {
            byte[] result = GZip.Decompress(null, CompressionSettings);
            Assert.Null(result);
        }

        [Fact]
        public void GZip_Compress5MB_ResultSmallerThanOriginal()
        {
            byte[] data = new byte[1024 * 1024 * 5];
            for (int i = data.Length - 1; i >= 0; i--)
            {
                data[i] = (byte)(new Random().NextDouble() > 0.5d ? 5 : 75);
            }

            byte[] result = GZip.Compress(data, new CompressionSettings { BufferSize = 1024 });
            Assert.True(result.Length < data.Length);
        }

        [Fact]
        public void GZip_CompressLongString_ReturnsSmallerString()
        {
            const string STRING = "mcvlmoqepoir4298DMKEKNKNEInofndogoidnoigorenoigniofoienoign983874389759835978465798469kdnfndiiudjfbniuigorenoigniofoienoign983874389759835978465798469kdnfndiiudjfbniuigorenoigniofoienoign983874389759835978465798469kdnfndiiudjfbniuigorenoigniofoienoign983874389759835978465798469kdnfndiiudjfbniuigorenoigniofoienoign983874389759835978465798469kdnfndiiudjfbniuigorenoigniofoienoign983874389759835978465798469kdnfndiiudjfbniuebujfneufsbunskjdfkje";
            string compressed = GZip.Compress(STRING);
            Assert.True(compressed.Length < STRING.Length);
        }

        [Fact]
        public void GZip_CompressDataAndThenDecompress_ResultIdenticalData()
        {
            byte[] data = new byte[1024 * 1024];
            for (int i = data.Length - 1; i >= 0; i--)
            {
                data[i] = (byte)(new Random().NextDouble() > 0.5d ? 5 : 75);
            }

            var compressionSettings = new CompressionSettings { BufferSize = 1024 };

            byte[] compressed = GZip.Compress(data, compressionSettings);
            byte[] decompressed = GZip.Decompress(compressed, compressionSettings);

            Assert.Equal(data, decompressed);
        }

        [Fact]
        public void GZip_DecompressCompressedString_ReturnsOriginalString()
        {
            const string STRING = "mcvlmoqepoir4298DMKfgfgdKNEInofndogoidnoigorenoigniofoienoign983874389759835978465798469kdnfndiiudjfbniuigorenoigniofoienoign983874389759835978465798469kdnfndiiudjfbniuigorenoigniofoienoign983874389759835978465798469kdnfndiiudjfbniuigorenoigniofoienoign983874389759835978465798469kdnfndiiudjfbniuigorenoigniofoienoign983874389759835978465798469kdnfndiiudjfbniuigorenoigniofoienoign983874389759835978465798469kdnfndiiudjfbniuebujfneufsbunskjdfkje";
            string compressed = GZip.Compress(STRING);
            Assert.Equal(STRING, GZip.Decompress(compressed));
        }

        [Fact]
        public void GZip_DecompressNonCompressedArray_ThrowsIOException()
        {
            byte[] data = new byte[1024 * 1024];
            for (int i = data.Length - 1; i >= 0; i--)
            {
                data[i] = (byte)(new Random().NextDouble() > 0.5d ? 5 : 75);
            }
            
            Assert.Throws<InvalidDataException>(() => GZip.Decompress(data, CompressionSettings));
        }
    }
}
