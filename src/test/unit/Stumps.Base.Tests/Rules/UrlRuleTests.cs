namespace Stumps.Rules
{
    using System;
    using System.Collections.Generic;
    using NUnit.Framework;

    [TestFixture]
    public class UrlRuleTests
    {
        [Test]
        public void Constructor_Default_NotInitialized()
        {
            var rule = new UrlRule();
            Assert.IsFalse(rule.IsInitialized);
        }

        [Test]
        public void Constuctor_ValueIsEmptyString_Accepted()
        {
            Assert.DoesNotThrow(() => new UrlRule(string.Empty));
        }

        [Test]
        public void Constuctor_ValueIsNull_Accepted()
        {
            Assert.DoesNotThrow(() => new UrlRule(null));
        }

        [Test]
        public void GetRuleSettings_WhenCalled_ReturnsList()
        {
            var rule = new UrlRule("a");
            var list = new List<RuleSetting>(rule.GetRuleSettings());
            Assert.AreEqual(1, list.Count);
        }

        [Test]
        public void InitializeFromSettings_WithNullSettings_ThrowsException()
        {
            var rule = new UrlRule();

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
                new RuleSetting { Name = "url.value", Value = httpMethod }
            };

            var rule = new UrlRule();
            rule.InitializeFromSettings(settings);

            Assert.IsTrue(rule.IsInitialized);
            Assert.AreEqual(expectedHttpMethod, rule.UrlTextMatch);
        }

        [Test]
        public void IsMatch_WithNullRequest_ReturnsFalse()
        {
            var rule = new UrlRule("/Something");
            Assert.IsFalse(rule.IsMatch(null));
        }

        [Test]
        public void IsMatch_ExactTextRuleInversedWithMatchingString_ReturnsFalse()
        {
            var request = new MockHttpRequest
            {
                RawUrl = "/failed/"
            };

            var rule = new UrlRule("not:/failed/");
            Assert.IsFalse(rule.IsMatch(request));
        }

        [Test]
        public void IsMatch_ExactTextRuleInversedWithNonMatchingString_ReturnsTrue()
        {
            var request = new MockHttpRequest
            {
                RawUrl = "/passed/"
            };

            var rule = new UrlRule("not:/failed/");
            Assert.IsTrue(rule.IsMatch(request));
        }

        [Test]
        public void IsMatch_ExactTextRuleWithMatchingString_ReturnsTrue()
        {
            var request = new MockHttpRequest
            {
                RawUrl = "/passed/"
            };

            var rule = new UrlRule("/passed/");
            Assert.IsTrue(rule.IsMatch(request));

            // test for case sensitivity
            request.RawUrl = "/PASSED/";
            Assert.IsTrue(rule.IsMatch(request));
        }

        [Test]
        public void IsMatch_ExactTextRuleWithNonMatchingString_ReturnsFalse()
        {
            var request = new MockHttpRequest
            {
                RawUrl = "/failed/"
            };

            var rule = new UrlRule("/passed/");
            Assert.IsFalse(rule.IsMatch(request));
        }

        [Test]
        public void IsMatch_NullValue_ReturnsFalse()
        {
            var request = new MockHttpRequest
            {
                RawUrl = null
            };

            var rule = new UrlRule(null);
            Assert.IsFalse(rule.IsMatch(request));

            rule = new UrlRule("/test");
            Assert.IsFalse(rule.IsMatch(request));
        }

        [Test]
        public void IsMatch_RegexRuleInversedWithMatchingStringAgainst_ReturnsFalse()
        {
            var request = new MockHttpRequest
            {
                RawUrl = "/passed/"
            };

            var rule = new UrlRule("not:regex:as*ed");
            Assert.IsFalse(rule.IsMatch(request));

            /* test for case sensitivity */

            request = new MockHttpRequest
            {
                RawUrl = "/PASSED/"
            };
            Assert.IsFalse(rule.IsMatch(request));
        }

        [Test]
        public void IsMatch_RegexRuleInversedWithNonMatchingString_ReturnsTrue()
        {
            var request = new MockHttpRequest
            {
                RawUrl = "/failed/"
            };

            var rule = new UrlRule("not:regex:as*ed");
            Assert.IsTrue(rule.IsMatch(request));

            /* test for case sensitivity */

            request = new MockHttpRequest
            {
                RawUrl = "/FAILED/"
            };

            Assert.IsTrue(rule.IsMatch(request));
        }

        [Test]
        public void IsMatch_RegexRuleWithMatchingString_ReturnsTrue()
        {
            var request = new MockHttpRequest
            {
                RawUrl = "/passed/"
            };

            var rule = new UrlRule("regex:as*ed");
            Assert.IsTrue(rule.IsMatch(request));

            /* test for case sensitivity */

            request = new MockHttpRequest
            {
                RawUrl = "/PASSED/"
            };

            Assert.IsTrue(rule.IsMatch(request));
        }

        [Test]
        public void IsMatch_RegexRuleWithNonMatchingString_ReturnsFalse()
        {
            var request = new MockHttpRequest
            {
                RawUrl = "/failed/"
            };

            var rule = new UrlRule("regex:as*ed");
            Assert.IsFalse(rule.IsMatch(request));
        }
    }
}
