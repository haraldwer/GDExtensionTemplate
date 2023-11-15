using System.Text;
using System.Text.RegularExpressions;

namespace RegAutomation.Core
{
    public static class PropertyBinder
    {
        public static (string propertyBindings, string functionBindings, string functionInject) GenerateBindings(
            string className, 
            string propertyName, 
            string propertyType, 
            PropertyReferenceType propertyReferenceType,
            PropertyExportFlags propertyExportFlags,
            string propertyHintType, 
            string propertyHintString, 
            string propertyUsageFlags)
        {
            // Figure out the variant type of the property first.
            // Note that even for TypedArrays, we use the same code path to figure out the element type here as well.
            string variantType;
            if(CppTypeToVariantType.ContainsKey(propertyType))
            {
                if(propertyReferenceType is PropertyReferenceType.Pointer)
                    throw new NotSupportedException("Pointer to Variant type cannot be registered!");
                variantType = CppTypeToVariantType[propertyType];
            }
            else if(propertyExportFlags.HasFlag(PropertyExportFlags.Node))
            {
                variantType = "OBJECT";
                propertyHintType = "PROPERTY_HINT_NODE_TYPE";
                propertyHintString = $"\"{propertyType}\"";
            }
            else if(propertyExportFlags.HasFlag(PropertyExportFlags.Resource))
            {
                variantType = "OBJECT";
                propertyHintType = "PROPERTY_HINT_RESOURCE_TYPE";
                propertyHintString = $"\"{propertyType}\"";
            }
            else
            {
                 throw new NotSupportedException($"Unable to map {propertyType} to a GDScript-accessible type. Please check if it is a Variant type or a registered class that is a subclass of Node or Resource.");
            }

            // For TypedArrays, move the element type's info inside the hint string, and add Array property info.
            if (propertyExportFlags.HasFlag(PropertyExportFlags.Array))
            {
                propertyHintString = $"""vformat("%s/%s:%s", Variant::{variantType}, {propertyHintType}, {propertyHintString})""";
                propertyHintType = "PROPERTY_HINT_ARRAY_TYPE";
                propertyType = $"TypedArray<{propertyType}>";
                variantType = "ARRAY";
            }

            // Combining the information above gives us the property info.
            string propertyInfo = $"Variant::{variantType}, \"{propertyName}\", {propertyHintType}, {propertyHintString}, {propertyUsageFlags}";

            var (getter, setter) = GetGetterSetter(variantType, propertyName);
            // With the getter and setter name decided, and the property info computed, we can construct the binding code.
            string addPropertyStatement = $"ClassDB::add_property(\"{className}\", PropertyInfo({propertyInfo}), \"{setter}\", \"{getter}\");\n\t";
            string bindGetterStatement = $"ClassDB::bind_method(D_METHOD(\"{getter}\"), &{className}::_gen_{getter});\n\t";
            string bindSetterStatement = $"ClassDB::bind_method(D_METHOD(\"{setter}\", \"p\"), &{className}::_gen_{setter});\n\t";

            // For Ref<T>s, re-wrap the property type with Ref to match with C++ definition.
            if(propertyReferenceType is PropertyReferenceType.Ref)
                propertyType = $"Ref<{propertyType}>";
            // For pointers, we insert the pointer asterisk back into the function declarations.
            // We can't put this on propertyType because the asterisk is placed right next to the type for the getter,
            // but right next to the parameter for the setter.
            string pointerSymbol;
            if (propertyReferenceType is PropertyReferenceType.Pointer)
                pointerSymbol = "*";
            else
                pointerSymbol = "";

            // Finally, construct the getters and setters.
            string genGetterDeclaration = $"\t{propertyType}{pointerSymbol} _gen_{getter}() const {{ return {propertyName}; }}\n";
            string genSetterDeclaration = $"\tvoid _gen_{setter}({propertyType} {pointerSymbol}p) {{ {propertyName} = p; }}\n";

            return new (
                addPropertyStatement, 
                bindGetterStatement + bindSetterStatement, 
                genGetterDeclaration + genSetterDeclaration);
        }
        private static (string get, string set) GetGetterSetter(string variant, string property)
        {
            switch (variant)
            {
                case "BOOL":
                {
                    if (property.StartsWith("is_"))
                        property = property[3..];
                    return ("is_" + property, "set_" + property);
                }
            }
            return ("get_" + property, "set_" + property);
        }
        // Rather than coming up with a hacky solution based on naming patterns that have no guarantees,
        // we just make the mapping explicit so it's easier to maintain.
        // This also lets us check if a Cpp type can be converted into a variant.
        // See also: list of variant types (https://docs.godotengine.org/en/stable/classes/index.html#variant-types).
        private static readonly Dictionary<string, string> CppTypeToVariantType = new()
        {
            ["AABB"] = "AABB",
            ["Array"] = "ARRAY",
            ["Basis"] = "BASIS",
            ["bool"] = "BOOL",
            ["Callable"] = "CALLABLE",
            ["Color"] = "COLOR",
            ["Dictionary"] = "DICTIONARY",

            // Aliases for float.
            ["float"] = "FLOAT",
            ["real_t"] = "FLOAT",
            ["double"] = "FLOAT",

            // Aliases for int.
            ["int"] = "INT",
            ["int32_t"] = "INT",
            ["int64_t"] = "INT",

            ["NodePath"] = "NODE_PATH",
            ["Object"] = "OBJECT",
            ["PackedByteArray"] = "PACKED_BYTE_ARRAY",
            ["PackedColorArray"] = "PACKED_COLOR_ARRAY",
            ["PackedFloat32Array"] = "PACKED_FLOAT32_ARRAY",
            ["PackedFloat64Array"] = "PACKED_FLOAT64_ARRAY",
            ["PackedInt32Array"] = "PACKED_INT32_ARRAY",
            ["PackedInt64Array"] = "PACKED_INT64_ARRAY",
            ["PackedStringArray"] = "PACKED_STRING_ARRAY",
            ["PackedVector2Array"] = "PACKED_VECTOR2_ARRAY",
            ["PackedVector3Array"] = "PACKED_VECTOR3_ARRAY",
            ["Plane"] = "PLANE",
            ["Projection"] = "PROJECTION",
            ["Quaternion"] = "QUATERNION",
            ["Rect2"] = "RECT2",
            ["Rect2i"] = "RECT2I",
            ["RID"] = "RID",
            ["Signal"] = "SIGNAL",
            ["String"] = "STRING",
            ["StringName"] = "STRING_NAME",
            ["Transform2D"] = "TRANSFORM2D",
            ["Transform3D"] = "TRANSFORM3D",
            ["Vector2"] = "VECTOR2",
            ["Vector2i"] = "VECTOR2I",
            ["Vector3"] = "VECTOR3",
            ["Vector3i"] = "VECTOR3I",
            ["Vector4"] = "VECTOR4",
            ["Vector4i"] = "VECTOR4I",
        };
    }
}
