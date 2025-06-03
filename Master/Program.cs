using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Text;
using System.Threading;
using System.Diagnostics;
using System.Runtime.InteropServices;



namespace Master
{
    class Program
    {
        static ConcurrentDictionary<string, int> totalWordCounts = new ConcurrentDictionary<string, int>();

        static void Main(string[] args)
        {
            try
            {
                SetProcessorAffinity(2);
                Log("Master started.");

                Console.WriteLine("Master - Starting named pipe servers...");
                Thread agentAThread = new Thread(() => ListenOnPipe("pipeA"));
                Thread agentBThread = new Thread(() => ListenOnPipe("pipeB"));
                agentAThread.IsBackground = true;
                agentBThread.IsBackground = true;

                agentAThread.Start();
                agentBThread.Start();

                agentAThread.Join();
                agentBThread.Join();

                Console.WriteLine("\nMaster: Combined Word Counts:");
                foreach (var entry in totalWordCounts.OrderBy(e => e.Key))
                {
                    Console.WriteLine($"{entry.Key}: {entry.Value}");
                }

                Log("Master processing completed.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                Log("Fatal error in Master Main: " + ex.ToString());
            }

            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
            Log("Master exited.");
        }

        static void ListenOnPipe(string pipeName)
        {
            try
            {
                using (var pipeServer = new NamedPipeServerStream(pipeName, PipeDirection.In))
                {
                    Log($"Master: Waiting for connection on {pipeName}...");
                    Console.WriteLine($"Master: Waiting for connection on {pipeName}...");
                    pipeServer.WaitForConnection();

                    using (var reader = new StreamReader(pipeServer))
                    {
                        string data = reader.ReadToEnd();
                        Log($"Data received on {pipeName}:\n{data}");
                        Console.WriteLine($"\nData received on {pipeName}:\n{data}");

                        foreach (var line in data.Split('\n'))
                        {
                            var parts = line.Split(':');
                            if (parts.Length == 2 && int.TryParse(parts[1], out int count))
                            {
                                totalWordCounts.AddOrUpdate(parts[0], count, (_, existing) => existing + count);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                Log($"Error in ListenOnPipe({pipeName}): " + ex.ToString());
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
