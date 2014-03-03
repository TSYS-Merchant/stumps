namespace Stumps.Utility
{

    using System.IO;
    using NUnit.Framework;

    [TestFixture]
    public class JsonUtilityTests
    {

        [Test]
        public void DeserializeFromDirectory_WithValidDirectory_LoadsAndDeserializes()
        {

            var tempFolder = CreateTempFolder();

            try
            {

                CreateJsonFile(tempFolder);
                CreateJsonFile(tempFolder);
                CreateJsonFile(tempFolder);

                var obj = JsonUtility.DeserializeFromDirectory<SampleJsonObject>(
                    tempFolder, "*.*", SearchOption.AllDirectories);

                Assert.AreEqual(3, obj.Count);

            }
            finally
            {
                DeleteTempFolder(tempFolder);
            }

        }

        [Test]
        public void DeserializeFromFile_WithValidFile_LoadsAndDeserializes()
        {

            var tempFolder = CreateTempFolder();

            try
            {
                var path = CreateJsonFile(tempFolder);
                var obj = JsonUtility.DeserializeFromFile<SampleJsonObject>(path);

                Assert.AreEqual("Red", obj.Color);
                Assert.AreEqual(SampleJsonObject.SampleValues.Value2, obj.EnumerationValue);
                Assert.AreEqual(50, obj.Number);

            }
            finally
            {
                DeleteTempFolder(tempFolder);
            }

        }

        [Test]
        public void SerializeToFile_WithValidObject_CreatesFile()
        {

            var tempFolder = CreateTempFolder();
            try
            {
                var obj = new SampleJsonObject
                {
                    Color = "Red",
                    EnumerationValue = SampleJsonObject.SampleValues.Value2,
                    Number = 50
                };

                var fileName = Path.GetRandomFileName();
                var path = Path.Combine(tempFolder, fileName);

                JsonUtility.SerializeToFile(obj, path);

                Assert.IsTrue(File.Exists(path));
            }
            finally
            {
                DeleteTempFolder(tempFolder);
            }

        }

        private string CreateJsonFile(string tempFolder)
        {
            var fileName = Path.GetRandomFileName();
            var path = Path.Combine(tempFolder, fileName);
            File.WriteAllText(path, TestData.JsonSampleData);

            return path;
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