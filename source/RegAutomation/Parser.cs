using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace RegAutomation
{
    public class Context
    {
        public int Start;
        public int End;
        public string Content;
        public List<string> Tokens; 
    }

    public class Params
    {
        public int Start;
        public int End;
        public List<string> Content;
    }

    public class Macro
    {
        public string Name;
        public Params Params; 
        public Context OuterContext;
        public Context InnerContext; 
    }
    
    public class Parser
    {
        public static List<Macro> Parse(string content, string[] macros)
        {
            // Look for macros
            
            // REG_CLASS() - needs to know class context
            // REG_ENUM() - needs to know complete enum scope
            // REG_FUNCTION() - needs to know complete function scope
            // REG_PROPERTY() - needs to know complete property scope
            
            // Find outer context
            // Find inner context
            // Find parameters

            List<Macro> result = new List<Macro>();
            foreach (string macro in macros)
            {
                foreach (Match match in Regex.Matches(content, macro))
                {
                    var parameters = FindParams(content, match.Index);
                    int contextStart = parameters.End + 1;
                    Context outerContext = FindOuterContext(content, contextStart);
                    Context innerContext = FindInnerContext(content, contextStart);
                    result.Add(new Macro()
                    {
                        Name = macro,
                        Params = parameters,
                        OuterContext = outerContext,
                        InnerContext = innerContext
                    });
                }
            }
            return result;
        }

        private static Params FindParams(string content, int startIndex)
        {
            int paramStart = content.IndexOf('(', startIndex, 20) + 1;
            int paramEnd = ScopeDepthSearch(content, paramStart - 1, '(', new[] {')'}) - 1;
            string text = content.Substring(paramStart, paramEnd - paramStart);
            List<string> parameters = text.Split(',').ToList(); // Consider scoped ,
            return new Params()
            {
                Start = paramStart,
                End = paramEnd,
                Content = parameters
            };
        }

        private static Context FindOuterContext(string content, int startIndex)
        {
            int start = ScopeOuterSearch(content, startIndex, '{', '}');
            int end = ScopeDepthSearch(content, start, '{', new[] {'}'});
            string text = content.Substring(start, end - start);
            List<string> tokens = Tokenize(text);
            return new Context()
            {
                Start = start,
                End = end,
                Content = text,
                Tokens = tokens
            };
        }
        
        private static Context FindInnerContext(string content, int startIndex)
        {
            int scopeEnd = ScopeDepthSearch(content, startIndex, '{', new[] {'}', ';'});
            string text = content.Substring(startIndex, scopeEnd - startIndex);
            List<string> tokens = Tokenize(text);
            return new Context()
            {
                Start = startIndex,
                End = scopeEnd,
                Content = text,
                Tokens = tokens
            };
        }
        
        private static List<string> Tokenize(string text)
        {
            // Strip comments
            string strip = StripComments(text);
            
            // Remove formatting
            string formatting = "\n\t\r";
            string formatted = strip;
            foreach (char c in formatting)
                formatted = formatted.Replace(c.ToString(), "");
            
            // Parse tokens
            // TODO: Blankspace
            const string tokens = ";:,(){}";
            int lastToken = 0;
            List<string> result = new List<string>();
            for (int i = 0; i < formatted.Length; i++)
            {
                if (!tokens.Contains(formatted[i])) 
                    continue;
                int start = lastToken + 1;
                int c = i - start;
                if (c > 0)
                    result.Add(formatted.Substring(start, c).Trim());
                result.Add(formatted[i].ToString());
                lastToken = i + 1;
            }
            string end = formatted.Substring(lastToken);
            if (end != "")
                result.Add(end);
            return result;
        }

        private static string StripComments(string text)
        {
            text = Strip(text, "//", "\n");
            text = Strip(text, "/*", "*/");
            return text;
        }
        
        private static string Strip(string text, string start, string end)
        {
            string result = "";
            int offset = 0;
            while (true)
            {
                int find = text.IndexOf(start, offset);
                if (find == -1)
                    break;
                result += text.Substring(offset, find - offset);
                offset = text.IndexOf(end, find) + 1;
                if (offset == -1) // No end
                    return result;
            }
            result += text.Substring(offset); 
            return result;
        }
        
        private static int ScopeOuterSearch(string content, int startIndex, char open, char close)
        {
            // TODO: Ignore comments
            
            int depth = 0;
            for (int i = startIndex; i >= 0; i--)
            {
                if (content[i] == close)
                    depth++;
                if (content[i] == open)
                {
                    depth--;
                    if (depth < 0)
                        return i;
                }
            }
            return -1; 
        }
        
        private static int ScopeDepthSearch(string content, int startIndex, char open, char[] close)
        {
            // TODO: Ignore comments
            
            int depth = 0;
            for (int i = startIndex; i < content.Length; i++)
            {
                if (content[i] == open)
                    depth++;
                if (close.Contains(content[i]))
                {
                    depth--;
                    if (depth <= 0)
                        return i + 1;
                }
            }
            return -1; 
        }
    }
}