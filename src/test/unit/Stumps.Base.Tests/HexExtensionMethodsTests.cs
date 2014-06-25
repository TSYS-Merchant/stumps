namespace Stumps
{

    using NUnit.Framework;

    [TestFixture]
    public class HexExtensionMethodsTests
    {

        [Test]
        public void ToHexString_WithNull_ReturnsNull()
        {

            byte[] sample = null;
            var result = sample.ToHexString();
            Assert.IsNull(result);

        }

        [Test]
        public void ToHexString_WithValidByteArray_ReturnsCorrectly()
        {

            var sample = new byte[] { 1, 2, 3, 4, 5 };
            var result = sample.ToHexString();
            Assert.AreEqual("0102030405", result);

        }

        [Test]
        public void ToByteArray_WithNull_ReturnsNull()
        {

            string sample = null;
            var result = sample.ToByteArray();
            Assert.IsNull(result);

        }

        [Test]
        public void ToByteArray_WithValidString_ReturnsCorrectly()
        {

            var sample = "0102030405";
            var expected = new byte[] { 1, 2, 3, 4, 5 };
            var result = sample.ToByteArray();
            CollectionAssert.AreEqual(expected, result);

        }

    }

}
