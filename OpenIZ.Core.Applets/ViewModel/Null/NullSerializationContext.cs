namespace OpenIZ.Core.Applets.ViewModel.Null
{
    /// <summary>
    /// Serialization context
    /// </summary>
    public class NullSerializationContext : SerializationContext
    {

        /// <summary>
        /// Serialization context
        /// </summary>
        public NullSerializationContext(string propertyName, IViewModelSerializer context, object instance, SerializationContext parent) :
            base(propertyName, context, instance, parent)
        {
        }

        /// <summary>
        /// Null context
        /// </summary>
        public NullViewModelSerializer NullContext {  get { return this.Context as NullViewModelSerializer; } }
    }
}