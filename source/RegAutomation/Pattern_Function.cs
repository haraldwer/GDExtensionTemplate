using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace RegAutomation
{
    public class Pattern_Function : Pattern
    {
        public static void Process(KeyValuePair<string, DB.Type> type)
        {
            MatchCollection matches = FindMatches(type.Value.Content, "REG_FUNCTION");
            if (matches == null)
                return;

            foreach (Match match in matches)
            {
                Console.WriteLine("REG_FUNCTION: " + Path.GetFileName(type.Key));
                int startIndex = match.Value.Length + match.Index + 2;
                string sub = type.Value.Content.Substring(startIndex, type.Value.Content.Length - startIndex);
                string content = sub.Substring(0, sub.IndexOf(';'));
                string func = content.Substring(0, content.IndexOf('(')).Trim();
                string name = func.Substring(func.IndexOf(' ') + 1);
                
                int paramIndex = content.IndexOf('(') + 1;
                int paramEndIndex = content.LastIndexOf(')');
                string parameters = content.Substring(paramIndex, paramEndIndex - paramIndex);
                List<string> paramResult = new List<string>();
                foreach (var parameter in parameters.Split(','))
                    paramResult.Add(parameter.Split(' ').Last().Trim());
                
                type.Value.Functions[name] = new DB.Func()
                {
                    Params = paramResult
                };
            }
        }

        public static void Generate(KeyValuePair<string, DB.Type> type, ref string content)
        {
            foreach (var func in type.Value.Functions)
            {
                content += "\tClassDB::bind_method(D_METHOD(\"" + func.Key + "\"";
                foreach (var param in func.Value.Params)
                    if (param != "")
                        content += ", \"" + param + "\"";
                content += "), &" + type.Value.Name + "::" + func.Key + ");\n";
            }
        }
    }
}