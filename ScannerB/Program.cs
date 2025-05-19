using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;

namespace ScannerB
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("ScannerB - Directory Reader and Word Counter");
            string directoryPath = "C:\\CSharp_File_Indexer\\ScannerB_Files";

            if (Directory.Exists(directoryPath))
            {
                var wordCounts = new Dictionary<string, int>();

                var files = Directory.GetFiles(directoryPath, "*.txt");
                foreach (var file in files)
                {
                    Console.WriteLine($"Reading file: {Path.GetFileName(file)}");

                    string[] words = File.ReadAllText(file)
                                         .ToLower()
                                         .Split(new char[] { ' ', '\t', '\n', '\r', '.', ',', '!', '?' }, StringSplitOptions.RemoveEmptyEntries);

                    foreach (var word in words)
                    {
                        if (wordCounts.ContainsKey(word))
                            wordCounts[word]++;
                        else
                            wordCounts[word] = 1;
                    }
                }

                // Print the word counts for testing
                Console.WriteLine("\nWord Counts:");
                foreach (var entry in wordCounts.OrderBy(e => e.Key))
                {
                    Console.WriteLine($"{entry.Key}: {entry.Value}");
                }
            }
            else
            {
                Console.WriteLine($"Directory not found: {directoryPath}");
            }

            Console.WriteLine("\nPress any key to exit...");
            Console.ReadKey();
        }
    }
}
