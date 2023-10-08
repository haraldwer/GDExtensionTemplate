using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace RegAutomation
{
    public class Pattern
    {
        protected static MatchCollection FindMatches(string content, string pattern)
        {
            if (content == null || pattern == null)
                return null;
            try
            {
                return Regex.Matches(content, pattern);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            return null;
        }
        protected static Dictionary<string, string> FindMetaProperties(string content, int searchStartIndex)
        {
            Dictionary<string, string> properties = new Dictionary<string, string>();
            // First, find property separator indices so we know the intervals where properties are
            int level = 0;
            List<int> propertySeparatorIndices = new List<int>();
            for(int i = searchStartIndex; i < content.Length; i++)
            {
                if(level > 0)
                {
                    // Don't do nested properties here. Let callers decide what to do with them (e.g., pass them verbatim like PropertyInfos)
                    if (level == 1 && content[i] == ',')
                    {
                        propertySeparatorIndices.Add(i);
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
            // Then, we retrieve every substring defined by the intervals.
            for(int i = 0; i < propertySeparatorIndices.Count - 1; i++)
            {
                int start = propertySeparatorIndices[i] + 1; // Skip the '(' or ','
                int end = propertySeparatorIndices[i + 1]; // Skip the ')' or ','
                string property = content.Substring(start, end - start).Trim();
                var split = property.Split('=');
                if (split.Length == 2) // <key> = <value>
                {
                    properties[split[0].Trim()] = split[1].Trim();
                }
                else if (split.Length < 2) // <key>
                {
                    properties[split[0].Trim()] = "";
                }
                else throw new Exception("Illegal meta property syntax detected.");
            }

            return properties;
        }
    }
}