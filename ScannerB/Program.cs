using System;
using System.IO;
using System.IO.Pipes;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Diagnostics;
using System.Runtime.InteropServices;


namespace ScannerB
{
    class Program
    {
        static void Main(string[] args)
        {
            SetProcessorAffinity(1); // Core 1
            Console.WriteLine("ScannerB: Starting threaded scanning...");

            Thread scanThread = new Thread(ProcessDirectory);
            scanThread.Start();
            scanThread.Join();

            Console.WriteLine("ScannerB: Completed.");
            Console.ReadKey();
        }

        static void ProcessDirectory()
        {
            Console.WriteLine("ScannerB: Entered ProcessDirectory.");

            string directoryPath = "C:\\CSharp_File_Indexer\\ScannerB_Files";
            var wordCounts = new Dictionary<string, int>();

            if (Directory.Exists(directoryPath))
            {
                Console.WriteLine("ScannerB: Directory exists. Reading files...");
                var files = Directory.GetFiles(directoryPath, "*.txt");

                foreach (var file in files)
                {
                    Console.WriteLine($"ScannerB: Reading file: {Path.GetFileName(file)}");

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
            else
            {
                Console.WriteLine($"ScannerB: Directory not found: {directoryPath}");
                return;
            }

            Console.WriteLine("ScannerB: Finished counting words. Preparing to send...");

            string payload = string.Join("\n", wordCounts.Select(kvp => $"{kvp.Key}:{kvp.Value}"));

            using (var pipeClient = new NamedPipeClientStream(".", "pipeB", PipeDirection.Out))
            {
                pipeClient.Connect(); // could block if Master not listening
                using (var writer = new StreamWriter(pipeClient))
                {
                    writer.AutoFlush = true;
                    writer.WriteLine(payload);
                }
            }

            Console.WriteLine("ScannerB: Sent data to master.");
            Console.ReadKey();
        }
        static void SetProcessorAffinity(int core)
        {
            Process process = Process.GetCurrentProcess();
            IntPtr mask = new IntPtr(1 << core);
            process.ProcessorAffinity = mask;
            Console.WriteLine($"Set processor affinity to core {core}.");
        }

    }
}

