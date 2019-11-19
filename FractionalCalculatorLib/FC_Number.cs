using GenericApplicationLogger;
using System;
using System.Runtime.CompilerServices;
using static GenericApplicationLogger.Common;

[assembly: InternalsVisibleTo("FractionalCalculatorLibTests")]
namespace FractionalCalculatorLib
{
    /// <summary>
    /// Class that stores and normalizes numbers for ccalculation and output.
    /// </summary>
    internal class FC_Number
    {
        /// <summary>
        /// (im)proper fraction numerator
        /// </summary>
        internal int Numerator { get; set; }
        /// <summary>
        /// (im)proper fraction denominator
        /// </summary>
        internal int Denominator { get; set; }
        /// <summary>
        /// application logging instance
        /// </summary>
        private static ILogger numLogger;

        /// <summary>
        /// Initialize object from Whole Number
        /// </summary>
        /// <param name="wholeNumber">Whole Number</param>
        /// <param name="logger">Application logger</param>
        internal FC_Number(int wholeNumber, ILogger logger)
        {
            Numerator = wholeNumber;
            Denominator = 1;
            numLogger = logger;

            numLogger.LogMessage($"Whole Number {wholeNumber} initialized as {Numerator}/{Denominator}", LogLevel.TRACE);
        }

        /// <summary>
        /// Initialize object from (im)proper fraction
        /// </summary>
        /// <param name="FractionNumerator">Fractional Numerator</param>
        /// <param name="fractionDenominator">Fractional Denominator</param>
        /// <param name="logger">Application logger</param>
        internal FC_Number(int FractionNumerator, int fractionDenominator, ILogger logger)
        {
            Numerator = FractionNumerator;
            Denominator = fractionDenominator;
            numLogger = logger;
            Normalize();

            numLogger.LogMessage($"Fraction {FractionNumerator}/{fractionDenominator} initialized as {Numerator}/{Denominator}", LogLevel.TRACE);
        }

        /// <summary>
        /// Initializes object from a mixed number
        /// </summary>
        /// <param name="wholeNumber">Whole Number Portion of Mixed Number</param>
        /// <param name="FractionNumerator">Fractional Numerator of a Mixed Number</param>
        /// <param name="fractionDenominator">Fractional Numerator of a Mixed Number</param>
        /// <param name="logger">Application logge</param>
        internal FC_Number(int wholeNumber, int FractionNumerator, int fractionDenominator, ILogger logger)
        {
            GenerateImproperFraction(wholeNumber, FractionNumerator, fractionDenominator);
            numLogger = logger;
            Normalize();

            numLogger.LogMessage($"Mixed Number {wholeNumber}_{FractionNumerator}/{fractionDenominator} initialized as {Numerator}/{Denominator}", LogLevel.TRACE);
        }

        /// <summary>
        /// Provide desired formatting of number in string format
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            string output = Formatter();
            numLogger.LogMessage($"ToString requested for {Numerator}/{Denominator} returnes as {output}", LogLevel.TRACE);
            return output;
        }

        /// <summary>
        /// Formatter takes (im)proper fraction and coverts to simplest sting of precedence whole number, proper fraction, mixed number
        /// </summary>
        /// <returns></returns>
        private string Formatter()
        {
            numLogger.LogMessage($"Begin FC_Number.Formatter", LogLevel.TRACE);
            try
            {                
                if (Denominator == 0)
                {
                    throw new InvalidOperationException("The value for this number is unexpectedly undefined with a denominator of zero.");
                }
                Reducer();
                if (Numerator == 0)
                {
                    return "0";
                }
                else if (Numerator % Denominator == 0)
                {
                    return (Numerator / Denominator).ToString();
                }
                else if (checked(Math.Abs(Numerator)) < checked(Math.Abs(Denominator)))
                {
                    return $"{Numerator}/{Denominator}";
                }
                else
                {
                    return $"{Numerator / Denominator }_{Math.Abs(Numerator % Denominator)}/{Denominator}";
                }
            }
            catch
            {
                throw new ArgumentException("Generating a numerical output putmat caused an intermediate value to overflow signed Int32.");
            }
        }
        
        /// <summary>
        /// Finds the greatest common denominator of the fraction and uses it to reduce the fraction to its simplest for if reduceable
        /// </summary>
        private void Reducer()
        {
            numLogger.LogMessage($"Begin FC_Number.Reducer", LogLevel.TRACE);
            if (Denominator == 0)
                throw new InvalidOperationException("A call was made to reduce a fraction with a denominator of 0.");
            if (Numerator == 0) return;
            int a;
            int b;

            try
            {
                a = checked(Math.Abs(Numerator));
                b = checked(Math.Abs(Denominator));
            }
            catch
            {
                throw new InvalidOperationException("Attempting to reduce an (im)proper fraction resulted in a numerical component encountering singed Int32 overflow");
            }

            int cycles = 1;
            // Euclid’s algorithm for GCD
            while (a != b)
            {
                if (a < b) b -= a;
                else a -= b;
                cycles++;
            }
            numLogger.LogMessage($"Reducer required {cycles} cycles to find the GCD of {a} for {Numerator}/{Denominator}", LogLevel.TRACE);
            Numerator /= a;
            Denominator /= a;
            numLogger.LogMessage($"End FC_Number.Reducer", LogLevel.TRACE);
        }

        /// <summary>
        /// Generates an improper fraction from a mixed number
        /// </summary>
        /// <param name="wholeNumber">Whole number of mixed number</param>
        /// <param name="FractionNumerator">Numerator of Mixed number</param>
        /// <param name="fractionDenominator">Denominator of Mixed Number</param>
        private void GenerateImproperFraction(int wholeNumber, int FractionNumerator, int fractionDenominator)
        {
            if (fractionDenominator == 0)
                throw new ArgumentException("Generating an improper fraction was attempted using a 0 denominator/undefinied fractional portion.");
            if (wholeNumber == 0)
            {
                Numerator = FractionNumerator;
            }
            else
            {
                try
                {
                    Numerator = checked(((Math.Abs(wholeNumber) * fractionDenominator) + FractionNumerator));
                    if (wholeNumber < 0) Numerator = checked(Numerator * -1);
                }
                catch 
                {
                    throw new ArgumentException("Generating an improper fraction resulted in an a signed Int32 overflow.");
                }
            }
            Denominator = fractionDenominator;
        }

        /// <summary>
        /// Normalize the sign of the internally held number as a fraction to have the numerator carry the sign
        /// It also flags undefined fractions
        /// </summary>
        private void Normalize()
        {
            if (Denominator == 0)
                throw new ArgumentException("Invalid undefined operand value with 0 in the denominator of a fraction or mixed number or as the result of an operand divisor equal to 0.");
            try
            {
                if (Denominator < 0 && Numerator > 0)
                {
                    Numerator = checked(Numerator *= -1);
                    Denominator = checked(Denominator *= -1);
                }
                else if(Denominator < 0 && Numerator < 0)
                {
                    Numerator = checked(Math.Abs(Numerator));
                    Denominator = checked(Math.Abs(Denominator));
                }
            }
            catch
            {
                throw new ArgumentException("Normalizing intermediate fraction resulted in a signed Int32 overflow.");
            }
        }
    }
}