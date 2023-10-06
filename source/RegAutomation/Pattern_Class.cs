using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace RegAutomation
{
    public class Pattern_Class : Pattern
    {
        public static void Process(KeyValuePair<string, DB.Type> type)
        {
            MatchCollection matches = FindMatches(type.Value.Content, "REG_CLASS");
            if (matches == null)
                return;

            foreach (Match match in matches)
            {
                Console.WriteLine("REG_CLASS: " + Path.GetFileName(type.Key));

                string search = type.Value.Content.Substring(0, match.Index);
                int classFind = search.LastIndexOf("class ");
                if (classFind == -1)
                    continue;

                string classDef = search.Substring(classFind + "class ".Length);
                // TODO: Error handling when trying to register a class that doesn't (directly or indirectly) inherit from godot::Object (not supported)
                // TODO: Detect multiple inheritance (not supported) and throw an exception accordingly
                int indexOfColon = classDef.IndexOf(':');
                string nameEnd = classDef.Substring(0, indexOfColon).Trim();
                type.Value.Name = nameEnd;
                string inheritance = classDef.Substring(indexOfColon + 1);
                string[] tokens = inheritance.Substring(0, inheritance.IndexOf('{'))
                    .Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                // Assuming no multiple inheritance (not supported anyway), the last space-separated token is the parent class name
                // Access modifiers like public/protected might appear before the parent class name
                // No need to trim here because we already split on whitespaces
                string parentName = tokens[tokens.Length - 1];
                type.Value.ParentName = parentName;
                
                //int startIndex = match.Index + match.Value.Length + 1;
                //string sub = type.Value.Content.Substring(startIndex, type.Value.Content.Length - startIndex);
                //string name = sub.Substring(0, sub.IndexOf(')'));
                //type.Value.Name = name;
            }
        }

        public static void Generate(KeyValuePair<string, DB.Type> type, ref string content, ref string inject)
        {
            content = content.Replace("REG_INCLUDE", "#include \"" + type.Key + "\"");

            inject += $"\tGDCLASS({type.Value.Name}, {type.Value.ParentName})\n";
            inject += "protected: \n";
            inject += "\tstatic void _bind_methods();\n";
        }
        
        public static string GetReg()
        {
            string reg = "";
            foreach (var type in DB.Types)
                if (type.Value.Name != "")
                    reg += $"ClassDB::register_class<{type.Value.Name}>();\n";
            return reg; 
        }

        public static string GetIncl()
        {
            string include = "";
            foreach (var type in DB.Types)
                if (type.Value.Name != "")
                    include += $"#include \".generated/{type.Value.Name}.generated.h\"\n";
            return include;
        }
    }
}