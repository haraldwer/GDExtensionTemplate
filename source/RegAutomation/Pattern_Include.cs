using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using RegAutomation.Core;

namespace RegAutomation
{
    public class Pattern_Include : Pattern
    {
        public static void ProcessHeader(KeyValuePair<string, DB.Header> header)
        {
            foreach(string includePath in IncludeParser.Instance.Parse(header.Key, header.Value.Content))
            {
                if (!DB.Headers.ContainsKey(includePath))
                {
                    throw new Exception($"Include path {includePath} wasn't picked up by the Database!");
                }
                header.Value.Includes.Add(includePath);
            }
        }

        public static void GenerateIncludes(KeyValuePair<string, DB.Header> header, string content, out string includes)
        {
            includes = $"#include \"{header.Key}\"\n";
        }

        public static string GetIncl()
        {
            return IncludeSolver.Solve(
                DB.Headers.Keys.ToList(), 
                DB.Headers.Values.Select(header => header.Includes).ToList(), 
                DB.Headers.Values.Select(header => header.Types.Count > 0).ToList(),
                DB.Headers.Values.Select(header => header.IncludeName).ToList());
        }
    }
}
