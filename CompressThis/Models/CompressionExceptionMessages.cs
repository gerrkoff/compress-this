namespace CompressThis.Models
{
    public static class CompressionExceptionMessages
    {
        public const string WrongFormat = "Unknown compression format, input file could be corrupted";
        public const string UnknownCompressException = "For some reason compression failed ¯\\_(ツ)_/¯";
        public const string TooManyBlocks = "Block count limit is reached, try to increase block size";
    }
}