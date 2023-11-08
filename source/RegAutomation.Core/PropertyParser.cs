using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace RegAutomation.Core
{
    public class PropertyMacro
    {
        public string Name = "";
        public string Type = "";
        public string Meta = "";
    }
    public class PropertyParser : MacroParser<PropertyMacro>
    {
        public static readonly PropertyParser Instance = new PropertyParser();
        protected override string MacroKey => "REG_PROPERTY";
        protected override PropertyMacro ParseMacroInstance(string content, Params parameters, int macroStart, int contextStart)
        {
            int declEnd = content.IndexOf(';', contextStart);
            var tokens = Tokenize(content[contextStart..declEnd]);
            if(tokens.Contains("const"))
                throw new Exception("Const properties are not allowed to be registered!");
            if(tokens.Contains("static"))
                throw new Exception("Static properties are not allowed to be registered!");
            string name = tokens[1];
            string type = tokens[0];
            string meta = parameters.Content.GetValueOrDefault("REG_P_Info", "");
            if(meta.Length > 0)
            {
                // Strip the opening and closing parentheses
                meta = meta.Substring(1, meta.Length - 2);
            }
            return new PropertyMacro()
            {
                Name = name,  
                Type = type,
                Meta = meta,
            };
        }
    }
}
