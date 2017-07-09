using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.Core.Model.Constants
{
    /// <summary>
    /// Represents act type keys
    /// </summary>
    public static class ObservationTypeKeys
    {

        public static readonly Guid Condition = Guid.Parse("236b5641-61d2-4d12-91f7-5dddbd7f8931");
        public static readonly Guid Symptom = Guid.Parse("10a0fb51-687d-41ec-8d50-ad6549e2ae58");
        public static readonly Guid Finding = Guid.Parse("5dbd3949-fda0-4c5d-a849-a673fd5565f6");
        public static readonly Guid Complaint = Guid.Parse("402051ae-fa84-45b7-ac3b-586d1323ebe7");
        public static readonly Guid Functionallimitation = Guid.Parse("bfc26b1f-af4c-4d50-a084-eb0d9eabd519");
        public static readonly Guid Problem = Guid.Parse("260ffe90-7882-4b38-a7af-d2110e91e752");
        public static readonly Guid Diagnosis = Guid.Parse("d5e0a5be-d227-413a-a752-b7d79d7d4ef3");
        public static readonly Guid Severity = Guid.Parse("05012084-3351-4045-8390-fbcbd7ec1d19");
        public static readonly Guid CauseOfDeath = Guid.Parse("d5e0a5be-d227-413a-a752-b7d79d7d4ede");
        public static readonly Guid ClinicalState = Guid.Parse("6fb8487c-cd6f-44f1-ab63-27dc65405792");
        public static readonly Guid FindingSite = Guid.Parse("25D9F855-F0C8-4718-884D-04D3B6439E5C");
    }
}
