namespace Stumps.Rules
{

    using System;
    using System.IO;
    using NUnit.Framework;

    [TestFixture]
    public class BodyMatchRuleTests
    {

        [Test]
        public void IsMatch_GivenBodyDifferentFromRuleButSameSize_ReturnsFalse()
        {

            var ruleBody = GenerateByteArray(50, Environment.TickCount);
            var requestBody = GenerateByteArray(50, Environment.TickCount + 1);

            var rule = new BodyMatchRule(ruleBody);

            using (var request = CreateRequest(requestBody))
            {
                Assert.IsFalse(rule.IsMatch(request));
            }

        }

        [Test]
        public void IsMatch_GivenBodyIsSameAsRule_ReturnsTrue()
        {

            var body = GenerateByteArray(50, Environment.TickCount);

            var rule = new BodyMatchRule(body);

            using (var request = CreateRequest(body))
            {
                Assert.IsTrue(rule.IsMatch(request));
            }

        }

        [Test]
        public void IsMatch_WithDifferentBodySizes_ReturnsFalse()
        {

            var ruleBody = GenerateByteArray(50, Environment.TickCount);
            var requestBody = GenerateByteArray(10, Environment.TickCount + 1);

            var rule = new BodyMatchRule(ruleBody);

            using (var request = CreateRequest(requestBody))
            {
                Assert.IsFalse(rule.IsMatch(request));
            }

        }

        private MockHttpRequest CreateRequest(byte[] requestBody)
        {

            var request = new MockHttpRequest
            {
                InputStream = new MemoryStream(requestBody)
            };

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