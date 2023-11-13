using NUnit.Framework;

namespace RegAutomation.Core.Tests
{
    [TestFixture]
    internal class Test_IncludeParser
    {
        [Test]
        public void TestParse()
        {
            var result = IncludeParser.Instance.Parse(
                "C://my_project/my_source/foo.h", 
                """
                #include "example_project/registration.h"
                #include "func.h"
                #include<cstdio>
                #include <cmath>
                #include"func2.h"

                """).ToArray();
            Assert.That(result, Is.EquivalentTo(new string[]
            {
                "func.h",
                "func2.h",
            }));
        }
    }
}
