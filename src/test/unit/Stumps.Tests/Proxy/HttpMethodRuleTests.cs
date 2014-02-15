namespace Stumps.Proxy
{

    using NUnit.Framework;

    [TestFixture]
    public class HttpMethodRuleTests
    {

        [Test]
        public void Constuctor_ValueIsEmptyString_Accepted()
        {

            Assert.DoesNotThrow(() => new HttpMethodRule(string.Empty));

        }

        [Test]
        public void Constuctor_ValueIsNull_Accepted()
        {

            Assert.DoesNotThrow(() => new HttpMethodRule(null));

        }

        [Test]
        public void IsMatch_ExactTextRuleInversedWithMatchingString_ReturnsFalse()
        {

            var request = new MockHttpRequest
            {
                HttpMethod = "POST"
            };

            var rule = new HttpMethodRule("not:POST");
            Assert.IsFalse(rule.IsMatch(request));

        }

        [Test]
        public void IsMatch_ExactTextRuleInversedWithNonMatchingString_ReturnsTrue()
        {

            var request = new MockHttpRequest
            {
                HttpMethod = "GET"
            };

            var rule = new HttpMethodRule("not:POST");
            Assert.IsTrue(rule.IsMatch(request));

        }

        [Test]
        public void IsMatch_ExactTextRuleWithMatchingString_ReturnsTrue()
        {

            var request = new MockHttpRequest
            {
                HttpMethod = "GET"
            };

            var rule = new HttpMethodRule("GET");
            Assert.IsTrue(rule.IsMatch(request));

            // test for case sensitivity
            request.HttpMethod = "get";
            Assert.IsTrue(rule.IsMatch(request));

        }

        [Test]
        public void IsMatch_ExactTextRuleWithNonMatchingString_ReturnsFalse()
        {

            var request = new MockHttpRequest
            {
                HttpMethod = "POST"
            };

            var rule = new HttpMethodRule("GET");
            Assert.IsFalse(rule.IsMatch(request));

        }

        [Test]
        public void IsMatch_NullValue_ReturnsFalse()
        {

            var request = new MockHttpRequest
            {
                HttpMethod = null
            };

            var rule = new HttpMethodRule(null);
            Assert.IsFalse(rule.IsMatch(request));

            rule = new HttpMethodRule("GET");
            Assert.IsFalse(rule.IsMatch(request));

        }

        [Test]
        public void IsMatch_RegexRuleInversedWithMatchingString_ReturnsFalse()
        {

            var request = new MockHttpRequest
            {
                HttpMethod = "GET"
            };

            var rule = new HttpMethodRule("not:regex:(get|put)");
            Assert.IsFalse(rule.IsMatch(request));

            // test for case sensitivity

            request = new MockHttpRequest
            {
                HttpMethod = "get"
            };
            Assert.IsFalse(rule.IsMatch(request));

        }

        [Test]
        public void IsMatch_RegexRuleInversedWithNonMatchingString_ReturnsTrue()
        {

            var request = new MockHttpRequest
            {
                HttpMethod = "POST"
            };

            var rule = new HttpMethodRule("not:regex:(get|put)");
            Assert.IsTrue(rule.IsMatch(request));

            // test for case sensitivity

            request = new MockHttpRequest
            {
                HttpMethod = "post"
            };

            Assert.IsTrue(rule.IsMatch(request));

        }

        [Test]
        public void IsMatch_RegexRuleWithMatchingString_ReturnsTrue()
        {

            var request = new MockHttpRequest
            {
                HttpMethod = "GET"
            };

            var rule = new HttpMethodRule("regex:(get|put)");
            Assert.IsTrue(rule.IsMatch(request));

            // test for case sensitivity

            request = new MockHttpRequest
            {
                HttpMethod = "get"
            };

            Assert.IsTrue(rule.IsMatch(request));

        }

        [Test]
        public void IsMatch_RegexRuleWithNonMatchingString_ReturnsFalse()
        {

            var request = new MockHttpRequest
            {
                HttpMethod = "POST"
            };

            var rule = new HttpMethodRule("regex:(get|put)");
            Assert.IsFalse(rule.IsMatch(request));

        }

    }

}