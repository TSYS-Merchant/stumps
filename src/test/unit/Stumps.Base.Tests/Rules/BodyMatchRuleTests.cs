namespace Stumps.Rules
{

    using System;
    using NUnit.Framework;
    using NSubstitute;

    [TestFixture]
    public class BodyMatchRuleTests
    {

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