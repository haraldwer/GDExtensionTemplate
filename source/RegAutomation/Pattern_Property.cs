using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace RegAutomation
{
    public class Pattern_Property : Pattern
    {
        public static void Process(KeyValuePair<string, DB.Type> type)
        {
            MatchCollection matches = FindMatches(type.Value.Content, "REG_PROPERTY");
            if (matches == null)
                return;

            foreach (Match match in matches)
            {
                Console.WriteLine("REG_PROPERTY: " + Path.GetFileName(type.Key));

                string metaStart = type.Value.Content.Substring(match.Index + match.Value.Length + 1);
                string meta = metaStart.Substring(0, metaStart.IndexOf(')'));
                
                int startIndex = match.Value.Length + match.Index + meta.Length + 2;
                string sub = type.Value.Content.Substring(startIndex, type.Value.Content.Length - startIndex);
                string content = sub.Substring(0, sub.IndexOf(';')).Trim();
                var declaration = content.Split(' ');
                if (declaration.Length < 2)
                {
                    Console.WriteLine("Incorrect declaration: " + declaration);
                    continue;
                }
                string var = declaration[0];
                string name = declaration[1];
                type.Value.Properties[name] = new DB.Prop()
                {
                    Type = var,
                    Meta = meta
                };
            }
        }

        public static void Generate(KeyValuePair<string, DB.Type> type, ref string content, ref string inject)
        {
            string propertyBindings = "";
            string functionBindings = "";
            
            foreach (var func in type.Value.Properties)
            {
                string variant = func.Value.Type.ToUpper();
                if (variant == "")
                {
                    Console.WriteLine("Unknown type: " + func.Value.Type);
                    continue;
                }
                
                // Property bindings
                propertyBindings += $"ClassDB::add_property(\"{type.Value.Name}\", ";
                string meta = func.Value.Meta == "" ? "" : $", {func.Value.Meta}"; 
                propertyBindings += $"PropertyInfo(Variant::{variant}, \"{func.Key}\"{meta}), ";
                
                string getPrefix = GetGetterPrefix(variant, func.Key);
                string setPrefix = GetSetterPrefix(variant, func.Key);
                propertyBindings += $"\"{setPrefix}{func.Key}\", ";
                propertyBindings += $"\"{getPrefix}{func.Key}\");\n\t\t";

                // Function generation
                inject += "\t" + func.Value.Type + " get_" + func.Key + "() const { return " + func.Key + "; }\n";
                inject += "\tvoid set_" + func.Key + "(" + func.Value.Type + " p) { " + func.Key + " = p; }\n";

                // Function bindings
                // The auto-generated C++ getters and setters still use the get_/set_ prefixes to avoid name collision.
                functionBindings += $"ClassDB::bind_method(D_METHOD(\"{getPrefix}\"), ";
                functionBindings += $"&{type.Value.Name}::get_{func.Key});\n\t";
                functionBindings += $"ClassDB::bind_method(D_METHOD(\"{setPrefix}\", \"p\"), ";
                functionBindings += $"&{type.Value.Name}::set_{func.Key});\n\t"; 
            }

            content = content.Replace("REG_BIND_PROPERTIES", propertyBindings);
            content = content.Replace("REG_BIND_PROPERTY_FUNCTIONS", functionBindings);
        }

        private static string GetGetterPrefix(string variant, string property)
        {
            switch (variant)
            {
                case "BOOL":
                    return property.StartsWith("is_") ? "" : "is_";
            }
            return "get_";
        }

        private static string GetSetterPrefix(string variant, string property)
        {
            return "set_";
        }
    }
}