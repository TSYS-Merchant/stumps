namespace Stumps
{
    using System;
    using NSubstitute;
    using NUnit.Framework;

    [TestFixture]
    public class StumpTests
    {
        [Test]
        public void Constructor_WithNullId_ThrowsException()
        {
            Assert.That(
                () => new Stump(null),
                Throws.Exception.TypeOf<ArgumentNullException>().With.Property("ParamName").EqualTo("stumpId"));
        }

        [Test]
        public void Constructor_WithWhiteSpaceId_ThrowsException()
        {
            Assert.That(
                () => new Stump("   "),
                Throws.Exception.TypeOf<ArgumentNullException>().With.Property("ParamName").EqualTo("stumpId"));
        }

        [Test]
        public void Constuctor_WithValidId_UpdatesProperty()
        {
            var stump = new Stump("ABC");
            Assert.AreEqual("ABC", stump.StumpId);
        }

        [Test]
        public void ResposeFactory_GetSet_ReturnsResponseFactory()
        {
            var responseFactory = new StumpResponseFactory();

            var stump = new Stump("ABC")
            {
                Responses = responseFactory
            };

            Assert.AreEqual(responseFactory, stump.Responses);
        }

        [Test]
        public void ResposeFactory_SetNull_ThrowsException()
        {
            Assert.That(
                () => new Stump("ABC").Responses = null,
                Throws.Exception.TypeOf<ArgumentNullException>().With.Property("ParamName").EqualTo("value"));
        }

        [Test]
        public void AddRule_WithNull_ThrowsException()
        {
            Assert.That(
                () => new Stump("ABC").AddRule(null),
                Throws.Exception.TypeOf<ArgumentNullException>().With.Property("ParamName").EqualTo("rule"));
        }

        [Test]
        public void AddRule_WithRule_AddedToCollection()
        {
            var stump = new Stump("ABC");
            stump.AddRule(Substitute.For<IStumpRule>());
            Assert.AreEqual(1, stump.Count);
        }

        [Test]
        public void IsMatch_WithNullContext_ReturnsFalse()
        {
            var stump = new Stump("ABC");
            stump.Responds();

            var rule1 = Substitute.For<IStumpRule>();
            rule1.IsMatch(null).Returns(true);

            stump.AddRule(rule1);

            Assert.IsFalse(stump.IsMatch(null));
        }

        [Test]
        public void IsMatch_WithNullResponse_ReturnsFalse()
        {
            var stump = new Stump("ABC");

            var rule1 = Substitute.For<IStumpRule>();
            rule1.IsMatch(null).Returns(true);

            stump.AddRule(rule1);

            Assert.IsFalse(stump.IsMatch(Substitute.For<IStumpsHttpContext>()));
        }

        [Test]
        public void IsMatch_WithoutRules_ReturnsFalse()
        {
            var stump = new Stump("ABC");
            stump.Responds();

            Assert.IsFalse(stump.IsMatch(Substitute.For<IStumpsHttpContext>()));
        }

        [Test]
        public void IsMatch_WithMultipleMatchingRules_TriesAllRulesReturnsTrue()
        {
            var stump = new Stump("ABC");

            var context = Substitute.For<IStumpsHttpContext>();
            var request = Substitute.For<IStumpsHttpRequest>();
            context.Request.Returns(request);

            var rule1 = Substitute.For<IStumpRule>();
            rule1.IsMatch(request).Returns(true);

            var rule2 = Substitute.For<IStumpRule>();
            rule2.IsMatch(request).Returns(true);

            stump.AddRule(rule1);
            stump.AddRule(rule2);

            stump.Responds();

            var matches = stump.IsMatch(context);
            rule1.Received(1).IsMatch(request);
            rule2.Received(1).IsMatch(request);
            Assert.IsTrue(matches);
        }

        [Test]
        public void IsMatch_WithFailingRule_TriesAllRulesReturnsFalse()
        {
            var stump = new Stump("ABC");

            var context = Substitute.For<IStumpsHttpContext>();
            var request = Substitute.For<IStumpsHttpRequest>();
            context.Request.Returns(request);

            var rule1 = Substitute.For<IStumpRule>();
            rule1.IsMatch(request).Returns(true);

            var rule2 = Substitute.For<IStumpRule>();
            rule2.IsMatch(request).Returns(false);

            stump.AddRule(rule1);
            stump.AddRule(rule2);

            stump.Responds();

            var matches = stump.IsMatch(context);
            rule1.Received(1).IsMatch(request);
            rule2.Received(1).IsMatch(request);
            Assert.IsFalse(matches);
        }

        [Test]
        public void IsMatch_WithResponseFactoryNoResponse_ReturnsFalse()
        {
            var stump = new Stump("ABC");

            var context = Substitute.For<IStumpsHttpContext>();
            var request = Substitute.For<IStumpsHttpRequest>();
            context.Request.Returns(request);

            var rule1 = Substitute.For<IStumpRule>();
            rule1.IsMatch(request).Returns(true);

            var matches = stump.IsMatch(context);
            rule1.DidNotReceive().IsMatch(request);
            Assert.IsFalse(matches);
        }

    }
}
