using System;

namespace OpenIZ.BusinessRules.JavaScript
{
    /// <summary>
    /// Business rule exception
    /// </summary>
    public class BusinessRulesExecutionException : Exception
    {

        public BusinessRulesExecutionException()
        {
        }

        public BusinessRulesExecutionException(string message) : base(message)
        {
        }

        public BusinessRulesExecutionException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}