using CppAst;

namespace RegAutomation
{
    internal class Program
    {
        public static int Main(string[] args)
        {
            SetCurrentDir(args);
            
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

        static void SetCurrentDir(string[] args)
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

        // Track written files for cleaning step
        private static Dictionary<string, string> WrittenFiles = new Dictionary<string, string>();

        // Clean files that have not been touched
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

        // Helper function for iterating over all entries in DB using threadpool 
        static void ThreadedIter(Action<DB.Header> action)
        {
            using (ManualResetEvent resetEvent = new ManualResetEvent(false))
            {
                int toProcess = DB.Headers.Count;
                foreach (DB.Header header in DB.Headers)
                {
                    ThreadPool.QueueUserWorkItem(x => {
                        
                        // Perform action
                        action((DB.Header)x); 
                        
                        // Safely decrement the counter
                        if (Interlocked.Decrement(ref toProcess)==0)
                            resetEvent.Set();
                    }, header);
                }
                resetEvent.WaitOne();
            }
        }
        
        // Process the files 
        static void Process()
        {
            var options = new CppParserOptions();
            options.ParseMacros = true;
            options.Defines.Add("REG_IN_IDE");
            options.IncludeFolders.Add("..\\");
            options.IncludeFolders.Add("..\\..\\godot-cpp\\gen\\include");
            options.IncludeFolders.Add("..\\..\\godot-cpp\\include");
            options.IncludeFolders.Add("..\\..\\godot-cpp\\gdextension");
            options.AdditionalArguments.Add("-std=c++17");
            
            ThreadedIter(h =>
            {
                try
                {
                    var compile = CppParser.ParseFile(h.File, options);
                    if (compile.HasErrors)
                        throw new ArgumentException(compile.Diagnostics.ToString(), nameof(compile));
                    h.Compile = compile;
                }
                catch (Exception e)
                {
                    ErrorHandler.HandleError($"Failed to process {h.Name}", e);
                }
            });
        }
        
        static void Generate()
        {
            // Set up patterns
            List<Pattern> patterns = new List<Pattern>()
            {
                new Pattern_Class(),
                new Pattern_Comment(),
                new Pattern_Enum(),
                new Pattern_Function(),
                new Pattern_Property()
            };
            
            // Read template content
            const string templatePath = "reg_class.template";
            if (!File.Exists(templatePath))
                return;
            string template = File.ReadAllText(templatePath);
            
            ThreadedIter(header =>
            {
                try
                {
                    if (header.Compile == null)
                        return;

                    GeneratedContent generatedContent = new GeneratedContent();
                    generatedContent.Includes.Append($"#include \"{header.File}\"\n");
                    
                    foreach (var pattern in patterns)
                        pattern.Generate(header, generatedContent);
                    
                    // Replace keywords in the template file here
                    string content = template;
                    content = content.Replace("REG_INCLUDE", generatedContent.Includes.ToString());
                    content = content.Replace("REG_INJECT", generatedContent.Injects.ToString());
                    content = content.Replace("REG_UNDEF", generatedContent.Undefs.ToString());
                    content = content.Replace("REG_BINDINGS", generatedContent.Bindings.ToString());
                    
                    // Write to file
                    string contentFile = header.Name + ".generated.h";
                    GenerateFile(contentFile, content);
                }
                catch (Exception e)
                {
                    ErrorHandler.HandleError($"Failed to generate {header.Name}", e);
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