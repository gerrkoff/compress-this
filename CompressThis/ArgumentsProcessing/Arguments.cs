namespace CompressThis.ArgumentsProcessing
{
    public class Arguments
    {
        public string InputFile { get; set; }
        public string OutputFile { get; set; }
        public bool IsCompressMode { get; set; }
        public bool IsVerbose { get; set; }
        public bool IsSingleThread { get; set; }
    }
}