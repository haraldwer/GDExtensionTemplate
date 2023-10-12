using System.Diagnostics;

namespace GodotRun
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            string dir = Directory.GetCurrentDirectory();
            int index = dir.IndexOf("source");
            if (index != -1)
                dir = dir.Substring(0, index);
            
            var process = new Process();
            var startInfo = new ProcessStartInfo();
            startInfo.WindowStyle = ProcessWindowStyle.Normal;
            startInfo.FileName = dir + "godot.exe";
            startInfo.Arguments = "project/project.godot " + args;
            startInfo.RedirectStandardOutput = true;
            startInfo.RedirectStandardError = true;
            startInfo.UseShellExecute = false;
            startInfo.WorkingDirectory = dir; 
            process.StartInfo = startInfo;
            
            
            //process.OutputDataReceived += OutputHandler;
            //process.ErrorDataReceived += OutputHandler;

            process.Start();
            while (!process.StandardOutput.EndOfStream)
                Console.WriteLine(process.StandardOutput.ReadLine());
            //process.BeginOutputReadLine();
            //process.BeginErrorReadLine();
            process.WaitForExit();
        }

        static void OutputHandler(object sendingProcess, DataReceivedEventArgs outLine) 
        {
        }
    }
}