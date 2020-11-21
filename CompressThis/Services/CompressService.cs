using System.IO;
using System.IO.Compression;
using CompressThis.Services.Interfaces;

namespace CompressThis.Services
{
    public class CompressService : ICompressService
    {
        public byte[] Compress(byte[] bytes)
        {
            using var compressedStream = new MemoryStream();
            using var compressionStream = new GZipStream(compressedStream, CompressionMode.Compress);

            compressionStream.Write(bytes, 0, bytes.Length);
            compressionStream.Flush();

            return compressedStream.ToArray();
        }

        public byte[] Decompress(byte[] bytes)
        {
            using var compressedStream = new MemoryStream(bytes);
            using var decompressionStream = new GZipStream(compressedStream, CompressionMode.Decompress);
            using var decompressedStream = new MemoryStream(); 

            decompressionStream.CopyTo(decompressedStream);

            return decompressedStream.ToArray();
        }
    }
}