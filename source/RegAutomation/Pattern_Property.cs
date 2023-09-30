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
                int startIndex = match.Value.Length + match.Index + 2;
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
                    Type = var
                };
            }
        }

        public static void Generate(KeyValuePair<string, DB.Type> type, ref string content)
        {
            foreach (var func in type.Value.Properties)
            {
                // ClassDB::bind_method(D_METHOD("get_speed"), &GDExample::get_speed);
                // ClassDB::bind_method(D_METHOD("set_speed", "p_speed"), &GDExample::set_speed);
                // ClassDB::add_property("GDExample", PropertyInfo(Variant::FLOAT, "speed", PROPERTY_HINT_RANGE, "0,20,0.01"), "set_speed", "get_speed");
                
                // TODO: Add get and set to header!
                //content += "\tClassDB::bind_method(D_METHOD(\"get_" + func.Key + "\", ";
                //content += "&" + type.Value.Name + "::" + func.Key + ");\n";
                //content += "\tClassDB::bind_method(D_METHOD(\"set_" + func.Key + "\", ";
                //content += "&" + type.Value.Name + "::" + func.Key + ");\n"; 

                
                // How to inject code? 

                string variant = "";
                switch (func.Value.Type)
                {
                    case "float":
                        variant = "FLOAT";
                        break;
                    default:
                        Console.WriteLine("Unknown type: " + func.Value.Type);
                        continue;
                }

                content += "\tClassDB::add_property(\"" + type.Value.Name + "\", ";
                
                // Type
                content += "PropertyInfo(Variant::" + variant + ", ";
                content += "\"" + func.Key + "\"";
                
                
                
                // TODO: Meta!
                
                content += "), ";
                
                // Getter / setter
                content += "\"set_" + func.Key + "\", ";
                content += "\"get_" + func.Key + "\");\n";
            }
        }
    }
}