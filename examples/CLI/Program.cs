using System;
using System.Threading.Tasks;
using GlitchedPolygons.Services.CompressionUtility;

namespace GlitchedPolygons.Services.CompressionUtility.Examples.CLI
{
    static class Program
    {
        private static readonly ICompressionUtilityAsync gzip = new GZipUtilityAsync();
        
        private static async Task Main(string[] args)
        {
            Console.WriteLine("Compression Utility Nuget Library");
            Console.WriteLine($"Version {typeof(ICompressionUtilityAsync).Assembly.GetName().Version} - Example CLI \n");

            if (args.Length != 2)
            {
                goto error;
            }
            
            if (args[0] == "-c")
            {
                string compressed = await gzip.Compress(args[1]);
                Console.WriteLine("COMPRESSED STRING:  " + compressed);
                return;
            }

            if (args[0] == "-d")
            {
                string decompressed = await gzip.Decompress(args[1]);
                Console.WriteLine("DECOMPRESSED STRING:  " + decompressed);
                return;
            }
            
            error:
            Console.WriteLine("INVALID ARGS. Correct usage:  \nCompressing:\t-c \"String to compress here *@% 123\" \nDecompressing:\t-d \"H4sIAAAAAAAECgtxDQ4BALiT6u4EAAAA\" \n\n");
        }
    }
}
