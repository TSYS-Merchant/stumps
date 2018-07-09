namespace Stumps.Server.Data
{
    using System;

    /// <summary>
    ///     An interface that represents access to a data store used to persist configuration information.
    /// </summary>
    public interface IConfigurationDataAccess
    {
        /// <summary>
        ///     Loads the <see cref="ConfigurationEntity"/> from the data store.
        /// </summary>
        /// <returns>
        ///     A <see cref="ConfigurationEntity"/> containing the configuration information for the application.
        /// </returns>
        ConfigurationEntity LoadConfiguration();

        /// <summary>
        ///     Ensures the configuration is correctly initialized.
        /// </summary>
        /// <param name="configureDefaultsAction">The action to execute after preparing the data access.</param>
        void EnsureConfigurationIsInitialized(Action configureDefaultsAction);

        /// <summary>
        ///     Persists the specified <see cref="ConfigurationEntity"/> to the data store.
        /// </summary>
        /// <param name="value">The <see cref="ConfigurationEntity"/> to persist in the store.</param>
        void SaveConfiguration(ConfigurationEntity value);
    }
}