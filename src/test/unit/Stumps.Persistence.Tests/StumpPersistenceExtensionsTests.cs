namespace Stumps
{

    using System;
    using System.IO;
    using NUnit.Framework;

    [TestFixture]
    public class StumpPersistenceExtensionsTests
    {

        [Test]
        public void SaveStumpToArchive_WithNullStump_ThrowsException()
        {

            Assert.That(
                () => StumpPersistenceExtensions.SaveToArchive(null, string.Empty),
                Throws.Exception.TypeOf<ArgumentNullException>().With.Property("ParamName").EqualTo("stump"));

        }

        [Test]
        public void SaveStumpToArchive_WithNullFileName_ThrowsException()
        {

            var stump = new Stump("abcd");

            Assert.That(
                () => stump.SaveToArchive(null),
                Throws.Exception.TypeOf<ArgumentNullException>().With.Property("ParamName").EqualTo("fileName"));

        }

        [Test, Ignore]
        public void SaveStumpToArchive_WhenFileExists_OverridesExistingFile()
        {

            var stump = new Stump("abcd");

            using (var file = new TemporaryFile())
            {

                file.EnsureExistsWithJunk();
                var expected = new FileInfo(file.FileAndPath);

                stump.SaveToArchive(file.FileAndPath);

                var result = new FileInfo(file.FileAndPath);

                Assert.AreNotEqual(expected.Length, result.Length);

            }

        }

        [Test, Ignore]
        public void SaveStumpToArchive_WithValidStumpAndFile_CreatesFile()
        {

            var stump = new Stump("abcd");

            using (var file = new TemporaryFile())
            {

                stump.SaveToArchive(file.FileAndPath);
                Assert.IsTrue(File.Exists(file.FileAndPath));

            }

        }

    }

}
