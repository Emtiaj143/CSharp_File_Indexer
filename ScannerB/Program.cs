using System;
using System.IO;

namespace ScannerB
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("ScannerB - Basic Directory Reader");
            string directoryPath = "C:\\CSharp_File_Indexer\\ScannerB_Files";

            if (Directory.Exists(directoryPath))
            {
                var files = Directory.GetFiles(directoryPath, "*.txt");
                foreach (var file in files)
                {
                    Console.WriteLine($"Reading file: {Path.GetFileName(file)}");
                    Console.WriteLine(File.ReadAllText(file));
                }
            }
            else
            {
                Console.WriteLine($"Directory not found: {directoryPath}");
            }

            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }
    }
}
