namespace CompressThis.Models
{
    public class CompressionResult
    {
        public CompressionResult(long inputFileSize, long outputFileSize)
        {
            InputFileSize = inputFileSize;
            OutputFileSize = outputFileSize;
        }

        public long InputFileSize { get; }
        public long OutputFileSize { get; }
        public CompressionMetaData MetaData { get; set; }
    }
}