using Xunit;
using System;
using System.IO;
using System.Threading.Tasks;

namespace GlitchedPolygons.Services.CompressionUtility.Tests
{
    public class GZipUtilityAsyncTests
    {
        private readonly ICompressionUtilityAsync gzip;
        private static readonly CompressionSettings COMPRESSION_SETTINGS= new CompressionSettings();

        public GZipUtilityAsyncTests()
        {
            gzip = new GZipUtilityAsync();
        }

        [Fact]
        public async Task GZip_CompressEmptyArray_ReturnsEmptyArray()
        {
            byte[] empty = new byte[0];
            byte[] result = await gzip.Compress(empty, COMPRESSION_SETTINGS);
            Assert.Empty(result);
        }

        [Fact]
        public async Task GZip_DecompressEmptyArray_ReturnsEmptyArray()
        {
            byte[] empty = new byte[0];
            byte[] result = await gzip.Decompress(empty, COMPRESSION_SETTINGS);
            Assert.Empty(result);
        }

        [Fact]
        public async Task GZip_CompressNullArray_ReturnsNull()
        {
            byte[] result = await gzip.Compress(null, COMPRESSION_SETTINGS);
            Assert.Null(result);
        }

        [Fact]
        public async Task GZip_DecompressNullArray_ReturnsNull()
        {
            byte[] result = await gzip.Decompress(null, COMPRESSION_SETTINGS);
            Assert.Null(result);
        }

        [Fact]
        public async Task GZip_Compress5MB_ResultSmallerThanOriginal()
        {
            byte[] data = new byte[1024 * 1024 * 5];
            for (int i = data.Length - 1; i >= 0; i--)
            {
                data[i] = (byte)(new Random().NextDouble() > 0.5d ? 5 : 75);
            }

            byte[] result = await gzip.Compress(data, new CompressionSettings { BufferSize = 1024 });
            Assert.True(result.Length < data.Length);
        }

        [Fact]
        public async Task GZip_CompressLongString_ReturnsSmallerString()
        {
            const string STRING = "mcvlmoqepoir4298DMKEKNKNEInofndogoidnoigorenoigniofoienoign983874389759835978465798469kdnfndiiudjfbniuigorenoigniofoienoign983874389759835978465798469kdnfndiiudjfbniuigorenoigniofoienoign983874389759835978465798469kdnfndiiudjfbniuigorenoigniofoienoign983874389759835978465798469kdnfndiiudjfbniuigorenoigniofoienoign983874389759835978465798469kdnfndiiudjfbniuigorenoigniofoienoign983874389759835978465798469kdnfndiiudjfbniuebujfneufsbunskjdfkje";
            string compressed = await gzip.Compress(STRING);
            Assert.True(compressed.Length < STRING.Length);
        }

        [Fact]
        public async Task GZip_CompressDataAndThenDecompress_ResultIdenticalData()
        {
            byte[] data = new byte[1024 * 1024];
            for (int i = data.Length - 1; i >= 0; i--)
            {
                data[i] = (byte)(new Random().NextDouble() > 0.5d ? 5 : 75);
            }

            var compressionSettings = new CompressionSettings { BufferSize = 1024 };

            byte[] compressed = await gzip.Compress(data, compressionSettings);
            byte[] decompressed = await gzip.Decompress(compressed, compressionSettings);

            Assert.Equal(data, decompressed);
        }

        [Fact]
        public async Task GZip_DecompressCompressedString_ReturnsOriginalString()
        {
            const string STRING = "mcvlmoqepoir4298DMKfgfgdKNEInofndogoidnoigorenoigniofoienoign983874389759835978465798469kdnfndiiudjfbniuigorenoigniofoienoign983874389759835978465798469kdnfndiiudjfbniuigorenoigniofoienoign983874389759835978465798469kdnfndiiudjfbniuigorenoigniofoienoign983874389759835978465798469kdnfndiiudjfbniuigorenoigniofoienoign983874389759835978465798469kdnfndiiudjfbniuigorenoigniofoienoign983874389759835978465798469kdnfndiiudjfbniuebujfneufsbunskjdfkje";
            string compressed = await gzip.Compress(STRING);
            Assert.Equal(STRING, await gzip.Decompress(compressed));
        }

        [Fact]
        public async Task GZip_DecompressNonCompressedArray_ThrowsIOException()
        {
            byte[] data = new byte[1024 * 1024];
            for (int i = data.Length - 1; i >= 0; i--)
            {
                data[i] = (byte)(new Random().NextDouble() > 0.5d ? 5 : 75);
            }
            
            await Assert.ThrowsAsync<InvalidDataException>(async () => await gzip.Decompress(data, COMPRESSION_SETTINGS));
        }
    }
}
