namespace CompressThis.Services.Interfaces
{
    public interface ICompressService
    {
        byte[] Compress(byte[] bytes);
        byte[] Decompress(byte[] bytes);
    }
}