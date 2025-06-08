using System;
using System.IO;
using System.IO.Pipes;
using System.Collections.Generic;
using System.Threading;
using System.Diagnostics;

class Program
{
    static Dictionary<string, Dictionary<string, int>> fileWordCounts = new Dictionary<string, Dictionary<string, int>>();
    static string directoryPath;

    static void Main(string[] args)
    {
        Console.Title = "Scanner A";

        Console.Write("Enter the directory path with .txt files: ");
        directoryPath = Console.ReadLine();

        Thread readerThread = new Thread(ReadFiles);
        Thread senderThread = new Thread(SendToMaster);

        readerThread.Start();
        readerThread.Join(); 

        senderThread.Start();
    }

    static void ReadFiles()
    {
        foreach (string file in Directory.GetFiles(directoryPath, "*.txt"))
        {
            string fileName = Path.GetFileName(file);
            string content = File.ReadAllText(file);
            string[] words = content.Split(' ', '\n', '\r', '\t', '.', ',', ';', ':', '-', '!', '?');

            foreach (string word in words)
            {
                string cleaned = word.Trim().ToLower();
                if (string.IsNullOrWhiteSpace(cleaned)) continue;

                if (!fileWordCounts.ContainsKey(fileName))
                    fileWordCounts[fileName] = new Dictionary<string, int>();

                if (!fileWordCounts[fileName].ContainsKey(cleaned))
                    fileWordCounts[fileName][cleaned] = 0;

                fileWordCounts[fileName][cleaned]++;
            }
        }
    }

    static void SendToMaster()
    {
        using (NamedPipeClientStream pipe = new NamedPipeClientStream(".", "agent1", PipeDirection.Out))
        using (StreamWriter writer = new StreamWriter(pipe))
        {
            pipe.Connect();
            foreach (var fileEntry in fileWordCounts)
            {
                foreach (var wordEntry in fileEntry.Value)
                {
                    string line = $"{fileEntry.Key}:{wordEntry.Key}:{wordEntry.Value}";
                    writer.WriteLine(line);
                }
            }
            writer.Flush();
        }

        Console.WriteLine("Data sent to Master.");
    }
}