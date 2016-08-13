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

        /// <summary>
        /// Saves the specified ManufacturedMaterial from data layer
        /// </summary>
        ManufacturedMaterial SaveManufacturedMaterial(ManufacturedMaterial ManufacturedMaterial);

        /// <summary>
        /// Inserts the ManufacturedMaterial in the persistence layer
        /// </summary>
        ManufacturedMaterial InsertManufacturedMaterial(ManufacturedMaterial ManufacturedMaterial);

        /// <summary>
        /// Gets the specified ManufacturedMaterial from the database
        /// </summary>
        ManufacturedMaterial GetManufacturedMaterial(Guid id, Guid versionId);

        /// <summary>
        /// Obsoletes the specified ManufacturedMaterial
        /// </summary>
        ManufacturedMaterial ObsoleteManufacturedMaterial(Guid key);

        /// <summary>
        /// Finds the specified ManufacturedMaterial
        /// </summary>
        IEnumerable<ManufacturedMaterial> FindManufacturedMaterial(Expression<Func<ManufacturedMaterial, bool>> expression);

        /// <summary>
        /// Finds the specified ManufacturedMaterial with the specified restrictions
        /// </summary>
        IEnumerable<ManufacturedMaterial> FindManufacturedMaterial(Expression<Func<ManufacturedMaterial, bool>> expression, int offset, int count, out int totalCount);

    }
}
