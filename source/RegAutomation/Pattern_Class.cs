using System.Text;

namespace RegAutomation
{
    public class Pattern_Class : Pattern
    {
        public static void ProcessHeader(KeyValuePair<string, DB.Header> header)
        {
            var result = Parser.Parse(header.Value.Content, "REG_CLASS");
            foreach (Macro macro in result)
            {
                Console.WriteLine("REG_CLASS: " + Path.GetFileName(header.Key));
                
                string className = macro.OuterContext.Declaration[1];
                int parentTokenIndex = Parser.FindTokenMatch(macro.OuterContext.Declaration, s => s != "public", 3);
                string parentClass = macro.OuterContext.Declaration[parentTokenIndex];
                // TODO: Multiple inheritance
                
                header.Value.Types.Add(new DB.Type() { 
                    FileName = header.Key,
                    Name = className, 
                    Content = macro.OuterContext.Content,
                    ParentName = parentClass, 
                    RegClassLineNumber = macro.LineNumber, 
                });
            }
        }
        
        public static void GenerateIncludes(KeyValuePair<string, DB.Header> header, string content, out string includes)
        {
            includes = $"#include \"{header.Key}\"\n";
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

        public static string GetIncl()
        {
            // TODO: Compute the correct include sequence to avoid compilation errors
            string include = "";
            foreach (var header in DB.Headers)
                if (header.Value.Types.Count > 0)
                    include += $"#include \".generated/{header.Value.IncludeName}.generated.h\"\n";
            return include;
        }
    }
}