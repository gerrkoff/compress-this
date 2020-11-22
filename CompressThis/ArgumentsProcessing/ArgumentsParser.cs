using System;
using CompressThis.Services.Interfaces;

namespace CompressThis.ArgumentsProcessing
{
    public class ArgumentsParser
    {
        private readonly IFileService _fileService;

        public ArgumentsParser(IFileService fileService)
        {
            _fileService = fileService;
        }

        public Arguments Parse(string[] args)
        {   
            if (args == null)
                throw new ArgumentException("Arguments cannot be null");
            if (args.Length < 3)
                throw new ArgumentException("Three mandatory arguments are expected");
            if (!string.Equals(args[0], "compress") && !string.Equals(args[0], "decompress"))
                throw new ArgumentException("Compressing mode value should be either 'compress' or 'decompress'");
            if (!_fileService.Exists(args[1]))
                throw new ArgumentException("Input file not found");
            if (!_fileService.CanCreate(args[2]))
                throw new ArgumentException("Cannot create output file");

            var arguments = new Arguments
            {
                IsCompressMode = args[0] == "compress",
                InputFile = args[1],
                OutputFile = args[2],
                IsVerbose = false,
                IsSingleThread = false,
            };

            for (int i = 3; i < args.Length; i++)
            {
                if (string.Equals(args[i], "-s"))
                    arguments.IsSingleThread = true;
                else if (string.Equals(args[i], "-v"))
                    arguments.IsVerbose = true;
                else if (string.Equals(args[i], "--block-size"))
                {
                    if (i + 1 == args.Length || !int.TryParse(args[i + 1], out int blockSize))
                        throw new ArgumentException("Block size value is expected");

                    arguments.BlockSize = blockSize;
                }
            }

            return arguments;
        }
    }
}