using FractionalCalculatorLib;
using GenericApplicationLogger;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Linq;
using System.Reflection;
using static FractionalCalculatorLib.Common;
using static GenericApplicationLogger.Common;

namespace FractionalCalculatorTests
{
    /// <summary>
    /// This class tests user input and the required parsing for processing of the input.
    /// </summary>
    [TestClass]
    public class FC_InputTests
    {
        Mock<ILogger> mockLogger;
        //Mock<Calculator> mockCalc;
        Calculator testCalc;
        PrivateObject poCalc;

        [TestInitialize]
        public void Setup()
        {
            mockLogger = new Mock<ILogger>();
            mockLogger.SetupGet(x => x.DefaultLogLevel).Returns(LogLevel.DEBUG);
            mockLogger.Setup(x => x.LogMessage(It.IsAny<string>()));
            mockLogger.Setup(x => x.LogMessage(It.IsAny<string>(), It.IsAny<LogLevel>()));

            //mockCalc = new Mock<Calculator>();
                       
            testCalc = new Calculator(mockLogger.Object);
            poCalc = new PrivateObject(testCalc);
        }

        [TestMethod]
        public void QueryTokenizationTests()
        {
            //SimpleResult + correct token count, tokenized in proper order
            Assert.IsTrue(((string[])poCalc.Invoke("TokenizeQuery", new object[1] { "1 + 2" })).SequenceEqual(new string[3] { "1", "+", "2" }));
            // Input too long (70 chars) fast fail
            string longString = "abcdefghijklmnopqustuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqr";
            var longEx = Assert.ThrowsException<TargetInvocationException>(() => poCalc.Invoke("TokenizeQuery", new object[1] { longString }));
            Assert.IsInstanceOfType(longEx.InnerException, typeof(ArgumentException));
            // Invalid token count
            string noToken = "";
            var noEx = Assert.ThrowsException<TargetInvocationException>(() => poCalc.Invoke("TokenizeQuery", new object[1] { noToken }));
            Assert.IsInstanceOfType(noEx.InnerException, typeof(ArgumentException));
            string shortToken = "1 2";
            var shortEx = Assert.ThrowsException<TargetInvocationException>(() => poCalc.Invoke("TokenizeQuery", new object[1] { shortToken }));
            Assert.IsInstanceOfType(shortEx.InnerException, typeof(ArgumentException));
            string longToken = "1 + 2 =";
            var longtEx = Assert.ThrowsException<TargetInvocationException>(() => poCalc.Invoke("TokenizeQuery", new object[1] { longToken }));
            Assert.IsInstanceOfType(longtEx.InnerException, typeof(ArgumentException));
        }

        [TestMethod]
        public void ParseOperatorTests()
        {
            // Test good operators
            Assert.IsTrue(((Operation)poCalc.Invoke("ParseOperator", new object[1] { "+" })) == Operation.Add);
            Assert.IsTrue(((Operation)poCalc.Invoke("ParseOperator", new object[1] { "-" })) == Operation.Subtract);
            Assert.IsTrue(((Operation)poCalc.Invoke("ParseOperator", new object[1] { "*" })) == Operation.Multiply);
            Assert.IsTrue(((Operation)poCalc.Invoke("ParseOperator", new object[1] { "/" })) == Operation.Divide);
            // Test invalid operators
            var tiEx = Assert.ThrowsException<TargetInvocationException>(() => poCalc.Invoke("ParseOperator", new object[1] { "x" }));
            Assert.IsInstanceOfType(tiEx.InnerException, typeof(ArgumentException));
        }

        [TestMethod]
        public void ParseNumberTests()
        {
            // Error test for decimal input
            var tiEx = Assert.ThrowsException<TargetInvocationException>(() => poCalc.Invoke("ParseNumber", new object[1] { "1.0" }));
            Assert.IsInstanceOfType(tiEx.InnerException, typeof(ArgumentException));
            // Needs modification to test conditional pathing
        }

        [TestMethod]
        public void ParseIntTests()
        {
            // Test Good nUmber
            Assert.IsTrue(((int)poCalc.Invoke("ParseInt", new object[1] { "42" })) == 42);
            // Test Garbage Number
            var tiEx = Assert.ThrowsException<TargetInvocationException>(() => poCalc.Invoke("ParseInt", new object[1] { "1D4" }));
            Assert.IsInstanceOfType(tiEx.InnerException, typeof(ArgumentException));
            // Test 10 digits positive no overflow
            Assert.IsTrue(((int)poCalc.Invoke("ParseInt", new object[1] { "2147483647" })) == 2147483647);
            // Test 10 digits negative no overflow
            Assert.IsTrue(((int)poCalc.Invoke("ParseInt", new object[1] { "-2147483648" })) == -2147483648);
            // Test 10 digits positive with overflow
            var posOverEx = Assert.ThrowsException<TargetInvocationException>(() => poCalc.Invoke("ParseInt", new object[1] { "2147483648" }));
            Assert.IsInstanceOfType(posOverEx.InnerException, typeof(ArgumentException));
            // Test 10 digits negative with overflow
            var negOverEx = Assert.ThrowsException<TargetInvocationException>(() => poCalc.Invoke("ParseInt", new object[1] { "-2147483649" }));
            Assert.IsInstanceOfType(negOverEx.InnerException, typeof(ArgumentException));
            // Test 11 Digits
            var superOverEx = Assert.ThrowsException<TargetInvocationException>(() => poCalc.Invoke("ParseInt", new object[1] { "21474836470" }));
            Assert.IsInstanceOfType(superOverEx.InnerException, typeof(ArgumentException));
        }

