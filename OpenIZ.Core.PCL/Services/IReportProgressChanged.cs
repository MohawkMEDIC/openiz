using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.Core.Services
{


    /// <summary>
    /// Represents a class which reports progress
    /// </summary>
    public interface IReportProgressChanged
    {

        /// <summary>
        /// Identifies that progress has changed
        /// </summary>
        event EventHandler<ProgressChangedEventArgs> ProgressChanged;
    }
}
