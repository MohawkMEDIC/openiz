using MARC.HI.EHRS.SVC.Core;
using MARC.HI.EHRS.SVC.Core.Services;
using OpenIZ.Core.Model.Attributes;
using System;
using System.ComponentModel;

namespace OpenIZ.Core.Model.DataTypes
{
    /// <summary>
    /// Display name of a code system or reference term
    /// </summary>
    public abstract class ReferenceTermName : BaseEntityData
    {

        // Id of the algorithm used to generate phonetic code
        private Guid m_phoneticAlgorithmId;
        // Algorithm used to generate the code
        private PhoneticAlgorithm m_phoneticAlgorithm;

        /// <summary>
        /// Back-reference to reference term
        /// </summary>
        public Guid ReferenceTermId { get; set; }

        /// <summary>
        /// Gets or sets the language code of the object
        /// </summary>
        public String Language { get; set; }

        /// <summary>
        /// Gets or sets the name of the reference term
        /// </summary>
        public String Name { get; set; }

        /// <summary>
        /// Gets or sets the phonetic code of the reference term
        /// </summary>
        public String PhoneticCode { get; set; }

        /// <summary>
        /// Gets or sets the identifier of the phonetic code
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Browsable(false)]
        public Guid PhoneticAlgorithmId
        {
            get { return this.m_phoneticAlgorithmId; }
            set
            {
                this.m_phoneticAlgorithmId = value;
                this.m_phoneticAlgorithm = null;
            }
        }

        /// <summary>
        /// Gets or sets the phonetic algorithm
        /// </summary>
        [DelayLoad]
        public PhoneticAlgorithm PhoneticAlgorithm
        {
            get
            {
                if(this.m_phoneticAlgorithm == null &&
                    this.DelayLoad &&
                    this.m_phoneticAlgorithmId != Guid.Empty)
                {
                    var dataService = ApplicationContext.Current.GetService<IDataPersistenceService<PhoneticAlgorithm>>();
                    this.m_phoneticAlgorithm = dataService.Get(new MARC.HI.EHRS.SVC.Core.Data.Identifier<Guid>(this.m_phoneticAlgorithmId), null, true);
                }
                return this.m_phoneticAlgorithm;
            }
            set
            {
                this.m_phoneticAlgorithm = value;
                if (value == null)
                    this.m_phoneticAlgorithmId = Guid.Empty;
                else
                    this.m_phoneticAlgorithmId = value.Key;
            }
        }

        /// <summary>
        /// Force reloading of delay load properties
        /// </summary>
        public override void Refresh()
        {
            base.Refresh();
            this.m_phoneticAlgorithm = null;
        }
    }
}