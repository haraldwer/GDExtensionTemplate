using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RegAutomation.Core.Tests
{
    [TestFixture]
    internal class TestFunctionParser
    {
        [Test]
        public void TestParse()
        {
            var result = FunctionParser.Instance.Parse(
                """
                REG_FUNCTION()
                void hello_world();

                REG_FUNCTION()
                int add(int x, int y);

                void hello_world_2();

                REG_FUNCTION()
                static void my_logger(String str);
                """).ToArray();
            Assert.That(result.Select(macro => macro.Name), Is.EquivalentTo(new string[]
            {
                "hello_world",
                "add",
                "my_logger"
            }));
            Assert.That(result.Select(macro => macro.Params), Is.EquivalentTo(new List<string>[]
            {
                new(), // No parameters
                new(){ "x", "y" },
                new(){ "str" },
            }));
            Assert.That(result.Select(macro => macro.IsStatic), Is.EquivalentTo(new bool[]
            {
                false,
                false,
                true,
            }));
        }
    }
}
