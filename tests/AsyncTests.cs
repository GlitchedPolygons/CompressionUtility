using Xunit;
using System;
using System.IO;
using System.Threading.Tasks;

namespace GlitchedPolygons.Services.CompressionUtility.Tests
{
    public class AsyncTests
    {
        private ICompressionUtilityAsync impl;
        private static readonly CompressionSettings COMPRESSION_SETTINGS = new CompressionSettings();

        ICompressionUtilityAsync GetImpl(Type type)
        {
            return impl ?? (impl = (ICompressionUtilityAsync)Activator.CreateInstance(type));
        }

        [Theory]
        [InlineData(typeof(GZipUtilityAsync))]
        [InlineData(typeof(BrotliUtilityAsync))]
        public async Task AsyncImpl_CompressEmptyArray_ReturnsEmptyArray(Type type)
        {
            byte[] empty = new byte[0];
            byte[] result = await GetImpl(type).Compress(empty, COMPRESSION_SETTINGS);
            Assert.Empty(result);
        }

        [Theory]
        [InlineData(typeof(GZipUtilityAsync))]
        [InlineData(typeof(BrotliUtilityAsync))]
        public async Task AsyncImpl_DecompressEmptyArray_ReturnsEmptyArray(Type type)
        {
            byte[] empty = new byte[0];
            byte[] result = await GetImpl(type).Decompress(empty, COMPRESSION_SETTINGS);
            Assert.Empty(result);
        }

        [Theory]
        [InlineData(typeof(GZipUtilityAsync))]
        [InlineData(typeof(BrotliUtilityAsync))]
        public async Task AsyncImpl_CompressNullArray_ReturnsNull(Type type)
        {
            byte[] result = await GetImpl(type).Compress(null, COMPRESSION_SETTINGS);
            Assert.Null(result);
        }

        [Theory]
        [InlineData(typeof(GZipUtilityAsync))]
        [InlineData(typeof(BrotliUtilityAsync))]
        public async Task AsyncImpl_DecompressNullArray_ReturnsNull(Type type)
        {
            byte[] result = await GetImpl(type).Decompress(null, COMPRESSION_SETTINGS);
            Assert.Null(result);
        }

        [Theory]
        [InlineData(typeof(GZipUtilityAsync))]
        [InlineData(typeof(BrotliUtilityAsync))]
        public async Task AsyncImpl_Compress5MB_ResultSmallerThanOriginal(Type type)
        {
            byte[] data = new byte[1024 * 1024 * 5];
            for (int i = data.Length - 1; i >= 0; i--)
            {
                data[i] = (byte)(new Random().NextDouble() > 0.5d ? 5 : 75);
            }

            byte[] result = await GetImpl(type).Compress(data, new CompressionSettings { BufferSize = 1024 });
            Assert.True(result.Length < data.Length);
        }

        [Theory]
        [InlineData(typeof(GZipUtilityAsync))]
        [InlineData(typeof(BrotliUtilityAsync))]
        public async Task AsyncImpl_CompressLongString_ReturnsSmallerString(Type type)
        {
            const string STRING = "mcvlmoqepoir4298DMKEKNKNEInofndogoidnoigorenoigniofoienoign983874389759835978465798469kdnfndiiudjfbniuigorenoigniofoienoign983874389759835978465798469kdnfndiiudjfbniuigorenoigniofoienoign983874389759835978465798469kdnfndiiudjfbniuigorenoigniofoienoign983874389759835978465798469kdnfndiiudjfbniuigorenoigniofoienoign983874389759835978465798469kdnfndiiudjfbniuigorenoigniofoienoign983874389759835978465798469kdnfndiiudjfbniuebujfneufsbunskjdfkje";
            string compressed = await GetImpl(type).Compress(STRING);
            Assert.True(compressed.Length < STRING.Length);
        }

        [Theory]
        [InlineData(typeof(GZipUtilityAsync))]
        [InlineData(typeof(BrotliUtilityAsync))]
        public async Task AsyncImpl_CompressDataAndThenDecompress_ResultIdenticalData(Type type)
        {
            byte[] data = new byte[1024 * 1024];
            for (int i = data.Length - 1; i >= 0; i--)
            {
                data[i] = (byte)(new Random().NextDouble() > 0.5d ? 5 : 75);
            }

            var compressionSettings = new CompressionSettings { BufferSize = 1024 };

            byte[] compressed = await GetImpl(type).Compress(data, compressionSettings);
            byte[] decompressed = await GetImpl(type).Decompress(compressed, compressionSettings);

            Assert.Equal(data, decompressed);
        }

        [Theory]
        [InlineData(typeof(GZipUtilityAsync))]
        [InlineData(typeof(BrotliUtilityAsync))]
        public async Task AsyncImpl_DecompressCompressedString_ReturnsOriginalString(Type type)
        {
            const string STRING = "mcvlmoqepoir4298DMKfgfgdKNEInofndogoidnoigorenoigniofoienoign983874389759835978465798469kdnfndiiudjfbniuigorenoigniofoienoign983874389759835978465798469kdnfndiiudjfbniuigorenoigniofoienoign983874389759835978465798469kdnfndiiudjfbniuigorenoigniofoienoign983874389759835978465798469kdnfndiiudjfbniuigorenoigniofoienoign983874389759835978465798469kdnfndiiudjfbniuigorenoigniofoienoign983874389759835978465798469kdnfndiiudjfbniuebujfneufsbunskjdfkje";
            string compressed = await GetImpl(type).Compress(STRING);
            Assert.Equal(STRING, await GetImpl(type).Decompress(compressed));
        }

        [Theory]
        [InlineData(typeof(GZipUtilityAsync))]
        [InlineData(typeof(BrotliUtilityAsync))]
        public async Task AsyncImpl_DecompressNonCompressedArray_ThrowsIOException(Type type)
        {
            byte[] data = new byte[1024 * 1024];
            for (int i = data.Length - 1; i >= 0; i--)
            {
                data[i] = (byte)(new Random().NextDouble() > 0.5d ? 5 : 75);
            }

            await Assert.ThrowsAsync<InvalidDataException>(async () => await GetImpl(type).Decompress(data, COMPRESSION_SETTINGS));
        }
    }
}
