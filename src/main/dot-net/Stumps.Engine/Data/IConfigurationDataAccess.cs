namespace Stumps.Data {

    using System;

    public interface IConfigurationDataAccess {

        ConfigurationEntity LoadConfiguration();

        void SaveConfiguration(ConfigurationEntity value);

    }

}
