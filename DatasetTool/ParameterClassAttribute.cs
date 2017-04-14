using System;

namespace OizDevTool
{
    /// <summary>
    /// Parameter class attribute
    /// </summary>
    public class ParameterClassAttribute : Attribute
    {

        /// <summary>
        /// Indicates the type that this parameter gets it args from
        /// </summary>
        public Type ParameterClass { get; set; }

        public ParameterClassAttribute(Type parameterClass)
        {
            this.ParameterClass = parameterClass;
        }
    }
}