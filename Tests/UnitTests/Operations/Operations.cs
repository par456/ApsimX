﻿using Models;
using NUnit.Framework;
using System.Reflection;

namespace UnitTests
{
    [TestFixture]
    public class OperationsTests
    {
        [Test]
        public void TestOperationParsing()
        {
            string[] passingStrings =
            {
                "2000-01-01 [NodeName].Function(1000)",
                " 2000-01-01 [NodeName].Function(1000) ",
                "2000-01-01\t[NodeName].Function(1000)",
                "\t2000-01-01\t[NodeName].Function(1000)\t",
                "2000/01/01 [NodeName].Function(1000)",
                "//2000-01-01 [NodeName].Function(1000)",
                " // 2000-01-01 [NodeName].Function(1000) ",
                "//\t2000-01-01\t[NodeName].Function(1000)",
                "\t//\t2000-01-01\t[NodeName].Function(1000)\t",
                "//2000/01/01 [NodeName].Function(1000)"
            };

            Operation[] expectedOperations =
            {
                new Operation(true, "2000-01-01", "[NodeName].Function(1000)", passingStrings[0]),
                new Operation(true, "2000-01-01", "[NodeName].Function(1000)", passingStrings[1]),
                new Operation(true, "2000-01-01", "[NodeName].Function(1000)", passingStrings[2]),
                new Operation(true, "2000-01-01", "[NodeName].Function(1000)", passingStrings[3]),
                new Operation(true, "2000-01-01", "[NodeName].Function(1000)", passingStrings[4]),
                new Operation(false, null, null, passingStrings[5]),
                new Operation(false, null, null, passingStrings[6]),
                new Operation(false, null, null, passingStrings[7]),
                new Operation(false, null, null, passingStrings[8]),
                new Operation(false, null, null, passingStrings[9])
            };

            for (int i = 0; i < passingStrings.Length; i++)
            {
                Operation actualOperation = Operation.ParseOperationString(passingStrings[i]);
                Assert.AreEqual(expectedOperations[i].Enabled, actualOperation.Enabled);
                Assert.AreEqual(expectedOperations[i].Date, actualOperation.Date);
                Assert.AreEqual(expectedOperations[i].Action, actualOperation.Action);
            }

            string[] failingStrings =
            {
                "2000-13-01 [NodeName].Function(1000)", //bad date
                "2000-01-01[NodeName].Function(1000)",  //missing whitespace
                "[NodeName].Function(1000) 2000-01-01", //wrong order
                "2000-01-01 ",                          //missing action
                " [NodeName].Function(1000)",           //missing date
                "",                                     //empty string
                null,                                   //null
                "/2000-01-01 [NodeName].Function(1000)", //not enough comments
            };

            for (int i = 0; i < failingStrings.Length; i++)
            {
                Assert.Null(Operation.ParseOperationString(failingStrings[i]));
            }
        }

        private void Method1(int a, string b) { }

        /// <summary>Ensure that named arguments work on an operations line.</summary>
        [Test]
        public void EnsureNamedArgumentsWork()
        {

            var method1 = GetType().GetMethod("Method1", BindingFlags.Instance | BindingFlags.NonPublic);

            Operations operations = new();
            var argumentValues = Utilities.CallMethod(operations, "GetArgumentsForMethod", new object[] { new string[] { "b:1", "a:2" }, method1 }) as object[];

            Assert.AreEqual(2, argumentValues[0]);
            Assert.AreEqual("1", argumentValues[1]);
        }

        private void Method2(int a, int[] b) { }

        /// <summary>Ensure that an array argument works on an operations line.</summary>
        [Test]
        public void EnsureArrayArgumentsWork()
        {
            var method2 = GetType().GetMethod("Method2", BindingFlags.Instance | BindingFlags.NonPublic);

            Operations operations = new();
            var arguments = new string[] { "1", "2 3" };
            var argumentValues = Utilities.CallMethod(operations, 
                                                      "GetArgumentsForMethod", 
                                                      new object[] { arguments, method2 }) as object[];

            Assert.AreEqual(1, argumentValues[0]);
            Assert.AreEqual(new int[] { 2, 3, }, argumentValues[1]);
        }
    }
}
