using GenericApplicationLogger;
using System;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using static FractionalCalculatorLib.Common;
using static GenericApplicationLogger.Common;

[assembly: InternalsVisibleTo("FractionalCalculatorLibTests")]
namespace FractionalCalculatorLib
{
    /// <summary>
    /// Fraction Calculator Class takes , whole, (im)proper and mixed numbers for simple operation of left operand operation right operand
    /// </summary>
    public class Calculator
    {
        /// <summary>
        /// Application Logger
        /// </summary>
        private static ILogger calcLogger;

        /// <summary>
        /// Left Operand Index in tokenized array
        /// </summary>
        private const int LeftOperand = 0;
        /// <summary>
        /// Query Operation Index in tokenized array
        /// </summary>
        private const int Operation = 1;
        /// <summary>
        /// Right Operand Index in tokenized array
        /// </summary>
        private const int RightOperand = 2;
        /// <summary>
        /// Regex to match min to max value of signed Int32
        /// Is aggressive in collection taking all 10 digit positive and negative numbers
        /// This overflow is handled during string to int parsing
        /// </summary>
        private const string TenDigitSignedInt = @"-?[0-9]{1,10}";

        /// <summary>
        /// Initiaes a Fractional Calculator with an application logger
        /// </summary>
        /// <param name="logger">Application Logger</param>
        public Calculator(ILogger logger)
        {
            calcLogger = logger ?? throw new ArgumentNullException("Calculator Library requires a valid ILogger instance.");
            calcLogger.LogMessage("Calculator Instantiated", LogLevel.TRACE);
        }

        /// <summary>
        /// Given an input string from a calling source it will attempt to parse, execute and return a value for a fraction capable operation
        /// </summary>
        /// <param name="inputQuery">The string containing the arithmetic input of format "leftOperand operator rightOperand"</param>
        /// <returns>returns the numeric result of the operation as a string if the calculation was possible</returns>
        public string ExecuteQuery(string inputQuery)
        {
            calcLogger.LogMessage("Begin Query Execution", LogLevel.TRACE);
            SimpleQuery sq = ParseQuery(inputQuery);
            FC_Number result = sq.ExecuteSimpleQuery();
            calcLogger.LogMessage("End Query Execution", LogLevel.TRACE);
            return result.ToString();
        }

        /// <summary>
        /// Attempts to parse query into required format of "leftOperand operator rightOperand"
        /// </summary>
        /// <param name="inputQuery">string to be parsed into memory for arithmetic execution</param>
        /// <returns>A parsed simple query of leftOperand, operator, right Operand</returns>
        private SimpleQuery ParseQuery(string inputQuery)
        {
            calcLogger.LogMessage("Begin query parsing", LogLevel.TRACE);
            string[] tokenizedQuery = TokenizeQuery(inputQuery);
            return new SimpleQuery(ParseNumber(tokenizedQuery[LeftOperand]), ParseNumber(tokenizedQuery[RightOperand]), ParseOperator(tokenizedQuery[Operation]), calcLogger);
        }

        /// <summary>
        /// Attempts to tokenize input string to three ordered values of leftOperand, operator, rightOperand
        /// </summary>
        /// <param name="inputQuery">The raw string requireing parsing</param>
        /// <returns>string array of the tokenized input of 3 values</returns>
        private string[] TokenizeQuery(string inputQuery)
        {
            calcLogger.LogMessage("Begin string tokenization", LogLevel.TRACE);
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

        /// <summary>
        /// Uses string inspection to discover and route the number to be parsed based on format of either whole number, (im)proper fraction, or mixed number
        /// </summary>
        /// <param name="operand">The number to be parsed</param>
        /// <returns>The parsed number in numeric/object format</returns>
        private FC_Number ParseNumber(string operand)
        {
            calcLogger.LogMessage($"Begin parsing the operand {operand}", LogLevel.TRACE);
            if (operand.Contains(".")) throw new ArgumentException($"Calulator does not support decimal numbers but found {operand}");
            else if (operand.Contains("_") && operand.Contains("/")) return ParseMixedNumber(operand);
            else if (operand.Contains("/")) return ParseFraction(operand);
            else return ParseWholeNumber(operand);
        }

        /// <summary>
        /// Parses mixed numbers into numerical/class representation for calculation
        /// </summary>
        /// <param name="operand">The Mixed number to be parsed</param>
        /// <returns>9im)proper fraction representation of the mixed number</returns>
        private FC_Number ParseMixedNumber(string operand)
        {
            calcLogger.LogMessage($"Begin parsing the Mixed Number {operand}", LogLevel.TRACE);
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

        /// <summary>
        /// Parses (im)proper fractions into numerical/class representation for calculation
        /// </summary>
        /// <param name="operand">The fraction to be parsed</param>
        /// <returns>9im)Numerical/Class representation of the fraction</returns>
        private FC_Number ParseFraction(string operand)
        {
            calcLogger.LogMessage($"Begin parsing the fraction {operand}", LogLevel.TRACE);
            Match match = Regex.Match(operand, $@"^{TenDigitSignedInt}\/{TenDigitSignedInt}$");
            if (match != null)
            {
                return new FC_Number(ParseInt(match.Value.Substring(0, match.Value.IndexOf("/"))),
                                     ParseInt(match.Value.Substring(match.Value.IndexOf("/") + 1)),
                                     calcLogger);
            }
            throw new ArgumentException($"Expected Fraction of format \"A/B\" consisting of 32-bit signed integers (-2,147,483,648 to 2,147,483,647) but found: {operand}");
        }

        /// <summary>
        /// Parses a whole number into numerical/class representation for calculation
        /// </summary>
        /// <param name="operand">The whole number to be parsed</param>
        /// <returns>9im)Numerical/Class representation of the whole number</returns>
        private FC_Number ParseWholeNumber(string operand)
        {
            calcLogger.LogMessage($"Begin parsing the whole number {operand}", LogLevel.TRACE);
            return new FC_Number(ParseInt(operand), calcLogger);
        }

        /// <summary>
        /// Parses an signed 32 bit integer from string into numerical representation for calculation
        /// </summary>
        /// <param name="operand">The string integer to be parsed</param>
        /// <returns>9im)Numerical representation of the integerr</returns>
        private int ParseInt(string number)
        {
            calcLogger.LogMessage($"Begin parsing the string integer {number}", LogLevel.TRACE);
            Match match = Regex.Match(number, $@"^{TenDigitSignedInt}$");
            if (match != null && int.TryParse(match.Value, out int parsedInt)) return parsedInt;
            throw new ArgumentException($"Expected 32-bit signed integer (-2,147,483,648 to 2,147,483,647) but found: {number}");  
        }

        /// <summary>
        /// Parses string representation of available arithmetic operations into its representative enum value
        /// </summary>
        /// <param name="operation">The arithmetic operation to be parsed</param>
        /// <returns>Enum representation of a valid requeast arithmetic operation</returns>
        private Operation ParseOperator(string operation)
        {
            calcLogger.LogMessage($"Begin parsing the requested arithmetic operation {operation}", LogLevel.TRACE);
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
