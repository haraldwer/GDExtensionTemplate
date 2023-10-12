
namespace RegAutomation
{
    public class DB
    {
        public class Func
        {
            public List<string> Params = new List<string>();
            public bool IsStatic = false;
        }
        
        public class Prop
        {
            public string Type = "";
            public string Meta = "";
        }

        public class Enum
        {
            public List<string> Keys = new List<string>();
            public bool IsBitField = false;
        }
        public class Type
        {
            public string FileName = "";
            public string Name = "";
            public string Content = "";
            public string ParentName = "";
            public int RegClassLineNumber = 0;

            public Dictionary<string, Func> Functions = new Dictionary<string, Func>();
            public Dictionary<string, Prop> Properties = new Dictionary<string, Prop>();
            public Dictionary<string, Enum> Enums = new Dictionary<string, Enum>();
        }

        public class Header
        {
            public string IncludeName = ""; // The corresponding generated header's name
            public string Content = "";
            public List<Type> Types = new List<Type>();
        }
        
        public static Dictionary<string, Header> Headers = new Dictionary<string, Header>();

        public static void Load()
        {
            var files = Directory.GetFiles(Directory.GetCurrentDirectory(), "*.h", SearchOption.AllDirectories);
            foreach (var file in files)
            {
                if (file.Contains(".generated.h") || file.Contains(".injected.h"))
                    continue;
                if (file.Contains("\\registration.h")) 
                    continue;
                if (file == "")
                    continue;
                Console.WriteLine("Reading file: " + file);
                string content = File.ReadAllText(file);
                if (content == "")
                    continue;
                Headers[file] = new Header
                {
                    IncludeName = file.Substring(Directory.GetCurrentDirectory().Length + 1).Replace('\\', '.'),
                    Content = content
                };
            }
        }
    }
}