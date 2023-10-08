using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
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
            // Build newline indices so we can query the line number of any char
            List<int> newlineIndices = new List<int>();
            for (int i = 0; i < header.Value.Content.Length; i++)
                if (header.Value.Content[i] == '\n') newlineIndices.Add(i);
            int prevMatchIndex = 0;
            foreach (Match match in matches)
            {
                Console.WriteLine("REG_CLASS: " + Path.GetFileName(header.Key));
                if(TryFindClass(header.Value.Content, prevMatchIndex, match.Index, out string classDef, out string classContent))
                {
                    prevMatchIndex = match.Index; // Update the search interval
                    // Retrieve REG_CLASS's line number
                    int lineNumber = newlineIndices.BinarySearch(match.Index);
                    if (lineNumber < 0)
                    {
                        // BinarySearch returns the negative of the next-largest element's index
                        // if the first newline's at position 42, and REG_CLASS is found at position 20,
                        // it is matched with the first newline, receiving a line number of 0
                        lineNumber = -lineNumber;
                    }
                    // TODO: Error handling when trying to register a class that doesn't (directly or indirectly) inherit from godot::Object (not supported)
                    // TODO: Detect multiple inheritance (not supported) and throw an exception accordingly
                    int indexOfColon = classDef.IndexOf(':');
                    string nameEnd = classDef.Substring(0, indexOfColon).Trim();
                    string inheritance = classDef.Substring(indexOfColon + 1);
                    string[] tokens = inheritance.Substring(0, inheritance.IndexOf('{'))
                        .Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    // Assuming no multiple inheritance (not supported anyway), the last space-separated token is the parent class name
                    // Access modifiers like public/protected might appear before the parent class name
                    string parentName = tokens[tokens.Length - 1].Trim();

                    header.Value.Types.Add(new DB.Type()
                    {
                        FileName = header.Key,
                        Name = nameEnd,
                        RegClassLineNumber = lineNumber,
                        ParentName = parentName,
                        Content = classContent,
                    });
                }
            }
        }
        public static void GenerateIncludes(KeyValuePair<string, DB.Header> header, out string includes)
        {
            includes = $"#include \"{header.Key}\"\n";
        }
        public static void Generate(DB.Type type, StringBuilder inject)
        {
            inject.Append($"\tGDCLASS({type.Name}, {type.ParentName})\n");
            inject.Append("protected: \n");
            inject.Append("\tstatic void _bind_methods();\n");
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
            // TODO: Compute the correct include sequence to avoid compilation errors
            string include = "";
            foreach (var header in DB.Headers)
                if (header.Value.Types.Count > 0)
                    include += $"#include \".generated/{header.Value.IncludeName}.generated.h\"\n";
            return include;
        }
        // Try to find the pattern "class <classDef> { <classContent> }"
        // The interval [searchStart, searchEnd) is used to locate the class
        // classContent will contain the opening brace, but not the closing brace
        private static bool TryFindClass(string content, int searchStart, int searchEnd, out string classDef, out string classContent)
        {
            classDef = "";
            classContent = "";
            string search = content.Substring(searchStart, searchEnd - searchStart);
            int classFind = search.LastIndexOf("class ");
            if (classFind == -1)
                return false;
            classContent = GetClosure(content, searchStart + classFind);
            classDef = search.Substring(classFind + "class ".Length);
            return true;
        }
        // Retrieves the closest { ... } to startFrom inside content string
        private static string GetClosure(string content, int startFrom)
        {
            int i;
            int closureCount = 0;
            bool isInClosure = false;
            StringBuilder closure = new StringBuilder();
            for(i = startFrom; i < content.Length; i++)
            {
                if (content[i] == '{') 
                {
                    if (closureCount == 0) 
                    { 
                        isInClosure = true;
                    }
                    closureCount++; 
                }
                else if (content[i] == '}')
                {
                    closureCount--;
                    if (closureCount == 0) 
                    {
                        break;
                    }
                }
                if(isInClosure) closure.Append(content[i]);
            }
            return closure.ToString();
        }
    }
}