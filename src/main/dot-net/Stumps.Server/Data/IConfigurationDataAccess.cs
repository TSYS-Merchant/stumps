namespace Stumps.Server.Data
{

    using System;

    /// <summary>
    ///     An interface that represents access to a data store used to persist configuration information.
    /// </summary>
    public interface IConfigurationDataAccess
    {

        /// <summary>
        ///     Loads the <see cref="T:Stumps.Server.Data.ConfigurationEntity"/> from the data store.
        /// </summary>
        /// <returns>
        ///     A <see cref="T:Stumps.Server.Data.ConfigurationEntity"/> containing the configuration information for the application.
        /// </returns>
        ConfigurationEntity LoadConfiguration();

        void EnsureConfigurationIsInitialized(Action configureDefaultsAction);

        /// <summary>
        ///     Persists the specified <see cref="T:Stumps.Server.Data.ConfigurationEntity"/> to the data store.
        /// </summary>
        /// <param name="value">The <see cref="T:Stumps.Server.Data.ConfigurationEntity"/> to persist in the store.</param>
        void SaveConfiguration(ConfigurationEntity value);

    }

}