namespace Stumps
{

    using System;
    using System.IO;
    using System.Threading.Tasks;
    using NUnit.Framework;

    [TestFixture]
    public class StreamUtilityTests
    {

        [Test]
        public void ConvertStreamToByteArray_NullValue_ThrowsException()
        {
            var ex = Assert.ThrowsAsync<ArgumentNullException>(async () => await StreamUtility.ConvertStreamToByteArray(null));
            Assert.That(ex.ParamName.Equals("stream", StringComparison.OrdinalIgnoreCase));
        }

        [Test]
        public async Task ConvertStreamToByteArray_ValidStream_ReturnsArray()
        {
            var buffer = CreateByteArray(100);

            using (var ms = new MemoryStream(buffer))
            {
                var newBuffer = await StreamUtility.ConvertStreamToByteArray(ms);
                CollectionAssert.AreEqual(buffer, newBuffer);
            }
        }

        [Test]
        public void CopyStreamWithStartingPosition_NullInputStream_ThrowsException()
        {
            var ex = Assert.ThrowsAsync<ArgumentNullException>(async () =>
            {
                using (var ms = new MemoryStream())
                {
                    await StreamUtility.CopyStream(null, ms, 5);
                }
            });

            Assert.That(ex.ParamName.Equals("inputStream", StringComparison.Ordinal));
        }

        [Test]
        public void CopyStreamWithStartingPosition_NullOutputStream_ThrowsException()
        {
            var ex = Assert.ThrowsAsync<ArgumentNullException>(async () =>
            {
                using (var ms = new MemoryStream())
                {
                    await StreamUtility.CopyStream(ms, null, 5);
                }
            });

            Assert.That(ex.ParamName.Equals("outputStream", StringComparison.Ordinal));
        }

        [Test]
        public async Task CopyStreamWithStartingPosition_ValidStreamsAndStartingPosition_CopiesPartiallyAndResetStart()
        {
            var buffer = CreateByteArray(100);

            using (var source = new MemoryStream(buffer))
            {
                using (var output = new MemoryStream())
                {
                    await StreamUtility.CopyStream(source, output, 5);
                    Assert.AreEqual(95, output.Length);
                    Assert.AreEqual(5, source.Position);
                }
            }
        }

        [Test]
        public void CopyStream_NullInputStream_ThrowsException()
        {
            var ex = Assert.ThrowsAsync<ArgumentNullException>(async () =>
            {
                using (var ms = new MemoryStream())
                {
                    await StreamUtility.CopyStream(null, ms);
                }
            });

            Assert.That(ex.ParamName.Equals("inputStream", StringComparison.Ordinal));
        }

        [Test]
        public void CopyStream_NullOutputStream_ThrowsException()
        {
            var ex = Assert.ThrowsAsync<ArgumentNullException>(async () =>
            {
                using (var ms = new MemoryStream())
                {
                    await StreamUtility.CopyStream(ms, null);
                }
            });

            Assert.That(ex.ParamName.Equals("outputStream", StringComparison.Ordinal));
        }

        [Test]
        public async Task CopyStream_ValidStreams_CopiesStream()
        {
            var buffer = CreateByteArray(100);

            using (var source = new MemoryStream(buffer))
            {
                source.Position = 0;

                using (var output = new MemoryStream())
                {
                    await StreamUtility.CopyStream(source, output);
                    Assert.AreEqual(100, output.Length);
                }
            }
        }

        [Test]
        public void WriteUtf8StringToStream_NullStream_ThrowsException()
        {
            var ex = Assert.ThrowsAsync<ArgumentNullException>(async () => await StreamUtility.WriteUtf8StringToStream("ABCD", null));
            Assert.That(ex.ParamName.Equals("stream", StringComparison.Ordinal));
        }

        [Test]
        public void WriteUtf8StringToStream_NullValue_ThrowsException()
        {
            var ex = Assert.ThrowsAsync<ArgumentNullException>(async () =>
            {
                using (var ms = new MemoryStream())
                {
                    await StreamUtility.WriteUtf8StringToStream(null, ms);
                }
            });

            Assert.That(ex.ParamName.Equals("value", StringComparison.Ordinal));
        }

        [Test]
        public async Task WriteUtfStringToStream_ValidString_WritesToStream()
        {
            var tempFolder = CreateTempFolder();
            var file = Path.GetRandomFileName();
            var path = Path.Combine(tempFolder, file);

            try
            {
                using (var stream = File.OpenWrite(path))
                {
                    await StreamUtility.WriteUtf8StringToStream("HelloWorld", stream);
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