namespace Stumps
{
    using System;
    using System.IO;
    using NUnit.Framework;

    [TestFixture]
    public class StreamUtilityTests
    {
        [Test]
        public void ConvertStreamToByteArray_NullValue_ThrowsException()
        {
            Assert.That(
                () => StreamUtility.ConvertStreamToByteArray(null),
                Throws.Exception.TypeOf<ArgumentNullException>().With.Property("ParamName").EqualTo("stream"));
        }

        [Test]
        public void ConvertStreamToByteArray_ValidStream_ReturnsArray()
        {
            var buffer = CreateByteArray(100);

            using (var ms = new MemoryStream(buffer))
            {
                var newBuffer = StreamUtility.ConvertStreamToByteArray(ms);
                CollectionAssert.AreEqual(buffer, newBuffer);
            }
        }

        [Test]
        public void CopyStreamWithStartingPosition_NullInputStream_ThrowsException()
        {
            Assert.That(
                () =>
                {
                    using (var ms = new MemoryStream())
                    {
                        StreamUtility.CopyStream(null, ms, 5);
                    }
                }, 
                Throws.Exception.TypeOf<ArgumentNullException>().With.Property("ParamName").EqualTo("inputStream"));
        }

        [Test]
        public void CopyStreamWithStartingPosition_NullOutputStream_ThrowsException()
        {
            Assert.That(
                () =>
                {
                    using (var ms = new MemoryStream())
                    {
                        StreamUtility.CopyStream(ms, null, 5);
                    }
                }, 
                Throws.Exception.TypeOf<ArgumentNullException>().With.Property("ParamName").EqualTo("outputStream"));
        }

        [Test]
        public void CopyStreamWithStartingPosition_ValidStreamsAndStartingPosition_CopiesPartiallyAndResetStart()
        {
            var buffer = CreateByteArray(100);

            using (var source = new MemoryStream(buffer))
            {
                using (var output = new MemoryStream())
                {

                    StreamUtility.CopyStream(source, output, 5);
                    Assert.AreEqual(95, output.Length);
                    Assert.AreEqual(5, source.Position);

                }
            }
        }

        [Test]
        public void CopyStream_NullInputStream_ThrowsException()
        {
            Assert.That(
                () =>
                {
                    using (var ms = new MemoryStream())
                    {
                        StreamUtility.CopyStream(null, ms);
                    }
                }, 
                Throws.Exception.TypeOf<ArgumentNullException>().With.Property("ParamName").EqualTo("inputStream"));
        }

        [Test]
        public void CopyStream_NullOutputStream_ThrowsException()
        {
            Assert.That(
                () =>
                {
                    using (var ms = new MemoryStream())
                    {
                        StreamUtility.CopyStream(ms, null);
                    }
                }, 
                Throws.Exception.TypeOf<ArgumentNullException>().With.Property("ParamName").EqualTo("outputStream"));
        }

        [Test]
        public void CopyStream_ValidStreams_CopiesStream()
        {
            var buffer = CreateByteArray(100);

            using (var source = new MemoryStream(buffer))
            {

                source.Position = 0;

                using (var output = new MemoryStream())
                {

                    StreamUtility.CopyStream(source, output);
                    Assert.AreEqual(100, output.Length);

                }

            }
        }

        [Test]
        public void WriteUtf8StringToStream_NullStream_ThrowsException()
        {
            Assert.That(
                () => StreamUtility.WriteUtf8StringToStream("ABCD", null),
                Throws.Exception.TypeOf<ArgumentNullException>().With.Property("ParamName").EqualTo("stream"));
        }

        [Test]
        public void WriteUtf8StringToStream_NullValue_ThrowsException()
        {
            Assert.That(
                () =>
                {
                    using (var ms = new MemoryStream())
                    {
                        StreamUtility.WriteUtf8StringToStream(null, ms);
                    }
                }, 
                Throws.Exception.TypeOf<ArgumentNullException>().With.Property("ParamName").EqualTo("value"));
        }

        [Test]
        public void WriteUtfStringToStream_ValidString_WritesToStream()
        {
            var tempFolder = CreateTempFolder();
            var file = Path.GetRandomFileName();
            var path = Path.Combine(tempFolder, file);

            try
            {
                using (var stream = File.OpenWrite(path))
                {
                    StreamUtility.WriteUtf8StringToStream("HelloWorld", stream);
                }

                var fi = new FileInfo(path);
                Assert.AreEqual(13, fi.Length);
            }
            finally
            {
                if (Directory.Exists(tempFolder))
                {
                    DeleteTempFolder(tempFolder);
                }
            }
        }

        private byte[] CreateByteArray(int length)
        {
            var buffer = new byte[length];
            var random = new Random();
            random.NextBytes(buffer);

            return buffer;
        }

        private string CreateTempFolder()
        {
            var path = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            Directory.CreateDirectory(path);

            return path;
        }

        private void DeleteTempFolder(string path)
        {
            if (Directory.Exists(path))
            {
                Directory.Delete(path, true);
            }
        }
    }
}