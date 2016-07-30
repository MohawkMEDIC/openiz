using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using OpenIZ.Core.Model;
using OpenIZ.Core.Model.Entities;

namespace OpenIZ.Core.Services
{
    /// <summary>
    /// Represents the repository handler for materials
    /// </summary>
    public interface IMaterialRepositoryService
    {
        /// <summary>
        /// Saves the specified material from data layer
        /// </summary>
        Material SaveMaterial(Material material);

        /// <summary>
        /// Inserts the material in the persistence layer
        /// </summary>
        Material InsertMaterial(Material material);

        /// <summary>
        /// Gets the specified material from the database
        /// </summary>
        Material GetMaterial(Guid id, Guid versionId);

        /// <summary>
        /// Obsoletes the specified material
        /// </summary>
        Material ObsoleteMaterial(Guid key);

        /// <summary>
        /// Finds the specified material
        /// </summary>
        IEnumerable<Material> FindMaterial(Expression<Func<Material, bool>> expression);

        /// <summary>
        /// Finds the specified material with the specified restrictions
        /// </summary>
        IEnumerable<Material> FindMaterial(Expression<Func<Material, bool>> expression, int offset, int count, out int totalCount);
    }
}
