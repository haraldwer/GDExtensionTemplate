using System.Text;
using RegAutomation.Core;

namespace RegAutomation
{
    public class Pattern_Enum : Pattern
    {
        public static void ProcessType(DB.Type type)
        {
            Console.WriteLine($"REG_ENUM: " + Path.GetFileName(type.FileName));
            foreach(EnumMacro macro in EnumParser.Instance.Parse(type.Content))
            {
                type.Enums[macro.Name] = new DB.Enum()
                {
                    IsBitField = macro.IsBitField,
                    Keys = macro.Keys,
                };
            }
        }

        public static void GenerateBindings(DB.Type type, StringBuilder bindings)
        {
            foreach (var @enum in type.Enums)
            {
                string isBitFieldString = @enum.Value.IsBitField ? "true" : "false";
                foreach (var key in @enum.Value.Keys)
                {
                    // ClassDB::bind_integer_constant(className, enumName, constName, constVal, isBitfield);
                    bindings.Append($"ClassDB::bind_integer_constant(\"{type.Name}\", \"{@enum.Key}\", \"{key}\", {key}, {isBitFieldString});\n\t");
                }
            }
        }
    }
}
