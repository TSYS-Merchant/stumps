namespace Stumps
{
    using System;
    using System.Text;
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
        public void Constructor_Default_EmptyBody()
        {
            var response = new BasicHttpResponse();
            Assert.AreEqual(0, response.BodyLength);
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
        public void AppendToBody_WithAdditionalString_AppendsToEnd()
        {
            var newString = "ABCDE";

            var expectedString = "ABCDEABCDE";

            var response = new BasicHttpResponse();
            response.AppendToBody(newString);
            response.AppendToBody(newString);
            Assert.AreEqual(10, response.BodyLength);
            CollectionAssert.AreEqual(expectedString, response.GetBodyAsString());
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
        public void AppendToBody_WithNullBytes_DoesNotTrowError()
        {
            var response = new BasicHttpResponse();
            Assert.DoesNotThrow(() => response.AppendToBody((byte[])null));
        }

        [Test]
        public void AppendToBody_WithNullString_DoesNotTrowError()
        {
            var response = new BasicHttpResponse();
            Assert.DoesNotThrow(() => response.AppendToBody((string)null));
        }

        [Test]
        public void AppendToBody_WithString_UpdatesBody()
        {
            var expected = "ABCD";

            var response = new BasicHttpResponse();
            response.AppendToBody(expected);
            Assert.AreEqual(expected.Length, response.BodyLength);
        }

        [Test]
        public void AppendToBody_WithStringAndEncoding_UpdatesBody()
        {
            var expected = "ABCD";
            var expectedLength = expected.Length * 2;

            var response = new BasicHttpResponse();
            response.AppendToBody(expected, Encoding.Unicode);
            Assert.AreEqual(expectedLength, response.BodyLength);
        }

        [Test]
        public void AppendToBody_WithStringAndNullEncoding_ThrowsException()
        {
            var response = new BasicHttpResponse();
            
            Assert.That(
                () => response.AppendToBody(string.Empty, null),
                Throws.Exception.TypeOf<ArgumentNullException>().With.Property("ParamName").EqualTo("encoding"));
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

        [Test]
        public void GetBody_WithNullEncoding_ThrowsException()
        {
            var response = new BasicHttpResponse();
            response.AppendToBody("ABCD");

            Assert.That(
                () => response.GetBodyAsString(null),
                Throws.Exception.TypeOf<ArgumentNullException>().With.Property("ParamName").EqualTo("encoding"));
        }
    }
}
