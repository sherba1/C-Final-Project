=======
# C# Final Project - File Content Indexer

## 🧠 Project Summary

This project was created for TEDU C# final. Since I’m still learning C#, I focused on the basics of multithreading, named pipes, and processor affinity.

The system is based on a **master-agent architecture**:
- Two scanners (Agent A and Agent B) read `.txt` files from a selected folder, extract words, and send them.
- A master process receives the data from both agents using named pipes and prints out the filename and each word from the text files.

## How to Run?
Just Open Each Program ( MasterProcess, Scanner1, Scanner2 )
Then Select a folder for your TXT files to be indexed.
Repeat Cycle with Scanner1&2.
Check Master for info.

## 🗂️ Applications

The system consists of 3 separate console applications:

### 👨‍💻 Scanner A / B
- You give it a folder containing `.txt` files.
- It reads the files, extracts the words, and sends them through a named pipe (agent1 / agent2).

### 🧠 Master
- Listens to both pipes (agent1 and agent2).
- Prints each received word along with its source `.txt` file.
- Uses multithreading to listen to both pipes at once.
- Uses processor affinity to run on Core 3.

### 💻 Technologies Used
- C# (.NET 8 Console Apps)
- NamedPipeServerStream / NamedPipeClientStream
- `Thread`, `Task`
- `Process.ProcessorAffinity`

## 📦 Sample Output

[agent1] example.txt: hello
[agent1] example.txt: world
[agent2] notes.txt: school
