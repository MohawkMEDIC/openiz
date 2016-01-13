using System.Linq;
using MARC.HI.EHRS.SVC.Core;
using MARC.HI.EHRS.SVC.Core.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace OpenIZ.Core.Model.DataTypes
{
    /// <summary>
    /// Represents a basic reference term
    /// </summary>
    public class ReferenceTerm : IdentifiedData
    {

        // Backing field for code system identifier
        private Guid m_codeSystemId;
        // Code system
        private CodeSystem m_codeSystem;
        // Display names
        private List<ReferenceTermName> m_displayNames;

        /// <summary>
        /// Gets or sets the mnemonic for the reference term
        /// </summary>
        public string Mnemonic { get; set; }
        
        /// <summary>
        /// Gets or sets the code system 
        /// </summary>
        public CodeSystem CodeSystem {
            get
            {
                if(this.m_codeSystem == null &&
                    this.DelayLoad &&
                    this.m_codeSystemId != Guid.Empty)
                {
                    var dataPersistence = ApplicationContext.Current.GetService<IDataPersistenceService<CodeSystem>>();
                    this.m_codeSystem = dataPersistence.Get(new MARC.HI.EHRS.SVC.Core.Data.Identifier<Guid>(this.m_codeSystemId), null, true);
                }
                return this.m_codeSystem;
            }
            set
            {
                this.m_codeSystem = value;
                if (value == null)
                    this.m_codeSystemId = Guid.Empty;
                else
                    this.m_codeSystemId = value.Key;
            }
        }
        
        /// <summary>
        /// Gets or sets the code system identifier
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Browsable(false)]
        public Guid CodeSystemId {
            get { return this.m_codeSystemId; }
            set
            {
                this.m_codeSystemId = value;
                this.m_codeSystem = null;
            }
        }

        /// <summary>
        /// Gets display names associated with the reference term
        /// </summary>
        public List<ReferenceTermName> DisplayNames {
            get
            {
                if(this.m_displayNames == null && this.DelayLoad)
                {
                    var dataService = ApplicationContext.Current.GetService<IDataPersistenceService<ReferenceTermName>>();
                    this.m_displayNames = dataService.Query(o => o.ReferenceTermId == this.Key && o.ObsoletionTime == null, null).ToList();
                }
                return this.m_displayNames;
            }
        }

    }
}