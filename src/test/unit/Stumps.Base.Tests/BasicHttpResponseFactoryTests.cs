namespace Stumps
{
    using System;
    using NUnit.Framework;

    [TestFixture]
    public class BasicHttpResponseFactoryTests
    {
        [Test]
        public void Constructor_WithResponse_PopulatesResponse()
        {
            var response = new BasicHttpResponse();
            var factory = new BasicHttpResponseFactory(response);

            Assert.AreSame(response, factory.Response);
        }

        [Test]
        public void Constructor_WithNullResponse_ThrowsException()
        {
            Assert.That(
                () => new BasicHttpResponseFactory(null),
                Throws.Exception.TypeOf<ArgumentNullException>().With.Property("ParamName").EqualTo("response"));
        }
    }
}
