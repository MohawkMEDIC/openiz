using System;
using System.Collections.Generic;

namespace OpenIZ.Core.Data.Warehouse
{
    /// <summary>
    /// Datamart schema property container
    /// </summary>
    public interface IDatamartSchemaPropertyContainer
    {
        /// <summary>
        /// Gets the id
        /// </summary>
        Guid Id { get; }

        /// <summary>
        /// Gets the name
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets the sub-properties
        /// </summary>
        List<DatamartSchemaProperty> Properties { get; } 
    }
}