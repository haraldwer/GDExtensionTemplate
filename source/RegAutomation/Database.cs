using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace RegAutomation
{
    public class DB
    {
        public class Func
        {
            public List<string> Params = new List<string>();
            public bool IsStatic = false;
        }
        
        public class Prop
        {
            public string Type = "";
            public string Meta = "";
        }

        public class Enum
        {
            public List<(string, int)> KeyValues = new List<(string, int)>();
        }

        public class Type
        {
            public string Name = "";
            public string Content = "";
            
            public Dictionary<string, Func> Functions = new Dictionary<string, Func>();
            public Dictionary<string, Prop> Properties = new Dictionary<string, Prop>();
            public Dictionary<string, Enum> Enums = new Dictionary<string, Enum>();
        }
        
        public static Dictionary<string, Type> Types = new Dictionary<string, Type>();

        public static void Load()
        {
            var files = Directory.GetFiles(Directory.GetCurrentDirectory(), "*.h", SearchOption.AllDirectories);
            foreach (var file in files)
            {
                if (file.Contains(".generated.h") || file.Contains(".injected.h"))
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
                    Content = StripComments(content)
                };
            }
        }
        private static string StripComments(string content)
        {
            // Algorithm to remove [// ... \n) and [/* ... */] from content in linear time
            var result = new StringBuilder();
            int scanLimit = content.Length;
            // We look-ahead one char every iteration, so the scan limit should retract by 1
            scanLimit -= 1;
            const int SCAN_BEGIN   = 0; // Scan for // or /* (Not in commented section)
            const int SCAN_NEWLINE = 1; // Scan for \n (In commented section)
            const int SCAN_STAR    = 2; // Scan for */ (In commented section)
            int state = SCAN_BEGIN;
            for (int i = 0; i < scanLimit; ++i)
            {
                switch (state)
                {
                    case SCAN_BEGIN:
                        if (content[i] == '/' && content[i + 1] == '/')
                        {
                            state = SCAN_NEWLINE;
                            ++i; // Skip both tokens
                        }
                        else if (content[i] == '/' && content[i + 1] == '*')
                        {
                            state = SCAN_STAR;
                            ++i; // Skip both tokens
                        }
                        else
                            result.Append(content[i]);
                        break;
                    case SCAN_NEWLINE:
                        if (content[i] == '\n')
                        {
                            state = SCAN_BEGIN;
                            result.Append(content[i]); // Newlines should be preserved!
                        }
                        break;
                    case SCAN_STAR:
                        if (content[i] == '*' && content[i + 1] == '/') 
                        { 
                            state = SCAN_BEGIN;
                            ++i; // Skip both tokens
                        }
                        break;
                }
            }
            // Get the last char if not in commented section
            if (state == SCAN_BEGIN) result.Append(content[content.Length - 1]);

            return result.ToString();
        }
    }
}