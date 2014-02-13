namespace Stumps.Data {

    using System;
    using System.IO;
    using NUnit.Framework;
    using NSubstitute;

    [TestFixture]
    public class DataAccessTests {

        private const string SampleHostName = "myserver.inc";

        private readonly byte[] _sampleBytes;
        private readonly ProxyServerEntity _sampleProxyServer;
        private readonly StumpEntity _sampleStump;

        public DataAccessTests() {

            _sampleProxyServer = new ProxyServerEntity {
                AutoStart = false,
                ExternalHostName = SampleHostName,
                Port = 500,
                ProxyId = "ABCD",
                UseSsl = false
            };

            _sampleStump = new StumpEntity {
                HttpMethod = "GET",
                MatchBodyContentType = null,
                MatchBodyFileName = null,
                MatchBodyIsImage = false,
                MatchBodyIsText = true,
                MatchBodyMaximumLength = -1,
                MatchBodyMinimumLength = -1,
                MatchBodyText = new string[] { },
                MatchHeaders = new HeaderEntity[] { },
                MatchHttpMethod = false,
                MatchRawUrl = true,
                RawUrl = "/",
                ResponseBodyContentType = "text/plain",
                ResponseBodyFileName = null,
                ResponseBodyIsImage = false,
                ResponseBodyIsText = true,
                ResponseHeaders = new HeaderEntity[] { },
                ResponseStatusCode = 200,
                ResponseStatusDescription = "OK",
                StumpCategory = "Uncategorized",
                StumpId = "ABCD",
                StumpName = "MyStump"
            };

            _sampleBytes = new byte[] { 1, 2, 3 };

        }

        [Test]
        public void Constructor_WithBadPath_ThrowsException() {

            Assert.That(
                () => new DataAccess(null),
                Throws.Exception
                    .TypeOf<ArgumentNullException>()
                    .With.Property("ParamName")
                    .EqualTo("dataPath")
            );

        }

        [Test]
        public void ProxyServerCreate_WithNullEntity_ThrowsException() {

            var dal = CreateDataAccessLayer();

            try {

                Assert.That(
                    () => dal.ProxyServerCreate(null),
                    Throws.Exception
                        .TypeOf<ArgumentNullException>()
                        .With.Property("ParamName")
                        .EqualTo("server")
                );

            }
            finally {
                DeleteDataAccessLayer(dal);
            }

        }

        [Test]
        public void ProxyServerCreate_WithValidEntity_PersistsToDisk() {

            var dal = CreateDataAccessLayer();

            try {

                dal.ProxyServerCreate(_sampleProxyServer);
                var proxyFile = Path.Combine(dal.DataPath, _sampleProxyServer.ProxyId + ".proxy");
                var proxyDirectory = Path.Combine(dal.DataPath, _sampleProxyServer.ProxyId);

                Assert.IsTrue(File.Exists(proxyFile));
                Assert.IsTrue(Directory.Exists(proxyDirectory));

            }
            finally {
                DeleteDataAccessLayer(dal);
            }

        }

        [Test]
        public void ProxyServerDelete_WithValidHostName_RemovesFromDisk() {

            var dal = CreateDataAccessLayer();

            try {

                dal.ProxyServerCreate(_sampleProxyServer);
                dal.ProxyServerDelete(_sampleProxyServer.ProxyId);

                var proxyFile = Path.Combine(dal.DataPath, _sampleProxyServer.ProxyId + ".proxy");
                var proxyDirectory = Path.Combine(dal.DataPath, _sampleProxyServer.ProxyId);

                Assert.IsFalse(File.Exists(proxyFile));
                Assert.IsFalse(Directory.Exists(proxyDirectory));

            }
            finally {
                DeleteDataAccessLayer(dal);
            }

        }

        [Test]
        public void ProxyServerFindAll_WithNoData_ReturnsEmptyList() {

            var dal = CreateDataAccessLayer();

            try {

                var list = dal.ProxyServerFindAll();
                Assert.IsNotNull(list);
                Assert.AreEqual(0, list.Count);

            }
            finally {
                DeleteDataAccessLayer(dal);
            }

        }

        [Test]
        public void ProxyServerFindAll_WithValidProxy_ReturnsList() {

            var dal = CreateDataAccessLayer();

            try {

                dal.ProxyServerCreate(_sampleProxyServer);
                var list = dal.ProxyServerFindAll();
                Assert.AreEqual(1, list.Count);

            }
            finally {
                DeleteDataAccessLayer(dal);
            }

        }

        [Test]
        public void StumpCreate_WithNullEntity_ThrowsException() {

            var dal = CreateDataAccessLayerWithProxy();

            try {

                Assert.That(
                    () => dal.StumpCreate(SampleHostName, null, null, null),
                    Throws.Exception
                        .TypeOf<ArgumentNullException>()
                        .With.Property("ParamName")
                        .EqualTo("entity")
                );

            }
            finally {
                DeleteDataAccessLayer(dal);
            }

        }
        
        [Test]
        public void StumpCreate_WithValidStump_PersistsToDisk() {

            var dal = CreateDataAccessLayerWithProxy();

            try {

                var stump = dal.StumpCreate(_sampleProxyServer.ProxyId, _sampleStump, null, null);

                Assert.IsNotNull(stump);
                Assert.IsTrue(String.IsNullOrWhiteSpace(stump.MatchBodyFileName));
                Assert.IsTrue(String.IsNullOrWhiteSpace(stump.ResponseBodyFileName));

                var stumpsFile = Path.Combine(dal.DataPath, _sampleProxyServer.ProxyId, DataAccess.StumpsPathName, "ABCD" + DataAccess.StumpFileExtension);
                var matchFile = Path.Combine(dal.DataPath, _sampleProxyServer.ProxyId, DataAccess.StumpsPathName, "ABCD" + DataAccess.BodyMatchFileExtension);
                var responseFile = Path.Combine(dal.DataPath, _sampleProxyServer.ProxyId, DataAccess.StumpsPathName, "ABCD" + DataAccess.BodyResponseFileExtension);

                Assert.IsTrue(File.Exists(stumpsFile));
                Assert.IsFalse(File.Exists(matchFile));
                Assert.IsFalse(File.Exists(responseFile));

            }
            finally {
                DeleteDataAccessLayer(dal);
            }

        }

        [Test]
        public void StumpCreate_WithValidStumpAndBody_PersistsToDisk() {

            var dal = CreateDataAccessLayerWithProxy();

            try {

                var stump = dal.StumpCreate(_sampleProxyServer.ProxyId, _sampleStump, _sampleBytes, _sampleBytes);

                Assert.IsNotNull(stump);
                Assert.IsNotNull(stump.MatchBodyFileName);
                Assert.IsNotNull(stump.ResponseBodyFileName);

                var stumpsFile = Path.Combine(dal.DataPath, _sampleProxyServer.ProxyId, DataAccess.StumpsPathName, "ABCD" + DataAccess.StumpFileExtension);
                var matchFile = Path.Combine(dal.DataPath, _sampleProxyServer.ProxyId, DataAccess.StumpsPathName, "ABCD" + DataAccess.BodyMatchFileExtension);
                var responseFile = Path.Combine(dal.DataPath, _sampleProxyServer.ProxyId, DataAccess.StumpsPathName, "ABCD" + DataAccess.BodyResponseFileExtension);

                Assert.IsTrue(File.Exists(stumpsFile));
                Assert.IsTrue(File.Exists(matchFile));
                Assert.IsTrue(File.Exists(responseFile));

            }
            finally {
                DeleteDataAccessLayer(dal);
            }

        }

        [Test]
        public void StumpDelete_WithValidStump_DeletesFromDisk() {

            var dal = CreateDataAccessLayerWithProxy();

            try {

                var stump = dal.StumpCreate(_sampleProxyServer.ProxyId, _sampleStump, _sampleBytes, _sampleBytes);
                dal.StumpDelete(_sampleProxyServer.ProxyId, stump.StumpId);

                var stumpsFile = Path.Combine(dal.DataPath, DataAccess.StumpsPathName, "ABCD" + DataAccess.StumpFileExtension);
                var matchFile = Path.Combine(dal.DataPath, DataAccess.StumpsPathName, "ABCD" + DataAccess.BodyMatchFileExtension);
                var responseFile = Path.Combine(dal.DataPath, DataAccess.StumpsPathName, "ABCD.body" + DataAccess.BodyResponseFileExtension);

                Assert.False(File.Exists(stumpsFile));
                Assert.False(File.Exists(matchFile));
                Assert.False(File.Exists(responseFile));

            }
            finally {
                DeleteDataAccessLayer(dal);
            }

        }

        [Test]
        public void StumpFindAll_WithNoStumps_RetunsEmptyList() {

            var dal = CreateDataAccessLayerWithProxy();

            try {

                var stump = dal.StumpCreate(_sampleProxyServer.ProxyId, _sampleStump, _sampleBytes, _sampleBytes);
                var list = dal.StumpFindAll(_sampleProxyServer.ProxyId);

                Assert.IsNotNull(list);
                Assert.AreEqual(1, list.Count);

            }
            finally {
                DeleteDataAccessLayer(dal);
            }

        }

        [Test]
        public void StumpFindAll_WithStumps_RetunsPopulatedList() {

            var dal = CreateDataAccessLayerWithProxy();

            try {

                var list = dal.StumpFindAll(_sampleProxyServer.ProxyId);
                
                Assert.IsNotNull(list);
                Assert.AreEqual(0, list.Count);

            }
            finally {
                DeleteDataAccessLayer(dal);
            }

        }

        private static DataAccess CreateDataAccessLayer() {

            var temporaryDirectory = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            Directory.CreateDirectory(temporaryDirectory);

            var dataAccess = new DataAccess(temporaryDirectory);
            return dataAccess;

        }

        private static DataAccess CreateDataAccessLayerWithProxy() {

            var temporaryDirectory = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            Directory.CreateDirectory(temporaryDirectory);

            var dataAccess = new DataAccess(temporaryDirectory);

            var entity = new ProxyServerEntity {
                AutoStart = false,
                ExternalHostName = SampleHostName,
                Port = 500,
                ProxyId = "ABCD",
                UseSsl = false
            };

            dataAccess.ProxyServerCreate(entity);

            return dataAccess;

        }

        private static void DeleteDataAccessLayer(DataAccess dataAccess) {

            Directory.Delete(dataAccess.DataPath, true);

        }

    }

}
