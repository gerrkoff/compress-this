using Xunit;

namespace CompressThis.Tests
{
    public partial class MultiThreadBlockCompressorTests
    {   
        [Fact]
        public void Test_Decompress_Positive()
        {
            SetupInputFile(new byte[]
            {
                2, 0, 0, 0, // block count
                3, 0, 0, 0, // 1 block size
                2, 0, 0, 0, // 2 block size
                1, 2, 3, // 1 block
                1, 2, // 2 block
            });
            SetupDecompressResult((new byte[] {1, 2, 3}, new byte[] {1, 2, 3, 4, 5}),
                (new byte[] {1, 2}, new byte[] {6, 7, 8})
            );

            var service = CreateService(-1);

            service.Decompress("input", "output");

            var result = _outputStream.ToArray();
            Assert.Equal(new byte[] {1, 2, 3, 4, 5, 6, 7, 8}, result);
        }
        
        [Fact]
        public void Test_Decompress_OnlyOneEqualBlock()
        {
            SetupInputFile(new byte[]
            {
                1, 0, 0, 0, // block count
                3, 0, 0, 0, // 1 block size
                1, 2, 3, // 1 block
            });
            SetupDecompressResult((new byte[] {1, 2, 3}, new byte[] {1}));

            var service = CreateService(-1);

            service.Decompress("input", "output");

            var result = _outputStream.ToArray();
            Assert.Equal(new byte[] {1}, result);
        }

        [Fact]
        public void Test_Decompress_ZeroCompressedFile()
        {
            SetupInputFile(new byte[]
            {
                0, 0, 0, 0, // block count
            });

            var service = CreateService(-1);

            service.Decompress("input", "output");

            var result = _outputStream.ToArray();
            Assert.Equal(new byte[] { }, result);
        }
    }
}