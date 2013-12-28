namespace Stumps.Proxy {

    using System;
    using System.IO;
    using NUnit.Framework;

    [TestFixture]
    public class BodyMatchRuleTests {

        [Test]
        public void IsMatch_WithDifferentBodySizes_ReturnsFalse() {

            var ruleBody = generateByteArray(50, Environment.TickCount);
            var requestBody = generateByteArray(10, Environment.TickCount + 1);

            var rule = new BodyMatchRule(ruleBody);

            using ( var request = createRequest(requestBody) ) {
                Assert.IsFalse(rule.IsMatch(request));
            }

        }

        [Test]
        public void IsMatch_GivenBodyDifferentFromRuleButSameSize_ReturnsFalse() {

            var ruleBody = generateByteArray(50, Environment.TickCount);
            var requestBody = generateByteArray(50, Environment.TickCount + 1);

            var rule = new BodyMatchRule(ruleBody);

            using ( var request = createRequest(requestBody) ) {
                Assert.IsFalse(rule.IsMatch(request));
            }

        }

        [Test]
        public void IsMatch_GivenBodyIsSameAsRule_ReturnsTrue() {

            var body = generateByteArray(50, Environment.TickCount);

            var rule = new BodyMatchRule(body);

            using ( var request = createRequest(body) ) {
                Assert.IsTrue(rule.IsMatch(request));
            }

        }

        private byte[] generateByteArray(int length, int seed) {

            var buffer = new byte[length];
            
            var rnd = new Random(seed);
            rnd.NextBytes(buffer);

            return buffer;

        }

        private MockHttpRequest createRequest(byte[] requestBody) {

            var request = new MockHttpRequest();
            request.InputStream = new MemoryStream(requestBody);
            return request;

        }

    }

}
