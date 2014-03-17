namespace Stumps
{

    using System;
    using NUnit.Framework;
    
    [TestFixture]
    public class BasicHttpResponseTests
    {

        [Test]
        public void Constructor_Default_InitializesHeader()
        {
            var response = new BasicHttpResponse();
            Assert.IsNotNull(response.Headers);
        }

        [Test]
        public void Constructor_Default_SetsTo200Ok()
        {
            var response = new BasicHttpResponse();
            Assert.AreEqual(200, response.StatusCode);
            Assert.AreEqual("OK", response.StatusDescription);
        }

        [Test]
        public void Constructor_Default_NoBody()
        {
            var response = new BasicHttpResponse();
            Assert.AreEqual(0, response.BodyLength);
        }

        [Test]
        public void AppendToBody_WithNullBytes_DoesNotTrowError()
        {
            var response = new BasicHttpResponse();
            Assert.DoesNotThrow(() => response.AppendToBody(null));
        }

        [Test]
        public void AppendToBody_WithBytes_UpdatesBody()
        {
            var newBytes = new byte[5]
            {
                1, 2, 3, 4, 5
            };
 
            var response = new BasicHttpResponse();
            response.AppendToBody(newBytes);
            Assert.AreEqual(5, response.BodyLength);
        }

        [Test]
        public void AppendToBody_WithAdditionalBytes_AppendsToEnd()
        {

            var newBytes = new byte[5]
            {
                1, 2, 3, 4, 5
            };

            var expected = new byte[10]
            {
                1, 2, 3, 4, 5, 1, 2, 3, 4, 5
            };

            var response = new BasicHttpResponse();
            response.AppendToBody(newBytes);
            response.AppendToBody(newBytes);
            Assert.AreEqual(10, response.BodyLength);
            CollectionAssert.AreEqual(expected, response.GetBody());

        }

        [Test]
        public void ClearBody_WhenCalled_RemovedAllBytes()
        {

            var newBytes = new byte[5]
            {
                1, 2, 3, 4, 5
            };

            var response = new BasicHttpResponse();
            response.AppendToBody(newBytes);
            response.ClearBody();
            Assert.AreEqual(0, response.BodyLength);
            var bodyBytes = response.GetBody();
            Assert.IsNotNull(bodyBytes);
            Assert.AreEqual(0, bodyBytes.Length);

        }

    }

}
