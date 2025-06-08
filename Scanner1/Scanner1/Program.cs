using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;
using System.Threading;

class Program
{
    static void Main(string[] args)
    {
        string directoryPath = "";

        // ASK Until Folder PATH
        while (true)
        {
            Console.Write("Enter the directory path with txt files: ");
            directoryPath = Console.ReadLine();

            if (Directory.Exists(directoryPath))
            {
                break;  // Correct
            }
            else if (File.Exists(directoryPath))
            {
                Console.WriteLine("Wrong: Please enter a folder path, not a file.");
            }
            else
            {
                Console.WriteLine("Wrong: Path does not exist. Please try again.");
            }
        }

        // SET CPU to Core 1
        Process.GetCurrentProcess().ProcessorAffinity = (IntPtr)(1);

        Thread readThread = new Thread(() => ProcessFiles(directoryPath));
        readThread.Start();
        readThread.Join();  // Wait Until done

        Console.WriteLine("Done. Press Enter to exit.");
        Console.ReadLine();
    }

    static void ProcessFiles(string directoryPath)
    {
        try
        {
            string pipeName = "agent1";

            using (NamedPipeClientStream pipeClient = new NamedPipeClientStream(".", pipeName, PipeDirection.Out))
            {
                Console.WriteLine($"Connecting to pipe '{pipeName}'...");
                pipeClient.Connect();

                using (StreamWriter writer = new StreamWriter(pipeClient))
                {
                    writer.AutoFlush = true;

                    // Search ALL txt files in the folder.
                    string[] txtFiles = Directory.GetFiles(directoryPath, "*.txt");
                    foreach (var file in txtFiles)
                    {
                        string text = File.ReadAllText(file);
                        var wordCounts = CountWords(text);
                        string fileName = Path.GetFileName(file);

                        foreach (var kvp in wordCounts)
                        {
                            // Format: filename:word:count
                            string line = $"{fileName}:{kvp.Key}:{kvp.Value}";
                            writer.WriteLine(line);
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("ERROR! during processing or pipe communication.");
            Console.WriteLine(ex.Message);
        }
    }

    static Dictionary<string, int> CountWords(string text)
    {
        var counts = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
        char[] separators = new char[] { ' ', '\r', '\n', '\t', ',', '.', '!', '?', ';', ':', '-', '_', '\"', '\'' };

        string[] words = text.Split(separators, StringSplitOptions.RemoveEmptyEntries);
        foreach (var w in words)
        {
            if (counts.ContainsKey(w))
                counts[w]++;
            else
                counts[w] = 1;
        }
        return counts;
    }
}
