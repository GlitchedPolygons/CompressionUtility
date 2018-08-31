using System.IO.Compression;

namespace GlitchedPolygons.Services.CompressionUtility
{
    /// <summary>
    /// The compression relevant settings (such as buffer size, compression level, etc...).
    /// </summary>
    public class CompressionSettings
    {
        /// <summary>
        /// The size of the underlying stream buffer.<para> </para>
        /// Try to give a rough estimate of how many bytes you'll need...
        /// </summary>
        public int BufferSize { get; set; } = 4096;

        /// <summary>
        /// Choose the desired compression level.<para> </para>
        /// The default value favors speed over maximum efficiency (<see cref="CompressionLevel.Fastest"/>).
        /// </summary>
        public CompressionLevel CompressionLevel { get; set; } = CompressionLevel.Fastest;
    }
}
