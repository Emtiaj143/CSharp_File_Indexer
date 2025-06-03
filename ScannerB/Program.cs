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
            try
            {
                SetProcessorAffinity(0); 
                Log("Scanner started.");

                Console.WriteLine("Starting threaded scanning...");
                Thread scanThread = new Thread(ProcessDirectory);
                scanThread.Start();
                scanThread.Join();

                Log("Scanning completed.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                Log("Fatal error in Main: " + ex.ToString());
            }

            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
            Log("Scanner exited.");
        }

        static void ProcessDirectory()
        {
            try
            {
                Log("Entered ProcessDirectory.");

                string directoryPath = "C:\\CSharp_File_Indexer\\ScannerB_Files";
                var wordCounts = new Dictionary<string, int>();

                if (Directory.Exists(directoryPath))
                {
                    Log("Directory exists: " + directoryPath);
                    var files = Directory.GetFiles(directoryPath, "*.txt");

                    foreach (var file in files)
                    {
                        Log("Reading file: " + file);
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
                    Log("Directory not found: " + directoryPath);
                    Console.WriteLine("Directory not found.");
                    return;
                }

                Log("Finished counting. Preparing to send.");

                string payload = string.Join("\n", wordCounts.Select(kvp => $"{kvp.Key}:{kvp.Value}"));

                using (var pipeClient = new NamedPipeClientStream(".", "pipeB", PipeDirection.Out))
                {
                    pipeClient.Connect(5000);
                    using (var writer = new StreamWriter(pipeClient))
                    {
                        writer.AutoFlush = true;
                        writer.WriteLine(payload);
                    }
                }

                Log("Data sent to master.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                Log("Error in ProcessDirectory: " + ex.ToString());
            }
        }

        static void Log(string message)
        {
            string logFile = "Logs/log.txt";
            Directory.CreateDirectory("Logs");
            File.AppendAllText(logFile, $"{DateTime.Now}: {message}\n");
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

