
namespace FractionalCalculatorLib
{
    internal class FC_Number
    {
        internal int Numerator { get; private set; }
        internal int Denomiator { get; private set; }

        internal FC_Number(int wholeNumber)
        {
            Numerator = wholeNumber;
            Denomiator = 1;
        }

        internal FC_Number(int FractionNumerator, int fractionDenominator)
        {
            Numerator = FractionNumerator;
            Denomiator = fractionDenominator;
        }

        internal FC_Number(int wholeNumber, int FractionNumerator, int fractionDenominator)
        {
            GenerateImproperFraction(wholeNumber, FractionNumerator, fractionDenominator);
        }

        private void GenerateImproperFraction(int wholeNumber, int FractionNumerator, int fractionDenominator)
        {
            Numerator = ((wholeNumber * fractionDenominator) + FractionNumerator);
            Denomiator = fractionDenominator;
        }
    }
}
