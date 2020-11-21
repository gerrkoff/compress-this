using System.IO;
using Xunit;

namespace CompressThis.Tests
{
    public partial class MultiThreadBlockCompressorTests
    {
        [Fact]
        public void Test_Exception_DecompressInputLessThan4Bytes()
        {
            SetupInputFile(new byte[] {0, 0, 0});

            var service = CreateService(-1);

            Assert.Throws<InvalidDataException>(() =>
                service.Decompress("input", "output"));
        }
        
        [Fact]
        public void Test_Exception_DecompressInputNotFoundAllBlockSizes()
        {
            SetupInputFile(new byte[]
            {
                2, 0, 0, 0, // block count
                0, 0, 0, 0, // 1 block size
                0, 0, 0, // 2 block size (not full)
            });

            var service = CreateService(-1);

            Assert.Throws<InvalidDataException>(() =>
                service.Decompress("input", "output"));
        }
        
        [Fact]
        public void Test_Exception_DecompressInputMissingBytesForBlock()
        {
            SetupInputFile(new byte[]
            {
                2, 0, 0, 0, // block count
                2, 0, 0, 0, // 1 block size
                3, 0, 0, 0, // 2 block size
                1, 2, // 1 block
                1, 2, // 2 block
            });

            var service = CreateService(-1);

            Assert.Throws<InvalidDataException>(() =>
                service.Decompress("input", "output"));
        }
        
        [Fact]
        public void Test_Exception_DecompressInputMissingBlock()
        {
            SetupInputFile(new byte[]
            {
                2, 0, 0, 0, // block count
                2, 0, 0, 0, // 1 block size
                3, 0, 0, 0, // 2 block size
                1, 2, // 1 block
            });

            var service = CreateService(-1);

            Assert.Throws<InvalidDataException>(() =>
                service.Decompress("input", "output"));
        }
        
        [Fact]
        public void Test_Exception_DecompressBlockFails()
        {
            _compressService.Setup(x => x.Decompress(new byte[] {1})).Throws<InvalidDataException>();
            SetupInputFile(new byte[]
            {
                1, 0, 0, 0, // block count
                1, 0, 0, 0, // 1 block size
                1, // 1 block
            });

            var service = CreateService(-1);

            Assert.Throws<InvalidDataException>(() =>
                service.Decompress("input", "output"));
        }
        
        [Fact]
        public void Test_Exception_CompressBlockFails()
        {
            _compressService.Setup(x => x.Compress(new byte[] {1})).Throws<InvalidDataException>();
            SetupInputFile(new byte[] {1});

            var service = CreateService(1);

            Assert.Throws<InvalidDataException>(() =>
                service.Compress("input", "output"));
        }
    }
}