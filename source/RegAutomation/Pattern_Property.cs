
namespace RegAutomation
{
    public class Pattern_Property : Pattern
    {
        public override void Generate(DB.Header header, GeneratedContent generated)
        {
            foreach (var macro in header.Compile.Macros)
            {
                if (macro.Name != "REG_PROPERTY")
                    continue;
                
                Console.WriteLine("REG_PROPERTY: " + Path.GetFileName(header.Name));
            }
        }

        private static (string get, string set) GetGetterSetter(string variant, string property)
        {
            switch (variant)
            {
                case "BOOL":
                {
                    if (property.StartsWith("is_"))
                        property = property.Substring(3);
                    return ("is_" + property, "set_" + property);
                }
            }
            return ("get_" + property, "set_" + property);
        }
    }
}