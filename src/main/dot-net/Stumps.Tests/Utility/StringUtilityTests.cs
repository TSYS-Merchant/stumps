namespace Stumps.Utility {

    using System;
    using System.Linq;
    using System.Text;
    using NUnit.Framework;

    [TestFixture]
    public class StringUtilityTests {

        [Test]
        public void IsText_BinaryValue_ReturnsFalse() {

            var buffer = new byte[100];
            var random = new Random();
            random.NextBytes(buffer);

            Assert.IsFalse(StringUtility.IsText(buffer));

        }

        [Test]
        public void IsText_EmptyValue_ReturnsFalse() {

            Assert.IsFalse(StringUtility.IsText(new byte[] { }));

        }

        [Test]
        public void IsText_MostlyTextValue_ReturnsTrue() {

            var text = string.Concat(Enumerable.Repeat("HelloWorld", 100));
            var buffer = Encoding.UTF8.GetBytes(text);
            var newBuffer = new byte[buffer.Length + 5];
            
            Buffer.BlockCopy(buffer, 0, newBuffer, 5, buffer.Length);
            
            newBuffer[0] = 198;
            newBuffer[1] = 199;
            newBuffer[2] = 200;
            newBuffer[3] = 201;
            newBuffer[4] = 202;

            Assert.IsTrue(StringUtility.IsText(newBuffer));

        }

        [Test]
        public void IsText_NullValue_ReturnsFalse() {

            Assert.IsFalse(StringUtility.IsText(null));

        }

        [Test]
        public void IsText_TextValue_ReturnsTrue() {

            var text = "for (int i = 0; i < 100; i++ ) { console.WriteLine(\"Hello {0}\", i); }" + Environment.NewLine;
            var buffer = Encoding.UTF8.GetBytes(text);
            Assert.IsTrue(StringUtility.IsText(buffer));

        }


    }

}
