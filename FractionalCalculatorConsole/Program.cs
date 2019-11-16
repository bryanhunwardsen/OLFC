using System;
using FractionalCalculatorLib;
using GenericApplicationLogger;
using static GenericApplicationLogger.Common;

namespace FractionalCalculatorConsole
{
    class Program
    {
        private static Calculator calculator;
        private static ILogger logger;

        static void Main(string[] args)
        {
            InitLogger(args);
            calculator = new Calculator(logger);
            PrintHeader();
            RunApplicationLoop();
        }

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

        private static void PrintHeader()
        {
            Console.WriteLine("*******************************************");
            Console.WriteLine("*** Welcome to the FractionalCalculator ***");
            Console.WriteLine("*******************************************");
            Console.WriteLine();
        }

        private static void RunApplicationLoop()
        {            
            while (true)
            {
                Console.WriteLine("Enter Arithmetic Query or \"Q\" to Quit: ");
                string inputQuery;
                try
                {
                    inputQuery = Console.ReadLine();
                }
                catch (Exception grossQueryException)
                {
                    Console.WriteLine("Invalid Query Input Error, consult application logs.");
                    logger.LogMessage($"{grossQueryException.Message}|{grossQueryException.StackTrace}");
                    continue;
                }
                if (!string.IsNullOrWhiteSpace(inputQuery) && inputQuery == "Q") return;
                try
                {
                    Console.WriteLine(calculator.ExecuteQuery(inputQuery));
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Unexpected Query Execution Error, consult application logs.");
                    logger.LogMessage($"{ex.Message}|{ex.StackTrace}");
                    continue;
                }                
            }
        }
    }
}
