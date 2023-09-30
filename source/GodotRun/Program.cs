using System;
using System.Diagnostics;
using System.IO;

namespace GodotRun
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine(Directory.GetCurrentDirectory());

            string dir = Directory.GetCurrentDirectory();
            int index = dir.IndexOf("source");
            if (index != -1)
                dir = dir.Substring(0, index);
            
            var process = new Process();
            var startInfo = new ProcessStartInfo();
            startInfo.WindowStyle = ProcessWindowStyle.Normal;
            startInfo.FileName = "cmd.exe";
            startInfo.Arguments = "/V /C godot.exe project/project.godot " + args;
            startInfo.RedirectStandardOutput = true;
            startInfo.RedirectStandardError = true;
            startInfo.UseShellExecute = false;
            startInfo.WorkingDirectory = dir; 
            process.StartInfo = startInfo;
            
            process.OutputDataReceived += OutputHandler;
            process.ErrorDataReceived += OutputHandler;
            
            process.Start();
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();
            process.WaitForExit();
        }

        static void OutputHandler(object sendingProcess, DataReceivedEventArgs outLine) 
        {
            Console.WriteLine(outLine.Data);
        }
    }
}