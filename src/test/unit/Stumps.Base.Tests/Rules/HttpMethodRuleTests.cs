namespace Stumps.Rules
{
    using System;
    using System.Collections.Generic;
    using NUnit.Framework;

    [TestFixture]
    public class HttpMethodRuleTests
    {
        [Test]
        public void Constructor_Default_NotInitialized()
        {
            var rule = new HttpMethodRule();
            Assert.IsFalse(rule.IsInitialized);
        }

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
        public void GetRuleSettings_WhenCalled_ReturnsList()
        {
            var rule = new HttpMethodRule("a");
            var list = new List<RuleSetting>(rule.GetRuleSettings());
            Assert.AreEqual(1, list.Count);
        }

        [Test]
        public void InitializeFromSettings_WithNullSettings_ThrowsException()
        {
            var rule = new HttpMethodRule();

            Assert.That(
                () => rule.InitializeFromSettings(null),
                Throws.Exception.TypeOf<ArgumentNullException>().With.Property("ParamName").EqualTo("settings"));
        }

        [TestCase("", "")]
        [TestCase(null, "")]
        [TestCase("a", "a")]
        public void InitializeFromSettings_WithValidSettings_InitializesCorrectly(string httpMethod, string expectedHttpMethod)
        {
            var settings = new[]
            {
                new RuleSetting { Name = "httpmethod.value", Value = httpMethod }
            };

            var rule = new HttpMethodRule();
            rule.InitializeFromSettings(settings);

            Assert.IsTrue(rule.IsInitialized);
            Assert.AreEqual(expectedHttpMethod, rule.HttpMethodTextMatch);
        }

        [Test]
        public void IsMatch_WithNullRequest_ReturnsFalse()
        {
            var rule = new HttpMethodRule("POST");
            Assert.IsFalse(rule.IsMatch(null));
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

            /* test for case sensitivity */

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

            /* test for case sensitivity */

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

            /* test for case sensitivity */

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
