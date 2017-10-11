namespace LightZ.ComponentModel.Services.Base
{
    /// <summary>
    /// Provides a set of functions and properties that represents a service.
    /// </summary>
    internal interface IService
    {
        /// <summary>
        /// Initialize the service.
        /// </summary>
        void Initialize();

        /// <summary>
        /// Reset the state of the service. The service is considered as not initialized.
        /// </summary>
        void Reset();
    }
}
