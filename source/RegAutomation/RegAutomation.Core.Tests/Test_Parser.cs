using NUnit.Framework;

namespace RegAutomation.Core.Tests
{
    [TestFixture]
    internal class Test_Parser : ParserBase
    {
        [Test]
        public void TestFindParams()
        {
            var result = FindParams("""
                REG_TEST(REG_T_KEY_1 = 0, REG_T_KEY_2 = "hi!!!", REG_T_KEY_3 = (x, y = "1"))
                """, 0);
            Assert.That(result.Start, Is.EqualTo(9));
            Assert.That(result.End, Is.EqualTo(75));
            Assert.That(result.Content.Keys, Is.EquivalentTo(new string[]
            {
                "REG_T_KEY_1",
                "REG_T_KEY_2",
                "REG_T_KEY_3",
            }));
            Assert.That(result.Content["REG_T_KEY_1"], Is.EqualTo("0"));
            Assert.That(result.Content["REG_T_KEY_2"], Is.EqualTo("\"hi!!!\""));
            Assert.That(result.Content["REG_T_KEY_3"], Is.EqualTo("(x, y = \"1\")"));
        }
        [Test]
        public void TestFindLineNumber()
        {
            string source = """
                #include <godot_cpp/classes/node3d.hpp>

                // This is a test node.
                class TestNode : Node3D
                {
                    REG_CLASS()
                public:
                    void hello_world();
                }
                """;
            Assert.That(FindLineNumber(source, source.IndexOf("REG_CLASS")), Is.EqualTo(6));
            source = """
                #include <godot_cpp/classes/node2d.hpp>
                
                /* This is a test node, 
                but with a different code style.
                */
                class TestNode : Node2D{


                    REG_CLASS()


                public:
                    void hello_world() const;
                }
                """;
            Assert.That(FindLineNumber(source, source.IndexOf("REG_CLASS")), Is.EqualTo(9));
        }
    }
}