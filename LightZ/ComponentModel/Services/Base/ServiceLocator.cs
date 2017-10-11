using LightZ.ComponentModel.Core;
using System.Collections.Generic;
using System.Linq;

namespace LightZ.ComponentModel.Services.Base
{
    /// <summary>
    /// Provides a set of functions designed to manage the services.
    /// </summary>
    internal static class ServiceLocator
    {
        #region Fields

        private static readonly List<IService> _services = new List<IService>();

        #endregion

        #region Methods

        /// <summary>
        /// Get an instance of the specified service.
        /// </summary>
        /// <returns>The current instance of the service</returns>
        internal static T GetService<T>() where T : IService, new()
        {
            T service;
            var serviceFound = _services.OfType<T>().ToList();

            if (serviceFound.Any())
            {
                service = serviceFound.Single();
            }
            else
            {
                service = new T();
                Requires.NotNull(service, nameof(service));

                _services.Add(service);
                service.Initialize();
            }

            return service;
        }

        /// <summary>
        /// Reset the state of all services. This method must be used in the unit test.
        /// </summary>
        internal static void ResetAll()
        {
            foreach (var service in _services)
            {
                service.Reset();
            }
        }

        #endregion
    }
}
