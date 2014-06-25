namespace Stumps.Server.Data
{

    using System;
    using System.Collections.Generic;
    using System.IO;
    using NUnit.Framework;

    [TestFixture]
    public class DataAccessTests
    {

        private const string SampleHostName = "myserver.inc";

        private readonly byte[] _sampleBytes;
        private readonly ServerEntity _sampleProxyServer;
        private readonly StumpEntity _sampleStump;

        public DataAccessTests()
        {

            _sampleProxyServer = new ServerEntity
            {
                AutoStart = false,
                RemoteServerHostName = SampleHostName,
                Port = 500,
                ServerId = "ABCD",
                UseSsl = false
            };

            var request = new HttpRequestEntity()
            {
                HttpMethod = "GET",
                BodyFileName = null,
                Headers = new List<NameValuePairEntity>(),
                ProtocolVersion = "1.1",
                RawUrl = "/"
            };

            var response = new HttpResponseEntity()
            {
                BodyFileName = null,
                Headers = new List<NameValuePairEntity>()
                {
                    new NameValuePairEntity
                    {
                        Name = "Content-Type",
                        Value = "text/plain"
                    }
                },
                RedirectAddress = null,
                StatusCode = 200,
                StatusDescription = "OK"
            };

            _sampleStump = new StumpEntity
            {
                Request = request,
                Response = response,
                Rules = new List<RuleEntity>(),
                StumpCategory = "Uncategorized",
                StumpId = "ABCD",
                StumpName = "MyStump"
            };

            _sampleBytes = new byte[]
            {
                1, 2, 3
            };

        }

        [Test]
        public void Constructor_WithBadPath_ThrowsException()
        {

            Assert.That(
                () => new DataAccess(null),
                Throws.Exception.TypeOf<ArgumentNullException>().With.Property("ParamName").EqualTo("storagePath"));

        }

        [Test]
        public void ProxyServerCreate_WithNullEntity_ThrowsException()
        {

            var dal = CreateDataAccessLayer();

            try
            {

                Assert.That(
                    () => dal.ServerCreate(null),
                    Throws.Exception.TypeOf<ArgumentNullException>().With.Property("ParamName").EqualTo("server"));

            }
            finally
            {
                DeleteDataAccessLayer(dal);
            }

        }

        [Test]
        public void ProxyServerCreate_WithValidEntity_PersistsToDisk()
        {

            var dal = CreateDataAccessLayer();

            try
            {

                dal.ServerCreate(_sampleProxyServer);
                var proxyFile = Path.Combine(dal.StoragePath, _sampleProxyServer.ServerId + ".server");
                var proxyDirectory = Path.Combine(dal.StoragePath, _sampleProxyServer.ServerId);

                Assert.IsTrue(File.Exists(proxyFile));
                Assert.IsTrue(Directory.Exists(proxyDirectory));

            }
            finally
            {
                DeleteDataAccessLayer(dal);
            }

        }

        [Test]
        public void ProxyServerDelete_WithValidHostName_RemovesFromDisk()
        {

            var dal = CreateDataAccessLayer();

            try
            {

                dal.ServerCreate(_sampleProxyServer);
                dal.ServerDelete(_sampleProxyServer.ServerId);

                var proxyFile = Path.Combine(dal.StoragePath, _sampleProxyServer.ServerId + ".proxy");
                var proxyDirectory = Path.Combine(dal.StoragePath, _sampleProxyServer.ServerId);

                Assert.IsFalse(File.Exists(proxyFile));
                Assert.IsFalse(Directory.Exists(proxyDirectory));

            }
            finally
            {
                DeleteDataAccessLayer(dal);
            }

        }

        [Test]
        public void ProxyServerFindAll_WithNoData_ReturnsEmptyList()
        {

            var dal = CreateDataAccessLayer();

            try
            {

                var list = dal.ServerFindAll();
                Assert.IsNotNull(list);
                Assert.AreEqual(0, list.Count);

            }
            finally
            {
                DeleteDataAccessLayer(dal);
            }

        }

        [Test]
        public void ProxyServerFindAll_WithValidProxy_ReturnsList()
        {

            var dal = CreateDataAccessLayer();

            try
            {

                dal.ServerCreate(_sampleProxyServer);
                var list = dal.ServerFindAll();
                Assert.AreEqual(1, list.Count);

            }
            finally
            {
                DeleteDataAccessLayer(dal);
            }

        }

        [Test]
        public void StumpCreate_WithNullEntity_ThrowsException()
        {

            var dal = CreateDataAccessLayerWithProxy();

            try
            {

                Assert.That(
                    () => dal.StumpCreate(SampleHostName, null, null, null),
                    Throws.Exception.TypeOf<ArgumentNullException>().With.Property("ParamName").EqualTo("entity"));

            }
            finally
            {
                DeleteDataAccessLayer(dal);
            }

        }

        [Test]
        public void StumpCreate_WithValidStumpAndBody_PersistsToDisk()
        {

            var dal = CreateDataAccessLayerWithProxy();

            try
            {

                var stump = dal.StumpCreate(_sampleProxyServer.ServerId, _sampleStump, _sampleBytes, _sampleBytes);

                Assert.IsNotNull(stump);
                Assert.IsNotNull(stump.Request.BodyFileName);
                Assert.IsNotNull(stump.Response.BodyFileName);

                var stumpsFile = Path.Combine(
                    dal.StoragePath, 
                    _sampleProxyServer.ServerId, 
                    DataAccess.StumpsPathName,
                    "ABCD" + DataAccess.StumpFileExtension);
                var matchFile = Path.Combine(
                    dal.StoragePath, 
                    _sampleProxyServer.ServerId, 
                    DataAccess.StumpsPathName,
                    "ABCD" + DataAccess.BodyMatchFileExtension);
                var responseFile = Path.Combine(
                    dal.StoragePath, 
                    _sampleProxyServer.ServerId, 
                    DataAccess.StumpsPathName,
                    "ABCD" + DataAccess.BodyResponseFileExtension);

                Assert.IsTrue(File.Exists(stumpsFile));
                Assert.IsTrue(File.Exists(matchFile));
                Assert.IsTrue(File.Exists(responseFile));

            }
            finally
            {
                DeleteDataAccessLayer(dal);
            }

        }

        [Test]
        public void StumpCreate_WithValidStump_PersistsToDisk()
        {

            var dal = CreateDataAccessLayerWithProxy();

            try
            {

                var stump = dal.StumpCreate(_sampleProxyServer.ServerId, _sampleStump, null, null);

                Assert.IsNotNull(stump);
                Assert.IsTrue(string.IsNullOrWhiteSpace(stump.Request.BodyFileName));
                Assert.IsTrue(string.IsNullOrWhiteSpace(stump.Response.BodyFileName));

                var stumpsFile = Path.Combine(
                    dal.StoragePath, 
                    _sampleProxyServer.ServerId, 
                    DataAccess.StumpsPathName,
                    "ABCD" + DataAccess.StumpFileExtension);
                var matchFile = Path.Combine(
                    dal.StoragePath, 
                    _sampleProxyServer.ServerId, 
                    DataAccess.StumpsPathName,
                    "ABCD" + DataAccess.BodyMatchFileExtension);
                var responseFile = Path.Combine(
                    dal.StoragePath, 
                    _sampleProxyServer.ServerId, 
                    DataAccess.StumpsPathName,
                    "ABCD" + DataAccess.BodyResponseFileExtension);

                Assert.IsTrue(File.Exists(stumpsFile));
                Assert.IsFalse(File.Exists(matchFile));
                Assert.IsFalse(File.Exists(responseFile));

            }
            finally
            {
                DeleteDataAccessLayer(dal);
            }

        }

        [Test]
        public void StumpDelete_WithValidStump_DeletesFromDisk()
        {

            var dal = CreateDataAccessLayerWithProxy();

            try
            {

                var stump = dal.StumpCreate(_sampleProxyServer.ServerId, _sampleStump, _sampleBytes, _sampleBytes);
                dal.StumpDelete(_sampleProxyServer.ServerId, stump.StumpId);

                var stumpsFile = Path.Combine(
                    dal.StoragePath, DataAccess.StumpsPathName, "ABCD" + DataAccess.StumpFileExtension);
                var matchFile = Path.Combine(
                    dal.StoragePath, DataAccess.StumpsPathName, "ABCD" + DataAccess.BodyMatchFileExtension);
                var responseFile = Path.Combine(
                    dal.StoragePath, DataAccess.StumpsPathName, "ABCD.body" + DataAccess.BodyResponseFileExtension);

                Assert.False(File.Exists(stumpsFile));
                Assert.False(File.Exists(matchFile));
                Assert.False(File.Exists(responseFile));

            }
            finally
            {
                DeleteDataAccessLayer(dal);
            }

        }

        [Test]
        public void StumpFindAll_WithNoStumps_RetunsEmptyList()
        {

            var dal = CreateDataAccessLayerWithProxy();

            try
            {

                var stump = dal.StumpCreate(_sampleProxyServer.ServerId, _sampleStump, _sampleBytes, _sampleBytes);
                var list = dal.StumpFindAll(_sampleProxyServer.ServerId);

                Assert.IsNotNull(list);
                Assert.AreEqual(1, list.Count);

            }
            finally
            {
                DeleteDataAccessLayer(dal);
            }

        }

        [Test]
        public void StumpFindAll_WithStumps_RetunsPopulatedList()
        {

            var dal = CreateDataAccessLayerWithProxy();

            try
            {

                var list = dal.StumpFindAll(_sampleProxyServer.ServerId);

                Assert.IsNotNull(list);
                Assert.AreEqual(0, list.Count);

            }
            finally
            {
                DeleteDataAccessLayer(dal);
            }

        }

        private static DataAccess CreateDataAccessLayer()
        {

            var temporaryDirectory = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            Directory.CreateDirectory(temporaryDirectory);

            var dataAccess = new DataAccess(temporaryDirectory);
            return dataAccess;

        }

        private static DataAccess CreateDataAccessLayerWithProxy()
        {

            var temporaryDirectory = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            Directory.CreateDirectory(temporaryDirectory);

            var dataAccess = new DataAccess(temporaryDirectory);

            var entity = new ServerEntity
            {
                AutoStart = false,
                RemoteServerHostName = SampleHostName,
                Port = 500,
                ServerId = "ABCD",
                UseSsl = false
            };

            dataAccess.ServerCreate(entity);

            return dataAccess;

        }

        private static void DeleteDataAccessLayer(DataAccess dataAccess)
        {

            Directory.Delete(dataAccess.StoragePath, true);

        }

    }

}