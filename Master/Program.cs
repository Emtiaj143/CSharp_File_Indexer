using System;
using System.IO;
using System.IO.Pipes;
using System.Text;
using System.Threading;

namespace Master
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Master - Starting named pipe servers...");

            Thread agentAThread = new Thread(() => ListenOnPipe("pipeA"));
            Thread agentBThread = new Thread(() => ListenOnPipe("pipeB"));

            agentAThread.Start();
            agentBThread.Start();

            agentAThread.Join();
            agentBThread.Join();

            Console.WriteLine("Master: Finished receiving data.");
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
                }
            }
        }
    }
}
