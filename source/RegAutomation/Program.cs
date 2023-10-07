using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;

namespace RegAutomation
{
    internal class Program
    {
        public static int Main(string[] args)
        {
            if (args.Length != 0)
                Directory.SetCurrentDirectory(args[0]);
            else
            {
                string dir = Directory.GetCurrentDirectory();
                int index = dir.IndexOf("source");
                if (index != -1)
                    Directory.SetCurrentDirectory(dir.Substring(0, index) + "source");
            }
            
            foreach (var dir in Directory.GetDirectories(Directory.GetCurrentDirectory()))
            {
                if (Directory.GetFiles(dir, "*.vcxproj").Length == 0)
                    continue;
                ProcessProject(dir);
            }

            if (!ErrorHandler.IsSuccess())
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("RegAutomation finished - Errors detected!");
                Console.ForegroundColor = ConsoleColor.White;
                return -1;
            }
            
            Console.WriteLine("RegAutomation finished");
            return 0;
        }

        static void ProcessProject(string directory)
        {
            Console.WriteLine("Processing project: " + directory);
            Directory.SetCurrentDirectory(directory);
            
            DB.Load();
            Process();
            Generate();
            GenerateRegister();
            
            Clean();            
        }

        private static Dictionary<string, string> WrittenFiles = new Dictionary<string, string>();

        static void Clean()
        {
            try
            {
                foreach (var file in Directory.GetFiles(Directory.GetCurrentDirectory(), "*.generated.h"))
                {
                    if (WrittenFiles.ContainsKey(file))
                        continue;
                    Console.WriteLine("Deleting untouched file: " + file);
                    File.Delete(file);
                }
            }
            catch (Exception e)
            {
                ErrorHandler.HandleError($"Clean failed", e, false);
            }
        }

        static void ThreadedIter(Action<KeyValuePair<string, DB.Header>> action)
        {
            using (ManualResetEvent resetEvent = new ManualResetEvent(false))
            {
                int toProcess = DB.Headers.Count;
                foreach (KeyValuePair<string, DB.Header> header in DB.Headers)
                {
                    ThreadPool.QueueUserWorkItem(x => {
                        
                        // Perform action
                        action((KeyValuePair<string, DB.Header>)x); 
                        
                        // Safely decrement the counter
                        if (Interlocked.Decrement(ref toProcess)==0)
                            resetEvent.Set();
                    }, header);
                }
                resetEvent.WaitOne();
            }
        }
        
        static void Process()
        {
            ThreadedIter(h =>
            {
                try
                {
                    // Headers
                    Pattern_Comment.ProcessHeader(h);
                    Pattern_Class.ProcessHeader(h);
                    
                    // Types
                    foreach (var t in h.Value.Types)
                    {
                        Pattern_Function.ProcessType(t);
                        Pattern_Enum.ProcessType(t);
                        Pattern_Property.ProcessType(t);
                    }
                }
                catch (Exception e)
                {
                    ErrorHandler.HandleError($"Failed to process {h.Key}", e);
                }
            });
        }
        
        static void Generate()
        {
            // Read template content
            const string templatePath = "reg_class.template";
            if (!File.Exists(templatePath))
                return;
            string template = File.ReadAllText(templatePath);
            
            ThreadedIter(header =>
            {
                try
                {
                    if (header.Key == "" || header.Value.Types.Count == 0 || header.Value.Content == "")
                        return;
                    
                    string content = template;
                    Pattern_Class.GenerateIncludes(header, ref content);
                    
                    StringBuilder bindClassMethods = new StringBuilder();
                    StringBuilder injects = new StringBuilder();
                    StringBuilder undefs = new StringBuilder();
                    foreach(var type in header.Value.Types)
                    {
                        string inject = $"#define REG_CLASS_LINE_{type.RegClassLineNumber}() \n";
                        StringBuilder bindings = new StringBuilder();
                        
                        Pattern_Class.GenerateInject(type, ref inject);
                        
                        Pattern_Function.GenerateBindings(type, bindings);
                        Pattern_Enum.GenerateBindings(type, bindings);
                        Pattern_Property.GenerateBindings(type, bindings, ref inject);

                        inject = inject.Replace("\n", "\\\n");
                        inject += "private: \n";
                        injects.Append(inject);

                        string bindMethod = $"void {type.Name}::_bind_methods()\n{{\n\t{bindings}\n}}\n\n";
                        bindClassMethods.Append(bindMethod);

                        string undef = $"#undef REG_CLASS_LINE_{type.RegClassLineNumber}\n";
                        undefs.Append(undef);
                    }

                    content = content.Replace("REG_UNDEF", undefs.ToString());
                    content = content.Replace("REG_INJECT", injects.ToString());
                    content = content.Replace("REG_BIND_CLASS_METHODS", bindClassMethods.ToString());
                    
                    // Write to file
                    string contentFile = header.Value.IncludeName + ".generated.h";
                    GenerateFile(contentFile, content);
                }
                catch (Exception e)
                {
                    ErrorHandler.HandleError($"Failed to generate {header.Key}", e);
                }

            });
        }
        
        static void GenerateRegister()
        {
            try
            {
                // Create reg and include code
                GenerateFile("reg_incl.generated.h", Pattern_Class.GetIncl());
                GenerateFile("reg_init.generated.h", Pattern_Class.GetReg());
                GenerateFile("reg_deinit.generated.h", "");
                File.SetLastWriteTime("extension.cpp", DateTime.Now);
            }
            catch (Exception e)
            {
                ErrorHandler.HandleError($"Failed to generate register", e);
            }
        }

        static void GenerateFile(string name, string content, bool gen = true)
        {
            const string dir = ".generated";
            string path = (gen ? dir + "\\" : "") + name;
            if (File.Exists(path))
            {
                string existingContent = File.ReadAllText(path);
                if (existingContent == content)
                    return; // File hasnt changed
            }
            else if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);
            Console.WriteLine("Generating file: " + name);
            File.WriteAllText(path, content);
            WrittenFiles[Directory.GetCurrentDirectory() + "\\" + path] = content;
        }
    }
}