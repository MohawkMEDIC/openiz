namespace OpenIZ.Core.Model.DataTypes
{
    /// <summary>
    /// Represents a code system which is a collection of reference terms
    /// </summary>
    public class CodeSystem : BaseEntityData
    {
        /// <summary>
        /// Gets or sets the name of the code system
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the Oid of the code system
        /// </summary>
        public string Oid { get; set; }

        /// <summary>
        /// Gets or sets the authority of the code system
        /// </summary>
        public string Authority { get; set; }

        /// <summary>
        /// Gets or sets the obsoletion reason of the code system
        /// </summary>
        public string ObsoletionReason { get; set; }

        /// <summary>
        /// Gets or sets the URL of the code system
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// Gets or sets the version text of the code system
        /// </summary>
        public string VersionText { get; set; }


    }
}