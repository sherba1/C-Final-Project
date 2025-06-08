using System;
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;
using System.Threading.Tasks;

class Program
{
    static void Main(string[] args)
    {        
        // CPU Core 3 ( 8 in binary )
        Process.GetCurrentProcess().ProcessorAffinity = (IntPtr)(8);

        string pipe1 = args.Length > 0 ? args[0] : "agent1";
        string pipe2 = args.Length > 1 ? args[1] : "agent2";

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
            using var pipeServer = new NamedPipeServerStream(pipeName, PipeDirection.In, 1, PipeTransmissionMode.Message);
            pipeServer.WaitForConnection();

            using var reader = new StreamReader(pipeServer);
            while (reader.ReadLine() != null)
            {

            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error on pipe '{pipeName}': {ex.Message}");
        }
    }
}
