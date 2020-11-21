using System.IO;
using System.IO.Compression;
using CompressThis.Models;

namespace CompressThis.Compressors
{
    public class SingleThreadCompressor : ICompressor
    {
        public CompressionResult Compress(string inputFilePath, string outputFilePath)
        {
            using FileStream originalFileStream = new FileInfo(inputFilePath).OpenRead();
            using FileStream compressedFileStream = File.Create(outputFilePath);
            using GZipStream compressionStream = new GZipStream(compressedFileStream, CompressionMode.Compress);
            
            originalFileStream.CopyTo(compressionStream);

            return new CompressionResult(originalFileStream.Length, compressedFileStream.Length);
        }

        public CompressionResult Decompress(string inputFilePath, string outputFilePath)
        {
            using FileStream originalFileStream = new FileInfo(inputFilePath).OpenRead();
            using GZipStream decompressionStream = new GZipStream(originalFileStream, CompressionMode.Decompress);
            using FileStream decompressedFileStream = File.Create(outputFilePath);

            decompressionStream.CopyTo(decompressedFileStream);
            
            return new CompressionResult(originalFileStream.Length, decompressedFileStream.Length);
        }
    }
}