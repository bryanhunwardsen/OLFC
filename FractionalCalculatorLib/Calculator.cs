using GenericApplicationLogger;
using System;
using static GenericApplicationLogger.Common;

namespace FractionalCalculatorLib
{
    public class Calculator
    {
        private static ILogger calcLogger;

        public Calculator(ILogger logger)
        {
            calcLogger = logger ?? throw new ArgumentNullException("Calculator Library requires a valid ILogger instance.");
            logger.LogMessage("Calculator Instantiated", LogLevel.TRACE);
        }

        public string ExecuteQuery(string inputQuery)
        {
            calcLogger.LogMessage("Begin Query Execution", LogLevel.TRACE);
            
            

            calcLogger.LogMessage("End Query Execution", LogLevel.TRACE);
            return $"All Done: {inputQuery}";
        }
    }
}
