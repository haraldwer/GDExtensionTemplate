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
            var result = Parser.Parse(type.Content, "REG_FUNCTION");

            foreach (Macro macro in result)
            {
                Console.WriteLine("REG_FUNCTION: " + Path.GetFileName(type.FileName));

                int paramStartIndex = Parser.FindTokenMatch(macro.InnerContext.Tokens, s => s is "(");
                string name = macro.InnerContext.Tokens[paramStartIndex - 1];
                List<string> paramResult = Parser.FindMatchingTokens(macro.InnerContext.Tokens, s => s is "," or ")", -1, paramStartIndex);
                
                type.Functions[name] = new DB.Func()
                {
                    Params = paramResult,
                    IsStatic = macro.InnerContext.Tokens.Contains("static"),
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