        [TestMethod]
        public void ParseFractionTests()
        {
            // Test Good Fraction Match
            FC_Number fraction = (FC_Number)poCalc.Invoke("ParseFraction", new object[1] { "3/4" });
            Assert.IsTrue(fraction.Numerator == 3 && fraction.Denominator == 4);
            // Test Not Fraction Number
            var tiEx = Assert.ThrowsException<TargetInvocationException>(() => poCalc.Invoke("ParseFraction", new object[1] { "45" }));
            Assert.IsInstanceOfType(tiEx.InnerException, typeof(ArgumentException));
            // Test Bad Fraction
            tiEx = Assert.ThrowsException<TargetInvocationException>(() => poCalc.Invoke("ParseFraction", new object[1] { "44/" }));
            Assert.IsInstanceOfType(tiEx.InnerException, typeof(ArgumentException));
            tiEx = Assert.ThrowsException<TargetInvocationException>(() => poCalc.Invoke("ParseFraction", new object[1] { "/44" }));
            Assert.IsInstanceOfType(tiEx.InnerException, typeof(ArgumentException));
            tiEx = Assert.ThrowsException<TargetInvocationException>(() => poCalc.Invoke("ParseFraction", new object[1] { "44/Q" }));
            Assert.IsInstanceOfType(tiEx.InnerException, typeof(ArgumentException));
            tiEx = Assert.ThrowsException<TargetInvocationException>(() => poCalc.Invoke("ParseFraction", new object[1] { "44//221" }));
            Assert.IsInstanceOfType(tiEx.InnerException, typeof(ArgumentException));
            tiEx = Assert.ThrowsException<TargetInvocationException>(() => poCalc.Invoke("ParseFraction", new object[1] { "/44/55/" }));
            Assert.IsInstanceOfType(tiEx.InnerException, typeof(ArgumentException));
            // Needs modification to test substinging
        }

        [TestMethod]
        public void ParseMixedNumberTests()
        {
            // Test Good Mixed Number Match
            FC_Number fraction = (FC_Number)poCalc.Invoke("ParseMixedNumber", new object[1] { "5_3/4" });
            Assert.IsTrue(fraction.Numerator == 23 && fraction.Denominator == 4);
            // Test Illegal Fractional Negatives
            var tiEx = Assert.ThrowsException<TargetInvocationException>(() => poCalc.Invoke("ParseMixedNumber", new object[1] { "5_-3/4" }));
            Assert.IsInstanceOfType(tiEx.InnerException, typeof(ArgumentException));
            tiEx = Assert.ThrowsException<TargetInvocationException>(() => poCalc.Invoke("ParseMixedNumber", new object[1] { "5_3/-4" }));
            Assert.IsInstanceOfType(tiEx.InnerException, typeof(ArgumentException));
            tiEx = Assert.ThrowsException<TargetInvocationException>(() => poCalc.Invoke("ParseMixedNumber", new object[1] { "5_-3/-4" }));
            Assert.IsInstanceOfType(tiEx.InnerException, typeof(ArgumentException));
            // Test Bad Mixed Number
            tiEx = Assert.ThrowsException<TargetInvocationException>(() => poCalc.Invoke("ParseMixedNumber", new object[1] { "3_/" }));
            Assert.IsInstanceOfType(tiEx.InnerException, typeof(ArgumentException));
            tiEx = Assert.ThrowsException<TargetInvocationException>(() => poCalc.Invoke("ParseMixedNumber", new object[1] { "a_c/6" }));
            Assert.IsInstanceOfType(tiEx.InnerException, typeof(ArgumentException));
            tiEx = Assert.ThrowsException<TargetInvocationException>(() => poCalc.Invoke("ParseMixedNumber", new object[1] { "_3/0" }));
            Assert.IsInstanceOfType(tiEx.InnerException, typeof(ArgumentException));
            tiEx = Assert.ThrowsException<TargetInvocationException>(() => poCalc.Invoke("ParseMixedNumber", new object[1] { "_/9" }));
            Assert.IsInstanceOfType(tiEx.InnerException, typeof(ArgumentException));
            tiEx = Assert.ThrowsException<TargetInvocationException>(() => poCalc.Invoke("ParseMixedNumber", new object[1] { "3/4_3" }));
            Assert.IsInstanceOfType(tiEx.InnerException, typeof(ArgumentException));
            tiEx = Assert.ThrowsException<TargetInvocationException>(() => poCalc.Invoke("ParseMixedNumber", new object[1] { "/3/4_4" }));
            Assert.IsInstanceOfType(tiEx.InnerException, typeof(ArgumentException));
            // Needs modification to test substinging
        }
    }
}