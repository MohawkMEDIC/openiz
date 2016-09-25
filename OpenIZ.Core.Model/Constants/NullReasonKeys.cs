using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.Core.Model.Constants
{
    /// <summary>
    /// Null reason keys
    /// </summary>
    public static class NullReasonKeys
    {
        public static Guid Unavailable = Guid.Parse("31E01921-82DC-4622-B3DB-21429EA9E406");
        public static Guid NotApplicable = Guid.Parse("FEA2CFB1-F231-413D-B113-372779092E56");
        public static Guid Derived = Guid.Parse("8EF137B3-E717-492B-8D8F-3817C99AED88");
        public static Guid Other = Guid.Parse("6052712A-340E-4480-AD6B-409BA320DB4F");
        public static Guid AskedUnknown = Guid.Parse("21B0FFC8-CA4E-408D-A104-41FC924D3A39");
        public static Guid Invalid = Guid.Parse("D3F92EB1-FECE-4DEA-BED2-515AF2B0FB38");
        public static Guid Trace = Guid.Parse("085069D8-0CA8-4771-986B-5EB3466580FF");
        public static Guid NegativeInfinity = Guid.Parse("FED3FE1B-B2C7-480B-B0AF-5FD2E0200CE5");
        public static Guid SufficientQuantity = Guid.Parse("C139841A-7D5A-40BA-9AC7-7628A7CDF443");
        public static Guid UnEncoded = Guid.Parse("7DA45C51-EB8E-4C75-A40B-7DB66CB3F3CB");
        public static Guid NotAsked = Guid.Parse("09919A72-808C-44C4-8B44-86FD3725F100");
        public static Guid Unknown = Guid.Parse("70FE34CE-CAFF-4F46-B6E6-9CD6D8F289D6");
        public static Guid PositiveInfinity = Guid.Parse("E6D6FEE2-FA53-4027-8EB8-9DD0F35D053D");
        public static Guid NoInformation = Guid.Parse("61D8F65C-747E-4A99-982F-A42AC5437473");
        public static Guid Masked = Guid.Parse("9B16BF12-073E-4EA4-B6C5-E1B93E8FD490");
    }
}
