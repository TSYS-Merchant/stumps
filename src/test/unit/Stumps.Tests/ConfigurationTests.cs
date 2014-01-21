namespace Stumps {

    using System;
    using System.IO;
    using NSubstitute;
    using NUnit.Framework;
    using Stumps.Data;

    [TestFixture]
    public class ConfigurationTests {

        [Test]
        public void Constructor_NullDataAccess_ThrowsException() {

            Assert.That(
                () => new Configuration(null),
                Throws.Exception
                    .TypeOf<ArgumentNullException>()
                    .With.Property("ParamName")
                    .EqualTo("dataAccess")
            );
            
        }

        [Test]
        public void Constructor_WithValidDataAccess_InitializesUsingDefaults() {

            var dal = Substitute.For<IConfigurationDataAccess>();
            var config = new Configuration(dal);

            Assert.AreEqual(DefaultConfigurationSettings.DataCompatibilityVersion, config.DataCompatibilityVersion);
            Assert.AreEqual(DefaultConfigurationSettings.StoragePath, config.StoragePath);
            Assert.AreEqual(DefaultConfigurationSettings.WebApiPort, config.WebApiPort);


        }

        [Test]
        public void LoadConfiguration_CallsDal() {

            var entity = CreateSampleConfigurationEntity();

            var configurationDal = Substitute.For<IConfigurationDataAccess>();
            configurationDal.LoadConfiguration().Returns(entity);

            var configuration = new Configuration(configurationDal);
            configuration.LoadConfiguration();

            configurationDal.Received(1).LoadConfiguration();

        }

        [Test]
        public void LoadConfiguration_UpdatesConfigurationValues() {

            var entity = CreateSampleConfigurationEntity();

            var configurationDal = Substitute.For<IConfigurationDataAccess>();
            configurationDal.LoadConfiguration().Returns(entity);

            var configuration = new Configuration(configurationDal);
            configuration.LoadConfiguration();

            Assert.AreEqual(entity.DataCompatibilityVersion, configuration.DataCompatibilityVersion);
            Assert.AreEqual(entity.StoragePath, configuration.StoragePath);
            Assert.AreEqual(entity.WebApiPort, configuration.WebApiPort);

        }
        
        [Test]
        public void SaveConfiguration_CallsDalWithCorrectArgs() {

            const int dataCompatibility = 15;
            const string storagePath = @"C:\StoragePath";
            const int port = 8000;

            var entity = CreateSampleConfigurationEntity();

            var configurationDal = Substitute.For<IConfigurationDataAccess>();

            var configuration = new Configuration(configurationDal) {
                DataCompatibilityVersion = dataCompatibility,
                StoragePath = storagePath,
                WebApiPort = port
            };

            configuration.SaveConfiguration();

            configurationDal.Received().SaveConfiguration(Arg.Is<ConfigurationEntity>(x =>
                x.DataCompatibilityVersion == dataCompatibility &&
                x.StoragePath.Equals(storagePath) &&
                x.WebApiPort == port)
            );

        }

        [Test]
        public void ValidConfiguration_WithValidValues_ReturnsTrue() {

            var configurationDal = Substitute.For<IConfigurationDataAccess>();
            var configuration = new Configuration(configurationDal) {
                DataCompatibilityVersion = 1,
                StoragePath = Path.GetTempPath(),
                WebApiPort = 8000
            };

            Assert.IsTrue(configuration.ValidateConfiguration());

        }
        
        [Test]
        public void ValidConfiguration_WithInvalidDatabaseCompatibility_ReturnsFalse() {

            var configurationDal = Substitute.For<IConfigurationDataAccess>();

            var configuration = new Configuration(configurationDal) {
                DataCompatibilityVersion = Configuration.MinimumDataCompatibilityVersion - 1,
                StoragePath = Path.GetTempPath(),
                WebApiPort = 8000
            };

            Assert.IsFalse(configuration.ValidateConfiguration());

            configuration.DataCompatibilityVersion = Configuration.MaximumDataCompatibilityVersion + 1;

            Assert.IsFalse(configuration.ValidateConfiguration());

        }

        [Test]
        public void ValidConfiguration_WithInvalidPort_ReturnsFalse() {

            var configurationDal = Substitute.For<IConfigurationDataAccess>();

            var configuration = new Configuration(configurationDal) {
                DataCompatibilityVersion = 1,
                StoragePath = Path.GetTempPath(),
                WebApiPort = -8000
            };

            Assert.IsFalse(configuration.ValidateConfiguration());

            configuration.WebApiPort = int.MaxValue;

            Assert.IsFalse(configuration.ValidateConfiguration());

        }

        [Test]
        public void ValidConfiguration_WithInvalidStoragePath_ReturnsFalse() {

            var configurationDal = Substitute.For<IConfigurationDataAccess>();

            var configuration = new Configuration(configurationDal) {
                DataCompatibilityVersion = 1,
                StoragePath = null,
                WebApiPort = 8000
            };

            Assert.IsFalse(configuration.ValidateConfiguration());

            configuration.StoragePath = "junkstorage";

            Assert.IsFalse(configuration.ValidateConfiguration());

            configuration.StoragePath = "test >> &&& // \\ || bad path";
            Assert.IsFalse(configuration.ValidateConfiguration());

        }

        private static ConfigurationEntity CreateSampleConfigurationEntity() {

            var entity = new ConfigurationEntity {
                DataCompatibilityVersion = 15,
                StoragePath = @"C:\temp\",
                WebApiPort = 8000
            };

            return entity;

        }

    }

}
