using System;
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;
using System.Threading.Tasks;

class Program
{
    static void Main(string[] args)
    {
        // Set CPU to Core 3 ( 8 in Binary )
        Process.GetCurrentProcess().ProcessorAffinity = (IntPtr)(8);

        string pipe1 = args.Length > 0 ? args[0] : "agent1";
        string pipe2 = args.Length > 1 ? args[1] : "agent2";

        Console.WriteLine("Master starting...");
        Task t1 = Task.Run(() => ListenPipe(pipe1));
        Task t2 = Task.Run(() => ListenPipe(pipe2));

        Task.WaitAll(t1, t2);

        Console.WriteLine("\nProcessing complete. Press Enter to exit.");
        Console.ReadLine();
    }

    static void ListenPipe(string pipeName)
    {
        try
        {
            using NamedPipeServerStream pipeServer = new NamedPipeServerStream(pipeName, PipeDirection.In, 1, PipeTransmissionMode.Message);

            Console.WriteLine($"Waiting for connection on pipe '{pipeName}'...");
            pipeServer.WaitForConnection();

            using StreamReader reader = new StreamReader(pipeServer);

            string line;
            while ((line = reader.ReadLine()) != null)
            {
                var parts = line.Split(':');
                if (parts.Length == 3)
                {
                    string fileName = parts[0];
                    string word = parts[1];
                    Console.WriteLine($"[{pipeName}] {fileName}: {word}");
                }
                else
                {
                    Console.WriteLine($"[{pipeName}] {line}");
                }
            }

            Console.WriteLine($"Pipe '{pipeName}' disconnected.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error on pipe '{pipeName}': {ex.Message}");
        }
    }
}
