using NUnit.Framework;

namespace RegAutomation.Core.Tests
{
    [TestFixture]
    internal class Test_PropertyParser
    {
        [Test]
        public void TestParse()
        {
            var result = PropertyParser.Instance.Parse(
                """
                REG_PROPERTY()
                int my_int = 0;
                
                float my_nonregistered_float = 1;

                REG_PROPERTY(
                    REG_P_HintType=PROPERTY_HINT_RANGE,
                    REG_P_HintString="0,20,0.01",
                    REG_P_UsageFlags=PROPERTY_USAGE_EDITOR | PROPERTY_USAGE_READ_ONLY)
                float my_float = 0;

                REG_PROPERTY()
                String my_string = "my_str";

                REG_PROPERTY(
                    REG_P_HintType=PROPERTY_HINT_RANGE,
                    REG_P_HintString="0,1,0.001")
                TypedArray<float> float_array;

                REG_PROPERTY(REG_P_ExportAsResource)
                Ref<Image> image_ref;

                REG_PROPERTY(REG_P_ExportAsResource)
                TypedArray<Image> image_array;

                REG_PROPERTY(REG_P_ExportAsNode)
                Node3D *node_pointer = nullptr;

                REG_PROPERTY(REG_P_ExportAsNode)
                TypedArray<Node3D> node_array;
                """).ToArray();
            Assert.That(result.Select(macro => macro.Name), Is.EquivalentTo(new string[]
            {
                "my_int",
                "my_float",
                "my_string",
                "float_array",
                "image_ref",
                "image_array",
                "node_pointer",
                "node_array"
            }));
            Assert.That(result.Select(macro => macro.Type), Is.EquivalentTo(new string[]
            {
                "int",
                "float",
                "String",
                "float",
                "Image",
                "Image",
                "Node3D",
                "Node3D",
            }));
            Assert.That(result.Select(macro => macro.ReferenceType), Is.EquivalentTo(new PropertyReferenceType[]
            {
                PropertyReferenceType.Value,
                PropertyReferenceType.Value,
                PropertyReferenceType.Value,
                PropertyReferenceType.Value,
                PropertyReferenceType.Ref,
                PropertyReferenceType.Value,
                PropertyReferenceType.Pointer,
                PropertyReferenceType.Value,
            }));
            Assert.That(result.Select(macro => macro.ExportFlags), Is.EquivalentTo(new PropertyExportFlags[]
            {
                PropertyExportFlags.Variant,
                PropertyExportFlags.Variant,
                PropertyExportFlags.Variant,
                PropertyExportFlags.Variant | PropertyExportFlags.Array,
                PropertyExportFlags.Resource,
                PropertyExportFlags.Resource | PropertyExportFlags.Array,
                PropertyExportFlags.Node,
                PropertyExportFlags.Node | PropertyExportFlags.Array,
            }));
            Assert.That(result.Select(macro => macro.HintType), Is.EquivalentTo(new string[]
            {
                "PROPERTY_HINT_NONE",
                "PROPERTY_HINT_RANGE",
                "PROPERTY_HINT_NONE",
                "PROPERTY_HINT_RANGE",
                "PROPERTY_HINT_NONE",
                "PROPERTY_HINT_NONE",
                "PROPERTY_HINT_NONE",
                "PROPERTY_HINT_NONE",
            }));
            Assert.That(result.Select(macro => macro.HintString), Is.EquivalentTo(new string[]
            {
                "\"\"",
                "\"0,20,0.01\"",
                "\"\"",
                "\"0,1,0.001\"",
                "\"\"",
                "\"\"",
                "\"\"",
                "\"\"",
            }));
            Assert.That(result.Select(macro => macro.UsageFlags), Is.EquivalentTo(new string[]
            {
                "PROPERTY_USAGE_DEFAULT",
                "PROPERTY_USAGE_EDITOR | PROPERTY_USAGE_READ_ONLY",
                "PROPERTY_USAGE_DEFAULT",
                "PROPERTY_USAGE_DEFAULT",
                "PROPERTY_USAGE_DEFAULT",
                "PROPERTY_USAGE_DEFAULT",
                "PROPERTY_USAGE_DEFAULT",
                "PROPERTY_USAGE_DEFAULT",
            }));
        }
    }
}
