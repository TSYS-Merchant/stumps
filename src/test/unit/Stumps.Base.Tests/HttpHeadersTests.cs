namespace Stumps
{
    using System;
    using NUnit.Framework;

    [TestFixture]
    public class HttpHeaderTests
    {
        [Test]
        public void Constructor_WithDefaults_HasNoHeaders()
        {
            var headers = new HttpHeaders();
            Assert.AreEqual(0, headers.Count);
        }

        [Test]
        public void Count_WhenItemsAdded_ReturnsRealNumber()
        {
            var headers = new HttpHeaders();
            headers["abc"] = "123";
            Assert.AreEqual(1, headers.Count);
        }

        [Test]
        public void HeaderNames_WithPopulatedItems_ReturnsCollection()
        {
            var headers = new HttpHeaders();
            headers["abc"] = "123";

            var headerNames = headers.HeaderNames;
            Assert.AreEqual(1, headerNames.Count);
        }

        [Test]
        public void IndexerGet_WithValidHeaderName_ReturnsValue()
        {
            var headers = new HttpHeaders();
            headers["abc"] = "123";
            Assert.AreEqual("123", headers["abc"]);
        }

        [Test]
        public void IndexerGet_WithValidHeaderNameDifferentCase_ReturnsValue()
        {
            var headers = new HttpHeaders();
            headers["abc"] = "123";
            Assert.AreEqual("123", headers["ABC"]);
        }
        
        [Test]
        public void IndexerGet_WithInvalidHeaderName_ReturnsNull()
        {
            var headers = new HttpHeaders();
            headers["abc"] = "123";
            Assert.IsNull(headers["NoHeader"]);
        }

        [Test]
        public void IndexerSet_WithNullName_DoesNothing()
        {
            var headers = new HttpHeaders();

            Assert.DoesNotThrow(() => headers[null] = "1234");

            Assert.AreEqual(0, headers.Count);
        }

        [Test]
        public void IndexerSet_WithNullValue_DoesNothing()
        {
            var headers = new HttpHeaders();

            Assert.DoesNotThrow(() => headers["abcd"] = null);

            Assert.AreEqual(0, headers.Count);
        }

        [Test]
        public void IndexerSet_WithNewValue_AddsToCollection()
        {
            var headers = new HttpHeaders();
            headers["abc"] = "123";
            Assert.AreEqual(1, headers.Count);
        }

        [Test]
        public void IndexerSet_WithSameName_UpdatesValue()
        {
            var headers = new HttpHeaders();
            headers["abc"] = "123";
            headers["abc"] = "456";
            Assert.AreEqual("456", headers["abc"]);
        }

        [Test]
        public void IndexerSet_WithSameNameDifferentCase_UpdatesValue()
        {
            var headers = new HttpHeaders();
            headers["abc"] = "123";
            headers["ABC"] = "456";
            Assert.AreEqual("456", headers["abc"]);
        }

        [Test]
        public void IsReadOnly_WithGet_ReturnsFalse()
        {
            var headers = new HttpHeaders();
            Assert.IsFalse(headers.IsReadOnly);
        }

        [Test]
        public void Clear_WithPopulatedList_RemoveAllItems()
        {
            var headers = new HttpHeaders();
            headers["abc"] = "123";
            headers.Clear();
            Assert.AreEqual(0, headers.Count);
        }

        [Test]
        public void CopyTo_WithNullHeaders_ThrowsException()
        {
            Assert.That(
                () => new HttpHeaders().CopyTo(null),
                Throws.Exception.TypeOf<ArgumentNullException>().With.Property("ParamName").EqualTo("httpHeaders"));
        }

        [Test]
        public void CopyTo_WithValidHeaders_CopiesAllHeaders()
        {
            var source = new HttpHeaders();
            var target = new HttpHeaders();

            source["abc"] = "123";
            source["def"] = "456";

            source.CopyTo(target);

            Assert.AreEqual(source.Count, target.Count);
        }

        [Test]
        public void CopyTo_WithValidHeaders_UpdatesExistingHeaders()
        {
            var source = new HttpHeaders();
            var target = new HttpHeaders();

            source["abc"] = "123";
            source["def"] = "456";
            target["abc"] = "999";

            source.CopyTo(target);

            Assert.AreEqual(source.Count, target.Count);
            Assert.AreEqual(source["abc"], target["abc"]);
        }

        [Test]
        public void Remove_WithValidName_ReturnsTrue()
        {
            var headers = new HttpHeaders();
            headers["abc"] = "123";
            Assert.IsTrue(headers.Remove("abc"));
        }

        [Test]
        public void Remove_WithValidName_RemoveFromList()
        {
            var headers = new HttpHeaders();
            headers["abc"] = "123";
            headers.Remove("abc");
            Assert.AreEqual(0, headers.Count);
        }

        [Test]
        public void Remove_WithValidNameDifferentCase_ReturnsTrue()
        {
            var headers = new HttpHeaders();
            headers["abc"] = "123";
            Assert.IsTrue(headers.Remove("ABC"));
        }

        [Test]
        public void Remove_WithInvalidName_ReturnsFalse()
        {
            var headers = new HttpHeaders();
            headers["abc"] = "123";
            Assert.IsFalse(headers.Remove("defg"));
        }
    }
}
