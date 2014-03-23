namespace Stumps.Rules
{

    using NSubstitute;
    using NUnit.Framework;
    using Stumps.Http;

    [TestFixture]
    public class HeaderRuleTests
    {

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

        public IStumpsHttpRequest CreateWithHeaders(string headerName, string headerValue)
        {

            var request = Substitute.For<IStumpsHttpRequest>();
            request.Headers.Returns(new HeaderDictionary());
            request.Headers.AddOrUpdate(headerName, headerValue);

            return request;

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
        public void IsMatch_HeadersAreNull_ReturnsFalse()
        {

            var rule = new HeaderRule(string.Empty, string.Empty);

            var request = Substitute.For<IStumpsHttpRequest>();
            IHeaderDictionary dict = null;

            request.Headers.Returns(dict);

            Assert.IsFalse(rule.IsMatch(request));

        }

        [Test]
        public void IsMatch_RequestIsNull_ReturnsFalse()
        {

            var rule = new HeaderRule(string.Empty, string.Empty);
            Assert.IsFalse(rule.IsMatch(null));

        }

    }

}