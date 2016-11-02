namespace OpenIZ.Core.Security.Tfa.Email.Configuration
{
    /// <summary>
    /// Represents template configuration
    /// </summary>
    public class TemplateConfiguration
    {

        /// <summary>
        /// Template configuration file
        /// </summary>
        public TemplateConfiguration(string lang, string file)
        {
            this.Language = lang;
            this.TemplateDefinitionFile = file;
        }

        /// <summary>
        /// Gets the language of the tempalte
        /// </summary>
        public string Language { get; private set; }

        /// <summary>
        /// Gets the file
        /// </summary>
        public string TemplateDefinitionFile { get; private set; }
    }
}