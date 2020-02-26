using Xunit;
using System;
using System.IO;

namespace GlitchedPolygons.Services.CompressionUtility.Tests
{
    public class SyncTests
    {
        private ICompressionUtility impl;
        private static readonly CompressionSettings COMPRESSION_SETTINGS = new CompressionSettings();

        ICompressionUtility GetImpl(Type type)
        {
            return impl ??= (ICompressionUtility)Activator.CreateInstance(type);
        }

        [Theory]
        [InlineData(typeof(GZipUtility))]
        [InlineData(typeof(LzmaUtility))]
        [InlineData(typeof(BrotliUtility))]
        public void SyncImpl_CompressEmptyArray_ReturnsEmptyArray(Type type)
        {
            impl = GetImpl(type);

            byte[] empty = new byte[0];
            byte[] result = impl.Compress(empty, COMPRESSION_SETTINGS);
            Assert.Empty(result);
        }

        [Theory]
        [InlineData(typeof(GZipUtility))]
        [InlineData(typeof(LzmaUtility))]
        [InlineData(typeof(BrotliUtility))]
        public void SyncImpl_DecompressEmptyArray_ReturnsEmptyArray(Type type)
        {
            impl = GetImpl(type);

            byte[] empty = new byte[0];
            byte[] result = impl.Decompress(empty, COMPRESSION_SETTINGS);
            Assert.Empty(result);
        }

        [Theory]
        [InlineData(typeof(LzmaUtility))]
        [InlineData(typeof(GZipUtility))]
        [InlineData(typeof(BrotliUtility))]
        public void SyncImpl_CompressNullArray_ReturnsNull(Type type)
        {
            impl = GetImpl(type);

            byte[] result = impl.Compress(null, COMPRESSION_SETTINGS);
            Assert.Null(result);
        }
        
        [Theory]
        [InlineData(typeof(LzmaUtility), "")]
        [InlineData(typeof(GZipUtility), "")]
        [InlineData(typeof(BrotliUtility), "")]
        [InlineData(typeof(LzmaUtility), null)]
        [InlineData(typeof(GZipUtility), null)]
        [InlineData(typeof(BrotliUtility), null)]
        public void SyncImpl_CompressNullOrEmptyString_ReturnsNull(Type type, string s)
        {
            impl = GetImpl(type);

            string result = impl.Compress(s);
            Assert.Null(result);
        }

        [Theory]
        [InlineData(typeof(LzmaUtility))]
        [InlineData(typeof(GZipUtility))]
        [InlineData(typeof(BrotliUtility))]
        public void SyncImpl_DecompressNullArray_ReturnsNull(Type type)
        {
            impl = GetImpl(type);

            byte[] result = impl.Decompress(null, COMPRESSION_SETTINGS);
            Assert.Null(result);
        }

        [Theory]
        [InlineData(typeof(LzmaUtility))]
        [InlineData(typeof(GZipUtility))]
        [InlineData(typeof(BrotliUtility))]
        public void SyncImpl_Compress5MB_ResultSmallerThanOriginal(Type type)
        {
            impl = GetImpl(type);

            byte[] data = new byte[1024 * 1024 * 5];
            for (int i = data.Length - 1; i >= 0; i--)
            {
                data[i] = (byte)(new Random().NextDouble() > 0.5d ? 5 : 75);
            }

            byte[] result = impl.Compress(data, new CompressionSettings { bufferSize = 1024 });
            Assert.True(result.Length < data.Length);
        }

        [Theory]
        [InlineData(typeof(LzmaUtility))]
        [InlineData(typeof(GZipUtility))]
        [InlineData(typeof(BrotliUtility))]
        public void SyncImpl_CompressLongString_ReturnsSmallerString(Type type)
        {
            impl = GetImpl(type);

            const string STRING = "mcvlmoqepoir4298DMKEKNKNEInofndogoidnoigorenoigniofoienoign983874389759835978465798469kdnfndiiudjfbniuigorenoigniofoienoign983874389759835978465798469kdnfndiiudjfbniuigorenoigniofoienoign983874389759835978465798469kdnfndiiudjfbniuigorenoigniofoienoign983874389759835978465798469kdnfndiiudjfbniuigorenoigniofoienoign983874389759835978465798469kdnfndiiudjfbniuigorenoigniofoienoign983874389759835978465798469kdnfndiiudjfbniuebujfneufsbunskjdfkje";
            string compressed = impl.Compress(STRING);
            Assert.True(compressed.Length < STRING.Length);
        }

        [Theory]
        [InlineData(typeof(LzmaUtility))]
        [InlineData(typeof(GZipUtility))]
        [InlineData(typeof(BrotliUtility))]
        public void SyncImpl_CompressDataAndThenDecompress_ResultIdenticalData(Type type)
        {
            impl = GetImpl(type);

            byte[] data = new byte[1024 * 1024];
            for (int i = data.Length - 1; i >= 0; i--)
            {
                data[i] = (byte)(new Random().NextDouble() > 0.5d ? 5 : 75);
            }

            var compressionSettings = new CompressionSettings { bufferSize = 1024 };

            byte[] compressed = impl.Compress(data, compressionSettings);
            byte[] decompressed = impl.Decompress(compressed, compressionSettings);

            Assert.Equal(data, decompressed);
        }

        [Theory]
        [InlineData(typeof(LzmaUtility))]
        [InlineData(typeof(GZipUtility))]
        [InlineData(typeof(BrotliUtility))]
        public void SyncImpl_DecompressCompressedString_ReturnsOriginalString(Type type)
        {
            impl = GetImpl(type);

            const string STRING = "mcvlmoqepoir4298DMKfgfgdKNEInofndogoidnoigorenoigniofoienoign983874389759835978465798469kdnfndiiudjfbniuigorenoigniofoienoign983874389759835978465798469kdnfndiiudjfbniuigorenoigniofoienoign983874389759835978465798469kdnfndiiudjfbniuigorenoigniofoienoign983874389759835978465798469kdnfndiiudjfbniuigorenoigniofoienoign983874389759835978465798469kdnfndiiudjfbniuigorenoigniofoienoign983874389759835978465798469kdnfndiiudjfbniuebujfneufsbunskjdfkje";
            string compressed = impl.Compress(STRING);
            Assert.Equal(STRING, impl.Decompress(compressed));
        }

        [Theory]
        [InlineData(typeof(LzmaUtility))]
        [InlineData(typeof(GZipUtility))]
        [InlineData(typeof(BrotliUtility))]
        public void SyncImpl_DecompressNonCompressedArray_ThrowsException(Type type)
        {
            impl = GetImpl(type);

            byte[] data = new byte[1024 * 1024];
            for (int i = data.Length - 1; i >= 0; i--)
            {
                data[i] = (byte)(new Random().NextDouble() > 0.5d ? 5 : 75);
            }

            Assert.ThrowsAny<Exception>(() => GetImpl(type).Decompress(data, COMPRESSION_SETTINGS));
        }
    }
}
