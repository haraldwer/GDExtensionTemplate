using System.Text;
using System.Text.RegularExpressions;

namespace RegAutomation
{
    public class Pattern_Class : Pattern
    {
        public static void ProcessHeader(KeyValuePair<string, DB.Header> header)
        {
            MatchCollection matches = FindMatches(header.Value.Content, "REG_CLASS");
            if (matches == null)
                return;

            var newLines = GetNewLines(header.Value.Content);
            int searchStartIndex = 0;
            foreach (Match match in matches)
            {
                Console.WriteLine("REG_CLASS: " + Path.GetFileName(header.Key));

                // Find next "class" token
                // // Start searching from the previous REG_CLASS, so multiple classes in same file is possible
                string search = header.Value.Content.Substring(searchStartIndex, match.Index - searchStartIndex);
                int classFind = search.LastIndexOf("class ");
                if (classFind == -1)
                    continue;
                
                // Find class scope
                var (closureStart, closureEnd) = FindClosure(header.Value.Content, searchStartIndex + classFind);
                string classContent = header.Value.Content.Substring(closureStart, closureEnd - closureStart);
                searchStartIndex = match.Index; 
                
                // Line number used for code injection 
                int lineNumber = newLines.BinarySearch(match.Index);
                if(lineNumber < 0)
                {
                    // BinarySearch returns the negative of the next-largest element's index
                    // if the first newline's at position 42, and REG_CLASS is found at position 20,
                    // it is matched with the first newline, receiving a line number of 0
                    lineNumber = -lineNumber;
                }
                
                // Parse class def
                string classDef = search.Substring(classFind + "class ".Length);
                // TODO: Error handling when trying to register a class that doesn't (directly or indirectly) inherit from godot::Object (not supported)
                // TODO: Detect multiple inheritance (not supported) and throw an exception accordingly
                int indexOfColon = classDef.IndexOf(':');
                string nameEnd = classDef.Substring(0, indexOfColon).Trim();
                string inheritance = classDef.Substring(indexOfColon + 1);
                string[] tokens = inheritance.Substring(0, inheritance.IndexOf('{'))
                    .Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                // Assuming no multiple inheritance (not supported anyway), the last space-separated token is the parent class name
                // Access modifiers like public/protected might appear before the parent class name
                string parentName = tokens[tokens.Length - 1].Trim();
                
                header.Value.Types.Add(new DB.Type() { 
                    FileName = header.Key,
                    Name = nameEnd, 
                    RegClassLineNumber = lineNumber, 
                    ParentName = parentName, 
                    Content = classContent,
                });
            }
        }
        
        private static List<int> GetNewLines(string content)
        {
            List<int> result = new List<int>();
            for (int i = 0; i < content.Length; ++i)
                if (content[i] == '\n') 
                    result.Add(i);
            return result;
        }
        
        private static (int, int) FindClosure(string content, int startFrom)
        {
            int closureCount = 0;
            int closureStart = startFrom;
            int closureEnd = -1;
            for(int i = startFrom; i < content.Length; i++)
            {
                if (content[i] == '{') 
                {
                    if (closureCount == 0) closureStart = i;
                    closureCount++; 
                }
                else if (content[i] == '}')
                {
                    closureCount--;
                    if (closureCount == 0) 
                    {
                        closureEnd = i;
                        break;
                    }
                }
            }
            return (closureStart, closureEnd);
        }
        
        public static void GenerateInject(DB.Type type, StringBuilder inject)
        {
            inject.Append($"\tGDCLASS({type.Name}, {type.ParentName})\n");
            inject.Append("protected: \n");
            inject.Append("\tstatic void _bind_methods();\n");
        }
        
        public static string GetReg()
        {
            string reg = "";
            foreach(var keyValue in DB.Headers)
                foreach (var type in keyValue.Value.Types)
                    if (type.Name != "")
                        reg += $"ClassDB::register_class<{type.Name}>();\n";
            return reg; 
        }
    }
}