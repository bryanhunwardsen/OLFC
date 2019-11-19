using FractionalCalculatorLib;
using GenericApplicationLogger;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Reflection;
using static GenericApplicationLogger.Common;

namespace FractionalCalculatorTests
{
    /// <summary>
    /// Summary description for FC_NumberTests
    /// </summary>
    [TestClass]
    public class FC_OutputTests
    {
        Mock<ILogger> mockLogger;
        FC_Number number;

        [TestInitialize]
        public void Setup()
        {
            mockLogger = new Mock<ILogger>();
            mockLogger.SetupGet(x => x.DefaultLogLevel).Returns(LogLevel.DEBUG);
            mockLogger.Setup(x => x.LogMessage(It.IsAny<string>()));
            mockLogger.Setup(x => x.LogMessage(It.IsAny<string>(), It.IsAny<LogLevel>()));
        }

        [TestMethod]
        public void NormalizeFractionTests()
        {
            // Set sign to only numerator +/+, +/-, -/-. -/+
            number = new FC_Number(5, 10, mockLogger.Object);
            PrivateObject nP1 = new PrivateObject(number);
            nP1.Invoke("Normalize");
            Assert.IsTrue((int)nP1.GetFieldOrProperty("Numerator") == 5 &&
                          (int)nP1.GetFieldOrProperty("Denominator") == 10);
            number = new FC_Number(5, -10, mockLogger.Object);
            nP1 = new PrivateObject(number);
            nP1.Invoke("Normalize");
            Assert.IsTrue((int)nP1.GetFieldOrProperty("Numerator") == -5 &&
                          (int)nP1.GetFieldOrProperty("Denominator") == 10);
            number = new FC_Number(-5, -10, mockLogger.Object);
            nP1 = new PrivateObject(number);
            nP1.Invoke("Normalize");
            Assert.IsTrue((int)nP1.GetFieldOrProperty("Numerator") == 5 &&
                          (int)nP1.GetFieldOrProperty("Denominator") == 10);
            number = new FC_Number(-5, 10, mockLogger.Object);
            nP1 = new PrivateObject(number);
            nP1.Invoke("Normalize");
            Assert.IsTrue((int)nP1.GetFieldOrProperty("Numerator") == -5 &&
                          (int)nP1.GetFieldOrProperty("Denominator") == 10);
            // Check for null denominator error as undefined
            number = new FC_Number(-5, 1, mockLogger.Object);
            number.Denominator = 0;
            nP1 = new PrivateObject(number);            
            var tEx = Assert.ThrowsException<TargetInvocationException>(() => nP1.Invoke("Normalize"));
            Assert.IsInstanceOfType(tEx.InnerException, typeof(ArgumentException));
        }

        [TestMethod]
        public void ImproperFractionGeneratorTests()
        {
            // Good Test
            number = new FC_Number(0, 0, 1, mockLogger.Object);
            PrivateObject nP1 = new PrivateObject(number);
            nP1.Invoke("GenerateImproperFraction", new object[] { 3, 5, 10 } );
            Assert.IsTrue((int)nP1.GetFieldOrProperty("Numerator") == 35 &&
                          (int)nP1.GetFieldOrProperty("Denominator") == 10);
            // Leading Zero
            number = new FC_Number(0, 0, 1, mockLogger.Object);
            nP1 = new PrivateObject(number);
            nP1.Invoke("GenerateImproperFraction", new object[] { 0, 5, 10 });
            Assert.IsTrue((int)nP1.GetFieldOrProperty("Numerator") == 5 &&
                          (int)nP1.GetFieldOrProperty("Denominator") == 10);
            // Numerator 0
            number = new FC_Number(0, 0, 1, mockLogger.Object);
            nP1 = new PrivateObject(number);
            nP1.Invoke("GenerateImproperFraction", new object[] { 3, 0, 10 });
            Assert.IsTrue((int)nP1.GetFieldOrProperty("Numerator") == 30 &&
                          (int)nP1.GetFieldOrProperty("Denominator") == 10);
            // Whole and Numerator 0
            number = new FC_Number(0, 0, 1, mockLogger.Object);
            nP1 = new PrivateObject(number);
            nP1.Invoke("GenerateImproperFraction", new object[] { 0, 0, 10 });
            Assert.IsTrue((int)nP1.GetFieldOrProperty("Numerator") == 0 &&
                          (int)nP1.GetFieldOrProperty("Denominator") == 10);
            // Negative moved from Whole to Numerator
            number = new FC_Number(0, 0, 1, mockLogger.Object);
            nP1 = new PrivateObject(number);
            nP1.Invoke("GenerateImproperFraction", new object[] { -3, 1, 2 });
            Assert.IsTrue((int)nP1.GetFieldOrProperty("Numerator") == -7 &&
                          (int)nP1.GetFieldOrProperty("Denominator") == 2);
            // Denominator 0
            number = new FC_Number(0, 0, 1, mockLogger.Object);
            nP1 = new PrivateObject(number);            
            var tEx = Assert.ThrowsException<TargetInvocationException>(() => nP1.Invoke("GenerateImproperFraction", new object[] { 1, 2, 0 }));
            Assert.IsInstanceOfType(tEx.InnerException, typeof(ArgumentException));
            // Numerator Int overflow Denominator Multiplication
            number = new FC_Number(0, 0, 1, mockLogger.Object);
            nP1 = new PrivateObject(number);
            tEx = Assert.ThrowsException<TargetInvocationException>(() => nP1.Invoke("GenerateImproperFraction", new object[] { 2147483645, 2, 2 }));
            Assert.IsInstanceOfType(tEx.InnerException, typeof(ArgumentException));
            // Numerator Int overflow Numerator Addition
            number = new FC_Number(0, 0, 1, mockLogger.Object);
            nP1 = new PrivateObject(number);
            tEx = Assert.ThrowsException<TargetInvocationException>(() => nP1.Invoke("GenerateImproperFraction", new object[] { 5, 2147483645, 2 }));
            Assert.IsInstanceOfType(tEx.InnerException, typeof(ArgumentException));
        }

