namespace Stumps
{
    using NUnit.Framework;

    [TestFixture]
    public class ServerManagerTests
    {
        [Test]
        public void DeleteAll_WhenCalled_RemovesAllSumps()
        {
            const int stumpCount = 3;

            var manager = new StumpsManager();

            AddStumpsToManager(
                manager: manager,
                count: 3
            );

            Assert.AreEqual(
                expected: stumpCount,
                actual: manager.Count
            );

            manager.DeleteAll();

            Assert.AreEqual(
                expected: 0,
                actual: manager.Count
            );
        }

        private void AddStumpsToManager(StumpsManager manager, int count)
        {
            for (var i = 1; i <= count; i++)
            {
                manager.AddStump(new Stump(i.ToString()));
            }
        }
    }
}
