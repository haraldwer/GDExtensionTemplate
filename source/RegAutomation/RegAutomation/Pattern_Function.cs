using System.Text;
using System.Text.RegularExpressions;
using RegAutomation.Core;

namespace RegAutomation
{
    public class Pattern_Function : Pattern
    {
        public static void ProcessType(DB.Type type)
        {
            Console.WriteLine($"REG_FUNCTION: " + Path.GetFileName(type.FileName));
            foreach(FunctionMacro macro in FunctionParser.Instance.Parse(type.Content))
            {
                type.Functions[macro.Name] = new DB.Func()
                {
                    Params = macro.Params,
                    IsStatic = macro.IsStatic,
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