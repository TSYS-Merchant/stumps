namespace Stumps.Data
{

    public interface IConfigurationDataAccess
    {

        ConfigurationEntity LoadConfiguration();

        void SaveConfiguration(ConfigurationEntity value);

    }

}