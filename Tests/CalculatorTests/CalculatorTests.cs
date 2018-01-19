using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;

namespace CalculatorTests
{
    [TestClass]
    public class BasicCalculatorTests
    {
        [TestMethod, TestCategory("Integration")]
        public void TestMethod1()
        {
            Assert.IsTrue(true);
           
        }

        
    }

    [TestClass]
    public class CientificCalculatorTests
    {
        [TestMethod, TestCategory("Unit")]
        public void TestMethod2()
        {
            Assert.IsTrue(true);
        }
    }
}
