namespace Stumps.Utility {

    using System;
    using System.IO;
    using NUnit.Framework;

    [TestFixture]
    public class JsonUtilityTests {

        [Test]
        public void SerializeToFile_WithValidObject_CreatesFile() {

            var tempFolder = createTempFolder();
            try {
                var obj = new SampleJsonObject() {
                    Color = "Red",
                    EnumerationValue = SampleJsonObject.SampleValues.Value2,
                    Number = 50
                };

                var fileName = Path.GetRandomFileName();
                var path = Path.Combine(tempFolder, fileName);

                JsonUtility.SerializeToFile(obj, path);

                Assert.IsTrue(File.Exists(path));
            }
            finally {
                deleteTempFolder(tempFolder);
            }

        }

        [Test]
        public void DeserializeFromFile_WithValidFile_LoadsAndDeserializes() {

            var tempFolder = createTempFolder();

            try {
                var path = createJsonFile(tempFolder);
                var obj = JsonUtility.DeserializeFromFile<SampleJsonObject>(path);

                Assert.AreEqual("Red", obj.Color);
                Assert.AreEqual(SampleJsonObject.SampleValues.Value2, obj.EnumerationValue);
                Assert.AreEqual(50, obj.Number);

            }
            finally {
                deleteTempFolder(tempFolder);
            }

        }

        [Test]
        public void DeserializeFromDirectory_WithValidDirectory_LoadsAndDeserializes() {

            var tempFolder = createTempFolder();

            try {

                createJsonFile(tempFolder);
                createJsonFile(tempFolder);
                createJsonFile(tempFolder);

                var obj = JsonUtility.DeserializeFromDirectory<SampleJsonObject>(tempFolder, "*.*", SearchOption.AllDirectories);

                Assert.AreEqual(3, obj.Count);

            }
            finally {
                deleteTempFolder(tempFolder);
            }

        }

        private string createTempFolder() {

            var path = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            Directory.CreateDirectory(path);

            return path;

        }

        private void deleteTempFolder(string path) {

            if ( Directory.Exists(path) ) {
                Directory.Delete(path, true);
            }

        }

        private string createJsonFile(string tempFolder) {
            var fileName = Path.GetRandomFileName();
            var path = Path.Combine(tempFolder, fileName);
            File.WriteAllText(path, TestData.JsonSampleData);

            return path;
        }

    }

}
