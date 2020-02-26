using System.IO.Compression;

namespace GlitchedPolygons.Services.CompressionUtility
{
    /// <summary>
    /// The compression relevant settings (such as buffer size, compression level, etc...).
    /// </summary>
    public struct CompressionSettings
    {
        /// <summary>
        /// The size of the underlying stream buffer. A reasonable value could be <c>65536</c><para> </para>
        /// </summary>
        public int bufferSize;

        /// <summary>
        /// Choose the desired compression level.<para> </para>
        /// The default value favors speed over maximum efficiency.
        /// </summary>
        public CompressionLevel compressionLevel;
        
        /// <summary>
        /// General-purpose default settings for compressing and decompressing all kinds of data. When unsure, use this!
        /// </summary>
        public static CompressionSettings Default => new CompressionSettings
        {
            bufferSize = 65536,
            compressionLevel = CompressionLevel.Fastest
        };
        
    }
}
