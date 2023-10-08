using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace RegAutomation
{
    public class Pattern_Enum : Pattern
    {
        public static void ProcessType(DB.Type type)
        {
            MatchCollection enumMatches = FindMatches(type.Content, "REG_ENUM");
            if (enumMatches != null)
                foreach (Match match in enumMatches)
                {
                    Console.WriteLine("REG_ENUM: " + Path.GetFileName(type.FileName));
                    ProcessEnum(type, match, false);
                }
            MatchCollection bitFieldMatches = FindMatches(type.Content, "REG_BITFIELD");
            if(bitFieldMatches != null)
                foreach (Match match in bitFieldMatches)
                {
                    Console.WriteLine("REG_BITFIELD: " + Path.GetFileName(type.FileName));
                    ProcessEnum(type, match, true);
                }
        }

        private static void ProcessEnum(DB.Type type, Match match, bool asBitField)
        {
            int startIndex = match.Value.Length + match.Index + 2;
            string sub = type.Content.Substring(startIndex, type.Content.Length - startIndex);
            string content = sub.Substring(0, sub.IndexOf('}'));
            string decl = content.Substring(0, content.IndexOf('{'));
            // Skip "enum " if not anonymous enum, otherwise use "" as name
            string name = decl.Contains("enum ") ? decl.Substring(decl.IndexOf("enum ") + 5).Trim() : "";
            string enumContent = content.Substring(content.IndexOf('{') + 1);
            var @enum = new DB.Enum();
            foreach (string enumDecl in enumContent.Split(','))
            {
                if (enumDecl.Trim().Length == 0) 
                    continue;
                string[] tokens = enumDecl.Split('=');
                if (tokens.Length == 0) 
                    continue;
                @enum.Keys.Add(tokens[0].Trim());
            }
            @enum.IsBitField = asBitField;
            type.Enums[name] = @enum;
        }

        public static void GenerateBindings(DB.Type type, StringBuilder bindings)
        {
            foreach (var @enum in type.Enums)
            {
                string isBitFieldString = @enum.Value.IsBitField ? "true" : "false";
                foreach (var constantName in @enum.Value.Keys)
                {
                    bindings.Append($"ClassDB::bind_integer_constant(\"{type.Name}\", \"{@enum.Key}\", \"{constantName}\", {constantName}, {isBitFieldString});\n");
                }
            }
        }
    }
}
