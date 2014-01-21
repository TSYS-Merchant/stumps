namespace Stumps.Data {

    using System;
    using System.IO;
    using NSubstitute;
    using NUnit.Framework;

    [TestFixture]
    public class ConfigurationDataAccessTests {

        [Test]
        public void Configuration_WithNullFileName_ThrowsException() {

            Assert.That(
                () => new ConfigurationDataAccess(null),
                Throws.Exception
                    .TypeOf<ArgumentNullException>()
                    .With.Property("ParamName")
                    .EqualTo("configurationFile")
            );

        }

        [Test]
        public void Configuration_WithValidFileName_DoesNotThrowError() {

            Assert.DoesNotThrow(() => new ConfigurationDataAccess("someValue"));

        }

        [Test]
        public void LoadConfiguration_LoadsExpectedEntity() {

            var tempFile = Path.GetTempFileName();
            File.WriteAllText(tempFile, TestData.SampleConfiguration);

            try {
                var dal = new ConfigurationDataAccess(tempFile);
                var entity = dal.LoadConfiguration();

                Assert.IsNotNull(entity);

                Assert.AreEqual(100, entity.DataCompatibilityVersion);
                StringAssert.AreEqualIgnoringCase("C:\\SomeValue\\", entity.StoragePath);
                Assert.AreEqual(8000, entity.WebApiPort);

            }
            finally {
                File.Delete(tempFile);
            }

        }

        [Test]
        public void SaveConfiguration_WithNullValue_ThrowsException() {

            var tempFile = Path.GetTempFileName();

            try {
                var dal = new ConfigurationDataAccess(tempFile);

                Assert.That(
                    () => dal.SaveConfiguration(null),
                    Throws.Exception
                        .TypeOf<ArgumentNullException>()
                        .With.Property("ParamName")
                        .EqualTo("value")
                );
            }
            finally {
                File.Delete(tempFile);
            }

        }

        [Test]
        public void SaveConfiguration_WithValidEntity_CreateExpectedFile() {

            var tempFile = Path.GetTempFileName();

            try {
                var dal = new ConfigurationDataAccess(tempFile);
                var entity = new ConfigurationEntity {
                    DataCompatibilityVersion = 100,
                    StoragePath = "C:\\SomeValue\\",
                    WebApiPort = 8000
                };

                dal.SaveConfiguration(entity);

                var savedText = File.ReadAllText(tempFile);

                StringAssert.AreEqualIgnoringCase(TestData.SampleConfiguration, savedText);
            }
            finally {
                File.Delete(tempFile);
            }

        }

    }

}
