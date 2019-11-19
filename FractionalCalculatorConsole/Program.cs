using FractionalCalculatorLib;
using GenericApplicationLogger;
using System;
using static GenericApplicationLogger.Common;

namespace FractionalCalculatorConsole
{
    /// <summary>
    /// Fraction Calculator containing class
    /// </summary>
    class Program
    {
        /// <summary>
        /// Static Calculator only performs operations
        /// </summary>
        private static Calculator calculator;
        /// <summary>
        /// Static Logger to be shared by the application
        /// </summary>
        private static ILogger logger;

        /// <summary>
        /// Launches the Fractional Calculator with any provided default logging level
        /// </summary>
        /// <param name="args">Default Logging Level (optional)</param>
        static void Main(string[] args)
        {
            InitLogger(args);
            calculator = new Calculator(logger);
            logger.LogMessage("Calculator Initialized", LogLevel.TRACE);
            PrintHeader();
            RunApplicationLoop();
            logger.LogMessage("Exiting Calculator Application", LogLevel.TRACE);
        }

        /// <summary>
        /// Initialize the logger to the default logging level
        /// </summary>
        /// <param name="args">The default Logging Level (optional)</param>
        private static void InitLogger(string[] args)
        {
            switch (args.Length)
            {
                case 0:
                    logger = new ConsoleLogger();
                    break;
                case 1:
                    bool parsed = Enum.TryParse(args[0], out LogLevel logLevel);
                    if (parsed)
                    {
                        logger = new ConsoleLogger(logLevel);
                    }
                    else
                    {
                        throw new ArgumentException($"Application terminated, unrecognized log level:{args[0]}");
                    }
                    break;
                default:
                    throw new ArgumentException($"Application terminated, incorrect log level input, only 1 paramter supported");
            }
            logger.LogMessage("Logger Initiated", LogLevel.TRACE);
        }

        /// <summary>
        /// Educatres user on application useage
        /// </summary>
        private static void PrintHeader()
        {

            Console.WriteLine("********************************************************************************");
            Console.WriteLine("* Welcome to the FractionalCalculator ******************************************");
            Console.WriteLine("* Query format is of the form \"Operand1 Operation Operand2\" ******************");
            Console.WriteLine("* Valid operations are in the set { +, - , * , / } *****************************");
            Console.WriteLine("* Operands are in the format on the set { A, -B, C/D, -E/F, G_H/I, -J_K/L } ****");
            Console.WriteLine("* All operand components A->L are restriced to singed 32bit Integers as noted **");
            Console.WriteLine("* All intermediate sub calculation values are also limited to Singned Int32 ****");
            Console.WriteLine("********************************************************************************");
            Console.WriteLine();
        }

        /// <summary>
        /// Main query loop
        /// Should log all known Arguement/Operation exception and continue, handle unknown exceptions gracefully
        /// </summary>
        private static void RunApplicationLoop()
        {
            logger.LogMessage("Begin Calculator UI loop", LogLevel.TRACE);
            while (true)
            {
                Console.WriteLine("Enter Arithmetic Query or \"Q\" to Quit: ");
                Console.Write("? ");
                string inputQuery;
                try
                {
                    inputQuery = Console.ReadLine();
                }
                catch (Exception grossQueryException)
                {
                    // Console.WriteLine("Invalid Query Input Error, consult application logs.");
                    logger.LogMessage($"{grossQueryException.Message}|{grossQueryException.StackTrace}");
                    continue;
                }
                if (!string.IsNullOrWhiteSpace(inputQuery) && inputQuery == "Q") return;
                try
                {
                    Console.WriteLine($"= {calculator.ExecuteQuery(inputQuery)}");
                }
                catch (Exception ex)
                {
                    //Console.WriteLine("Unexdpected Query Execution Error, consult application logs.");
                    logger.LogMessage($"{ex.Message}"); //|{grossQueryException.StackTrace}
                    continue;
                }                
            }
        }
    }
}