        [TestMethod]
        public void FractionReducerTests()
        {
            // Proper Fraction
            number = new FC_Number(3, 9, mockLogger.Object);
            PrivateObject nP1 = new PrivateObject(number);
            nP1.Invoke("Reducer");
            Assert.IsTrue((int)nP1.GetFieldOrProperty("Numerator") == 1 &&
                          (int)nP1.GetFieldOrProperty("Denominator") == 3);
            // Improper Fraction
            number = new FC_Number(9, 3, mockLogger.Object);
            nP1 = new PrivateObject(number);
            nP1.Invoke("Reducer");
            Assert.IsTrue((int)nP1.GetFieldOrProperty("Numerator") == 3 &&
                          (int)nP1.GetFieldOrProperty("Denominator") == 1);
            // Unreducable
            number = new FC_Number(5, 3, mockLogger.Object);
            nP1 = new PrivateObject(number);
            nP1.Invoke("Reducer");
            Assert.IsTrue((int)nP1.GetFieldOrProperty("Numerator") == 5 &&
                          (int)nP1.GetFieldOrProperty("Denominator") == 3);
            // Reduce to 1
            number = new FC_Number(5, 5, mockLogger.Object);
            nP1 = new PrivateObject(number);
            nP1.Invoke("Reducer");
            Assert.IsTrue((int)nP1.GetFieldOrProperty("Numerator") == 1 &&
                          (int)nP1.GetFieldOrProperty("Denominator") == 1);
            // Reduce to 0
            number = new FC_Number(0, 5, mockLogger.Object);
            nP1 = new PrivateObject(number);
            nP1.Invoke("Reducer");
            Assert.IsTrue((int)nP1.GetFieldOrProperty("Numerator") == 0 &&
                          (int)nP1.GetFieldOrProperty("Denominator") == 5);
            // Reduce Undefined
            number = new FC_Number(10, 1, mockLogger.Object);
            number.Denominator = 0;
            nP1 = new PrivateObject(number);
            var tEx = Assert.ThrowsException<TargetInvocationException>(() => nP1.Invoke("Reducer"));
            Assert.IsInstanceOfType(tEx.InnerException, typeof(InvalidOperationException));
        }

        [TestMethod]
        public void NumberFormatterTest()
        {
            // Whole Number
            number = new FC_Number(333, 111, mockLogger.Object);
            PrivateObject nP1 = new PrivateObject(number);            
            Assert.IsTrue((string)nP1.Invoke("Formatter") == "3");
            // Proper Fraction
            number = new FC_Number(1, 3, mockLogger.Object);
            nP1 = new PrivateObject(number);
            Assert.IsTrue((string)nP1.Invoke("Formatter") == "1/3");
            // Improper Fraction -> Mixed
            number = new FC_Number(5, 4, mockLogger.Object);
            nP1 = new PrivateObject(number);
            Assert.IsTrue((string)nP1.Invoke("Formatter") == "1_1/4");            
            // Zero
            number = new FC_Number(0, 111, mockLogger.Object);
            nP1 = new PrivateObject(number);
            Assert.IsTrue((string)nP1.Invoke("Formatter") == "0");
            // Negative Whole
            number = new FC_Number(-333, 111, mockLogger.Object);
            nP1 = new PrivateObject(number);
            Assert.IsTrue((string)nP1.Invoke("Formatter") == "-3");
            // Negative Fraction
            number = new FC_Number(-11, 33, mockLogger.Object);
            nP1 = new PrivateObject(number);
            Assert.IsTrue((string)nP1.Invoke("Formatter") == "-1/3");
            // Negative Mixed
            number = new FC_Number(-8, 3, mockLogger.Object);
            nP1 = new PrivateObject(number);
            Assert.IsTrue((string)nP1.Invoke("Formatter") == "-2_2/3");
            // Fraction Undefined Denominator 0
            number = new FC_Number(10, 1, mockLogger.Object);
            number.Denominator = 0;
            nP1 = new PrivateObject(number);
            var tEx = Assert.ThrowsException<TargetInvocationException>(() => nP1.Invoke("Formatter"));
            Assert.IsInstanceOfType(tEx.InnerException, typeof(ArgumentException));
        }
    }
}
