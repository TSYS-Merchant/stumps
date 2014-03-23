namespace Stumps
{

    using System;
    using NUnit.Framework;

    [TestFixture]
    public class ProxyHandlerTests
    {

        [Test]
        public void Constructor_NullExternalHostUri_ThowsException()
        {

            Assert.That(
                () => new ProxyHandler(null),
                Throws.Exception.TypeOf<ArgumentNullException>().With.Property("ParamName").EqualTo("externalHostUri"));

        }

        [Test]
        public void ProcessRequest_NullValue_ThrowsException()
        {

            var proxy = new ProxyHandler(new Uri("http://localhost/"));

            Assert.That(
                () => proxy.ProcessRequest(null),
                Throws.Exception.TypeOf<ArgumentNullException>().With.Property("ParamName").EqualTo("context"));

        }

    }

}