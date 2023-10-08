using System.Collections.Generic;
using System.Text;

namespace RegAutomation
{
    public class Pattern_Comment : Pattern
    {
        public static void Process(KeyValuePair<string, DB.Header> header)
        {
            string content = header.Value.Content;
            
            // Algorithm to remove [// ... \n) and [/* ... */] from content in linear time
            var result = new StringBuilder();
            int scanLimit = content.Length;
            // We look-ahead one char every iteration, so the scan limit should retract by 1
            scanLimit -= 1;
            const int SCAN_BEGIN   = 0; // Scan for // or /* (Not in commented section)
            const int SCAN_NEWLINE = 1; // Scan for \n (In commented section)
            const int SCAN_STAR    = 2; // Scan for */ (In commented section)
            int state = SCAN_BEGIN;
            for (int i = 0; i < scanLimit; ++i)
            {
                switch (state)
                {
                case SCAN_BEGIN:
                    if (content[i] == '/' && content[i + 1] == '/')
                    {
                        state = SCAN_NEWLINE;
                        ++i; // Skip both tokens
                    }
                    else if (content[i] == '/' && content[i + 1] == '*')
                    {
                        state = SCAN_STAR;
                        ++i; // Skip both tokens
                    }
                    else
                        result.Append(content[i]);
                    break;
                case SCAN_NEWLINE:
                    if (content[i] == '\n')
                    {
                        state = SCAN_BEGIN;
                        result.Append(content[i]); // Newlines should be preserved!
                    }
                    break;
                case SCAN_STAR:
                    if (content[i] == '\n')
                    {
                        result.Append(content[i]); // Newlines should be preserved!
                    }
                    else if (content[i] == '*' && content[i + 1] == '/') 
                    { 
                        state = SCAN_BEGIN;
                        ++i; // Skip both tokens
                    }
                    break;
                }
            }
            // Get the last char if not in commented section
            if (state == SCAN_BEGIN) 
                result.Append(content[content.Length - 1]);

            header.Value.Content = result.ToString();
        }

        public static void Generate(DB.Type type, StringBuilder inject)
        {
            // TODO: Inject comments as documentation
        }
    }
}