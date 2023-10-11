
namespace RegAutomation
{
    public class Pattern_Enum : Pattern
    {
        public override void Generate(DB.Header header, GeneratedContent generated)
        {
            foreach (var macro in header.Compile.Macros)
            {
                if (macro.Name != "REG_ENUM")
                    continue;
                
                Console.WriteLine("REG_ENUM: " + Path.GetFileName(header.Name));
                
                // Check for meta properties
                // REG_P_Bitfield
                
                //generated.Bindings.Append($"ClassDB::bind_integer_constant(\"{type.Name}\", \"{@enum.Key}\", \"{constantName}\", {constantName}, {isBitFieldString});\n");
            }
        }
    }
}
