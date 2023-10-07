using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace RegAutomation
{
    public class Pattern_Function : Pattern
    {
        public static void ProcessType(DB.Type type)
        {
            MatchCollection matches = FindMatches(type.Content, "REG_FUNCTION");
            if (matches == null)
                return;

            foreach (Match match in matches)
            {
                Console.WriteLine("REG_FUNCTION: " + Path.GetFileName(type.FileName));
                int startIndex = match.Value.Length + match.Index + 2;
                string sub = type.Content.Substring(startIndex, type.Content.Length - startIndex);
                string content = sub.Substring(0, sub.IndexOf(';'));
                string func = content.Substring(0, content.IndexOf('(')).Trim();
                string[] tokens = func.Split(' ');
                // The last token is always the function name
                string name = tokens[tokens.Length - 1];

                // TODO: Virtual
                // Use Contains here because static could be before or after the return type
                bool isStatic = tokens.Contains("static");
                
                int paramIndex = content.IndexOf('(') + 1;
                int paramEndIndex = content.LastIndexOf(')');
                string parameters = content.Substring(paramIndex, paramEndIndex - paramIndex);
                List<string> paramResult = new List<string>();
                foreach (var parameter in parameters.Split(','))
                    paramResult.Add(parameter.Split(' ').Last().Trim());
                
                type.Functions[name] = new DB.Func()
                {
                    Params = paramResult,
                    IsStatic = isStatic,
                };
            }
        }

        public static void GenerateBindings(DB.Type type, StringBuilder bindings)
        {
            foreach (var func in type.Functions)
            {
                if (func.Value.IsStatic)
                    bindings.Append($"ClassDB::bind_static_method(\"{type.Name}\", D_METHOD(\"{func.Key}\"");
                else
                    bindings.Append($"ClassDB::bind_method(D_METHOD(\"{func.Key}\"");
                foreach (var param in func.Value.Params)
                    if (param != "")
                        bindings.Append($", \"{param}\"");
                bindings.Append($"), &{type.Name}::{func.Key});\n\t");
            }
        }
    }
}