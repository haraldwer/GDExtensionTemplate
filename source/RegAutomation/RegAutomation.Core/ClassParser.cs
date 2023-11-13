using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace RegAutomation.Core
{
    public class ClassMacro
    {
	    public string ClassName = "";
	    public string ParentClassName = "";
	    public string Content = "";
	    public int LineNumber;
    }
    public class ClassParser : MacroParser<ClassMacro>
    {
        public static readonly ClassParser Instance = new();
        protected override string MacroKey => "REG_CLASS";
        protected override ClassMacro ParseMacroInstance(string content, Params parameters, int macroStart, int contextStart)
        {
            Context outerContext = FindOuterContext(content, contextStart);
            int classDeclSeparatorIndex = outerContext.Declaration.IndexOf(':');
            int classNameTokenIndex = outerContext.Declaration.IndexOf("class ") + "class ".Length;
            string className = outerContext.Declaration[classNameTokenIndex..classDeclSeparatorIndex].Trim();
            // Get the last token on the declaration, which is always the parent class name.
            string parentClassName = Tokenize(outerContext.Declaration[(classDeclSeparatorIndex + 1)..])[^1].Trim();
            int lineNumber = FindLineNumber(content, macroStart);
            return new ClassMacro()
            {
                ClassName = className,
                ParentClassName = parentClassName,
                Content = outerContext.Content,
                LineNumber = lineNumber
            };
        }
    }
}
