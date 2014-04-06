namespace Stumps.Rules
{

    using System;
    using System.Collections.Generic;
    using NSubstitute;
    using NUnit.Framework;

    [TestFixture]
    public class BodyMatchRuleTests
    {

        [Test]
        public void Constructor_Default_NotInitialized()
        {

            var rule = new BodyMatchRule();
            Assert.IsFalse(rule.IsInitialized);

        }

        [Test]
        public void Constructor_WithNullBytes_ThrowsException()
        {

            Assert.That(
                () => new BodyMatchRule(null),
                Throws.Exception.TypeOf<ArgumentNullException>().With.Property("ParamName").EqualTo("value"));

        }

        [Test]
        public void GetRuleSettings_WhenCalled_ReturnsList()
        {
            var rule = new BodyMatchRule(new byte[] { 1, 2, 3 });
            var list = new List<RuleSetting>(rule.GetRuleSettings());
            Assert.AreEqual(1, list.Count);
        }

        [Test]
        public void InitializeFromSettings_WithNullSettings_ThrowsException()
        {

            var rule = new BodyMatchRule();

            Assert.That(
                () => rule.InitializeFromSettings(null),
                Throws.Exception.TypeOf<ArgumentNullException>().With.Property("ParamName").EqualTo("settings"));

        }

        [Test]
        public void InitializeFromSettings_WithValidSettings_InitializesCorrectly()
        {

            var settings = new[]
            {
                new RuleSetting { Name = "body.base64", Value = "AQIDBAU=" }
            };

            var rule = new BodyMatchRule();
            rule.InitializeFromSettings(settings);

            Assert.IsTrue(rule.IsInitialized);
            Assert.AreEqual(5, rule.BodyLength);

        }

        [Test]
        public void IsMatch_WithNullRequest_ReturnsFalse()
        {

            var ruleBody = GenerateByteArray(50, Environment.TickCount);

            var rule = new BodyMatchRule(ruleBody);

            Assert.IsFalse(rule.IsMatch(null));

        }

        [Test]
        public void IsMatch_GivenBodyDifferentFromRuleButSameSize_ReturnsFalse()
        {

            var ruleBody = GenerateByteArray(50, Environment.TickCount);
            var requestBody = GenerateByteArray(50, Environment.TickCount + 1);

            var rule = new BodyMatchRule(ruleBody);

            var request = CreateRequest(requestBody);
            Assert.IsFalse(rule.IsMatch(request));

        }

        [Test]
        public void IsMatch_GivenBodyIsSameAsRule_ReturnsTrue()
        {

            var requestBody = GenerateByteArray(50, Environment.TickCount);

            var rule = new BodyMatchRule(requestBody);

            var request = CreateRequest(requestBody);
            Assert.IsTrue(rule.IsMatch(request));

        }

        [Test]
        public void IsMatch_WithDifferentBodySizes_ReturnsFalse()
        {

            var ruleBody = GenerateByteArray(50, Environment.TickCount);
            var requestBody = GenerateByteArray(10, Environment.TickCount + 1);

            var rule = new BodyMatchRule(ruleBody);

            var request = CreateRequest(requestBody);
            Assert.IsFalse(rule.IsMatch(request));

        }

        private IStumpsHttpRequest CreateRequest(byte[] requestBody)
        {

            var request = Substitute.For<IStumpsHttpRequest>();
            request.GetBody().Returns(requestBody);
            request.BodyLength.Returns(requestBody.Length);

            return request;

        }

        private byte[] GenerateByteArray(int length, int seed)
        {

            var buffer = new byte[length];

            var rnd = new Random(seed);
            rnd.NextBytes(buffer);

            return buffer;

        }

    }

}