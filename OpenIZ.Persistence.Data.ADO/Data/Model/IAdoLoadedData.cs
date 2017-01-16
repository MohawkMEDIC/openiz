namespace OpenIZ.Persistence.Data.ADO.Data.Model
{
    /// <summary>
    /// Represents ADO loaded data
    /// </summary>
    public interface IAdoLoadedData
    {

        /// <summary>
        /// Gets the data context
        /// </summary>
        DataContext Context { get; set; }
    }
}