using System;

namespace OpenIZ.OrmLite.Providers
{
    /// <summary>
    /// Represents features of SQL engine
    /// </summary>
    [Flags]
    public enum SqlEngineFeatures
    {
        None = 0,
        ReturnedInserts = 1,
        AutoGenerateGuids = 2,
        AutoGenerateTimestamps = 4
    }
}