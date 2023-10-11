
namespace RegAutomation
{
    public class Pattern_Function : Pattern
    {
        public override void Generate(DB.Header header, GeneratedContent generated)
        {
            foreach (var macro in header.Compile.Macros)
            {
                if (macro.Name != "REG_FUNCTION")
                    continue;
                
                Console.WriteLine("REG_FUNCTION: " + Path.GetFileName(header.Name));
            }
        }
    }
}