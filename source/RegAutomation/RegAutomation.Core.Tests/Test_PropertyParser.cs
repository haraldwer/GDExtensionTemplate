using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

                REG_PROPERTY(REG_P_Info=(PROPERTY_HINT_RANGE, "0,20,0.01"))
                float my_float = 0;

                REG_PROPERTY()
                String my_string = "my_str";
                """).ToArray();
            Assert.That(result.Select(macro => macro.Name), Is.EquivalentTo(new string[]
            {
                "my_int",
                "my_float",
                "my_string",
            }));
            Assert.That(result.Select(macro => macro.Type), Is.EquivalentTo(new string[]
            {
                "int",
                "float",
                "String",
            }));
            Assert.That(result.Select(macro => macro.Meta), Is.EquivalentTo(new string[]
            {
                string.Empty,
                "PROPERTY_HINT_RANGE, \"0,20,0.01\"",
                string.Empty,
            }));
        }
    }
}
