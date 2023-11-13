using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace RegAutomation.Core
{
    public class EnumMacro
    {
        public string Name = "";
        public bool IsBitField;
        // We only record the keys as they can be used directly as enum values when binding.
        public List<string> Keys = new();
    }
    public class EnumParser : MacroParser<EnumMacro>
    {
        public static readonly EnumParser Instance = new();
        protected override string MacroKey => "REG_ENUM";
        protected override EnumMacro ParseMacroInstance(string content, Params parameters, int macroStart, int contextStart)
        {
            Context innerContext = FindContext(content, contextStart);
            string declaration = content[contextStart..innerContext.Start];
            List<string> declTokens = Tokenize(declaration);
            if(declTokens[0] != "enum")
                throw new Exception("REG_ENUM can only be used on enums or enum classes!");
            EnumMacro macro = new();
            macro.Name = declTokens[^1];
            macro.IsBitField = parameters.Content.ContainsKey("REG_P_Bitfield");
            // Strip the opening and closing curly braces when parsing the inner context.
            foreach(var entryDecl in innerContext.Content[1..^1].Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
            {
                int equalOperatorIndex = entryDecl.IndexOf('=');
                if(equalOperatorIndex != -1)
                {
                    macro.Keys.Add(entryDecl[0..equalOperatorIndex].Trim());
                }
                else
                {
                    macro.Keys.Add(entryDecl);
                }
            }
            return macro;
        }
    }
}
