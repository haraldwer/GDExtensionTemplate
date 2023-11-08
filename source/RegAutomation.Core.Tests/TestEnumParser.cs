using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RegAutomation.Core.Tests
{
    [TestFixture]
    internal class TestEnumParser
    {
        [Test]
        public void TestParse()
        {
            var result = EnumParser.Instance.Parse(
                """
                REG_ENUM()
                enum MyEnum
                {
                    A,
                    B = 5,
                    C = 6,
                    D,
                    E = 15,
                };

                REG_ENUM(REG_P_Bitfield)
                enum class Animals
                {
                    Dog = 1 << 0,
                    Cat = 1 << 1,
                    Giraffe = 1 << 2,
                    Elephant = 1 << 3,
                };
                """).ToArray();
            Assert.That(result.Select(macro => macro.Name), Is.EquivalentTo(new string[]
            {
                "MyEnum",
                "Animals",
            }));
            Assert.That(result.Select(macro => macro.IsBitField), Is.EquivalentTo(new bool[]
            {
                false,
                true,
            }));
            Assert.That(result.Select(macro => macro.Keys), Is.EquivalentTo(new List<string>[]
            {
                new(){ "A", "B", "C", "D", "E" },
                new(){ "Dog", "Cat", "Giraffe", "Elephant" },
            }));
        }
    }
}
