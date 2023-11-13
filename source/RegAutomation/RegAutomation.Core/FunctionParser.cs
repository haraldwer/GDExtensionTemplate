using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace RegAutomation.Core
{
    public class FunctionMacro
    {
        public string Name = "";
        public List<string> Params = new();
        public bool IsStatic;
    }
    public class FunctionParser : MacroParser<FunctionMacro>
    {
        public static readonly FunctionParser Instance = new();
        protected override string MacroKey => "REG_FUNCTION";
        protected override FunctionMacro ParseMacroInstance(string content, Params parameters, int macroStart, int contextStart)
        {
            // Parse parameter context
            Context paramContext = FindContext(content, contextStart, '(', ')');
            paramContext.Tokens.RemoveAll(token => token is "," or "(" or ")");
            paramContext.Tokens = paramContext.Tokens.Where((token, index) => index % 2 == 1).ToList();
            // Declaration is before the parameter context
            string declaration = content.Substring(contextStart, paramContext.Start - contextStart).Trim();
            var declTokens = Tokenize(declaration);
            // Function name is the last token in the declaration
            string name = declTokens[^1];
            bool isStatic = declTokens.Contains("static");
            return new FunctionMacro()
            {
                Name = name,
                Params = paramContext.Tokens,
                IsStatic = isStatic,
            };
        }
    }
}
