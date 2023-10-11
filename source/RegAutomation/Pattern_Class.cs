
namespace RegAutomation
{
    public class Pattern_Class : Pattern
    {
        public override void Generate(DB.Header header, GeneratedContent generated)
        {
            foreach (var macro in header.Compile.Macros)
            {
                if (macro.Name != "REG_CLASS")
                    continue;
                
                Console.WriteLine("REG_CLASS: " + Path.GetFileName(header.Name));
            }
        }
        
        public static string GetReg()
        {
            string reg = "";
            //reg += $"ClassDB::register_class<{type.Name}>();\n";
            return reg; 
        }

        public static string GetIncl()
        {
            // TODO: Compute the correct include sequence to avoid compilation errors
            string include = "";
            //include += $"#include \".generated/{includeFile}.generated.h\"\n";
            return include;
        }
    }
}