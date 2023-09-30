using System;
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
    }
}