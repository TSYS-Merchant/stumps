namespace Stumps.Proxy {

    using System.IO;
    using System.Text;
    using NUnit.Framework;

    [TestFixture]
    public class BodyContentRuleTests {

        [Test]
        public void Constructor_ValueIsNull_Accepted() {

            Assert.DoesNotThrow(() => new BodyContentRule(null));

        }

        [Test]
        public void Constructor_ListOfStrings_Accepted() {

            Assert.DoesNotThrow(() => new BodyContentRule(new string[] { "test1", "test2" }));

        }

        [Test]
        public void IsMatch_BinaryContentWithTextString_ReturnsFalse() {

            using ( var request = CreateBinaryRequest() ) {

                var rule = new BodyContentRule(new string[] { "passed" });
                Assert.IsFalse(rule.IsMatch(request));

            }

        }

        [Test]
        public void IsMatch_ContainsTextWithMatchingString_ReturnsTrue() {

            var rule = new BodyContentRule(new string[] {
                "passed",
                "AAA"
            });

            using ( var request = CreateTextRequest("passed") ) {
                Assert.IsTrue(rule.IsMatch(request));
            }

        }

        [Test]
        public void IsMatch_ContainsTextWithNonMatchingString_ReturnsFalse() {

            var ruleCaseMatches = new BodyContentRule(new string[] {
                "failed",
                "AAA"
            });

            var ruleCaseDoesNotMatch = new BodyContentRule(new string[] {
                "PASSED",
                "AAA"
            });

            using ( var request = CreateTextRequest("passed") ) {

                Assert.IsFalse(ruleCaseMatches.IsMatch(request));

                // correct text, wrong case
                Assert.IsFalse(ruleCaseDoesNotMatch.IsMatch(request));

            }

        }

        [Test]
        public void IsMatch_ContainsTextInversedWithMatchingText_ReturnsTrue() {

            var rule = new BodyContentRule(new string[] {
                "not:failed"
            });

            using ( var request = CreateTextRequest("passed") ) {

                Assert.IsTrue(rule.IsMatch(request));

            }

        }

        [Test]
        public void IsMatch_ContainsTextInversedWithNonMatchingText_ReturnsFalse() {

            var rule = new BodyContentRule(new string[] {
                "not:passed"
            });

            using ( var request = CreateTextRequest("passed") ) {

                Assert.IsFalse(rule.IsMatch(request));

            }

        }

        [Test]
        public void IsMatch_RegexTextInversedWithMatchingString_ReturnsFalse() {

            var rule = new BodyContentRule(new string[] {
                "not:regex:AA.*ssed.*D"
            });

            using ( var request = CreateTextRequest("passed") ) {

                Assert.IsFalse(rule.IsMatch(request));

            }

        }

        [Test]
        public void IsMatch_RegexTextInversedWithNonMatchingString_ReturnsTrue() {

            var rule = new BodyContentRule(new string[] {
                "not:regex:AA.*ssed.*D"
            });

            using ( var request = CreateTextRequest("failed") ) {

                Assert.IsTrue(rule.IsMatch(request));

            }

        }
        
        [Test]
        public void IsMatch_RegexTextWithMatchingString_ReturnsTrue() {

            var rule = new BodyContentRule(new string[] {
                "regex:AA.*ssed.*D"
            });

            using ( var request = CreateTextRequest("passed") ) {

                Assert.IsTrue(rule.IsMatch(request));

            }

        }

        [Test]
        public void IsMatch_RegexTextWithNonMatchingString_ReturnsFalse() {

            var rule = new BodyContentRule(new string[] {
                "regex:AA.*ssed.*D"
            });

            using ( var request = CreateTextRequest("failed") ) {

                Assert.IsFalse(rule.IsMatch(request));

            }

        }

        private MockHttpRequest CreateBinaryRequest() {

            var buffer = new byte[] { 200, 172, 203, 199, 166, 180, 7 };
            var stream = new MemoryStream(buffer);
            var request = new MockHttpRequest {
                InputStream = stream
            };

            return request;

        }

        private MockHttpRequest CreateTextRequest(string text) {

            var myString = "AAAAAABBBBBB" + text + "CCCCCCDDDDDD";
            var buffer = Encoding.UTF8.GetBytes(myString);
            var stream = new MemoryStream(buffer);
            var request = new MockHttpRequest {
                InputStream = stream
            };

            return request;

        }

    }

}
