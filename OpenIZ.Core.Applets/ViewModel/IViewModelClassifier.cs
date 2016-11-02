using OpenIZ.Core.Model;
using System;
using System.Collections;
using System.Collections.Generic;

namespace OpenIZ.Core.Applets.ViewModel
{
    /// <summary>
    /// Represents a classifier which is responsible for taking a particular instance of type X
    /// and determining the appropriate classification
    /// </summary>
    public interface IViewModelClassifier
    {

        /// <summary>
        /// Gets the type this classifier handles
        /// </summary>
        Type HandlesType { get; }

        /// <summary>
        /// Gets the appropriate classifier for the specified data
        /// </summary>
        Dictionary<string, IList> Classify(IList data);

        /// <summary>
        /// Re-compose the classified data 
        /// </summary>
        IList Compose(Dictionary<string, object> values);
    }
}