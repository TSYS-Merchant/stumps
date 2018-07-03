namespace Stumps.Http
{
    using System;
    using NUnit.Framework;

    [TestFixture]
    public class StumpsHttpContextTests
    {
        [Test]
        public void InitializeInstance_WithNullContext_ThrowsException()
        {

            var stumpsContext = new StumpsHttpContext();

            var ex = Assert.ThrowsAsync<ArgumentNullException>(async () => await stumpsContext.InitializeInstance(null));

            Assert.That(ex.ParamName.Equals("context", StringComparison.Ordinal));

        }

        [Test]
        public void InitializeInstance_CalledTwice_ThrowsException()
        {
            var stumpsContext = new StumpsHttpContext();

            Assert.ThrowsAsync<ArgumentNullException>(async () => await stumpsContext.InitializeInstance(null));
            Assert.ThrowsAsync<InvalidOperationException>(async () => await stumpsContext.InitializeInstance(null));
        }
    }
}