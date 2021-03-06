using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using CompressThis.Models;
using CompressThis.Services.Interfaces;

namespace CompressThis.Compressors
{
    public class MultiThreadBlockCompressor : ICompressor
    {
        private const int MaxBlockSize = 2_146_435_071;
        private readonly Dictionary<long, byte[]> _processedBlocks = new Dictionary<long, byte[]>();
        
        private readonly ICompressService _compressService;
        private readonly IFileService _fileService;
        private readonly ICompressionMetaDataService _metaDataService;
        private readonly IThreadPool _threadPool;

        public MultiThreadBlockCompressor(ICompressService compressService, IFileService fileService, ICompressionMetaDataService metaDataService, IThreadPool threadPool)
        {
            _compressService = compressService;
            _fileService = fileService;
            _metaDataService = metaDataService;
            _threadPool = threadPool;
        }

        public int BlockSize { get; set; } = 1024 * 1024;
        public event EventHandler<ProgressUpdatedEventArgs> ProgressUpdated;

        public CompressionResult Compress(string inputFilePath, string outputFilePath)
        {
            using var inputFileStream = _fileService.OpenRead(inputFilePath);

            var blockCount = Math.Ceiling((double) inputFileStream.Length / BlockSize);
            if (blockCount > MaxBlockSize)
                throw new InvalidDataException(CompressionExceptionMessages.TooManyBlocks);

            var metaData = new CompressionMetaData((int) blockCount);

            long outputFileSize = 0;
            bool isSuccess = false;
            var writerThread =
                _threadPool.RunDedicated(() => WriteCompressionToFile(outputFilePath, metaData, out outputFileSize, out isSuccess));

            for (int i = 0; i < metaData.BlockSizes.Length; i++)
            {
                var blockIndex = i;
                var blockBytes = ReadBlockFromStream(inputFileStream, BlockSize);
                _threadPool.Run(() =>
                    ProcessBlock(blockBytes, blockIndex, _compressService.Compress, metaData.BlockSizes));
            }

            writerThread.Join();
            
            if (!isSuccess)
                throw new InvalidDataException(CompressionExceptionMessages.UnknownCompressException);

            return new CompressionResult(inputFileStream.Length, outputFileSize) {MetaData = metaData};
        }
        
        private void WriteCompressionToFile(string filePath, CompressionMetaData metaData, out long fileSize, out bool isSuccess)
        {
            using var fileStream = _fileService.Create(filePath);
            fileStream.Seek(metaData.MetaDataSize, SeekOrigin.Begin);
            isSuccess = WriteBlocksToStream(fileStream, metaData.BlockSizes.Length);
            _metaDataService.WriteToStream(fileStream, metaData);
            fileSize = fileStream.Length;
        }
        
        public CompressionResult Decompress(string inputFilePath, string outputFilePath)
        {
            using Stream inputFileStream = _fileService.OpenRead(inputFilePath);

            var metaData = _metaDataService.ReadFromStream(inputFileStream);
            
            long outputFileSize = 0;
            bool isSuccess = false;
            var writerThread = _threadPool.RunDedicated(() =>
                WriteDecompressionToFile(outputFilePath, metaData, out outputFileSize, out isSuccess));

            for (int i = 0; i < metaData.BlockSizes.Length; i++)
            {
                var blockIndex = i;
                var blockBytes = ReadBlockFromStream(inputFileStream, metaData.BlockSizes[i]);
                if (blockBytes.Length != metaData.BlockSizes[i])
                    throw new InvalidDataException(CompressionExceptionMessages.WrongFormat);
                _threadPool.Run(() => ProcessBlock(blockBytes, blockIndex, _compressService.Decompress));
            }

            writerThread.Join();
            
            if (!isSuccess)
                throw new InvalidDataException(CompressionExceptionMessages.WrongFormat);

            return new CompressionResult(inputFileStream.Length, outputFileSize) {MetaData = metaData};
        }
        
        private void WriteDecompressionToFile(string filePath, CompressionMetaData metaData, out long fileSize, out bool isSuccess)
        {
            using var fileStream = _fileService.Create(filePath);
            isSuccess = WriteBlocksToStream(fileStream, metaData.BlockSizes.Length);
            fileSize = fileStream.Length;
        }
        
        private bool WriteBlocksToStream(Stream output, int totalBlockCount)
        {
            lock (_processedBlocks)
            {
                var blockCount = 0;
                while (blockCount < totalBlockCount)
                {
                    while (!_processedBlocks.ContainsKey(blockCount))
                        Monitor.Wait(_processedBlocks);

                    if (_processedBlocks[blockCount] == null)
                        return false;

                    output.Write(_processedBlocks[blockCount], 0, _processedBlocks[blockCount].Length);
                    _processedBlocks.Remove(blockCount);
                    blockCount++;
                    OnProgressUpdated(blockCount, totalBlockCount);
                }
            }

            return true;
        }
        
        private void ProcessBlock(byte[] block, int blockIndex, Func<byte[], byte[]> process, int[] blockSizesToSave = null)
        {
            byte[] processedBlock = null;
            try
            {
                processedBlock = process(block);
            }
            catch (InvalidDataException) {}

            if (blockSizesToSave != null && processedBlock != null)
                blockSizesToSave[blockIndex] = processedBlock.Length;

            lock (_processedBlocks)
            {
                _processedBlocks.Add(blockIndex, processedBlock);
                Monitor.Pulse(_processedBlocks);
            }
        }

        private byte[] ReadBlockFromStream(Stream stream, int blockSize)
        {
            var totalReadBytes = 0;
            int readBytes;
            var buffer = new byte[blockSize];

            do
            {
                readBytes = stream.Read(buffer, totalReadBytes, buffer.Length - totalReadBytes);
                totalReadBytes += readBytes;
            } while (totalReadBytes < blockSize && readBytes > 0);

            var result = new byte[totalReadBytes];
            Array.Copy(buffer, result, result.Length);
            return result;
        }

        private void OnProgressUpdated(int processedBlockIndex, int totalBlockCount)
        {
            var handler = ProgressUpdated;
            handler?.Invoke(this, new ProgressUpdatedEventArgs((double) processedBlockIndex / totalBlockCount));
        }
    }
}