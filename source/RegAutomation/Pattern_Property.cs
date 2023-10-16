using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace RegAutomation
{
    public class Pattern_Property : Pattern
    {
        public static void ProcessType(DB.Type type)
        {
            var result = Parser.Parse(type.Content, "REG_PROPERTY");
            foreach (Macro macro in result)
            {
                Console.WriteLine("REG_PROPERTY: " + Path.GetFileName(type.FileName));

                int decEnd = Parser.FindTokenMatch(macro.InnerContext.Tokens, s => s is ";" or "=" or "{");
                string name = macro.InnerContext.Tokens[decEnd - 1];
                string var = macro.InnerContext.Tokens[decEnd - 2];
                
                macro.Params.Content.TryGetValue("REG_P_Info", out string meta);
                if (meta == null)
                    meta = "";
                
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