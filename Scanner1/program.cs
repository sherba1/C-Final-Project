using System;
using System.IO;
using System.IO.Pipes;
using System.Collections.Generic;
using System.Diagnostics;

class Program
{
    static void Main(string[] args)
    {
        Console.Title = "Scanner A";

        Process.GetCurrentProcess().ProcessorAffinity = (IntPtr)(1);

        Console.Write("Enter directory path with .txt files: ");
        string directoryPath = Console.ReadLine();

        var fileWordCounts = new Dictionary<string, Dictionary<string, int>>();

        foreach (string file in Directory.GetFiles(directoryPath, "*.txt"))
        {
            string fileName = Path.GetFileName(file);
            string content = File.ReadAllText(file);
            string[] words = content.Split(' ', '\n', '\r', '\t', '.', ',', ';', ':', '-', '!', '?');

            foreach (string word in words)
            {
                string w = word.Trim().ToLower();
                if (string.IsNullOrEmpty(w)) continue;

                if (!fileWordCounts.ContainsKey(fileName))
                    fileWordCounts[fileName] = new Dictionary<string, int>();

                if (!fileWordCounts[fileName].ContainsKey(w))
                    fileWordCounts[fileName][w] = 0;

                fileWordCounts[fileName][w]++;
            }
        }

        using (var pipe = new NamedPipeClientStream(".", "agent", PipeDirection.Out))
        using (var writer = new StreamWriter(pipe))
        {
            pipe.Connect();
            foreach (var fileEntry in fileWordCounts)
            {
                foreach (var pair in fileEntry.Value)
                {
                    writer.WriteLine($"{fileEntry.Key}:{pair.Key}:{pair.Value}");
                }
            }
            writer.Flush();
        }

        Console.WriteLine("Data sent to Master.");
    }
}
