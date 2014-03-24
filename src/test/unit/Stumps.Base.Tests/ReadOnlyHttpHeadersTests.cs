namespace Stumps
{

    using System;
    using NUnit.Framework;

    [TestFixture]
    public class ReadOnlyHttpHeadersTests
    {

        [Test]
        public void Constructor_WithDefaults_HasNoHeaders()
        {
            var headers = new ReadOnlyHttpHeaders();
            Assert.AreEqual(0, headers.Count);
        }

        [Test]
        public void AddOrUpdateInternal_WithValidValues_UpdatesCollection()
        {
            var headers = new ReadOnlyHttpHeaders();
            headers.AddOrUpdateInternal("abc", "123");
            Assert.AreEqual(1, headers.Count);
        }

        [Test]
        public void AddOrUpdateInternal_WithNullHeaderName_DoesNothing()
        {
            var headers = new ReadOnlyHttpHeaders();
            Assert.DoesNotThrow(() => headers.AddOrUpdateInternal(null, "123"));
            Assert.AreEqual(0, headers.Count);
        }

        [Test]
        public void AddOrUpdateInternal_WithNullHeaderValue_DoesNothing()
        {
            var headers = new ReadOnlyHttpHeaders();
            Assert.DoesNotThrow(() => headers.AddOrUpdateInternal("abc", null));
            Assert.AreEqual(0, headers.Count);
        }


        [Test]
        public void IndexerGet_WithValidHeaderName_ReturnsValue()
        {
            var headers = new ReadOnlyHttpHeaders();
            headers.AddOrUpdateInternal("abc", "123");
            Assert.AreEqual("123", headers["abc"]);
        }

        [Test]
        public void IndexerGet_WithValidHeaderNameDifferentCase_ReturnsValue()
        {
            var headers = new ReadOnlyHttpHeaders();
            headers.AddOrUpdateInternal("abc", "123");
            Assert.AreEqual("123", headers["ABC"]);
        }

        [Test]
        public void IndexerGet_WithInvalidHeaderName_ReturnsNull()
        {
            var headers = new ReadOnlyHttpHeaders();
            headers.AddOrUpdateInternal("abc", "123");
            Assert.IsNull(headers["NoHeader"]);
        }

        [Test]
        public void IndexerSet_WithValue_ThrowsException()
        {
            var headers = new ReadOnlyHttpHeaders();
            Assert.That(() => headers["abc"] = "123", Throws.Exception.TypeOf<NotSupportedException>());
        }


        [Test]
        public void IsReadOnly_WithGet_ReturnsTrue()
        {
            var headers = new ReadOnlyHttpHeaders();
            Assert.IsTrue(headers.IsReadOnly);
        }

        [Test]
        public void Clear_WhenCalled_ThrowsException()
        {
            Assert.That(() => new ReadOnlyHttpHeaders().Clear(), Throws.Exception.TypeOf<NotSupportedException>());
        }

        [Test]
        public void ClearInternal_WithPopulatedList_RemoveAllItems()
        {
            var headers = new ReadOnlyHttpHeaders();
            headers.AddOrUpdateInternal("abc", "123");
            headers.ClearInternal();
            Assert.AreEqual(0, headers.Count);
        }

        [Test]
        public void Remove_WhenCalled_ThrowsException()
        {
            Assert.That(() => new ReadOnlyHttpHeaders().Remove("abcd"), Throws.Exception.TypeOf<NotSupportedException>());
        }

        [Test]
        public void RemoveInternal_WithValidName_ReturnsTrue()
        {
            var headers = new ReadOnlyHttpHeaders();
            headers.AddOrUpdateInternal("abc", "123");
            Assert.IsTrue(headers.RemoveInternal("abc"));
        }

        [Test]
        public void RemoveInternal_WithValidName_RemoveFromList()
        {
            var headers = new ReadOnlyHttpHeaders();
            headers.AddOrUpdateInternal("abc", "123");
            headers.RemoveInternal("abc");
            Assert.AreEqual(0, headers.Count);
        }

    }

}
