using System.IO;
using System.Text;
using CompressThis.Models;
using CompressThis.Services.Interfaces;

namespace CompressThis.Services
{
    public class CompressionMetaDataService : ICompressionMetaDataService
    {
        public void WriteToStream(Stream stream, CompressionMetaData metaData)
        {
            stream.Seek(0, SeekOrigin.Begin);
            using var binaryWriter = new BinaryWriter(stream, Encoding.UTF8, true);
            binaryWriter.Write(metaData.BlockSizes.Length);
            foreach (var blockSize in metaData.BlockSizes)
            {
                binaryWriter.Write(blockSize);
            }
        }

        public CompressionMetaData ReadFromStream(Stream stream)
        {
            using var binaryReader = new BinaryReader(stream, Encoding.UTF8, true);
            CheckBytesExist(stream, 4);
            var metaData = new CompressionMetaData(binaryReader.ReadInt32());
            CheckBytesExist(stream, (long) 4 * metaData.BlockSizes.Length);
            for (int i = 0; i < metaData.BlockSizes.Length; i++)
            {
                var value = binaryReader.ReadInt32();
                if (value < 1)
                    throw new InvalidDataException(CompressionExceptionMessages.WrongFormat);
                metaData.BlockSizes[i] = value;
            }

            return metaData;
        }
        
        // ReSharper disable ParameterOnlyUsedForPreconditionCheck.Local
        private void CheckBytesExist(Stream stream, long bytesCount)
        {
            if (stream.Length - stream.Position < bytesCount)
                throw new InvalidDataException(CompressionExceptionMessages.WrongFormat);
        }
        // ReSharper restore ParameterOnlyUsedForPreconditionCheck.Local
    }
}