using GenericApplicationLogger;
using System;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using static FractionalCalculatorLib.Common;
using static GenericApplicationLogger.Common;

[assembly: InternalsVisibleTo("FractionalCalculatorLibTests")]
namespace FractionalCalculatorLib
{
    public class Calculator
    {
        private static ILogger calcLogger;

        private const int LeftOperand = 0;
        private const int Operation = 1;
        private const int RightOperand = 2;

        private const string TenDigitSignedInt = @"-?[0-9]{1,10}";

        public Calculator(ILogger logger)
        {
            calcLogger = logger ?? throw new ArgumentNullException("Calculator Library requires a valid ILogger instance.");
            calcLogger.LogMessage("Calculator Instantiated", LogLevel.TRACE);
        }

        public string ExecuteQuery(string inputQuery)
        {
            calcLogger.LogMessage("Begin Query Execution", LogLevel.TRACE);
            SimpleQuery sq = ParseQuery(inputQuery);
            FC_Number result = sq.ExecuteSimpleQuery();
            calcLogger.LogMessage("End Query Execution", LogLevel.TRACE);
            return result.ToString();
        }

        private SimpleQuery ParseQuery(string inputQuery)
        {
            string[] tokenizedQuery = TokenizeQuery(inputQuery);

            return new SimpleQuery(ParseNumber(tokenizedQuery[LeftOperand]), ParseNumber(tokenizedQuery[RightOperand]), ParseOperator(tokenizedQuery[Operation]), calcLogger);
        }

        private string[] TokenizeQuery(string inputQuery)
        {
            if (inputQuery.Trim().Length > 69)
                throw new ArgumentException("Input query character length exceeds the maximum allowable defined value");

            string[] tokenizedQuery;
            try
            {
                tokenizedQuery = inputQuery.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            }
            catch (ArgumentException ae)
            {
                throw new ArgumentException($"Unable to tokenize input query: {ae.Message}", ae);
            }

            if (tokenizedQuery.Length != 3)
                throw new ArgumentException($"Invalid input query, expected 3 values but found {tokenizedQuery.Length}");

            return tokenizedQuery;
        }

        private FC_Number ParseNumber(string operand)
        {
            if (operand.Contains(".")) throw new ArgumentException($"Calulator does not support decimal numbers but found {operand}");
            else if (operand.Contains("_") && operand.Contains("/")) return ParseMixedNumber(operand);
            else if (operand.Contains("/")) return ParseFraction(operand);
            else return ParseWholeNumber(operand);
        }

        private FC_Number ParseMixedNumber(string operand)
        {
            Match match = Regex.Match(operand, $@"^{TenDigitSignedInt}_{TenDigitSignedInt}\/{TenDigitSignedInt}$");
            if (match != null)
            {
                string stringMixedWhole = match.Value.Substring(0, match.Value.IndexOf("_"));
                string stringMixedNumerator = match.Value.Substring(match.Value.IndexOf("_") + 1, match.Value.IndexOf("/") - match.Value.IndexOf("_") - 1);
                string stringMicedDenominator = match.Value.Substring(match.Value.IndexOf("/") + 1);
                if (stringMixedNumerator.StartsWith("-") || stringMicedDenominator.StartsWith("-"))
                    throw new ArgumentException($"Found mixed number with improper signs in eitherthe numerator and/or the denominator when it should only exist in front of the whole number portion of the value: {match.Value}");
                return new FC_Number(ParseInt(stringMixedWhole), ParseInt(stringMixedNumerator), ParseInt(stringMicedDenominator), calcLogger);
            }
            throw new ArgumentException($"Expected Mixed Number consisting of 32-bit signed integers (-2,147,483,648 to 2,147,483,647) but found: {operand}");
        }

        private FC_Number ParseFraction(string operand)
        {
            Match match = Regex.Match(operand, $@"^{TenDigitSignedInt}\/{TenDigitSignedInt}$");
            if (match != null)
            {
                return new FC_Number(ParseInt(match.Value.Substring(0, match.Value.IndexOf("/"))),
                                     ParseInt(match.Value.Substring(match.Value.IndexOf("/") + 1)),
                                     calcLogger);
            }
            throw new ArgumentException($"Expected Fraction of format \"A/B\" consisting of 32-bit signed integers (-2,147,483,648 to 2,147,483,647) but found: {operand}");
        }

        private FC_Number ParseWholeNumber(string operand)
        {
            return new FC_Number(ParseInt(operand), calcLogger);
        }

        private int ParseInt(string number)
        {
            Match match = Regex.Match(number, $@"^{TenDigitSignedInt}$");
            if (match != null && int.TryParse(match.Value, out int parsedInt)) return parsedInt;
            throw new ArgumentException($"Expected 32-bit signed integer (-2,147,483,648 to 2,147,483,647) but found: {number}");  
        }

        private Operation ParseOperator(string operation)
        {
            switch (operation)
            {
                case "+":
                    return Common.Operation.Add;
                case "-":
                    return Common.Operation.Subtract;
                case "*":
                    return Common.Operation.Multiply;
                case "/":
                    return Common.Operation.Divide;
                default:
                    throw new ArgumentException($"Invalid input query, expected arithmetic operation token between operands but found: {operation}");
            }
        }
    }
}
