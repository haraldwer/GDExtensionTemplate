using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace RegAutomation.Core
{
    /// <summary>
    /// A generic implementation of ParserBase. Provides a common API for all macro parsers.
    /// </summary>
    /// <typeparam name="T">A class that can contain parsed data.</typeparam>
    public abstract class MacroParser<T> : ParserBase
    {
        /// <summary>
        /// The token that begins a macro instance. For instance, REG_CLASS.
        /// </summary>
        protected abstract string MacroKey { get; }
		public IEnumerable<T> Parse(string content)
        {
            foreach (Match match in Regex.Matches(content, MacroKey))
            {
                var parameters = FindParams(content, match.Index);
                int contextStart = parameters.End + 1;
                yield return ParseMacroInstance(content, parameters, match.Index, contextStart);
            }
        }
        /// <summary>
        /// Implement this method to parse a single instance of the macro.
        /// </summary>
        /// <param name="content">The text data containing the macro.</param>
        /// <param name="parameters">The macro's own parameters.</param>
        /// <param name="macroStart">The starting index of the macro. Can be used to track line numbers.</param>
        /// <param name="contextStart">The index after the macro, basically the macro's "target."</param>
        /// <returns>Parsed data.</returns>
        protected abstract T ParseMacroInstance(string content, Params parameters, int macroStart, int contextStart);
    }
}
