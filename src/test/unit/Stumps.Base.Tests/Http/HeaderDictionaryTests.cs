namespace Stumps.Http
{

    using System;
    using System.Globalization;
    using System.IO;
    using System.Net;
    using NSubstitute;
    using NUnit.Framework;

    [TestFixture]
    public class HeaderDictionaryTests
    {

        [Test]
        public void Constructor_WithDefaults_HasNoHeaders()
        {
            var dict = new HeaderDictionary();
            Assert.AreEqual(0, dict.Count);
        }

        [Test]
        public void Count_WhenItemsAdded_ReturnsRealNumber()
        {
            var dict = new HeaderDictionary();
            dict.AddOrUpdate("abc", "123");
            Assert.AreEqual(1, dict.Count);
        }

        [Test]
        public void HeaderNames_WithPopulatedItems_ReturnsCollection()
        {
            var dict = new HeaderDictionary();
            dict.AddOrUpdate("abc", "123");

            var headerNames = dict.HeaderNames;
            Assert.AreEqual(1, headerNames.Count);
        }

        [Test]
        public void Indexer_WithValidHeaderName_ReturnsValue()
        {
            var dict = new HeaderDictionary();
            dict.AddOrUpdate("abc", "123");
            Assert.AreEqual("123", dict["abc"]);
        }

        [Test]
        public void Indexer_WithValidHeaderNameDifferentCase_ReturnsValue()
        {
            var dict = new HeaderDictionary();
            dict.AddOrUpdate("abc", "123");
            Assert.AreEqual("123", dict["ABC"]);
        }
        
        [Test]
        public void Indexer_WithInvalidHeaderName_ReturnsNull()
        {
            var dict = new HeaderDictionary();
            dict.AddOrUpdate("abc", "123");
            Assert.IsNull(dict["NoHeader"]);
        }

        [Test]
        public void AddOrUpdate_WithNullName_DoesNothing()
        {
            var dict = new HeaderDictionary();

            Assert.DoesNotThrow(() => dict.AddOrUpdate(null, "1234"));

            Assert.AreEqual(0, dict.Count);
        }

        [Test]
        public void AddOrUpdate_WithNullValue_DoesNothing()
        {
            var dict = new HeaderDictionary();

            Assert.DoesNotThrow(() => dict.AddOrUpdate("abcd", null));

            Assert.AreEqual(0, dict.Count);
        }

        [Test]
        public void AddOrUpdate_WithNewValue_AddsToCollection()
        {
            var dict = new HeaderDictionary();
            dict.AddOrUpdate("abc", "123");
            Assert.AreEqual(1, dict.Count);
        }

        [Test]
        public void AddOrUpdate_WithSameName_UpdatesValue()
        {
            var dict = new HeaderDictionary();
            dict.AddOrUpdate("abc", "123");
            dict.AddOrUpdate("abc", "456");
            Assert.AreEqual("456", dict["abc"]);
        }

        [Test]
        public void AddOrUpdate_WithSameNameDifferentCase_UpdatesValue()
        {
            var dict = new HeaderDictionary();
            dict.AddOrUpdate("abc", "123");
            dict.AddOrUpdate("ABC", "456");
            Assert.AreEqual("456", dict["abc"]);
        }

        [Test]
        public void Clear_WithPopulatedList_RemoveAllItems()
        {
            var dict = new HeaderDictionary();
            dict.AddOrUpdate("abc", "123");
            dict.Clear();
            Assert.AreEqual(0, dict.Count);
        }

        [Test]
        public void Remove_WithValidName_ReturnsTrue()
        {
            var dict = new HeaderDictionary();
            dict.AddOrUpdate("abc", "123");
            Assert.IsTrue(dict.Remove("abc"));
        }

        [Test]
        public void Remove_WithValidName_RemoveFromList()
        {
            var dict = new HeaderDictionary();
            dict.AddOrUpdate("abc", "123");
            dict.Remove("abc");
            Assert.AreEqual(0, dict.Count);
        }

        [Test]
        public void Remove_WithValidNameDifferentCase_ReturnsTrue()
        {
            var dict = new HeaderDictionary();
            dict.AddOrUpdate("abc", "123");
            Assert.IsTrue(dict.Remove("ABC"));
        }

        public void Remove_WithInvalidName_ReturnsFalse()
        {
            var dict = new HeaderDictionary();
            dict.AddOrUpdate("abc", "123");
            Assert.IsFalse(dict.Remove("defg"));
        }

    }

}
