using System.Text;
using RegAutomation.Core;

namespace RegAutomation
{
    public class Pattern_Class : Pattern
    {
        public static void ProcessHeader(KeyValuePair<string, DB.Header> header)
        {
            Console.WriteLine("REG_CLASS: " + Path.GetFileName(header.Key));
            foreach(ClassMacro macro in ClassParser.Instance.Parse(header.Value.Content))
            {
                header.Value.Types.Add(new DB.Type() { 
                    FileName = header.Key,
                    Name = macro.ClassName, 
                    Content = macro.Content,
                    ParentName = macro.ParentClassName, 
                    RegClassLineNumber = macro.LineNumber, 
                });
            }
        }
        
        public static void GenerateInject(DB.Type type, StringBuilder inject)
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
    }
}