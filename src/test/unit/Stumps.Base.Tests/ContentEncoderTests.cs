namespace Stumps
{

    using NUnit.Framework;

    [TestFixture]
    public class ContentEncoderTests
    {

        private readonly byte[] _helloWorldUtf8 = new byte[]
        {
            72, 101, 108, 108, 111, 32, 87, 111, 114, 108, 100
        };

        private readonly byte[] _helloWorldGZip = new byte[]
        {
            31, 139, 8, 0, 0, 0, 0, 0, 4, 0, 243, 72, 205, 201, 201, 87, 8, 207, 47, 202, 73, 1, 0, 86, 177, 23, 74, 11,
            0, 0, 0
        };

        private readonly byte[] _helloWorldDeflate = new byte[]
        {
            243, 72, 205, 201, 201, 87, 8, 207, 47, 202, 73, 1, 0
        };

        [Test]
        public void Constructor_WithNullMethod_MethodIsStringEmpty()
        {

            var encoding = new ContentEncoder(null);
            Assert.AreEqual(string.Empty, encoding.Method);

        }

        [Test]
        public void Constructor_WithValue_MethodUpdated()
        {

            const string Expected = "gzip";

            var encoding = new ContentEncoder(Expected);
            Assert.AreEqual(Expected, encoding.Method);

        }

        [Test]
        public void Decode_UsingDeflate_ReturnsDecompressed()
        {

            var encoding = new ContentEncoder("deflate");
            var actual = encoding.Decode(_helloWorldDeflate);
            CollectionAssert.AreEqual(_helloWorldUtf8, actual);

        }

        [Test]
        public void Decode_UsingGzipUppercase_ReturnsDecompressed()
        {

            var encoding = new ContentEncoder("GZIP");
            var actual = encoding.Decode(_helloWorldGZip);
            CollectionAssert.AreEqual(_helloWorldUtf8, actual);

        }

        [Test]
        public void Decode_UsingGzip_ReturnsDecompressed()
        {

            var encoding = new ContentEncoder("gzip");
            var actual = encoding.Decode(_helloWorldGZip);
            CollectionAssert.AreEqual(_helloWorldUtf8, actual);

        }

        [Test]
        public void Decode_UsingUnknownMethod_ReturnsSame()
        {

            var encoding = new ContentEncoder("badmethod");
            var actual = encoding.Decode(_helloWorldGZip);
            CollectionAssert.AreEqual(_helloWorldGZip, actual);

        }

        [Test]
        public void Decode_WithNull_ReturnsNull()
        {

            var encoding = new ContentEncoder("gzip");
            var nullBytes = encoding.Decode(null);

            Assert.IsNull(nullBytes);

        }

        [Test]
        public void Encode_UsingDeflate_ReturnsCompressed()
        {

            var encoding = new ContentEncoder("deflate");
            var actual = encoding.Encode(_helloWorldUtf8);
            CollectionAssert.AreEqual(_helloWorldDeflate, actual);

        }

        [Test]
        public void Encode_UsingGzipUppercase_ReturnsCompressed()
        {

            var encoding = new ContentEncoder("GZIP");
            var actual = encoding.Encode(_helloWorldUtf8);
            CollectionAssert.AreEqual(_helloWorldGZip, actual);

        }

        [Test]
        public void Encode_UsingGzip_ReturnsCompressed()
        {

            var encoding = new ContentEncoder("gzip");
            var actual = encoding.Encode(_helloWorldUtf8);
            CollectionAssert.AreEqual(_helloWorldGZip, actual);

        }

        [Test]
        public void Encode_UsingUnknownMethod_ReturnsSame()
        {

            var encoding = new ContentEncoder("badmethod");
            var actual = encoding.Encode(_helloWorldUtf8);
            CollectionAssert.AreEqual(_helloWorldUtf8, actual);

        }

        [Test]
        public void Encode_WithNull_ReturnsNull()
        {

            var encoding = new ContentEncoder("gzip");
            var nullBytes = encoding.Encode(null);

            Assert.IsNull(nullBytes);

        }

    }

}