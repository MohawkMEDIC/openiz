using OpenIZ.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.Persistence.Data.MSSQL.Linq
{
    public interface IModelConverter
    {

        /// <summary>
        /// Gets the domain type 
        /// </summary>
        Type DomainType { get; }

        /// <summary>
        /// Get the model type
        /// </summary>
        Type ModelType { get; }

        /// <summary>
        /// Map one member reference to another
        /// </summary>
        MemberInfo MapMember(MemberInfo member);

        /// <summary>
        /// Convert from model instance to another
        /// </summary>
        Object MapInstance(BaseData modelInstance);

        /// <summary>
        /// Convert from domain model
        /// </summary>
        BaseData MapInstance(Object domainModel);
    }

   
}
