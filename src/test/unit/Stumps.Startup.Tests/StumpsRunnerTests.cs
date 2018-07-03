namespace Stumps.Startup
{
    using System;
    using NSubstitute;
    using NUnit.Framework;
    using Stumps.Server;
    using Stumps.Server.Data;

    [TestFixture]
    public class StumpsRunnerTests
    {
        [Test]
        public void Constructor_WithNullConfiguration_ThrowsException()
        {
            Assert.That(
                () => new StumpsRunner(null),
                Throws.Exception.TypeOf<ArgumentNullException>().With.Property("ParamName").EqualTo("configuration"));
        }

        [Test]
        public void Constructor_WithValidConfiguration_SetsConfigurationProperty()
        {
            var dal = Substitute.For<IConfigurationDataAccess>();
            var config = new StumpsConfiguration(dal);

            using (var server = new StumpsRunner(config))
            {
                Assert.AreSame(config, server.Configuration);
            }
        }
    }
}
