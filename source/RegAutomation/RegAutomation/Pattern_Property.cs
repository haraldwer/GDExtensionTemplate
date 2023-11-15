using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using RegAutomation.Core;

namespace RegAutomation
{
    public class Pattern_Property : Pattern
    {
        public static void ProcessType(DB.Type type)
        {
            Console.WriteLine($"REG_PROPERTY: " + Path.GetFileName(type.FileName));
            foreach(PropertyMacro macro in PropertyParser.Instance.Parse(type.Content))
            {
                type.Properties[macro.Name] = new DB.Prop()
                {
                    Type = macro.Type,
                    ReferenceType = macro.ReferenceType,
                    ExportFlags = macro.ExportFlags,
                    HintType = macro.HintType,
                    HintString = macro.HintString,
                    UsageFlags = macro.UsageFlags,
                };
            }
        }

        public static void GenerateBindings(DB.Type type, StringBuilder bindings, StringBuilder inject)
        {
            StringBuilder propertyBindings = new();
            StringBuilder functionBindings = new();
            
            foreach (var prop in type.Properties)
            {
                var(properties, functions, functionInjects) = PropertyBinder.GenerateBindings(
                    type.Name,
                    prop.Key,
                    prop.Value.Type,
                    prop.Value.ReferenceType,
                    prop.Value.ExportFlags,
                    prop.Value.HintType,
                    prop.Value.HintString,
                    prop.Value.UsageFlags);
                propertyBindings.Append(properties);
                functionBindings.Append(functions);
                inject.Append(functionInjects);
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