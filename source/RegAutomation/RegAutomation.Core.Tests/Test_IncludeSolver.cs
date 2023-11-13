using NUnit.Framework;

namespace RegAutomation.Core.Tests
{
    [TestFixture]
    internal class Test_IncludeSolver
    {
        [Test]
        public void TestSolve()
        {
            /*          C
             * A -> B <   > E <- G
             *          D   |   /
             *              v /
             *              F
             * Cycle E-F-G is made up of headers with no types registered, so this should not throw.
             */
            var result = IncludeSolver.Solve(
                new List<string>(){ "A", "B", "C", "D", "E", "F", "G" },
                new List<List<string>>
                {
                    new List<string> { "B" }, // A
                    new List<string> { "C", "D" }, // B
                    new List<string> { "E" }, // C
                    new List<string> { "E" }, // D
                    new List<string> { "F" }, // E
                    new List<string> { "G" }, // F
                    new List<string> { "E" }, // G
                },
                new List<bool>{ true, false, true, true, false, false, false },
                new List<string>{ "a", "b", "c", "d", "e", "f", "g" });
            Assert.That(result, Is.EqualTo(
                "#include \".generated/d.generated.h\"\n"
                + "#include \".generated/c.generated.h\"\n"
                + "#include \".generated/a.generated.h\"\n")
                .Or.EqualTo(
                "#include \".generated/c.generated.h\"\n"
                + "#include \".generated/d.generated.h\"\n"
                + "#include \".generated/a.generated.h\"\n"));
            // Cycle E-F-G is now made up of headers with types registered, causing cyclic dependency, so this should throw.
            Assert.Throws(typeof(Exception), () =>
            {
                IncludeSolver.Solve(
                new List<string>(){ "A", "B", "C", "D", "E", "F", "G" },
                new List<List<string>>
                {
                    new List<string> { "B" }, // A
                    new List<string> { "C", "D" }, // B
                    new List<string> { "E" }, // C
                    new List<string> { "E" }, // D
                    new List<string> { "F" }, // E
                    new List<string> { "G" }, // F
                    new List<string> { "E" }, // G
                },
                new List<bool>{ true, false, true, true, true, true, true },
                new List<string>{ "a", "b", "c", "d", "e", "f", "g" });
            });
        }
    }
}
