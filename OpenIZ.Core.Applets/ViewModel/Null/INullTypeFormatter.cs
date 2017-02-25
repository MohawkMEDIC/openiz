using OpenIZ.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.Core.Applets.ViewModel.Null
{
    /// <summary>
    /// View model serializer which is a null view model serializer
    /// </summary>
    public interface INullTypeFormatter : IViewModelTypeFormatter
    {

        /// <summary>
        /// Serialize specified object <paramref name="o"/> into the oblivion known as null
        /// </summary>
        /// <param name="o">The object to be graphed</param>
        /// <param name="context">The current serialization context</param>
        void Serialize(IdentifiedData o, NullSerializationContext context);

    }
}
