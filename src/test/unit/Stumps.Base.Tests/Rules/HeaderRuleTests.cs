namespace Stumps.Rules
{
    using System;
    using System.Collections.Generic;
    using NSubstitute;
    using NUnit.Framework;

    [TestFixture]
    public class HeaderRuleTests
    {
        [Test]
        public void Constructor_Default_NotInitialized()
        {
            var rule = new HeaderRule();
            Assert.IsFalse(rule.IsInitialized);
        }

        [Test]
        public void Constructor_ValuesAreNull_Accepted()
        {
            Assert.DoesNotThrow(() => new HeaderRule(null, null));
        }

        [Test]
        public void Constructor_ValuesIsEmptyString_Accepted()
        {
            Assert.DoesNotThrow(() => new HeaderRule(string.Empty, string.Empty));
        }

        [Test]
        public void GetRuleSettings_WhenCalled_ReturnsList()
        {
            var rule = new HeaderRule("a", "b");
            var list = new List<RuleSetting>(rule.GetRuleSettings());
            Assert.AreEqual(2, list.Count);
        }

        [Test]
        public void InitializeFromSettings_WithNullSettings_ThrowsException()
        {
            var rule = new HeaderRule();

            Assert.That(
                () => rule.InitializeFromSettings(null),
                Throws.Exception.TypeOf<ArgumentNullException>().With.Property("ParamName").EqualTo("settings"));
        }

        [TestCase("a", "b", "a", "b")]
        [TestCase("", "", "", "")]
        [TestCase(null, null, "", "")]
        [TestCase("a", null, "a", "")]
        [TestCase(null, "b", "", "b")]
        public void InitializeFromSettings_WithValidSettings_InitializesCorrectly(string headerName, string headerValue, string expectedName, string expectedValue)
        {
            var settings = new[]
            {
                new RuleSetting { Name = "header.name", Value = headerName },
                new RuleSetting { Name = "header.value", Value = headerValue }
            };

            var rule = new HeaderRule();
            rule.InitializeFromSettings(settings);

            Assert.IsTrue(rule.IsInitialized);
            Assert.AreEqual(expectedName, rule.HeaderNameTextMatch);
            Assert.AreEqual(expectedValue, rule.HeaderValueTextMatch);
        }

        [TestCase("headername", "headervalue", true)]
        [TestCase("HEADERNAME", "headervalue", true)]
        [TestCase("headername", "HEADERVALUE", true)]
        [TestCase("HEADERNAME", "HEADERVALUE", true)]
        [TestCase("headername", "headervaluez", false)]
        [TestCase("headernamez", "headervalue", false)]
        [TestCase("", "", false)]
        [TestCase(null, null, false)]
        public void IsMatch_WithExactNameAndExactValueRule_ReturnsExpected(
            string headerName, string headerValue, bool expectedResult)
        {
            var rule = new HeaderRule("headername", "headervalue");

            var request = CreateWithHeaders(headerName, headerValue);
            Assert.AreEqual(expectedResult, rule.IsMatch(request));
        }

        [TestCase("headername", "headervalue", true)]
        [TestCase("x-headername", "headervalue", true)]
        [TestCase("somename", "headervalue", false)]
        public void IsMatch_WithRegexNameAndExactValue_ReturnsExpected(
            string headerName, string headerValue, bool expectedResult)
        {
            var rule = new HeaderRule("regex:he.*me", "headervalue");

            var request = CreateWithHeaders(headerName, headerValue);
            Assert.AreEqual(expectedResult, rule.IsMatch(request));
        }

        [TestCase("headername", "headervalue", true)]
        [TestCase("headername", "x-headervalue", true)]
        [TestCase("headername", "somevalue", false)]
        public void IsMatch_WithExactNameAndRegexValue_ReturnsExpected(
            string headerName, string headerValue, bool expectedResult)
        {
            var rule = new HeaderRule("headername", "regex:he.*ue");

            var request = CreateWithHeaders(headerName, headerValue);
            Assert.AreEqual(expectedResult, rule.IsMatch(request));
        }

        [TestCase("headername", "headervalue", false)]
        [TestCase("x-headername", "headervalue", false)]
        [TestCase("somename", "headervalue", true)]
        public void IsMatch_WithRegexNameInversedAndExactValue_ReturnsExpected(
            string headerName, string headerValue, bool expectedResult)
        {
            var rule = new HeaderRule("not:regex:he.*me", "headervalue");

            var request = CreateWithHeaders(headerName, headerValue);
            Assert.AreEqual(expectedResult, rule.IsMatch(request));
        }

        [TestCase("headername", "headervalue", false)]
        [TestCase("headername", "x-headervalue", false)]
        [TestCase("headername", "somevalue", true)]
        public void IsMatch_WithExactNameAndRegexValueInversed_ReturnsExpected(
            string headerName, string headerValue, bool expectedResult)
        {
            var rule = new HeaderRule("headername", "not:regex:he.*ue");

            var request = CreateWithHeaders(headerName, headerValue);
            Assert.AreEqual(expectedResult, rule.IsMatch(request));
        }

        [TestCase("headername", "headervalue", true)]
        [TestCase("x-headername", "headervalue", true)]
        [TestCase("someheader", "headervalue", true)]
        public void IsMatch_WithRegexAnyNameAndExactValue_ReturnsExpected(
            string headerName, string headerValue, bool expectedResult)
        {
            var rule = new HeaderRule("regex:.*", "headervalue");

            var request = CreateWithHeaders(headerName, headerValue);
            Assert.AreEqual(expectedResult, rule.IsMatch(request));
        }

        [TestCase("headername", "headervalue", true)]
        [TestCase("headername", "x-headervalue", true)]
        [TestCase("headername", "somevalue", true)]
        [TestCase("headername", "", true)]
        public void IsMatch_WithExactNameAndRegexAnyValue_ReturnsExpected(
            string headerName, string headerValue, bool expectedResult)
        {
            var rule = new HeaderRule("headername", "regex:.*");

            var request = CreateWithHeaders(headerName, headerValue);
            Assert.AreEqual(expectedResult, rule.IsMatch(request));
        }

        [TestCase("headername", "headervalue", false)]
        [TestCase("x-whatever", "headervalue", true)]
        public void IsMatch_WithExactNameInversedAndExactValueRule_ReturnsExpected(
            string headerName, string headerValue, bool expectedResult)
        {
            var rule = new HeaderRule("not:headername", "headervalue");

            var request = CreateWithHeaders(headerName, headerValue);
            Assert.AreEqual(expectedResult, rule.IsMatch(request));
        }

        [TestCase("headername", "headervalue", false)]
        [TestCase("headername", "x-headervalue", true)]
        public void IsMatch_WithExactNameAndExactValueInversedRule_ReturnsExpected(
            string headerName, string headerValue, bool expectedResult)
        {
            var rule = new HeaderRule("headername", "not:headervalue");

            var request = CreateWithHeaders(headerName, headerValue);
            Assert.AreEqual(expectedResult, rule.IsMatch(request));
        }
        
        [Test]
        public void IsMatch_HeadersAreNull_ReturnsFalse()
        {
            var rule = new HeaderRule(string.Empty, string.Empty);

            var request = Substitute.For<IStumpsHttpRequest>();
            IHttpHeaders dict = null;

            request.Headers.Returns(dict);

            Assert.IsFalse(rule.IsMatch(request));
        }

        [Test]
        public void IsMatch_RequestIsNull_ReturnsFalse()
        {
            var rule = new HeaderRule(string.Empty, string.Empty);
            Assert.IsFalse(rule.IsMatch(null));
        }

        public IStumpsHttpRequest CreateWithHeaders(string headerName, string headerValue)
        {
            var request = Substitute.For<IStumpsHttpRequest>();
            request.Headers.Returns(new HttpHeaders());
            request.Headers[headerName] = headerValue;

            return request;
        }
    }
}
