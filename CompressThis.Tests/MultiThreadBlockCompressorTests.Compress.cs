using Xunit;

namespace CompressThis.Tests
{
    public partial class MultiThreadBlockCompressorTests
    {
        [Fact]
        public void Test_Compress_Positive()
        {
            SetupInputFile(new byte[] {1, 2, 3, 4, 5, 6, 7, 8});
            SetupCompressResult((new byte[] {1, 2, 3, 4, 5}, new byte[] {1, 2, 3}),
                (new byte[] {6, 7, 8}, new byte[] {1, 2}));

            var service = CreateService(5);

            service.Compress("input", "output");

            var result = _outputStream.ToArray();
            Assert.Equal(new byte[]
            {
                2, 0, 0, 0, // block count
                3, 0, 0, 0, // 1 block size
                2, 0, 0, 0, // 2 block size
                1, 2, 3, // 1 block
                1, 2, // 2 block
            }, result);
        }
        
        [Fact]
        public void Test_Compress_OnlyOneEqualBlock()
        {
            SetupInputFile(new byte[] {1, 2, 3, 4, 5});
            SetupCompressResult((new byte[] {1, 2, 3, 4, 5}, new byte[] {1, 2, 3}));

            var service = CreateService(5);

            service.Compress("input", "output");

            var result = _outputStream.ToArray();
            Assert.Equal(new byte[]
            {
                1, 0, 0, 0, // block count
                3, 0, 0, 0, // 1 block size
                1, 2, 3, // 1 block
            }, result);
        }
        
        [Fact]
        public void Test_Compress_OneBlockMoreThanSize()
        {
            SetupInputFile(new byte[] {1, 2, 3, 4});
            SetupCompressResult((new byte[] {1, 2, 3, 4}, new byte[] {1, 2, 3}));

            var service = CreateService(5);

            service.Compress("input", "output");

            var result = _outputStream.ToArray();
            Assert.Equal(new byte[]
            {
                1, 0, 0, 0, // block count
                3, 0, 0, 0, // 1 block size
                1, 2, 3, // 1 block
            }, result);
        }
        
        [Fact]
        public void Test_Compress_VeryBigBlockSize()
        {
            SetupInputFile(new byte[] {1, 2, 3, 4});
            SetupCompressResult((new byte[] {1, 2, 3, 4}, new byte[] {1, 2, 3}));

            var service = CreateService(10000);

            service.Compress("input", "output");

            var result = _outputStream.ToArray();
            Assert.Equal(new byte[]
            {
                1, 0, 0, 0, // block count
                3, 0, 0, 0, // 1 block size
                1, 2, 3, // 1 block
            }, result);
        }
        
        [Fact]
        public void Test_Compress_ZeroInput()
        {
            SetupInputFile(new byte[] {});

            var service = CreateService(10000);

            service.Compress("input", "output");

            var result = _outputStream.ToArray();
            Assert.Equal(new byte[]
            {
                0, 0, 0, 0, // block count
            }, result);
        }
    }
}