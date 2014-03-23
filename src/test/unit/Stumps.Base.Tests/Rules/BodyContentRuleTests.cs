namespace Stumps.Rules
{

    using System.Text;
    using NSubstitute;
    using NUnit.Framework;

    [TestFixture]
    public class BodyContentRuleTests
    {

        [Test]
        public void Constructor_ListOfStrings_Accepted()
        {

            Assert.DoesNotThrow(
                () => new BodyContentRule(
                          new string[]
                          {
                              "test1", "test2"
                          }));

        }

        [Test]
        public void Constructor_ValueIsNull_Accepted()
        {

            Assert.DoesNotThrow(() => new BodyContentRule(null));

        }

        [Test]
        public void IsMatch_NullContext_ReturnsFalse()
        {

            var rule = new BodyContentRule(
                new string[]
                {
                    "passed"
                });

            Assert.IsFalse(rule.IsMatch(null));

        }

        [Test]
        public void IsMatch_WithoutBody_ReturnsFalse()
        {

            var request = Substitute.For<IStumpsHttpRequest>();
            request.BodyLength.Returns(0);

            var rule = new BodyContentRule(
                new string[]
                {
                    "passed"
                });

            Assert.IsFalse(rule.IsMatch(null));

        }

        [Test]
        public void IsMatch_BinaryContentWithTextString_ReturnsFalse()
        {

            var request = CreateBinaryRequest();

            var rule = new BodyContentRule(
                new string[]
                {
                    "passed"
                });

            Assert.IsFalse(rule.IsMatch(request));

        }

        [Test]
        public void IsMatch_ContainsTextInversedWithMatchingText_ReturnsTrue()
        {

            var rule = new BodyContentRule(
                new string[]
                {
                    "not:failed"
                });

            var request = CreateTextRequest("passed");
            Assert.IsTrue(rule.IsMatch(request));

        }

        [Test]
        public void IsMatch_ContainsTextInversedWithNonMatchingText_ReturnsFalse()
        {

            var rule = new BodyContentRule(
                new string[]
                {
                    "not:passed"
                });

            var request = CreateTextRequest("passed");
            Assert.IsFalse(rule.IsMatch(request));

        }

        [Test]
        public void IsMatch_ContainsTextWithMatchingString_ReturnsTrue()
        {

            var rule = new BodyContentRule(
                new string[]
                {
                    "passed", "AAA"
                });

            var request = CreateTextRequest("passed");
            Assert.IsTrue(rule.IsMatch(request));

        }

        [Test]
        public void IsMatch_ContainsTextWithNonMatchingString_ReturnsFalse()
        {

            var ruleCaseMatches = new BodyContentRule(
                new string[]
                {
                    "failed", "AAA"
                });

            var ruleCaseDoesNotMatch = new BodyContentRule(
                new string[]
                {
                    "PASSED", "AAA"
                });

            var request = CreateTextRequest("passed");

            Assert.IsFalse(ruleCaseMatches.IsMatch(request));

            // correct text, wrong case
            Assert.IsFalse(ruleCaseDoesNotMatch.IsMatch(request));

        }

        [Test]
        public void IsMatch_RegexTextInversedWithMatchingString_ReturnsFalse()
        {

            var rule = new BodyContentRule(
                new string[]
                {
                    "not:regex:AA.*ssed.*D"
                });

            var request = CreateTextRequest("passed");
            Assert.IsFalse(rule.IsMatch(request));

        }

        [Test]
        public void IsMatch_RegexTextInversedWithNonMatchingString_ReturnsTrue()
        {

            var rule = new BodyContentRule(
                new string[]
                {
                    "not:regex:AA.*ssed.*D"
                });

            var request = CreateTextRequest("failed");
            Assert.IsTrue(rule.IsMatch(request));

        }

        [Test]
        public void IsMatch_RegexTextWithMatchingString_ReturnsTrue()
        {

            var rule = new BodyContentRule(
                new string[]
                {
                    "regex:AA.*ssed.*D"
                });

            var request = CreateTextRequest("passed");
            Assert.IsTrue(rule.IsMatch(request));

        }

        [Test]
        public void IsMatch_RegexTextWithNonMatchingString_ReturnsFalse()
        {

            var rule = new BodyContentRule(
                new string[]
                {
                    "regex:AA.*ssed.*D"
                });

            var request = CreateTextRequest("failed");
            Assert.IsFalse(rule.IsMatch(request));

        }

        private IStumpsHttpRequest CreateBinaryRequest()
        {

            var buffer = new byte[]
            {
                200, 172, 203, 199, 166, 180, 7
            };

            var request = Substitute.For<IStumpsHttpRequest>();
            request.GetBody().Returns(buffer);
            request.BodyLength.Returns(buffer.Length);

            return request;

        }

        private IStumpsHttpRequest CreateTextRequest(string text)
        {

            var exampleString = "AAAAAABBBBBB" + text + "CCCCCCDDDDDD";
            var buffer = Encoding.UTF8.GetBytes(exampleString);

            var request = Substitute.For<IStumpsHttpRequest>();
            request.GetBody().Returns(buffer);
            request.BodyLength.Returns(buffer.Length);

            return request;

        }

    }

}