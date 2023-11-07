using System.Text.RegularExpressions;

namespace RegAutomation
{
    using TokenCollection = List<string>;
    
    public class Context
    {
        public int Start;
        public int End;
        public string Content;
        public TokenCollection Tokens; 
        public TokenCollection Declaration;
    }

    public class Params
    {
        public int Start;
        public int End;
        public Dictionary<string, string> Content;
    }

    public class Macro
    {
        public string Name;
        public Params Params; 
        public Context OuterContext;
        public Context InnerContext;
        public int LineNumber;
    }
    
    public class Parser
    {
        public static List<Macro> Parse(string content, string macro)
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
            foreach (Match match in Regex.Matches(content, macro))
            {
                var parameters = FindParams(content, match.Index);
                int contextStart = parameters.End + 1;
                Context outerContext = FindOuterContext(content, contextStart);
                Context innerContext = FindInnerContext(content, contextStart);
                int lineNumber = FindLineNumber(content, match.Index);
                result.Add(new Macro()
                {
                    Name = macro,
                    Params = parameters,
                    OuterContext = outerContext,
                    InnerContext = innerContext,
                    LineNumber = lineNumber
                });
            }
            return result;
        }

        private static Params FindParams(string content, int startIndex)
        {
            // Greedy param collection by splitting on ,
            int paramStart = content.IndexOf('(', startIndex, 20) + 1;
            int paramEnd = ScopeDepthSearch(content, paramStart - 1, '(', new[] {')'}) - 1;
            string text = content.Substring(paramStart, paramEnd - paramStart);
            List<string> parameters = text.Split(',').ToList(); // Consider scoped
            if (parameters.Count == 1 && parameters[0] == "")
                parameters.Clear();
            
            // Capture meta scope as one param
            int scope = 0;
            int scopeStart = -1;
            string scopeParam = "";
            Dictionary<string, string> scopedParams = new Dictionary<string, string>();
            for (int i = 0; i < parameters.Count; i++)
            {
                scope += parameters[i].Count(x => x == '('); // Enter scope
                if (scope > 0)
                {
                    if (scopeStart == -1) // Enter scope
                        scopeStart = i;
                    if (scopeParam != "")
                        scopeParam += ", ";
                    scopeParam += parameters[i];
                    scope -= parameters[i].Count(x => x == ')'); // Exit scope
                    if (scope <= 0) // Exit scope
                    {
                        int metaStart = scopeParam.IndexOf('(') + 1;
                        int metaEnd = scopeParam.LastIndexOf(')') - 1;
                        string param = parameters[scopeStart].Split("=")[0].Trim();
                        scopedParams[param] = scopeParam.Substring(metaStart, metaEnd - metaStart);
                        scopeParam = "";
                        scopeStart = -1;
                    }
                }
                else
                {
                    scopedParams[parameters[i]] = "";
                }
            }
            
            return new Params()
            {
                Start = paramStart,
                End = paramEnd,
                Content = scopedParams
            };
        }

        private static Context FindOuterContext(string content, int startIndex)
        {
            int start = ScopeOuterSearch(content, startIndex, '{', '}');
            int end = ScopeDepthSearch(content, start, '{', new[] {'}'});
            string text = content.Substring(start, end - start);
            TokenCollection decl = Tokenize(FindDeclaration(content, start));
            TokenCollection tokens = Tokenize(text);
            return new Context()
            {
                Declaration = decl,
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
            TokenCollection tokens = Tokenize(text);
            return new Context()
            {
                Start = startIndex,
                End = scopeEnd,
                Content = text,
                Tokens = tokens
            };
        }
        
        private static int FindLineNumber(string content, int matchIndex)
        {
            int c = 1;
            for (int i = 0; i < Math.Min(matchIndex, content.Length); i++)
                if (content[i] == '\n')
                    c++;
            return c; 
        }
        
        private static string FindDeclaration(string content, int startIndex)
        {
            if (startIndex < 1 || startIndex >= content.Length)
                return "";
            
            // Somewhere before start is a declaration

            // Possible delimiters 
            string[] delim = {
                ";", "{", "}", "#pragma once", ">",
            };

            // Find delimiter index
            int dIndex = 0; 
            foreach (var d in delim)
            {
                int i = content.LastIndexOf(d, startIndex - 1);
                if (i > dIndex)
                    dIndex = i;
            }
            
            if (dIndex == 0)
                return "";

            string decl = content.Substring(dIndex + 1, startIndex - dIndex - 1);
            string stripped = StripComments(decl); 
            return stripped.Trim();
        }
        
        private static TokenCollection Tokenize(string text)
        {
            // Strip comments
            string strip = StripComments(text);
            
            // Remove formatting
            string formatting = "\n\t\r";
            string formatted = strip;
            foreach (char c in formatting)
                formatted = formatted.Replace(c.ToString(), "");
            
            // Parse tokens
            const string separators = ";:,(){}";
            int lastToken = 0;
            TokenCollection result = new TokenCollection();
            for (int i = 0; i < formatted.Length; i++)
            {
                if (!separators.Contains(formatted[i])) 
                    continue;
                int start = lastToken;
                int c = i - start;
                if (c > 0)
                {
                    string[] tokens = formatted.Substring(start, c).Trim().Split(" ");
                    foreach (string t in tokens)
                        if (t != "")
                            result.Add(t);
                }
                result.Add(formatted[i].ToString());
                lastToken = i + 1;
            }
            
            string[] end = formatted.Substring(lastToken).Trim().Split(" ");
            foreach (string t in end)
                if (t != "")
                    result.Add(t);
            
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

        public static int FindTokenMatch(TokenCollection InTokens, Func<string, bool> InMatch, int InSearchStart = 0)
        {
            for (int i = InSearchStart; i < InTokens.Count; i++)
                if (InMatch(InTokens[i]))
                    return i; 
            return -1; 
        }

        public static List<string> FindMatchingTokens(TokenCollection InTokens, Func<string, bool> InMatch, int InOffset = 0, int InSearchStart = 0)
        {
            List<string> result = new List<string>();
            while (true)
            {
                int find = FindTokenMatch(InTokens, InMatch, InSearchStart);
                if (find < 0)
                    break;
                InSearchStart = find + 1;
                string token = InTokens[find + InOffset];
                result.Add(token);
            }

            return result; 
        }
    }
}