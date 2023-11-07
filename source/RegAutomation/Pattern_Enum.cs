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

                List<Tuple<string, int>> values = new List<Tuple<string, int>>();
                int prevVal = -1;
                for (int i = 0; i < macro.InnerContext.Tokens.Count; i++)
                {
                    string token = macro.InnerContext.Tokens[i];
                    if (token is not ("{" or ",")) 
                        continue;
                    
                    // Next token might be val!
                    int keyIndex = i + 1; 
                    string key = macro.InnerContext.Tokens[keyIndex];
                    if (key == "}")
                        continue;

                    int value = prevVal + 1;
                    int valueIndex = i + 3; 
                    if (valueIndex < macro.InnerContext.Tokens.Count && 
                        macro.InnerContext.Tokens[keyIndex + 1] == "=")
                        value = Convert.ToInt32(macro.InnerContext.Tokens[valueIndex]);
                    
                    values.Add(new (key, value));
                    prevVal = value; 
                }
                
                type.Enums[name] = new DB.Enum()
                {
                    IsBitField = macro.Params.Content.ContainsKey("REG_P_Bitfield"),
                    Values = values
                };
            }
        }

        public static void GenerateBindings(DB.Type type, StringBuilder bindings)
        {
            foreach (var @enum in type.Enums)
            {
                string isBitFieldString = @enum.Value.IsBitField ? "true" : "false";
                foreach (var pair in @enum.Value.Values)
                {
                    // ClassDB::bind_integer_constant(className, enumName, constName, constVal, isBitfield);
                    bindings.Append($"ClassDB::bind_integer_constant(\"{type.Name}\", \"{@enum.Key}\", \"{pair.Item1}\", {pair.Item2}, {isBitFieldString});\n\t");
                }
            }
        }
    }
}
