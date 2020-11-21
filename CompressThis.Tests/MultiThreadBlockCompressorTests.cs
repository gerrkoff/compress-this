using System.IO;
using CompressThis.Compressors;
using CompressThis.Services;
using CompressThis.Services.Interfaces;
using Moq;

namespace CompressThis.Tests
{
    public partial class MultiThreadBlockCompressorTests
    {
        private const string InputFilePath = "input";
        private const string OutputFilePath = "output";
        
        private readonly Mock<ICompressService> _compressService = new Mock<ICompressService>();
        private readonly Mock<IFileService> _fileService = new Mock<IFileService>();
        private readonly MemoryStream _outputStream = new MemoryStream();

        private void SetupCompressResult(params (byte[], byte[])[] bytesResults)
        {
            foreach (var (bytesIn, bytesOut) in bytesResults)
            {
                _compressService.Setup(x => x.Compress(bytesIn)).Returns(bytesOut);
            }
        }
        
        private void SetupDecompressResult(params (byte[], byte[])[] bytesResults)
        {
            foreach (var (bytesIn, bytesOut) in bytesResults)
            {
                _compressService.Setup(x => x.Decompress(bytesIn)).Returns(bytesOut);
            }
        }

        private void SetupInputFile(byte[] bytes)
        {
            var inputFileStream = new MemoryStream(bytes);
            _fileService.Setup(x => x.OpenRead(InputFilePath)).Returns(inputFileStream);
            _fileService.Setup(x => x.Create(OutputFilePath)).Returns(_outputStream);
        }

        private MultiThreadBlockCompressor CreateService(int blockSize) =>
            new MultiThreadBlockCompressor(_compressService.Object, _fileService.Object,
                new CompressionMetaDataService(), new ThreadPool()) {BlockSize = blockSize};
    }
}