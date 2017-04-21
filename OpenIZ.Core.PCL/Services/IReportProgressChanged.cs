using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.Core.Services
{

    public class ProgressChangedEventArgs : EventArgs
    {
        /// <summary>
        /// Gets or sets the progress
        /// </summary>
        public float Progress { get; set; }

        /// <summary>
        /// Display attached with the progress
        /// </summary>
        public Object State { get; set; }

        /// <summary>
        /// Progress changed event args
        /// </summary>
        public ProgressChangedEventArgs(float progress, object state)
        {
            this.Progress = progress;
            this.State = state;
        }
    }

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
