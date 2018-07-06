namespace Stumps
{
    using System;
    using NUnit.Framework;

    [TestFixture]
    public class SingleResponseFactoryTests
    {
        [Test]
        public void Constructor_WithResponse_PopulatesResponse()
        {
            var response = new BasicHttpResponse();
            var factory = new SingleHttpResponseFactory(response);

            Assert.AreSame(response, factory.Response);
        }

        [Test]
        public void Constructor_WithNullResponse_ThrowsException()
        {
            Assert.That(
                () => new SingleHttpResponseFactory(null),
                Throws.Exception.TypeOf<ArgumentNullException>().With.Property("ParamName").EqualTo("response"));
        }
    }
}
