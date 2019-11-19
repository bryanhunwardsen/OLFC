using GenericApplicationLogger;
using System;
using static FractionalCalculatorLib.Common;

namespace FractionalCalculatorLib
{
    internal class SimpleQuery
    {
        internal FC_Number LeftOperand { get; set; }
        internal FC_Number RightOperand { get; set; }
        internal Operation Operation { get; set; }

        private static ILogger queryLogger;

        internal SimpleQuery(FC_Number leftOperand, FC_Number rightOperand, Operation operation, ILogger logger)
        {
            LeftOperand = leftOperand;
            RightOperand = rightOperand;
            Operation = operation;
            queryLogger = logger;
        }

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

        private FC_Number Add()
        {
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

        private FC_Number Subtract()
        {
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

        private FC_Number Multiply()
        {
            try
            {
                return new FC_Number(checked(LeftOperand.Numerator * RightOperand.Numerator), checked(LeftOperand.Denominator * RightOperand.Denominator), queryLogger);
            }
            catch
            {
                throw new ArgumentException(Int32ResultOverflow("Multiplication"));
            }            
        }

        private FC_Number Divide()
        {
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
                return new FC_Number(numerator, denominator, queryLogger);

            }
            catch
            {
                throw new ArgumentException(Int32ResultOverflow("Division"));
            }
        }

        private void NormalizeImproperFraction()
        {   
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

        private string Int32ResultOverflow(string operation)
        {
            return $"Requested {operation} operation results in an intermediate or resultant signed Int32 overflow.";
        }
    }
}
