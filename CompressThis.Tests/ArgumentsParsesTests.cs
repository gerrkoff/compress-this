using System;
using CompressThis.ArgumentsProcessing;
using CompressThis.Services.Interfaces;
using Moq;
using Xunit;

namespace CompressThis.Tests
{
    public class ArgumentsParserTests
    {
        private readonly Mock<IFileService> _fileService = new Mock<IFileService>();

        public ArgumentsParserTests()
        {
            _fileService.Setup(x => x.Exists("input")).Returns(true);
            _fileService.Setup(x => x.CanCreate("output")).Returns(true);
        }
        
        [Fact]
        public void Test_ParseCompressMode_Compress()
        {
            var args = new[] { "compress", "input", "output" };
            
            var service = new ArgumentsParser(_fileService.Object);
            var result = service.Parse(args);

            Assert.True(result.IsCompressMode);
        }
        
        [Fact]
        public void Test_ParseCompressMode_Decompress()
        {
            var args = new[] { "decompress", "input", "output" };
            
            var service = new ArgumentsParser(_fileService.Object);
            var result = service.Parse(args);

            Assert.False(result.IsCompressMode);
        }
        
        [Fact]
        public void Test_ParseCompressMode_WrongValue()
        {
            var args = new[] { "decompress111", "input", "output" };
            
            var service = new ArgumentsParser(_fileService.Object);

            Assert.Throws<ArgumentException>(() => service.Parse(args));
        }
        
        [Fact]
        public void Test_ParseInputFile_Positive()
        {
            var args = new[] { "compress", "input", "output" };
            
            var service = new ArgumentsParser(_fileService.Object);
            var result = service.Parse(args);

            Assert.Equal("input", result.InputFile);
        }
        
        [Fact]
        public void Test_ParseInputFile_FileNotFound()
        {
            _fileService.Setup(x => x.Exists("input")).Returns(false);

            var args = new[] { "compress", "input", "output" };
            
            var service = new ArgumentsParser(_fileService.Object);
            
            Assert.Throws<ArgumentException>(() => service.Parse(args));
        }
        
        [Fact]
        public void Test_ParseOutputFile_Positive()
        {
            var args = new[] { "compress", "input", "output" };
            
            var service = new ArgumentsParser(_fileService.Object);
            var result = service.Parse(args);

            Assert.Equal("output", result.OutputFile);
        }
        
        [Fact]
        public void Test_ParseOutputFile_CannotCreate()
        {
            _fileService.Setup(x => x.CanCreate("output")).Returns(false);
            
            var args = new[] { "compress", "input", "output" };
            
            var service = new ArgumentsParser(_fileService.Object);
            
            Assert.Throws<ArgumentException>(() => service.Parse(args));
        }
        
        [Fact]
        public void Test_ParseVerbose()
        {
            var args = new[] { "compress", "input", "output", "-v" };
            
            var service = new ArgumentsParser(_fileService.Object);
            var result = service.Parse(args);

            Assert.True(result.IsVerbose);
        }
        
        [Fact]
        public void Test_ParseSingleThread()
        {
            var args = new[] { "compress", "input", "output", "-s" };
            
            var service = new ArgumentsParser(_fileService.Object);
            var result = service.Parse(args);

            Assert.True(result.IsSingleThread);
        }
        
        [Fact]
        public void Test_IgnoreUnknownArgs()
        {
            var args = new[] { "compress", "input", "output", "unknown" };
            
            var service = new ArgumentsParser(_fileService.Object);
            service.Parse(args);
        }
        
        [Fact]
        public void Test_NullArgs()
        {
            var service = new ArgumentsParser(_fileService.Object);
            
            Assert.Throws<ArgumentException>(() => service.Parse(null));
        }
        
        [Fact]
        public void Test_ArgsLengthLessThanThree()
        {
            var args = new[] { "compress", "input" };
            
            var service = new ArgumentsParser(_fileService.Object);
            
            Assert.Throws<ArgumentException>(() => service.Parse(args));
        }
        
        [Fact]
        public void Test_ParseBlockSize_NoValueProvided()
        {
            var args = new[] { "compress", "input", "output", "--block-size" };
            
            var service = new ArgumentsParser(_fileService.Object);
            
            Assert.Throws<ArgumentException>(() => service.Parse(args));
        }
        
        [Fact]
        public void Test_ParseBlockSize_CannotParseValue()
        {
            var args = new[] { "compress", "input", "output", "--block-size", "qqq" };
            
            var service = new ArgumentsParser(_fileService.Object);
            
            Assert.Throws<ArgumentException>(() => service.Parse(args));
        }
        
        [Fact]
        public void Test_ParseBlockSize_Positive()
        {
            var args = new[] { "compress", "input", "output", "--block-size", "10485760" };
            
            var service = new ArgumentsParser(_fileService.Object);
            var result = service.Parse(args);
            
            Assert.Equal(10485760, result.BlockSize);
        }
        
        [Fact]
        public void Test_ParseBlockSize_NoValue()
        {
            var args = new[] { "compress", "input", "output" };
            
            var service = new ArgumentsParser(_fileService.Object);
            var result = service.Parse(args);

            Assert.Null(result.BlockSize);
        }
    }
}