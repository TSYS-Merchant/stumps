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
        public void Constructor_WithInvalidLength_ThrowsException()
        {
            Assert.That(
                () => new BodyMatchRule(-1, "000000"),
                Throws.Exception.TypeOf<ArgumentOutOfRangeException>().With.Property("ParamName").EqualTo("length"));
        }

        [Test]
        public void Constructor_WithNullHash_ThrowsException()
        {
            Assert.That(
                () => new BodyMatchRule(50, null),
                Throws.Exception.TypeOf<ArgumentNullException>().With.Property("ParamName").EqualTo("md5Hash"));
        }

        [Test]
        public void GetRuleSettings_WhenCalled_ReturnsList()
        {
            var rule = new BodyMatchRule(3, "010203");
            var list = new List<RuleSetting>(rule.GetRuleSettings());
            Assert.AreEqual(2, list.Count);
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
                new RuleSetting { Name = "body.length", Value = "3" },
                new RuleSetting { Name = "body.md5", Value = "010203" }
            };

            var rule = new BodyMatchRule();
            rule.InitializeFromSettings(settings);

            Assert.IsTrue(rule.IsInitialized);
            Assert.AreEqual(3, rule.BodyLength);
            Assert.AreEqual("010203", rule.Md5Hash);
        }

        [Test]
        public void IsMatch_WithNullRequest_ReturnsFalse()
        {
            var rule = new BodyMatchRule(3, "010203");
            Assert.IsFalse(rule.IsMatch(null));
        }

        [Test]
        public void IsMatch_GivenBodyDifferentFromRuleButSameSize_ReturnsFalse()
        {
            var requestBody = GenerateByteArray(3, Environment.TickCount);

            var rule = new BodyMatchRule(3, "010203");

            var request = CreateRequest(requestBody);
            Assert.IsFalse(rule.IsMatch(request));
        }

        [Test]
        public void IsMatch_GivenBodyIsSameAsRule_ReturnsTrue()
        {
            var requestBody = new byte[] { 1, 2, 3 };

            var rule = new BodyMatchRule(3, "5289DF737DF57326FCDD22597AFB1FAC");

            var request = CreateRequest(requestBody);
            Assert.IsTrue(rule.IsMatch(request));
        }

        [Test]
        public void IsMatch_WithDifferentBodySizes_ReturnsFalse()
        {
            var requestBody = GenerateByteArray(10, Environment.TickCount);

            var rule = new BodyMatchRule(3, "010203");

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
