using CppAst;

namespace RegAutomation
{
    public class DB
    {
        public class Header
        {
            public string File = "";
            public string Name = "";
            public CppCompilation Compile = null;
        }
        
        public static readonly List<Header> Headers = new();

        public static void Load()
        {
            string currDir = Directory.GetCurrentDirectory();
            var files = Directory.GetFiles(currDir, "*.h", SearchOption.AllDirectories);
            foreach (var file in files)
            {
                if (file.Contains(".generated.h") || file.Contains(".injected.h"))
                    continue;
                if (file.Contains("\\registration.h")) 
                    continue;
                if (file == "")
                    continue;
                string name = Path.GetRelativePath(currDir, file);
                Headers.Add(new Header()
                {
                    File = file,
                    Name = name,
                });
            }
        }
    }
}