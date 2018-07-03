namespace Stumps.Rules
{
    using System;
    using System.Collections.Generic;
    using NSubstitute;
    using NUnit.Framework;

    [TestFixture]
    public class BodyLengthRuleTests
    {
        [Test]
        public void Constructor_Default_NotInitialized()
        {
            var rule = new BodyLengthRule();
            Assert.IsFalse(rule.IsInitialized);
        }

        [Test]
        public void GetRuleSettings_WhenCalled_ReturnsList()
        {
            var rule = new BodyLengthRule(10, 10);
            var list = new List<RuleSetting>(rule.GetRuleSettings());
            Assert.AreEqual(2, list.Count);
        }

        [Test]
        public void InitializeFromSettings_WhenInitialized_ThrowsException()
        {
            var rule = new BodyLengthRule(10, 10);
            var settings = new[] 
            { 
                    new RuleSetting { Name = "length.maximum", Value = "10" }, 
                    new RuleSetting { Name = "length.minimum", Value = "10" }
            };

            Assert.That(
                () => rule.InitializeFromSettings(settings),
                Throws.Exception.TypeOf<InvalidOperationException>());
        }
        
        [Test]
        public void InitializeFromSettings_WithNullSettings_ThrowsException()
        {
            var rule = new BodyLengthRule();

            Assert.That(
                () => rule.InitializeFromSettings(null),
                Throws.Exception.TypeOf<ArgumentNullException>().With.Property("ParamName").EqualTo("settings"));
        }

        [Test]
        public void InitializeFromSettings_WithValidSettings_InitializesCorrectly()
        {
            var settings = new[]
            {
                new RuleSetting { Name = "length.maximum", Value = "15" },
                new RuleSetting { Name = "length.minimum", Value = "10" },
            };

            var rule = new BodyLengthRule();
            rule.InitializeFromSettings(settings);

            Assert.IsTrue(rule.IsInitialized);
            Assert.AreEqual(15, rule.MaximumBodyLength);
            Assert.AreEqual(10, rule.MinimumBodyLength);
        }

        [TestCase(0, true)]
        [TestCase(15, false)]
        public void IsMatch_EnforcesZeroLengthBodySize(int bodyLength, bool expectedResult)
        {
            var rule = new BodyLengthRule(0, 0);

            var request = CreateRequest(bodyLength);
            Assert.AreEqual(expectedResult, rule.IsMatch(request));
        }

        [Test]
        public void IsMatch_WithNullRequest_ReturnsFalse()
        {
            var rule = new BodyLengthRule(15, 15);
            Assert.IsFalse(rule.IsMatch(null));
        }

        [TestCase(0, false)]
        [TestCase(14, false)]
        [TestCase(16, false)]
        [TestCase(15, true)]
        public void IsMatch_EnforcesExactBodySizeOf15Bytes(int bodyLength, bool expectedResult)
        {
            var rule = new BodyLengthRule(15, 15);

            var request = CreateRequest(bodyLength);
            Assert.AreEqual(expectedResult, rule.IsMatch(request));
        }

        [TestCase(2, false)]
        [TestCase(5, true)]
        [TestCase(30, true)]
        [TestCase(50, true)]
        [TestCase(51, false)]
        public void IsMatch_AllowsBodySizeBetween5and50Bytes(int bodyLength, bool expectedResult)
        {
            var rule = new BodyLengthRule(5, 50);

            var request = CreateRequest(bodyLength);
            Assert.AreEqual(expectedResult, rule.IsMatch(request));
        }

        private IStumpsHttpRequest CreateRequest(int bodySize)
        {
            var buffer = new byte[bodySize];

            for (int i = 0; i < bodySize; i++)
            {
                buffer[i] = 0x20;
            }

            var request = Substitute.For<IStumpsHttpRequest>();
            request.GetBody().Returns(buffer);
            request.BodyLength.Returns(bodySize);

            return request;
        }
    }
}
