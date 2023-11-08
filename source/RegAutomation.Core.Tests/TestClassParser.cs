using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RegAutomation.Core.Tests
{
    [TestFixture]
    internal class TestClassParser
    {
        [Test]
        public void TestParse()
        {
            var result = ClassParser.Instance.Parse(
                """
                #include <godot_cpp/classes/node3d.hpp>
                #include <godot_cpp/classes/node2d.hpp>

                class GDExample : public Node3D
                {

                    REG_CLASS()

                    REG_ENUM()
                    enum MyEnum
                    {
                	    A = 1,
                        B = 2,
                        C, // implicit 3
                        D, // implicit 4
                        E = 10,
                        F, // implicit 11
                    };

                public:
                    REG_FUNCTION()
                    void update(double delta);
                private:
                    double time_passed = 0;
                };
                
                class GDExample2 : public Node2D
                {
                
                    REG_CLASS()
                
                    REG_ENUM()
                    enum MyEnum
                    {
                	    A = 1,
                        B = 2,
                    };
                
                public:
                    REG_FUNCTION()
                    void update2(double delta);
                private:
                    double time_passed2 = 0;
                };
                """).ToArray();
            Assert.That(result.Select(macro => macro.ClassName), Is.EquivalentTo(new string[]
            { 
                "GDExample", 
                "GDExample2" 
            }));
            Assert.That(result.Select(macro => macro.ParentClassName), Is.EquivalentTo(new string[]
            { 
                "Node3D", 
                "Node2D" 
            }));
            Assert.That(result.Select(macro => macro.Content), Is.EquivalentTo(new string[]
            {
                """
                {
                
                    REG_CLASS()
                
                    REG_ENUM()
                    enum MyEnum
                    {
                	    A = 1,
                        B = 2,
                        C, // implicit 3
                        D, // implicit 4
                        E = 10,
                        F, // implicit 11
                    };
                
                public:
                    REG_FUNCTION()
                    void update(double delta);
                private:
                    double time_passed = 0;
                }
                """, 
                """
                {
                
                    REG_CLASS()
                
                    REG_ENUM()
                    enum MyEnum
                    {
                	    A = 1,
                        B = 2,
                    };
                
                public:
                    REG_FUNCTION()
                    void update2(double delta);
                private:
                    double time_passed2 = 0;
                }
                """
            }));
            Assert.That(result.Select(macro => macro.LineNumber), Is.EquivalentTo(new int[]{ 7, 30 }));
        }
    }
}
