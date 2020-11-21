using System.IO;
using CompressThis.Services.Interfaces;

namespace CompressThis.Services
{
    public class FileService : IFileService
    {
        public Stream OpenRead(string path) => new FileInfo(path).OpenRead();

        public Stream Create(string path) => new FileInfo(path).Create();

        public bool Exists(string path) => File.Exists(path);

        public bool CanCreate(string path)
        {
            try
            {
                using var s = Create(path);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}