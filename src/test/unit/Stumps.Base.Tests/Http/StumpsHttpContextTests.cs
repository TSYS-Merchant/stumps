namespace Stumps.Http
{

    using System;
    using NUnit.Framework;

    [TestFixture]
    public class StumpsHttpContextTests
    {

        [Test]
        public void Constructor_WithNullContext_ThrowsException()
        {

            Assert.That(
                () => new StumpsHttpContext(null),
                Throws.Exception.TypeOf<ArgumentNullException>().With.Property("ParamName").EqualTo("context"));

        }

    }

}