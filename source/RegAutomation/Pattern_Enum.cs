using System.Text;

namespace RegAutomation
{
    public class Pattern_Enum : Pattern
    {
        public static void ProcessType(DB.Type type)
        {
            var result = Parser.Parse(type.Content, "REG_ENUM");

            foreach (Macro macro in result)
            {
                Console.WriteLine("REG_ENUM: " + Path.GetFileName(type.FileName));
                
                int nameIndex = Parser.FindTokenMatch(macro.InnerContext.Tokens, s => s != "enum" && s != "class");
                string name = macro.InnerContext.Tokens[nameIndex];
                List<string> keys = Parser.FindMatchingTokens(macro.InnerContext.Tokens, s => s is "}" or ",", -1);
                
                type.Enums[name] = new DB.Enum()
                {
                    IsBitField = macro.Params.Content.ContainsKey("REG_P_Bitfield"),
                    Keys = keys
                };
            }
        }

        public static void GenerateBindings(DB.Type type, StringBuilder bindings)
        {
            foreach (var @enum in type.Enums)
            {
                string isBitFieldString = @enum.Value.IsBitField ? "true" : "false";
                foreach (var constantName in @enum.Value.Keys)
                {
                    bindings.Append($"ClassDB::bind_integer_constant(\"{type.Name}\", \"{@enum.Key}\", \"{constantName}\", {constantName}, {isBitFieldString});\n\t");
                }
            }
        }
    }
}
