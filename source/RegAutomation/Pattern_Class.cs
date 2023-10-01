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
                int startIndex = match.Index + match.Value.Length + 1;
                string sub = type.Value.Content.Substring(startIndex, type.Value.Content.Length - startIndex);
                string name = sub.Substring(0, sub.IndexOf(')'));
                type.Value.Name = name;
            }
        }

        public static void Generate(KeyValuePair<string, DB.Type> type, ref string content, ref string inject)
        {
            content = content.Replace("REG_INCLUDE", "#include \"" + type.Key + "\"");

            inject += "\tGDCLASS(" + type.Value.Name + ", Node3D)\n";
            inject += "protected: \n";
            inject += "\tstatic void _bind_methods();\n";
        }
        
        public static string GetReg()
        {
            string reg = "";
            foreach (var type in DB.Types)
                if (type.Value.Name != "")
                    reg += "ClassDB::register_class<" + type.Value.Name + ">();\n";
            return reg; 
        }

        public static string GetIncl()
        {
            string include = "";
            foreach (var type in DB.Types)
                if (type.Value.Name != "")
                    include += "#include \".generated/" + type.Value.Name + ".generated.h\"\n";
            return include;
        }
    }
}