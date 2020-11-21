using System.IO;

namespace CompressThis.Services.Interfaces
{
    public interface IFileService
    {
        Stream OpenRead(string path);
        Stream Create(string path);
        bool Exists(string path);
        bool CanCreate(string path);
    }
}