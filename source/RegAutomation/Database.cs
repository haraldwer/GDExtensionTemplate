using System;
using System.Collections.Generic;
using System.IO;

namespace RegAutomation
{
    public class DB
    {
        public class Func
        {
            public List<string> Params = new List<string>();
        }
        
        public class Prop
        {
            public string Type = ""; 
        }

        public class Type
        {
            public string Name = "";
            public string Content = "";
            public string Parent = "";
            
            public Dictionary<string, Func> Functions = new Dictionary<string, Func>();
            public Dictionary<string, Prop> Properties = new Dictionary<string, Prop>();
        }
        
        public static Dictionary<string, Type> Types = new Dictionary<string, Type>();

        public static void Load()
        {
            var files = Directory.GetFiles(Directory.GetCurrentDirectory(), "*.h", SearchOption.AllDirectories);
            foreach (var file in files)
            {
                if (file.Contains(".generated.h"))
                    continue;
                if (file.Contains("\\registration.h")) 
                    continue;
                if (file == "")
                    continue;
                Console.WriteLine("Reading file: " + file);
                string content = File.ReadAllText(file);
                if (content == "")
                    continue;
                Types[file] = new Type
                {
                    Content = content
                };
            } 
        }
    }
}