using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace RegAutomation
{
    public class Pattern_Enum : Pattern
    {
        public static void Process(DB.Type type)
        {
            MatchCollection matches = FindMatches(type.Content, "REG_ENUM");
            if (matches == null)
                return;

            foreach (Match match in matches)
            {
                Console.WriteLine("REG_ENUM: " + Path.GetFileName(type.FileName));
                int startIndex = match.Value.Length + match.Index + 2;
                string sub = type.Content.Substring(startIndex, type.Content.Length - startIndex);
                string content = sub.Substring(0, sub.IndexOf('}'));
                string decl = content.Substring(0, content.IndexOf('{'));
                // Skip "enum " if not anonymous enum, otherwise use "" as name
                string name = decl.Contains("enum ") ? decl.Substring(decl.IndexOf("enum ") + 5).Trim() : "";
                string enumContent = content.Substring(content.IndexOf('{') + 1);
                var @enum = new DB.Enum();
                foreach(string enumDecl in enumContent.Split(','))
                {
                    if (enumDecl.Trim().Length == 0) continue;
                    string[] tokens = enumDecl.Split('=');
                    if (tokens.Length == 0) continue;
                    else if(tokens.Length == 1)
                    {
                        if (@enum.KeyValues.Count == 0) 
                        { 
                            @enum.KeyValues.Add((tokens[0].Trim(), 0));
                        }
                        else 
                        {
                            var (_, previousNumbering) = @enum.KeyValues[@enum.KeyValues.Count - 1];
                            @enum.KeyValues.Add((tokens[0].Trim(), previousNumbering + 1)); 
                        }
                    }
                    else
                    {
                        @enum.KeyValues.Add((tokens[0].Trim(), int.Parse(tokens[1].Trim())));
                    }
                }
                
                type.Enums[name] = @enum;
            }
        }

        public static void GenerateBindings(DB.Type type, StringBuilder bindings)
        {
            foreach (var @enum in type.Enums)
            {
                foreach (var (constantName, constantValue) in @enum.Value.KeyValues)
                {
                    bindings.Append($"ClassDB::bind_integer_constant(\"{type.Name}\", \"{@enum.Key}\", \"{constantName}\", {constantValue}, false);\n");
                }
            }
        }
    }
}
