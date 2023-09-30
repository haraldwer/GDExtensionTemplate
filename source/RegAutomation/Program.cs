using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace RegAutomation
{
    internal class Program
    {
        public static void Main(string[] args)
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
            
            Console.WriteLine("Finished");
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
                foreach (var file in Directory.GetFiles(Directory.GetCurrentDirectory(), "*.generated.cpp"))
                {
                    if (WrittenFiles.ContainsKey(file))
                        continue;
                    Console.WriteLine("Deleting untouched file: " + file);
                    File.Delete(file);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        static void ThreadedIter(Action<KeyValuePair<string, DB.Type>> action)
        {
            using (ManualResetEvent resetEvent = new ManualResetEvent(false))
            {
                int toProcess = DB.Types.Count;
                foreach (KeyValuePair<string, DB.Type> type in DB.Types)
                {
                    ThreadPool.QueueUserWorkItem(x => {
                        
                        // Perform action
                        action((KeyValuePair<string, DB.Type>)x); 
                        
                        // Safely decrement the counter
                        if (Interlocked.Decrement(ref toProcess)==0)
                            resetEvent.Set();
                    }, type);
                }
                resetEvent.WaitOne();
            }
        }
        
        static void Process()
        {
            ThreadedIter(t =>
            {
                Pattern_Class.Process(t);
                Pattern_Function.Process(t);
                Pattern_Property.Process(t);
            });
        }
        
        static void Generate()
        {
            ThreadedIter(type =>
            {
                if (type.Key == "" || type.Value.Name == "" || type.Value.Content == "")
                    return;

                string content = "";

                // Add includes!
                content += "#include \"" + type.Key + "\"\n";
                content += "#include <godot_cpp/core/class_db.hpp>\n\n";
                content += "using namespace godot;\n\n";

                // Add functionality
                content += "void GDExample::_bind_methods() \n{\n";

                Pattern_Class.Generate(type, ref content);
                Pattern_Function.Generate(type, ref content);
                Pattern_Property.Generate(type, ref content);

                content += "}\n";

                // Write to file
                string file = type.Value.Name + ".generated.cpp";
                GenerateFile(file, content);
            });
        }
        
        static void GenerateRegister()
        {
            // Create reg and include code
            string reg = "";
            string include = "#pragma once \n\n";
            
            reg += Pattern_Class.GetReg();
            include += Pattern_Class.GetIncl(); 
            
            // Add reg to template
            const string templatePath = "extension.cpp.template";
            if (File.Exists(templatePath))
            {
                string template = File.ReadAllText(templatePath);
                template = template.Replace("REG_EXT_INITIALIZE()", reg);
                template = template.Replace("REG_EXT_DEINITIALIZE()", "");
                GenerateFile("extension.generated.cpp", template);
            }
            
            // Add includes
            GenerateFile("reg.generated.h", include);
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