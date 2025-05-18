using System;
using System.IO;

namespace ScannerA
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("ScannerA - Basic Directory Reader");
            string directoryPath = "C:\\CSharp_File_Indexer\\ScannerA_Files";

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
