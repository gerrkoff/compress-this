using System.IO;
using CompressThis.Models;

namespace CompressThis.Services.Interfaces
{
    public interface ICompressionMetaDataService
    {
        void WriteToStream(Stream stream, CompressionMetaData metaData);
        CompressionMetaData ReadFromStream(Stream stream);
    }
}