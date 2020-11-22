using System;
using CompressThis.ArgumentsProcessing;
using CompressThis.Models;

namespace CompressThis.Ui
{
    public class UiController
    {
        private const int ProgressBarSymbolCount = 50;
        private int _currentProgress;
        
        public void PrintProgressBar()
        {
            _currentProgress = 0;
            var space = new String('.', ProgressBarSymbolCount - 6);
            var bar = $"0%{space}100%";
            Console.WriteLine(bar);
        }

        public void UpdateProgressBar(double completed)
        {
            var newProgress = (int) Math.Round(ProgressBarSymbolCount * completed);
            var bar = new String('#', newProgress - _currentProgress);
            _currentProgress = newProgress;
            Console.Write(bar);
        }

        public void PrintCompressionResult(CompressionResult result, bool isVerbose)
        {
            Console.WriteLine();
            if (isVerbose)
                Console.Write(result.MetaData);
            else if (result.MetaData != null)
                Console.WriteLine($"     # of blocks: {result.MetaData.BlockSizes.Length}");
            Console.WriteLine($" Input file size: {result.InputFileSize}");
            Console.WriteLine($"Output file size: {result.OutputFileSize}");
        }
        
        public void PrintFinish()
        {
            Console.WriteLine();
        }

        public void PrintArguments(Arguments arguments)
        {
            if (arguments.IsSingleThread)
                Console.WriteLine("Warning: running in Single Thread mode");

            if (arguments.IsVerbose)
            {
                Console.WriteLine($"Input file:\t{arguments.InputFile}");
                Console.WriteLine($"Output file:\t{arguments.OutputFile}");
                Console.WriteLine($"Is compressing:\t{arguments.IsCompressMode}");
            }
        }

        public void PrintRuntimeException(Exception exception)
        {
            Console.WriteLine($"Failed: {exception.Message}");
            Console.WriteLine();
        }
        
        public void PrintPasingArgumentsException(Exception exception)
        {
            Console.WriteLine($"Failed to parse arguments: {exception.Message}");
            Console.WriteLine();
        }

        public void PrintHelp()
        {
            Console.WriteLine("Using: app [compress|decompress] [path-to-input-file] [path-to-output-file] [options]");
            Console.WriteLine();
            Console.WriteLine("options:");
            Console.WriteLine("  -s\t\t\tSingle Thread Mode");
            Console.WriteLine("  -v\t\t\tVerbose");
            Console.WriteLine("  --block-size [VALUE]\tBlock Size");
            Console.WriteLine();
        }
    }
}