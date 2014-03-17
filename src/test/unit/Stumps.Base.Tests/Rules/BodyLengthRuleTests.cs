namespace Stumps.Rules
{

    using NUnit.Framework;
    using NSubstitute;

    [TestFixture]
    public class BodyLengthRuleTests
    {

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