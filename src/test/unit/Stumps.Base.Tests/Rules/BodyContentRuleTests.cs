namespace Stumps.Rules
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using NSubstitute;
    using NUnit.Framework;

    [TestFixture]
    public class BodyContentRuleTests
    {
        [Test]
        public void Constructor_Default_NotInitialized()
        {
            var rule = new BodyContentRule();
            Assert.IsFalse(rule.IsInitialized);
        }

        [Test]
        public void Constructor_ListOfStrings_Initialized()
        {
            var rule = new BodyContentRule(new string[] { "Test1", "Test2" });
            Assert.True(rule.IsInitialized);
        }

        [Test]
        public void Constructor_ValueIsNull_Accepted()
        {
            Assert.DoesNotThrow(() => new BodyContentRule(null));
        }

        [Test]
        public void GetRuleSettings_WhenCalled_ReturnsList()
        {
            var rule = new BodyContentRule(new string[] { "Hello" });
            var list = new List<RuleSetting>(rule.GetRuleSettings());
            Assert.AreEqual(1, list.Count);
        }

        [Test]
        public void InitializeFromSettings_WhenInitialized_ThrowsException()
        {
            var rule = new BodyContentRule(new string[] { "ABCD" });
            var settings = new[] { new RuleSetting { Name = "text.evaluation", Value = "passed" } };

            Assert.That(
                () => rule.InitializeFromSettings(settings),
                Throws.Exception.TypeOf<InvalidOperationException>());
        }

        [Test]
        public void InitializeFromSettings_WithNullSettings_ThrowsException()
        {
            var rule = new BodyContentRule();

            Assert.That(
                () => rule.InitializeFromSettings(null),
                Throws.Exception.TypeOf<ArgumentNullException>().With.Property("ParamName").EqualTo("settings"));
        }

        [Test]
        public void InitializeFromSettings_WithValidSettings_MatchesCorrectly()
        {
            var settings = new[]
            {
                new RuleSetting { Name = "text.evaluation", Value = "passed" },
                new RuleSetting { Name = string.Empty, Value = string.Empty },
                new RuleSetting { Name = null, Value = string.Empty },
                new RuleSetting { Name = "text.evaluation", Value = string.Empty }
            };

            var rule = new BodyContentRule();
            rule.InitializeFromSettings(settings);

            Assert.IsTrue(rule.IsInitialized);

            var request = CreateTextRequest("passed");
            Assert.IsTrue(rule.IsMatch(request));
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
