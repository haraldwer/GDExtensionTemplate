using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace RegAutomation
{
    public class Pattern_Class : Pattern
    {
        public static void Process(KeyValuePair<string, DB.Header> header)
        {
            MatchCollection matches = FindMatches(header.Value.Content, "REG_CLASS");
            if (matches == null)
                return;
            List<int> newlineIndices = new List<int>();
            for (int i = 0; i < header.Value.Content.Length; ++i)
                if (header.Value.Content[i] == '\n') newlineIndices.Add(i);

            int searchStartIndex = 0;
            foreach (Match match in matches)
            {
                Console.WriteLine("REG_CLASS: " + Path.GetFileName(header.Key));

                string search = header.Value.Content.Substring(searchStartIndex, match.Index - searchStartIndex);
                int classFind = search.LastIndexOf("class ");
                if (classFind == -1)
                    continue;
                var classClosure = FindClosure(header.Value.Content, searchStartIndex + classFind);
                string classContent = header.Value.Content.Substring(classClosure.Item1, classClosure.Item2 - classClosure.Item1);
                //Console.WriteLine($"Class Closure: {classContent}");
                searchStartIndex = match.Index; // Start searching from the previous REG_CLASS, so multiple classes in same file is possible
                int lineNumber = newlineIndices.BinarySearch(match.Index);
                if(lineNumber < 0)
                {
                    // BinarySearch returns the negative of the next-largest element's index
                    // if the first newline's at position 42, and REG_CLASS is found at position 20,
                    // it is matched with the first newline, receiving a line number of 0
                    lineNumber = -lineNumber;
                }
                string classDef = search.Substring(classFind + "class ".Length);
                // TODO: Error handling when trying to register a class that doesn't (directly or indirectly) inherit from godot::Object (not supported)
                // TODO: Detect multiple inheritance (not supported) and throw an exception accordingly
                int indexOfColon = classDef.IndexOf(':');
                string nameEnd = classDef.Substring(0, indexOfColon).Trim();
                Console.WriteLine("Name: " + nameEnd);
                string inheritance = classDef.Substring(indexOfColon + 1);
                string[] tokens = inheritance.Substring(0, inheritance.IndexOf('{'))
                    .Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                // Assuming no multiple inheritance (not supported anyway), the last space-separated token is the parent class name
                // Access modifiers like public/protected might appear before the parent class name
                string parentName = tokens[tokens.Length - 1].Trim();
                Console.WriteLine("Inherits from: " + parentName);
                //int startIndex = match.Index + match.Value.Length + 1;
                //string sub = type.Value.Content.Substring(startIndex, type.Value.Content.Length - startIndex);
                //string name = sub.Substring(0, sub.IndexOf(')'));
                //type.Value.Name = name;

                header.Value.Types.Add(new DB.Type() { 
                    FileName = header.Key,
                    Name = nameEnd, 
                    RegClassLineNumber = lineNumber, 
                    ParentName = parentName, 
                    Content = classContent,
                });
            }
        }
        public static void GenerateIncludes(DB.Header header, ref string content)
        {
            string includes = "";
            foreach(var type in header.Types)
            {
                includes += $"#include \"{type.FileName}\"\n";
            }
            content = content.Replace("REG_INCLUDE", includes);
        }
        public static void Generate(DB.Type type, ref string content, ref string inject)
        {
            //content = content.Replace("REG_INCLUDE", "#include \"" + type.FileName + "\"");

            inject += $"\tGDCLASS({type.Name}, {type.ParentName})\n";
            inject += "protected: \n";
            inject += "\tstatic void _bind_methods();\n";
        }
        
        public static string GetReg()
        {
            string reg = "";
            foreach(var keyValue in DB.Headers)
                foreach (var type in keyValue.Value.Types)
                    if (type.Name != "")
                        reg += $"ClassDB::register_class<{type.Name}>();\n";
            return reg; 
        }

        public static string GetIncl()
        {
            string include = "";
            foreach (var header in DB.Headers)
                if (header.Value.Types.Count > 0)
                    include += $"#include \".generated/{header.Key.Replace('\\', '_').Replace(':', '_')}.generated.h\"\n";
            return include;
        }

        private static (int, int) FindClosure(string content, int startFrom)
        {
            int i;
            int closureCount = 0;
            int closureStart = startFrom;
            int closureEnd = -1;
            for(i = startFrom; i < content.Length; i++)
            {
                if (content[i] == '{') 
                {
                    if (closureCount == 0) closureStart = i;
                    closureCount++; 
                }
                else if (content[i] == '}')
                {
                    closureCount--;
                    if (closureCount == 0) 
                    {
                        closureEnd = i;
                        break;
                    }
                }
            }
            return (closureStart, closureEnd);
        }
    }
}