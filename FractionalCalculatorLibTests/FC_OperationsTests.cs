using FractionalCalculatorLib;
using GenericApplicationLogger;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Reflection;
using static FractionalCalculatorLib.Common;
using static GenericApplicationLogger.Common;

namespace FractionalCalculatorTests
{
    [TestClass]
    public class FC_OperationsTests
    {
        Mock<ILogger> mockLogger;
        SimpleQuery sq;

        [TestInitialize]
        public void Setup()
        {
            mockLogger = new Mock<ILogger>();
            mockLogger.SetupGet(x => x.DefaultLogLevel).Returns(LogLevel.DEBUG);
            mockLogger.Setup(x => x.LogMessage(It.IsAny<string>()));
            mockLogger.Setup(x => x.LogMessage(It.IsAny<string>(), It.IsAny<LogLevel>()));            
        }

        [TestMethod]
        public void ImproperFractionNormalizationTests()
        {
            // Test Proper Normalization
            sq = new SimpleQuery(new FC_Number(10, 5, mockLogger.Object), new FC_Number(29, 3, mockLogger.Object), Operation.Add, mockLogger.Object);
            PrivateObject sqP1 = new PrivateObject(sq);
            sqP1.Invoke("NormalizeImproperFraction");
            Assert.IsTrue(((FC_Number)sqP1.GetProperty("LeftOperand")).Numerator == 30 &&
                          ((FC_Number)sqP1.GetProperty("LeftOperand")).Denominator == 15 &&
                          ((FC_Number)sqP1.GetProperty("RightOperand")).Numerator == 145 &&
                          ((FC_Number)sqP1.GetProperty("RightOperand")).Denominator == 15);
            // Test Int32 Overflow Numerator and Denominator
            sq = new SimpleQuery(new FC_Number(7, 1500000000, mockLogger.Object), new FC_Number(1, 3, mockLogger.Object), Operation.Add, mockLogger.Object);
            PrivateObject sqP2 = new PrivateObject(sq);
            var dtEx = Assert.ThrowsException<TargetInvocationException>(() => sqP2.Invoke("NormalizeImproperFraction"));
            Assert.IsInstanceOfType(dtEx.InnerException, typeof(ArgumentException));
            sq = new SimpleQuery(new FC_Number(7, 5, mockLogger.Object), new FC_Number(1500000000, 3, mockLogger.Object), Operation.Add, mockLogger.Object);
            PrivateObject sqP3 = new PrivateObject(sq);
            var ntEx = Assert.ThrowsException<TargetInvocationException>(() => sqP3.Invoke("NormalizeImproperFraction"));
            Assert.IsInstanceOfType(ntEx.InnerException, typeof(ArgumentException));
        }

        [TestMethod]
        public void AdditionTests()
        {
            // Simple working test
            sq = new SimpleQuery(new FC_Number(5, 10, mockLogger.Object), new FC_Number(29, 3, mockLogger.Object), Operation.Add, mockLogger.Object);
            PrivateObject sqP1 = new PrivateObject(sq);
            FC_Number addResult = (FC_Number)sqP1.Invoke("Add");
            Assert.IsTrue(addResult.Numerator == 305 && addResult.Denominator == 30);
            // Addition of Negatives Tests
            sq = new SimpleQuery(new FC_Number(-5, 10, mockLogger.Object), new FC_Number(9, 3, mockLogger.Object), Operation.Add, mockLogger.Object);
            PrivateObject sqP2 = new PrivateObject(sq);
            FC_Number lNegResult = (FC_Number)sqP2.Invoke("Add");
            Assert.IsTrue(lNegResult.Numerator == 75 && lNegResult.Denominator == 30);
            sq = new SimpleQuery(new FC_Number(5, 10, mockLogger.Object), new FC_Number(-9, 3, mockLogger.Object), Operation.Add, mockLogger.Object);
            PrivateObject sqP3 = new PrivateObject(sq);
            FC_Number rNegResult = (FC_Number)sqP3.Invoke("Add");
            Assert.IsTrue(rNegResult.Numerator == -75 && rNegResult.Denominator == 30);
            sq = new SimpleQuery(new FC_Number(-5, 10, mockLogger.Object), new FC_Number(-9, 3, mockLogger.Object), Operation.Add, mockLogger.Object);
            PrivateObject sqP4 = new PrivateObject(sq);
            FC_Number bNegResult = (FC_Number)sqP4.Invoke("Add");
            Assert.IsTrue(bNegResult.Numerator == -105 && bNegResult.Denominator == 30);
            // Int32 Overflow Test Numerator
            sq = new SimpleQuery(new FC_Number(2147483647, 1, mockLogger.Object), new FC_Number(1, 1, mockLogger.Object), Operation.Add, mockLogger.Object);
            PrivateObject sqP5 = new PrivateObject(sq);
            var tEx = Assert.ThrowsException<TargetInvocationException>(() => sqP5.Invoke("Add"));
            Assert.IsInstanceOfType(tEx.InnerException, typeof(ArgumentException));
        }

