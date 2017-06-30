using System;

namespace OizDevTool
{
    /// <summary>
    /// Parameter class attribute.
    /// </summary>
    public class ParameterClassAttribute : Attribute
    {
		/// <summary>
		/// Initializes a new instance of the <see cref="ParameterClassAttribute"/> class.
		/// </summary>
		/// <param name="parameterClass">The parameter class.</param>
		public ParameterClassAttribute(Type parameterClass)
        {
            this.ParameterClass = parameterClass;
        }

	    /// <summary>
	    /// Indicates the type that this parameter gets it arguments from.
	    /// </summary>
	    public Type ParameterClass { get; }
	}
}