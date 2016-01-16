using MARC.HI.EHRS.SVC.Core;
using MARC.HI.EHRS.SVC.Core.Services;
using System;

namespace OpenIZ.Core.Model.DataTypes
{
    /// <summary>
    /// Represents a phonetic algorithm record in the model
    /// </summary>
    public class PhoneticAlgorithm : IdentifiedData
    {
        // Phonetic algorithm
        private static PhoneticAlgorithm s_nullPhoneticAlgorithm = null;
        private static object s_lockObject = new object();

        /// <summary>
        /// Gets the phonetic algorithm which is the "empty" algorithm
        /// </summary>
        public static PhoneticAlgorithm EmptyAlgorithm
        {
            get
            {
                if(s_nullPhoneticAlgorithm == null)
                    lock(s_lockObject)
                        if(s_nullPhoneticAlgorithm == null)
                        {
                            var persistenceService = ApplicationContext.Current.GetService<IDataPersistenceService<PhoneticAlgorithm>>();
                            s_nullPhoneticAlgorithm = persistenceService.Get(new MARC.HI.EHRS.SVC.Core.Data.Identifier<Guid>(Guid.Parse("402CD339-D0E4-46CE-8FC2-12A4B0E17226")), null, true);
                        }
                return s_nullPhoneticAlgorithm;
            }
        }

        /// <summary>
        /// Gets the name of the phonetic algorithm
        /// </summary>
        public String Name { get; set; }
        /// <summary>
        /// Gets the handler (or generator) for the phonetic algorithm
        /// </summary>
        public Type Handler { get; set; }

    }
}