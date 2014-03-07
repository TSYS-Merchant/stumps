namespace Stumps.Rules
{

    using System.IO;
    using NUnit.Framework;

    [TestFixture]
    public class BodyLengthRuleTests
    {

        [TestCase(0, true)]
        [TestCase(15, false)]
        public void IsMatch_EnforcesZeroLengthBodySize(int bodyLenth, bool expectedResult)
        {

            var rule = new BodyLengthRule(0, 0);

            using (var request = CreateRequest(bodyLenth))
            {
                Assert.AreEqual(expectedResult, rule.IsMatch(request));
            }

        }

        [TestCase(0, false)]
        [TestCase(14, false)]
        [TestCase(16, false)]
        [TestCase(15, true)]
        public void IsMatch_EnforcesExactBodySizeOf15Bytes(int bodyLength, bool expectedResult)
        {

            var rule = new BodyLengthRule(15, 15);

            using (var request = CreateRequest(bodyLength))
            {
                Assert.AreEqual(expectedResult, rule.IsMatch(request));
            }

        }

        [TestCase(2, false)]
        [TestCase(5, true)]
        [TestCase(30, true)]
        [TestCase(50, true)]
        [TestCase(51, false)]
        public void IsMatch_AllowsBodySizeBetween5and50Bytes(int bodyLength, bool expectedResult)
        {

            var rule = new BodyLengthRule(5, 50);

            using (var request = CreateRequest(bodyLength))
            {
                Assert.AreEqual(expectedResult, rule.IsMatch(request));
            }

        }

        private MockHttpRequest CreateRequest(int bodySize)
        {

            var buffer = new byte[bodySize];

            for (int i = 0; i < bodySize; i++)
            {
                buffer[i] = 0x20;
            }

            var stream = new MemoryStream(buffer);

            var request = new MockHttpRequest
            {
                InputStream = stream
            };

            return request;

        }

    }

}