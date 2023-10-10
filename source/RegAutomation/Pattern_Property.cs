using System.Text;
using System.Text.RegularExpressions;

namespace RegAutomation
{
    public class Pattern_Property : Pattern
    {
        public static void ProcessType(DB.Type type)
        {
            MatchCollection matches = FindMatches(type.Content, "REG_PROPERTY");
            if (matches == null)
                return;

            foreach (Match match in matches)
            {
                Console.WriteLine("REG_PROPERTY: " + Path.GetFileName(type.FileName));

                string metaStart = type.Content.Substring(match.Index + match.Value.Length + 1);
                string meta = metaStart.Substring(0, metaStart.IndexOf(')'));
                
                int startIndex = match.Value.Length + match.Index + meta.Length + 2;
                string sub = type.Content.Substring(startIndex, type.Content.Length - startIndex);
                string content = sub.Substring(0, sub.IndexOf(';')).Trim();
                var declaration = content.Split(' ');
                if (declaration.Length < 2)
                {
                    Console.WriteLine("Incorrect declaration: " + declaration);
                    continue;
                }
                string var = declaration[0];
                string name = declaration[1];
                type.Properties[name] = new DB.Prop()
                {
                    Type = var,
                    Meta = meta
                };
            }
        }

        public static void GenerateBindings(DB.Type type, StringBuilder bindings, StringBuilder inject)
        {
            string propertyBindings = "";
            string functionBindings = "";
            
            foreach (var func in type.Properties)
            {
                string variant = func.Value.Type.ToUpper();
                if (variant == "")
                {
                    Console.WriteLine("Unknown type: " + func.Value.Type);
                    continue;
                }
                
                // Property bindings
                propertyBindings += $"ClassDB::add_property(\"{type.Name}\", ";
                string meta = func.Value.Meta == "" ? "" : $", {func.Value.Meta}"; 
                propertyBindings += $"PropertyInfo(Variant::{variant}, \"{func.Key}\"{meta}), ";
                
                var (get, set) = GetGetterSetter(variant, func.Key);
                propertyBindings += $"\"{set}\", ";
                propertyBindings += $"\"{get}\");\n\t";

                // Function generation
                inject.Append("\t" + func.Value.Type + " _gen_" + get + "() const { return " + func.Key + "; }\n");
                inject.Append("\tvoid _gen_" + set + "(" + func.Value.Type + " p) { " + func.Key + " = p; }\n");

                // Function bindings
                // The auto-generated C++ getters and setters still use the get_/set_ prefixes to avoid name collision.
                functionBindings += $"ClassDB::bind_method(D_METHOD(\"{get}\"), ";
                functionBindings += $"&{type.Name}::_gen_{get});\n\t";
                functionBindings += $"ClassDB::bind_method(D_METHOD(\"{set}\", \"p\"), ";
                functionBindings += $"&{type.Name}::_gen_{set});\n\t";
            }
            bindings.Append(functionBindings);
            bindings.Append(propertyBindings);
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