        [TestMethod]
        public void SubtractionTests()
        {
            // Simple working test
            sq = new SimpleQuery(new FC_Number(5, 10, mockLogger.Object), new FC_Number(29, 3, mockLogger.Object), Operation.Subtract, mockLogger.Object);
            PrivateObject sqP1 = new PrivateObject(sq);
            FC_Number subResult = (FC_Number)sqP1.Invoke("Subtract");
            Assert.IsTrue(subResult.Numerator == -275 && subResult.Denominator == 30);
            // Subtraction of Negatives Tests
            sq = new SimpleQuery(new FC_Number(-5, 10, mockLogger.Object), new FC_Number(9, 3, mockLogger.Object), Operation.Subtract, mockLogger.Object);
            PrivateObject sqP2 = new PrivateObject(sq);
            FC_Number lNegResult = (FC_Number)sqP2.Invoke("Subtract");
            Assert.IsTrue(lNegResult.Numerator == -105 && lNegResult.Denominator == 30);
            sq = new SimpleQuery(new FC_Number(5, 10, mockLogger.Object), new FC_Number(-9, 3, mockLogger.Object), Operation.Subtract, mockLogger.Object);
            PrivateObject sqP3 = new PrivateObject(sq);
            FC_Number rNegResult = (FC_Number)sqP3.Invoke("Subtract");
            Assert.IsTrue(rNegResult.Numerator == 105 && rNegResult.Denominator == 30);
            sq = new SimpleQuery(new FC_Number(-5, 10, mockLogger.Object), new FC_Number(-9, 3, mockLogger.Object), Operation.Subtract, mockLogger.Object);
            PrivateObject sqP4 = new PrivateObject(sq);
            FC_Number bNegResult = (FC_Number)sqP4.Invoke("Subtract");
            Assert.IsTrue(bNegResult.Numerator == 75 && bNegResult.Denominator == 30);
            // Int32 Overflow Test Numerator
            sq = new SimpleQuery(new FC_Number(-2147483648, 1, mockLogger.Object), new FC_Number(1, 1, mockLogger.Object), Operation.Subtract, mockLogger.Object);
            PrivateObject sqP5 = new PrivateObject(sq);
            var tEx = Assert.ThrowsException<TargetInvocationException>(() => sqP5.Invoke("Subtract"));
            Assert.IsInstanceOfType(tEx.InnerException, typeof(ArgumentException));
        }

        [TestMethod]
        public void MultiplicationTests()
        {
            // Simple working test
            sq = new SimpleQuery(new FC_Number(5, 10, mockLogger.Object), new FC_Number(29, 3, mockLogger.Object), Operation.Multiply, mockLogger.Object);
            PrivateObject sqP1 = new PrivateObject(sq);
            FC_Number multResult = (FC_Number)sqP1.Invoke("Multiply");
            Assert.IsTrue(multResult.Numerator == 145 && multResult.Denominator == 30);
            // Multiplication of Negatives Tests
            sq = new SimpleQuery(new FC_Number(-5, 10, mockLogger.Object), new FC_Number(9, 3, mockLogger.Object), Operation.Multiply, mockLogger.Object);
            PrivateObject sqP2 = new PrivateObject(sq);
            FC_Number lNegResult = (FC_Number)sqP2.Invoke("Multiply");
            Assert.IsTrue(lNegResult.Numerator == -45 && lNegResult.Denominator == 30);
            sq = new SimpleQuery(new FC_Number(5, 10, mockLogger.Object), new FC_Number(-9, 3, mockLogger.Object), Operation.Multiply, mockLogger.Object);
            PrivateObject sqP3 = new PrivateObject(sq);
            FC_Number rNegResult = (FC_Number)sqP3.Invoke("Multiply");
            Assert.IsTrue(rNegResult.Numerator == -45 && rNegResult.Denominator == 30);
            sq = new SimpleQuery(new FC_Number(-5, 10, mockLogger.Object), new FC_Number(-9, 3, mockLogger.Object), Operation.Multiply, mockLogger.Object);
            PrivateObject sqP4 = new PrivateObject(sq);
            FC_Number bNegResult = (FC_Number)sqP4.Invoke("Multiply");
            Assert.IsTrue(bNegResult.Numerator == 45 && bNegResult.Denominator == 30);
            // Int32 Overflow Test Numerator and Denominator
            sq = new SimpleQuery(new FC_Number(2147483647, 1, mockLogger.Object), new FC_Number(2, 1, mockLogger.Object), Operation.Multiply, mockLogger.Object);
            PrivateObject sqP5 = new PrivateObject(sq);
            var tEx = Assert.ThrowsException<TargetInvocationException>(() => sqP5.Invoke("Multiply"));
            Assert.IsInstanceOfType(tEx.InnerException, typeof(ArgumentException));
            sq = new SimpleQuery(new FC_Number(1, 2, mockLogger.Object), new FC_Number(1, 2147483647, mockLogger.Object), Operation.Multiply, mockLogger.Object);
            PrivateObject sqP6 = new PrivateObject(sq);
            tEx = Assert.ThrowsException<TargetInvocationException>(() => sqP6.Invoke("Multiply"));
            Assert.IsInstanceOfType(tEx.InnerException, typeof(ArgumentException));
        }

