namespace Stumps.Proxy {

    using System;
    using NUnit.Framework;

    [TestFixture]
    public class ContentEncodingTests {

        private readonly byte[] HelloWorldUtf8 = new byte[] { 
            72, 101, 108, 108, 111, 32, 87, 111,
            114, 108, 100
        };

        private readonly byte[] HelloWorldGZip = new byte[] {
            31, 139, 8, 0, 0, 0, 0, 0, 
            4, 0, 243, 72, 205, 201, 201, 87, 
            8, 207, 47, 202, 73, 1, 0, 86, 
            177, 23, 74, 11, 0, 0, 0
        };

        private readonly byte[] HelloWorldDeflate = new byte[] {
            243, 72, 205, 201, 201, 87, 8, 207, 47, 202, 73, 1, 0
        };

        [Test]
        public void Constructor_WithNullMethod_MethodIsStringEmpty() {

            var encoding = new ContentEncoding(null);
            Assert.AreEqual(string.Empty, encoding.Method);

        }

        [Test]
        public void Constructor_WithValue_MethodUpdated() {

            var expected = "gzip";

            var encoding = new ContentEncoding(expected);
            Assert.AreEqual(expected, encoding.Method);

        }

        [Test]
        public void Encode_WithNull_ReturnsNull() {

            byte[] nullBytes = null;
            var encoding = new ContentEncoding("gzip");
            nullBytes = encoding.Encode(nullBytes);

            Assert.IsNull(nullBytes);

        }

        [Test]
        public void Encode_UsingUnknownMethod_ReturnsSame() {

            var encoding = new ContentEncoding("badmethod");
            var actual = encoding.Encode(HelloWorldUtf8);
            CollectionAssert.AreEqual(HelloWorldUtf8, actual);

        }

        [Test]
        public void Encode_UsingGzip_ReturnsCompressed() {

            var encoding = new ContentEncoding("gzip");
            var actual = encoding.Encode(HelloWorldUtf8);
            CollectionAssert.AreEqual(HelloWorldGZip, actual);

        }

        [Test]
        public void Encode_UsingGzipUppercase_ReturnsCompressed() {

            var encoding = new ContentEncoding("GZIP");
            var actual = encoding.Encode(HelloWorldUtf8);
            CollectionAssert.AreEqual(HelloWorldGZip, actual);

        }

        [Test]
        public void Encode_UsingDeflate_ReturnsCompressed() {

            var encoding = new ContentEncoding("deflate");
            var actual = encoding.Encode(HelloWorldUtf8);
            CollectionAssert.AreEqual(HelloWorldDeflate, actual);

        }

        [Test]
        public void Decode_WithNull_ReturnsNull() {

            byte[] nullBytes = null;
            var encoding = new ContentEncoding("gzip");
            nullBytes = encoding.Decode(nullBytes);

            Assert.IsNull(nullBytes);

        }

        [Test]
        public void Decode_UsingUnknownMethod_ReturnsSame() {

            var encoding = new ContentEncoding("badmethod");
            var actual = encoding.Decode(HelloWorldGZip);
            CollectionAssert.AreEqual(HelloWorldGZip, actual);

        }

        [Test]
        public void Decode_UsingGzip_ReturnsDecompressed() {

            var encoding = new ContentEncoding("gzip");
            var actual = encoding.Decode(HelloWorldGZip);
            CollectionAssert.AreEqual(HelloWorldUtf8, actual);

        }

        [Test]
        public void Decode_UsingGzipUppercase_ReturnsDecompressed() {

            var encoding = new ContentEncoding("GZIP");
            var actual = encoding.Decode(HelloWorldGZip);
            CollectionAssert.AreEqual(HelloWorldUtf8, actual);

        }

        [Test]
        public void Decode_UsingDeflate_ReturnsDecompressed() {

            var encoding = new ContentEncoding("deflate");
            var actual = encoding.Decode(HelloWorldDeflate);
            CollectionAssert.AreEqual(HelloWorldUtf8, actual);

        }

    }

}
