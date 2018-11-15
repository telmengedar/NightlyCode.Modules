namespace NightlyCode.Modules.Context {

    /// <summary>
    /// interface for an interceptor
    /// </summary>
    public interface IInterceptor {

        /// <summary>
        /// sets a property on the object
        /// </summary>
        /// <param name="property">property</param>
        /// <param name="value">property value</param>
        void SetProperty(string property, object value);

        /// <summary>
        /// gets a property value from the object
        /// </summary>
        /// <param name="property">property</param>
        /// <returns>property value</returns>
        object GetProperty(string property);

        /// <summary>
        /// calls a method
        /// </summary>
        /// <param name="method">name of called method</param>
        /// <param name="parameters">method parameters</param>
        /// <returns>method return value, null if method return type is void</returns>
        object CallMethod(string method, object[] parameters);
    }
}