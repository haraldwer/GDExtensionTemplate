using System;
using System.Diagnostics;
using System.IO;
using System.Threading;

namespace RegAutomation
{
    public class ErrorHandler
    {
        private static bool Success = true;
        private const bool PrintEx = true;

        public static void HandleError(string message, Exception e, bool breaking = true)
        {
            Console.ForegroundColor = ConsoleColor.Red; // TODO: Race condition for console color
            
            if (breaking)
                Success = false;

            message = message.Replace(Directory.GetCurrentDirectory(), "");
            
            StackTrace st = new StackTrace(e, true);
            StackFrame[] frames = st.GetFrames();
            if (frames != null && frames.Length > 0)
            {
                string file = frames[0].GetFileName(); // TODO: This is sometimes empty
                string func = frames[0].GetMethod().Name;
                int line = frames[0].GetFileLineNumber();
                
                Console.WriteLine($"Error - {file} : {func} : {line} - {message}");
            }
            else
            {
                Console.WriteLine($"Unknown error - {message}");
            }
            if (PrintEx)
                Console.WriteLine(e);
            
            Console.ForegroundColor = ConsoleColor.White;
        }

        public static bool IsSuccess()
        {
            return Success;
        }
    }
}