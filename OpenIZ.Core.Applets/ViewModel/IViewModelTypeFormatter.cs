using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.Core.Applets.ViewModel
{
    /// <summary>
    /// Represents the view model type formatter
    /// </summary>
    public interface IViewModelTypeFormatter
    {

        /// <summary>
        /// Gets or sets the type that the serializer handles
        /// </summary>
        Type HandlesType { get; }
        
    }
}
