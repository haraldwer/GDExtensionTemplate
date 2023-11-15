namespace RegAutomation.Core
{
    /// <summary>
    /// Flags used to determine how the property should be exported.
    /// </summary>
    [Flags]
    public enum PropertyExportFlags : byte
    {
        None = 0,
        Variant = 1 << 0,
        Node = 1 << 1,
        Resource = 1 << 2,
        Array = 1 << 3,
    }
    /// <summary>
    /// How the property's type is treated. Note that Ref here means Godot's Ref, not C++ reference types.
    /// C++ reference types are not supported.
    /// </summary>
    public enum PropertyReferenceType : byte
    {
        Value, // e.g., int, Vector2, usually corresponds to Variant types.
        Ref, // e.g., Ref<Image>, usually corresponds to Resource types.
        Pointer, // e.g., Node3D*, usually corresponds to Node types.
    }
    public class PropertyMacro
    {
        public string Name = "";
        public string Type = ""; // The property's type. Or, if PropertyExportFlags.Array is set in ExportFlags, the array's element type.
        public PropertyReferenceType ReferenceType = PropertyReferenceType.Value;
        public PropertyExportFlags ExportFlags = PropertyExportFlags.None;
        public string HintType = ""; // PropertyHint in string form.
        public string HintString = ""; // The hint string used by the PropertyHint. Should be enclosed by a pair of double-quotes (").
        public string UsageFlags = ""; // Constexpr that evaluates to the property's usage flag value.
    }
    public class PropertyParser : MacroParser<PropertyMacro>
    {
        public static readonly PropertyParser Instance = new PropertyParser();
        protected override string MacroKey => "REG_PROPERTY";
        protected override PropertyMacro ParseMacroInstance(string content, Params parameters, int macroStart, int contextStart)
        {
            // Properties can be "<type> <name>;" or "<type> <name> = <default value>;"
            // We call "<type> <name>" the declaration, and the following code extracts the declaration.
            int equalOperatorIndex = content.IndexOf('=', contextStart);
            bool foundEqualOperator = equalOperatorIndex >= 0;
            int semicolonIndex = content.IndexOf(';', contextStart);
            int declEnd;
            bool hasDefaultValue = false;
            if (foundEqualOperator && equalOperatorIndex < semicolonIndex)
            {
                hasDefaultValue = true;
                declEnd = equalOperatorIndex;
            }
            else 
                declEnd = semicolonIndex;
            string declaration = content[contextStart..declEnd];

            // Detect pointer types here.
            bool isPointer = false;
            int pointerCount = declaration.Where(x => x is '*').Count();
            if(pointerCount > 1)
                throw new Exception("Pointer to pointer cannot be registered!");
            if(pointerCount == 1)
            {
                isPointer = true;
                // We have to replace pointer with whitespace to cover cases like "Node3D*node", which is unfortunately legal C++.
                declaration = declaration.Replace('*', ' ');
            }
            if(declaration.Contains('&'))
                throw new Exception("C++ references (&) cannot be registered!");
            // Trim here so we can use IndexOf(' ') to find an inner whitespace, which separates type and name.
            declaration = declaration.Trim();
            int typeNameSeparatorIndex = declaration.IndexOf(' ');
            // (Error detection) Here we use index + 1 to include the separating whitespace.
            if(declaration[..(typeNameSeparatorIndex + 1)].Contains("const "))
                throw new NotSupportedException("Const properties are not allowed to be registered!");
            if(declaration[..(typeNameSeparatorIndex + 1)].Contains("static "))
                throw new NotSupportedException("Static properties are not allowed to be registered!");
            // As there could be one or more inner whitespaces, we still have to trim again here.
            string name = declaration[(typeNameSeparatorIndex + 1)..].Trim();
            string type = declaration[..typeNameSeparatorIndex].Trim();
            // Check if the property's type is a template, and extract the template type, so we can handle Ref<T> and TypedArray<T>.
            // Note that for template types, the "type" variable now means the type used in the template.
            string templateType = "";
            if(type.Contains('<'))
            {
                templateType = type[..type.IndexOf('<')].Trim();
                type = type[(type.IndexOf('<') + 1)..type.LastIndexOf('>')].Trim();
            }
            bool isRef = templateType == "Ref";
            if(isPointer && isRef)
                throw new NotSupportedException("Pointer to Ref<T> cannot be registered!");

            string hintType = parameters.Content.GetValueOrDefault("REG_P_HintType", "PROPERTY_HINT_NONE");
            string hintString = parameters.Content.GetValueOrDefault("REG_P_HintString", "\"\"");
            if(!hintString.StartsWith('"') && !hintString.EndsWith('"'))
                throw new ArgumentException("The key REG_P_HintString expects a C++ string literal as its value.");
            string usageFlags = parameters.Content.GetValueOrDefault("REG_P_UsageFlags", "PROPERTY_USAGE_DEFAULT");

            PropertyExportFlags exportFlags = PropertyExportFlags.None;
            if(templateType == "TypedArray")
            {
                // Note that the pointer could've been inside or outside TypedArray, hence the message here.
                if (isPointer)
                    throw new NotSupportedException("Pointer to TypedArray<T> or Array of pointers cannot be registered!");
                if(type.StartsWith("Ref<"))
                    throw new NotSupportedException("Array of Ref<T>s cannot be registered!");
                exportFlags |= PropertyExportFlags.Array;
            }
            // Note that Variant flag, Node flag, and Resource flag are mutually exclusive. This is made clear with the else-if.
            if(parameters.Content.ContainsKey("REG_P_ExportAsNode"))
            {
                if(!isPointer && !exportFlags.HasFlag(PropertyExportFlags.Array))
                    throw new NotSupportedException("Properties exported as nodes must be pointers.");
                if(!hasDefaultValue && !exportFlags.HasFlag(PropertyExportFlags.Array))
                    throw new Exception("Pointer properties without default values will crash the editor. Consider assigning nullptr to them.");
                exportFlags |= PropertyExportFlags.Node;
            }
            else if(parameters.Content.ContainsKey("REG_P_ExportAsResource"))
            {
                if(!isRef && !exportFlags.HasFlag(PropertyExportFlags.Array))
                    throw new NotSupportedException("Properties exported as resources must be Ref<T>s.");
                exportFlags |= PropertyExportFlags.Resource;
            }
            else
                exportFlags |= PropertyExportFlags.Variant;

            // Convert the bools isPointer and isRef into PropertyReferenceType as we've made sure they're mutually exclusive now.
            PropertyReferenceType referenceType = PropertyReferenceType.Value;
            if(isPointer)
                referenceType = PropertyReferenceType.Pointer;
            else if(isRef)
                referenceType = PropertyReferenceType.Ref;
            
            return new PropertyMacro()
            {
                Name = name,  
                Type = type,
                ReferenceType = referenceType,
                ExportFlags = exportFlags,
                HintType = hintType,
                HintString = hintString,
                UsageFlags = usageFlags,
            };
        }
    }
}
