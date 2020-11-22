using System;
using System.IO;
using CompressThis.ArgumentsProcessing;
using CompressThis.Compressors;
using CompressThis.Models;
using CompressThis.Services;
using CompressThis.Ui;

namespace CompressThis
{
    public static class Program
    {
        private static readonly UiController Ui = new UiController();
        
        public static int Main(string[] args)
        {
            var isParseArgumentsSuccess = ParseArguments(args, out Arguments arguments);
            if (!isParseArgumentsSuccess)
                return 1;

            ICompressor compressor = CreateCompressor(arguments);
            
            var isRunSuccess = Run(compressor, arguments);
            if (!isRunSuccess)
                return 1;

            Ui.PrintFinish();
            return 0;
        }

        private static bool ParseArguments(string[] args, out Arguments arguments)
        {
            arguments = null;
            try
            {
                arguments = new ArgumentsParser(new FileService()).Parse(args);
            }
            catch (ArgumentException e)
            {
                Ui.PrintPasingArgumentsException(e);
                Ui.PrintHelp();
                return false;
            }
            
            Ui.PrintArguments(arguments);
            return true;
        }

        private static ICompressor CreateCompressor(Arguments arguments)
        {
            if (arguments.IsSingleThread)
            {
                return new SingleThreadCompressor();
            }

            var compressor = new MultiThreadBlockCompressor(
                new CompressService(),
                new FileService(),
                new CompressionMetaDataService(),
                new ThreadPool());
            if (arguments.BlockSize.HasValue)
                compressor.BlockSize = arguments.BlockSize.Value;
            compressor.ProgressUpdated += (sender, eventArgs) => Ui.UpdateProgressBar(eventArgs.Completed);
            Ui.PrintProgressBar();
            return compressor;
        }

        private static bool Run(ICompressor compressor, Arguments arguments)
        {
            CompressionResult result;
            try
            {
                result = arguments.IsCompressMode
                    ? compressor.Compress(arguments.InputFile, arguments.OutputFile)
                    : compressor.Decompress(arguments.InputFile, arguments.OutputFile);
            }
            catch (InvalidDataException e)
            {
                Ui.PrintRuntimeException(e);
                return false;
            }
            
            Ui.PrintCompressionResult(result, arguments.IsVerbose);
            return true;
        }
    }
}