using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Text;
using System.Threading;


namespace Master
{
    class Program
    {
        static ConcurrentDictionary<string, int> totalWordCounts = new ConcurrentDictionary<string, int>();

        static void Main(string[] args)
        {
            Console.WriteLine("Master - Starting named pipe servers...");

            Thread agentAThread = new Thread(() => ListenOnPipe("pipeA"));
            Thread agentBThread = new Thread(() => ListenOnPipe("pipeB"));
            agentAThread.IsBackground = true;
            agentBThread.IsBackground = true;

            agentAThread.Start();
            agentBThread.Start();

            

            Console.WriteLine("\nMaster: Combined Word Counts:");
            foreach (var entry in totalWordCounts.OrderBy(e => e.Key))
            {
                Console.WriteLine($"{entry.Key}: {entry.Value}");
            }

            Console.WriteLine("\nMaster: Finished processing.");
            Console.ReadKey();
        }

        static void ListenOnPipe(string pipeName)
        {
            using (var pipeServer = new NamedPipeServerStream(pipeName, PipeDirection.In))
            {
                Console.WriteLine($"Master: Waiting for connection on {pipeName}...");
                pipeServer.WaitForConnection();
                using (var reader = new StreamReader(pipeServer))
                {
                    string data = reader.ReadToEnd();
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
    }
}
