﻿using System.Diagnostics;
using System.IO.Pipes;
using System.Reflection;

namespace RandomFight
{
    internal class Program
    {
        private const string _binDebugNetPath = @"bin\Debug\net8.0";
        static void Main(string[] args)
        {
            var assemblyDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var randomFightHeadDirectory = GetRandomFightHeadDirectory(assemblyDirectory!);

            var playerOneExePath = @$"{randomFightHeadDirectory}\PlayerOne\{_binDebugNetPath}\PlayerOne.exe";
            var playerTwoExePath = @$"{randomFightHeadDirectory}\PlayerTwo\{_binDebugNetPath}\PlayerTwo.exe";

            var processStartInfoOne = new ProcessStartInfo
            {
                FileName = playerOneExePath,
                WorkingDirectory = Path.GetDirectoryName(playerOneExePath),
                UseShellExecute = true,
                CreateNoWindow = false 
            };

            var processStartInfoTwo = new ProcessStartInfo
            {
                FileName = playerTwoExePath,
                WorkingDirectory = Path.GetDirectoryName(playerTwoExePath),
                UseShellExecute = true,
                CreateNoWindow = false
            };

            Process.Start(processStartInfoOne);
            Thread.Sleep(2000);
            Process.Start(processStartInfoTwo);

            Console.WriteLine("Match In Progress...");

            var namedPipeClientStream = new NamedPipeServerStream("GameHost");           
            using var streamReader = new StreamReader(namedPipeClientStream);

            namedPipeClientStream.WaitForConnection();
            var matchResult = streamReader.ReadLine();
            Console.WriteLine(matchResult);

        }

        private static string GetRandomFightHeadDirectory(string assemblyDirectory)
        {
            while(Path.GetFileName(assemblyDirectory) != "RandomFight")
            {
                assemblyDirectory = Directory.GetParent(assemblyDirectory)!.FullName;
            }

            return Directory.GetParent(assemblyDirectory).FullName;
        }
    }
}
