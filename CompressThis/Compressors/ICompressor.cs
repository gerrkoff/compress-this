using CompressThis.Models;

namespace CompressThis.Compressors
{
    public interface ICompressor
    {
        CompressionResult Compress(string inputFilePath, string outputFilePath);
        CompressionResult Decompress(string inputFilePath, string outputFilePath);
    }
}