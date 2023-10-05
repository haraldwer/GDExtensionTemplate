using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace RegAutomation
{
    public class Pattern_Enum : Pattern
    {
        public static void Process(KeyValuePair<string, DB.Type> type)
        {
            MatchCollection matches = FindMatches(type.Value.Content, "REG_ENUM");
            if (matches == null)
                return;

            foreach (Match match in matches)
            {
                Console.WriteLine("REG_ENUM: " + Path.GetFileName(type.Key));
                int startIndex = match.Value.Length + match.Index + 2;
                string sub = type.Value.Content.Substring(startIndex, type.Value.Content.Length - startIndex);
                string content = sub.Substring(0, sub.IndexOf('}'));
                string name = content.Substring(0, content.IndexOf('{')).Trim();
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
                
                type.Value.Enums[name] = @enum;
            }
        }

        public static void Generate(KeyValuePair<string, DB.Type> type, ref string content, ref string inject)
        {
            // ClassDB::bind_integer_constant("class", "enum", "constant", <int value>, <bool bitfield>)
            var bindings = new StringBuilder();
            foreach(var @enum in type.Value.Enums)
            {
                foreach(var(constantName, constantValue) in @enum.Value.KeyValues)
                {
                    bindings.Append($"ClassDB::bind_integer_constant(\"{type.Value.Name}\", \"{@enum.Key}\", \"{constantName}\", {constantValue}, false);\n");
                }
            }
            content = content.Replace("REG_BIND_ENUMS", bindings.ToString());
        }
    }
}
