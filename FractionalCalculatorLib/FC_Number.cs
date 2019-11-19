using GenericApplicationLogger;
using System;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("FractionalCalculatorLibTests")]
namespace FractionalCalculatorLib
{
    internal class FC_Number
    {
        internal int Numerator { get; set; }
        internal int Denominator { get; set; }
        private static ILogger numLogger;

        internal FC_Number(int wholeNumber, ILogger logger)
        {
            Numerator = wholeNumber;
            Denominator = 1;
            numLogger = logger;
        }

        internal FC_Number(int FractionNumerator, int fractionDenominator, ILogger logger)
        {
            Numerator = FractionNumerator;
            Denominator = fractionDenominator;
            numLogger = logger;
            Normalize();
        }

        internal FC_Number(int wholeNumber, int FractionNumerator, int fractionDenominator, ILogger logger)
        {
            GenerateImproperFraction(wholeNumber, FractionNumerator, fractionDenominator);
            numLogger = logger;
            Normalize();
        }

        public override string ToString()
        {   
            return Formatter();
        }

        private string Formatter()
        {
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

        private void Reducer()
        {
            if (Denominator == 0)
                throw new InvalidOperationException("A call was made to reduce a fraction with a denominator of 0.");
            if (Numerator == 0) return;
            int a = Math.Abs(Numerator);
            int b = Math.Abs(Denominator);
            // Euclid’s algorithm for GCD
            while (a != b)
            {
                if (a < b) b -= a;
                else a -= b;
            }
            Numerator /= a;
            Denominator /= a;
        }

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