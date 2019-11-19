using GenericApplicationLogger;
using System;
using static FractionalCalculatorLib.Common;
using static GenericApplicationLogger.Common;

namespace FractionalCalculatorLib
{
    /// <summary>
    /// In the absence of multiple operations and parenthetic nesting the simple query behave as a store/executor for: left operand operation right operand
    /// </summary>
    internal class SimpleQuery
    {
        /// <summary>
        /// Query left operand
        /// </summary>
        internal FC_Number LeftOperand { get; set; }
        /// <summary>
        /// Query right operand
        /// </summary>
        internal FC_Number RightOperand { get; set; }
        /// <summary>
        /// Query operation
        /// </summary>
        internal Operation Operation { get; set; }
        /// <summary>
        /// Query Logger
        /// </summary>
        private static ILogger queryLogger;

        /// <summary>
        /// Instantiates required values for a simple query
        /// </summary>
        /// <param name="leftOperand">Left query operand</param>
        /// <param name="rightOperand">Right Query Operand</param>
        /// <param name="operation">Query Operation</param>
        /// <param name="logger">Application Logger</param>
        internal SimpleQuery(FC_Number leftOperand, FC_Number rightOperand, Operation operation, ILogger logger)
        {
            LeftOperand = leftOperand;
            RightOperand = rightOperand;
            Operation = operation;
            queryLogger = logger;

            queryLogger.LogMessage($"Simple Query instanitiated as {leftOperand} {operation} {rightOperand}", LogLevel.TRACE);
        }

        /// <summary>
        /// Routes query execution to appropriate arithmetic sub operation
        /// </summary>
        /// <returns></returns>
        internal FC_Number ExecuteSimpleQuery()
        {
            switch (Operation)
            {
                case Operation.Add:
                    return Add();
                case Operation.Subtract:
                    return Subtract();
                case Operation.Multiply:
                    return Multiply();
                case Operation.Divide:
                    return Divide();
                default:
                    throw new System.Exception();
            }
        }

        /// <summary>
        /// Adds (im)proper fractions and reutns the result
        /// </summary>
        /// <returns>Resulting fraction from add operation</returns>
        private FC_Number Add()
        {
            queryLogger.LogMessage($"Begin add execution", LogLevel.TRACE);
            NormalizeImproperFraction();
            try
            {
                return new FC_Number(checked(LeftOperand.Numerator + RightOperand.Numerator), LeftOperand.Denominator, queryLogger);
            }
            catch
            {
                throw new ArgumentException(Int32ResultOverflow("Addition"));
            }
        }

        /// <summary>
        /// Subtracts (im)proper fractions and reutns the result
        /// </summary>
        /// <returns>Resulting fraction from subtraction operation</returns>
        private FC_Number Subtract()
        {
            queryLogger.LogMessage($"Begin subtraction execution", LogLevel.TRACE);
            NormalizeImproperFraction();
            try
            {
                return new FC_Number(checked(LeftOperand.Numerator - RightOperand.Numerator), LeftOperand.Denominator, queryLogger);
            }
            catch
            {
                throw new ArgumentException(Int32ResultOverflow("Subtraction"));
            }
        }

        /// <summary>
        /// Multiplie (im)proper fractions and reutns the result
        /// </summary>
        /// <returns>Resulting fraction from multiplication operation</returns>
        private FC_Number Multiply()
        {
            queryLogger.LogMessage($"Begin multiplication execution", LogLevel.TRACE);
            try
            {
                return new FC_Number(checked(LeftOperand.Numerator * RightOperand.Numerator), checked(LeftOperand.Denominator * RightOperand.Denominator), queryLogger);
            }
            catch
            {
                throw new ArgumentException(Int32ResultOverflow("Multiplication"));
            }            
        }

        /// <summary>
        /// Divides (im)proper fractions and reutns the result
        /// </summary>
        /// <returns>Resulting fraction from division operation</returns>
        private FC_Number Divide()
        {
            queryLogger.LogMessage($"Begin division execution", LogLevel.TRACE);

            int numerator;
            int denominator;

            try
            {
                numerator = checked(LeftOperand.Numerator * RightOperand.Denominator);
                denominator = checked(LeftOperand.Denominator * RightOperand.Numerator);
                if (RightOperand.Numerator < 0)
                {
                    numerator = checked(numerator * -1);
                    denominator = checked(denominator * -1);
                }
                // TODO: investigate which/why each operation may not created a fully reduced result
                // Possible trace logging tostring is impact some prior to unit test validation
                return new FC_Number(numerator, denominator, queryLogger);

            }
            catch
            {
                throw new ArgumentException(Int32ResultOverflow("Division"));
            }
        }

        /// <summary>
        /// Normalizes (im)proper fractions to have common denominators to aid in arithmetic operations
        /// </summary>
        private void NormalizeImproperFraction()
        {
            queryLogger.LogMessage($"Begin FractionNormalization execution", LogLevel.TRACE);

            int lhd = LeftOperand.Denominator;
            int rhd = RightOperand.Denominator;

            try
            {
                LeftOperand.Denominator = checked(LeftOperand.Denominator * rhd);
                RightOperand.Denominator = checked(RightOperand.Denominator * lhd);
                LeftOperand.Numerator = checked(LeftOperand.Numerator * rhd);
                RightOperand.Numerator = checked(RightOperand.Numerator * lhd);
            }
            catch
            {
                throw new ArgumentException("Requested operation results in an intermediate or resultant signed Int32 overflow.");
            }

        }

        /// <summary>
        /// Returns an operation specific error regarding an argumentexception caused byt a signed Int32 overflow condition
        /// </summary>
        /// <param name="operation">Operation where overflow occurred</param>
        /// <returns></returns>
        private string Int32ResultOverflow(string operation)
        {
            return $"Requested {operation} operation results in an intermediate or resultant signed Int32 overflow.";
        }
    }
}
