using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RegAutomation.Core
{
    using TokenCollection = List<string>;
    /// <summary>
    /// A class that holds information about a context.
    /// The declaration will only be filled if the context is an outer context.
    /// </summary>
    public class Context
    {
        public int Start;
        public int End;
        public string Content = "";
        public TokenCollection Tokens = new(); 
        public string Declaration = "";
    }
    /// <summary>
    /// A class that holds information about the scope of a reg macro's meta properties.
    /// Properties are stored as string-based key-value pairs.
    /// </summary>
    public class Params
    {
        public int Start;
        public int End;
        public Dictionary<string, string> Content = new();
    }
    /// <summary>
    /// Base class for all parsers. Contains utility methods that implementations can use.
    /// See also: <see cref="MacroParser{T}"></see>, which specializes for macros.
    /// </summary>
    public class ParserBase
    {
		// Put utility methods that parser implementations could use here.
		protected static Params FindParams(string content, int startIndex)
        {
            Dictionary<string, string> properties = new Dictionary<string, string>();
            int level = 0;
            List<int> propertySeparatorIndices = new List<int>();
            List<int> equalOperatorIndices = new List<int>();
            bool isParsingString = false;
            for(int i = startIndex; i < content.Length; i++)
            {
                // Handle strings here.
                if(content[i] == '"')
                {
                    isParsingString = !isParsingString;
                    continue;
                }
                else if(i < content.Length - 1 && content[i] == '\\' && content[i + 1] == '"')
                {
                    i++; // Skip the escaped double-quotes.
                    continue;
                }
                else if(isParsingString)
                {
                    continue; // Passing through string literal, so don't check for separators until another double-quote is found.
                }
                if(level == 1)
                {
                    if (content[i] == ',')
                    {
                        propertySeparatorIndices.Add(i);
                    }
                    else if (content[i] == '=')
                    {
                        equalOperatorIndices.Add(i);
                    }
                }
                if (content[i] == '(')
                {
                    if (level == 0) 
                    {
                        propertySeparatorIndices.Add(i);
                    }
                    level++;
                }
                else if (content[i] == ')')
                {
                    level--;
                    if (level == 0) 
                    {
                        propertySeparatorIndices.Add(i);
                        break;
                    }
                }
            }
            if(propertySeparatorIndices.Count == 0)
                throw new Exception("Opening parenthesis not found.");
            if(level != 0)
                throw new Exception("Closing parenthesis not found.");
            int equalOperatorIndexPointer = 0;
            for(int i = 0; i < propertySeparatorIndices.Count - 1; i++)
            {
                int start = propertySeparatorIndices[i] + 1; // Skip the '(' or ','
                int end = propertySeparatorIndices[i + 1]; // Skip the ')' or ','
                while(equalOperatorIndexPointer < equalOperatorIndices.Count && equalOperatorIndices[equalOperatorIndexPointer] <= start)
                {
                    equalOperatorIndexPointer++;
                }
                if(equalOperatorIndexPointer < equalOperatorIndices.Count && equalOperatorIndices[equalOperatorIndexPointer] < end)
                {
                    int equalOperatorIndex = equalOperatorIndices[equalOperatorIndexPointer];
                    properties[content[start..equalOperatorIndex].Trim()] = content[(equalOperatorIndex + 1)..end].Trim();
                    equalOperatorIndexPointer++;
                }
                else
                {
                    properties[content[start..end].Trim()] = "";
                }
            }
            
            return new Params()
            {
                Start = propertySeparatorIndices[0] + 1,
                End = propertySeparatorIndices[^1],
                Content = properties,
            };
        }

        protected static Context FindOuterContext(string content, int startIndex)
        {
            int start = ScopeOuterSearch(content, startIndex, '{', '}');
            var (_, end) = ScopeDepthSearch(content, start, '{', '}');
            string text = content.Substring(start, end - start);
            string decl = FindDeclaration(content, start);
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

        protected static Context FindContext(string content, int startIndex, char open = '{', char close = '}')
        {
            var (scopeStart, scopeEnd) = ScopeDepthSearch(content, startIndex, open, close);
            string text = content.Substring(scopeStart, scopeEnd - scopeStart);
            TokenCollection tokens = Tokenize(text);
            return new Context()
            {
                Start = scopeStart,
                End = scopeEnd,
                Content = text,
                Tokens = tokens
            };
        }
        
        protected static int FindLineNumber(string content, int matchIndex)
        {
            int c = 1;
            for (int i = 0; i < Math.Min(matchIndex, content.Length); i++)
                if (content[i] == '\n')
                    c++;
            return c; 
        }
        
        protected static string FindDeclaration(string content, int startIndex)
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
        
        protected static TokenCollection Tokenize(string text)
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

        protected static string StripComments(string text)
        {
            text = Strip(text, "//", "\n");
            text = Strip(text, "/*", "*/");
            return text;
        }
        
        protected static string Strip(string text, string start, string end)
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
        
        protected static int ScopeOuterSearch(string content, int startIndex, char open, char close)
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
        
        private static (int, int) ScopeDepthSearch(string content, int startIndex, char open, char close)
        {
            // TODO: Ignore comments
            
            int depth = 0;
            int scopeStart = 0;
            for (int i = startIndex; i < content.Length; i++)
            {
                if (content[i] == open)
                {
                    if(depth == 0)
                        scopeStart = i;
                    depth++;
                }
                if (content[i] == close)
                {
                    depth--;
                    if (depth <= 0)
                        return (scopeStart, i + 1);
                }
            }
            return default; 
        }

        protected static int FindTokenMatch(TokenCollection InTokens, Func<string, bool> InMatch, int InSearchStart = 0)
        {
            for (int i = InSearchStart; i < InTokens.Count; i++)
                if (InMatch(InTokens[i]))
                    return i; 
            return -1; 
        }

        protected static TokenCollection FindMatchingTokens(TokenCollection InTokens, Func<string, bool> InMatch, int InOffset = 0, int InSearchStart = 0)
        {
            TokenCollection result = new();
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