        [TestMethod]
        public void DivisionTests()
        {
            // Simple working test
            sq = new SimpleQuery(new FC_Number(5, 10, mockLogger.Object), new FC_Number(1, 4, mockLogger.Object), Operation.Divide, mockLogger.Object);
            PrivateObject sqP1 = new PrivateObject(sq);
            FC_Number divResult = (FC_Number)sqP1.Invoke("Divide");
            Assert.IsTrue(divResult.Numerator == 20 && divResult.Denominator == 10);
            // Division of Negatives Tests
            sq = new SimpleQuery(new FC_Number(-5, 10, mockLogger.Object), new FC_Number(1, 4, mockLogger.Object), Operation.Divide, mockLogger.Object);
            PrivateObject sqP2 = new PrivateObject(sq);
            FC_Number lNegResult = (FC_Number)sqP2.Invoke("Divide");
            Assert.IsTrue(lNegResult.Numerator == -20 && lNegResult.Denominator == 10);
            sq = new SimpleQuery(new FC_Number(5, 10, mockLogger.Object), new FC_Number(-1, 4, mockLogger.Object), Operation.Divide, mockLogger.Object);
            PrivateObject sqP3 = new PrivateObject(sq);
            FC_Number rNegResult = (FC_Number)sqP3.Invoke("Divide");
            Assert.IsTrue(rNegResult.Numerator == -20 && rNegResult.Denominator == 10);
            sq = new SimpleQuery(new FC_Number(-5, 10, mockLogger.Object), new FC_Number(-1, 4, mockLogger.Object), Operation.Divide, mockLogger.Object);
            PrivateObject sqP4 = new PrivateObject(sq);
            FC_Number bNegResult = (FC_Number)sqP4.Invoke("Divide");
            Assert.IsTrue(bNegResult.Numerator == 20 && bNegResult.Denominator == 10);
            // Int32 Overflow Test Numerator and Denominator
            sq = new SimpleQuery(new FC_Number(2147483647, 1, mockLogger.Object), new FC_Number(1, 2, mockLogger.Object), Operation.Divide, mockLogger.Object);
            PrivateObject sqP5 = new PrivateObject(sq);
            var tEx = Assert.ThrowsException<TargetInvocationException>(() => sqP5.Invoke("Divide"));
            Assert.IsInstanceOfType(tEx.InnerException, typeof(ArgumentException));
            sq = new SimpleQuery(new FC_Number(2, 1, mockLogger.Object), new FC_Number(1, 2147483647, mockLogger.Object), Operation.Divide, mockLogger.Object);
            PrivateObject sqP6 = new PrivateObject(sq);
            tEx = Assert.ThrowsException<TargetInvocationException>(() => sqP6.Invoke("Divide"));
            Assert.IsInstanceOfType(tEx.InnerException, typeof(ArgumentException));
            //Division by Zero Test
            sq = new SimpleQuery(new FC_Number(2, 1, mockLogger.Object), new FC_Number(0, 10, mockLogger.Object), Operation.Divide, mockLogger.Object);
            PrivateObject sqP7 = new PrivateObject(sq);
            tEx = Assert.ThrowsException<TargetInvocationException>(() => sqP7.Invoke("Divide"));
            Assert.IsInstanceOfType(tEx.InnerException, typeof(ArgumentException));
        }
    }
}
