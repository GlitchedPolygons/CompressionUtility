using System.IO.Compression;

namespace GlitchedPolygons.Services.CompressionUtility
{
    /// <summary>
    /// The compression relevant settings (such as buffer size, compression level, etc...).
    /// </summary>
    public class CompressionSettings
    {
        /// <summary>
        /// The size of the underlying stream buffer. Default value is <c>65536</c><para> </para>
        /// </summary>
        public int BufferSize { get; set; } = 65536;

        /// <summary>
        /// Choose the desired compression level.<para> </para>
        /// The default value favors speed over maximum efficiency (<see cref="System.IO.Compression.CompressionLevel.Fastest"/>).
        /// </summary>
        public CompressionLevel CompressionLevel { get; set; } = CompressionLevel.Fastest;
    }
}
