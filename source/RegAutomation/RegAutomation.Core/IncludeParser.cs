using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace RegAutomation.Core
{
    public class IncludeParser : ParserBase
    {
        public static readonly IncludeParser Instance = new();
        public IEnumerable<string> Parse(string headerPath, string headerContent)
        {
            // This pattern matches "#include<zero or more whitespaces><opening quotation mark>"
            // Example: #include "MyNode.h", #include"TestNode.h"
            MatchCollection matches = Regex.Matches(headerContent, "#include\\s*\"");
            if (matches == null)
            {
                Console.WriteLine("No include found");
                yield break;
            }
            foreach (Match match in matches)
            {
                int includePathStart = match.Index + match.Length; // Starts after the opening quotating mark.
                int includePathEnd = headerContent.IndexOf('\"', includePathStart);
                if(includePathEnd < 0 )
                {
                    throw new Exception("Closing quotation mark not found in include statement.");
                }
                string includePath = headerContent.Substring(includePathStart, includePathEnd - includePathStart).Replace('/', '\\');
                // TODO: Find a way to share ignore list with DB.Load.
                if (includePath.Contains("\\registration.h"))
                    continue;
                // Find this header's directory so we can convert relative filepaths to absolute filepaths.
                int currentDirectoryEndIndex = headerPath.LastIndexOf('\\');
                if(currentDirectoryEndIndex >= 0)
                {
                    includePath = $"{headerPath.Substring(0, currentDirectoryEndIndex)}\\{includePath}";
                }
                yield return includePath;
            }
        }
    }
}
