using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.Core.Model.Constants
{
    /// <summary>
    /// Act relationship types
    /// </summary>
    public static class ActRelationshipTypeKeys
    {
        /// <summary>
        /// Indicates that the source act refers to the target act
        /// </summary>
        public static readonly Guid RefersTo = Guid.Parse("8FCE259A-B859-4AE3-8160-0221F6AB1650");
        /// <summary>
        /// Links the transortation act from another act
        /// </summary>
        public static readonly Guid Arrival = Guid.Parse("26FE590C-3684-4574-9359-057FDD06BA61");
        /// <summary>
        /// Links a transporation act from another act indicating departure of the subject
        /// </summary>
        public static readonly Guid Departure = Guid.Parse("28C81CDC-CA56-4C92-B691-094E89630642");
        /// <summary>
        /// Used to link a goal to an observation
        /// </summary>
        public static readonly Guid Evaluates = Guid.Parse("8DBEAC94-CCCB-4412-A990-09BAB26DD048");
        /// <summary>
        /// Relationship from an act to one or more control variables (for example: device settings, or environment)
        /// </summary>
        public static readonly Guid HasControlVariable = Guid.Parse("85F68168-2A43-4532-BC79-191FA0B47C8B");
        /// <summary>
        /// The source act documents the target act
        /// </summary>
        public static readonly Guid Documents = Guid.Parse("0F4BA634-5107-4EAB-9658-25BE293CD831");
        /// <summary>
        /// Indicates an existing act is suggesting evidence for a new observation.
        /// </summary>
        public static readonly Guid HasSupport = Guid.Parse("3209E3F1-2258-4B63-8182-2C888DA66CF0");
        /// <summary>
        /// Links two instances of the same act over time (example: chronic conditions)
        /// </summary>
        public static readonly Guid EpisodeLink = Guid.Parse("EBF9AC10-B5C9-407A-91A4-360BFB7E0FB9");
        /// <summary>
        /// The source act replaces the target act
        /// </summary>
        public static readonly Guid Replaces = Guid.Parse("D1578637-E1CB-415E-B319-4011DA033813");
        /// <summary>
        /// The assertion that a new observation may be a manifestation of another
        /// </summary>
        public static readonly Guid IsManfestationOf = Guid.Parse("22918D17-D3DC-4135-A003-4C1C52E57E75");
        /// <summary>
        /// Indicates that the source act appends information contained in the target act
        /// </summary>
        public static readonly Guid Appends = Guid.Parse("DC3DF205-18EF-4854-AC00-68C295C9C744");
        /// <summary>
        /// Indicates the subject of a particular act (example: clinical act is a subject of a control act)
        /// </summary>
        public static readonly Guid HasSubject = Guid.Parse("9871C3BC-B57A-479D-A031-7B56CB06FA84");
        /// <summary>
        /// Indicates that the source act fulfills the target act
        /// </summary>
        public static readonly Guid Fulfills = Guid.Parse("646542BC-72E4-488B-BBF4-865D452E62EC");
        /// <summary>
        /// Indicates the source act is derived from information contained in the target act
        /// </summary>
        public static readonly Guid IsDerivedFrom = Guid.Parse("81B6A0F8-B86A-495F-9D5D-8A4073FDD882");
        /// <summary>
        /// Indicates that the source act is the cause of the target act
        /// </summary>
        public static readonly Guid IsCauseOf = Guid.Parse("57D81685-E399-4ABD-8744-96454188A9FA");
        /// <summary>
        /// Indicates that the source act is an excerpt of the target act
        /// </summary>
        public static readonly Guid IsExcerptOf = Guid.Parse("FFC6E905-161D-4C0B-8CDE-A04E9E9D0CD5");
        /// <summary>
        /// Indicates that the target act is a component of the source act
        /// </summary>
        public static readonly Guid HasComponent = Guid.Parse("78B9540F-438B-4B6F-8D83-AAF4979DBC64");
        /// <summary>
        /// Indicates that the source act starts after the start of another act
        /// </summary>
        public static readonly Guid StartsAfterStartOf = Guid.Parse("C66D7CA9-C6C2-46B1-9276-AD76BAF04B07");
        /// <summary>
        /// Indicates that the target act is a pre-condition of the source act
        /// </summary>
        public static readonly Guid HasPrecondition = Guid.Parse("5A280FC0-8C26-4191-B204-B1B1E4E19462");
        /// <summary>
        /// Indicates a reasoning as to why the source act is occurring
        /// </summary>
        public static readonly Guid HasReason = Guid.Parse("55DA61A2-7B86-47F3-9B0B-BA47DC99C950");
        /// <summary>
        /// Indicates that the target act authorizes the source act
        /// </summary>
        public static readonly Guid HasAuthorization = Guid.Parse("29894070-A76B-47EF-8C16-D84E0ACD9EA6");
        /// <summary>
        /// Indicates that the source act contains reference values from the target
        /// </summary>
        public static readonly Guid HasReferenceValues = Guid.Parse("99488A1D-6D97-4013-8C91-DED6AD3B8E89");
        /// <summary>
        /// Indicates that the source act transforms the target act
        /// </summary>
        public static readonly Guid Transforms = Guid.Parse("DB2AE02A-FF12-4C1B-9C5B-ECDD41AF8583");
    }
}
