using System;
using System.IO;
using System.IO.Pipes;
using System.Text;
using System.Collections.Generic;
using System.Linq;

namespace ScannerA
{
    class Program
    {
        static void Main(string[] args)
        {
            string directoryPath = "C:\\CSharp_File_Indexer\\ScannerA_Files";
            var wordCounts = new Dictionary<string, int>();

            if (Directory.Exists(directoryPath))
            {
                var files = Directory.GetFiles(directoryPath, "*.txt");
                foreach (var file in files)
                {
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
            }

            // Convert dictionary to string format
            string payload = string.Join("\n", wordCounts.Select(kvp => $"{kvp.Key}:{kvp.Value}"));

            using (var pipeClient = new NamedPipeClientStream(".", "pipeA", PipeDirection.Out))
            {
                pipeClient.Connect();
                using (var writer = new StreamWriter(pipeClient))
                {
                    writer.AutoFlush = true;
                    writer.WriteLine(payload);
                }
            }

            Console.WriteLine("ScannerA: Sent data to master.");
            Console.ReadKey();
        }
    }
}
