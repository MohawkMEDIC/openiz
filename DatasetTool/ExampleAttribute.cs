using System;

namespace OizDevTool
{
    /// <summary>
    /// Example attribute
    /// </summary>
    public class ExampleAttribute : Attribute
    {
        /// <summary>
        /// Gets or sets the description of the example
        /// </summary>
        public String Description { get; set; }

        /// <summary>
        /// Gets or sets the example text
        /// </summary>
        public String ExampleText { get; set; }

        public ExampleAttribute(String description, String exampleText)
        {
            this.Description = description;
            this.ExampleText = exampleText;

        }
    }
}