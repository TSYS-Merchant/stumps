namespace Stumps
{
    using System;
    using NUnit.Framework;

    [TestFixture]
    public class RuleSettingsHelperTests
    {
        [Test]
        public void Constructor_Default_InitializesEmptyDictionary()
        {
            var helper = new RuleSettingsHelper();
            Assert.AreEqual(0, helper.Count);
        }

        [TestCase("Good", "Good", 1)]
        [TestCase(null, "Good", 0)]
        [TestCase("", "Good", 0)]
        [TestCase("Good", null, 0)]
        [TestCase("Good", "", 1)]
        public void Constructor_WithSetting_Initializes(string settingName, string settingValue, int expectedCount)
        {
            var setting = new RuleSetting
            {
                Name = settingName,
                Value = settingValue
            };

            var helper = new RuleSettingsHelper(
                new RuleSetting[]
                {
                    setting
                });

            Assert.AreEqual(expectedCount, helper.Count);
        }

        [Test]
        public void AddForByteArray_WithNullSettingName_ThrowsException()
        {
            var helper = new RuleSettingsHelper();
            Assert.That(
                () => helper.Add(null, new byte[] { 1, 2, 3 }),
                Throws.Exception.TypeOf<ArgumentNullException>().With.Property("ParamName").EqualTo("settingName"));
        }

        [Test]
        public void AddForByteArray_WithNullValue_ThrowsException()
        {
            var helper = new RuleSettingsHelper();
            byte[] bytes = null;
            Assert.That(
                () => helper.Add("good", bytes),
                Throws.Exception.TypeOf<ArgumentNullException>().With.Property("ParamName").EqualTo("value"));
        }

        [Test]
        public void AddForByteArray_WithEmptyValue_ThrowsException()
        {
            var helper = new RuleSettingsHelper();
            byte[] bytes = new byte[0];
            Assert.That(
                () => helper.Add("good", bytes),
                Throws.Exception.TypeOf<ArgumentOutOfRangeException>().With.Property("ParamName").EqualTo("value"));
        }

        [Test]
        public void AddForByteArray_WithValidValue_UpdatesDictionary()
        {
            var helper = new RuleSettingsHelper();
            helper.Add("good", new byte[] { 1, 2, 3 });
            Assert.AreEqual(1, helper.Count);
        }

        [Test]
        public void AddForBoolean_WithNullSettingName_ThrowsException()
        {
            var helper = new RuleSettingsHelper();
            Assert.That(
                () => helper.Add(null, false),
                Throws.Exception.TypeOf<ArgumentNullException>().With.Property("ParamName").EqualTo("settingName"));
        }

        [Test]
        public void AddForBoolean_WithValidValue_UpdatesDictionary()
        {
            var helper = new RuleSettingsHelper();
            helper.Add("good", true);
            Assert.AreEqual(1, helper.Count);
        }

        [Test]
        public void AddForInteger_WithNullSettingName_ThrowsException()
        {
            var helper = new RuleSettingsHelper();
            Assert.That(
                () => helper.Add(null, 15),
                Throws.Exception.TypeOf<ArgumentNullException>().With.Property("ParamName").EqualTo("settingName"));
        }

        [Test]
        public void AddForInteger_WithValidValue_UpdatesDictionary()
        {
            var helper = new RuleSettingsHelper();
            helper.Add("good", 5);
            Assert.AreEqual(1, helper.Count);
        }

        [Test]
        public void AddForString_WithNullSettingName_ThrowsException()
        {
            var helper = new RuleSettingsHelper();
            string s = "hello world";

            Assert.That(
                () => helper.Add(null, s),
                Throws.Exception.TypeOf<ArgumentNullException>().With.Property("ParamName").EqualTo("settingName"));
        }

        [Test]
        public void AddForString_WithNullValue_ThrowsException()
        {
            var helper = new RuleSettingsHelper();
            string s = null;

            Assert.That(
                () => helper.Add("good", s),
                Throws.Exception.TypeOf<ArgumentNullException>().With.Property("ParamName").EqualTo("value"));
        }

        [Test]
        public void AddForStringr_WithValidValue_UpdatesDictionary()
        {
            var helper = new RuleSettingsHelper();
            helper.Add("good", "good");
            Assert.AreEqual(1, helper.Count);
        }

        [Test]
        public void AddForExistingSetting_UpdatesSetting_UpdatesDictionary()
        {
            var expected = "override";
            var helper = new RuleSettingsHelper();

            helper.Add("good", true);
            helper.Add("good", expected);
            Assert.AreEqual(1, helper.Count);
            Assert.AreEqual(expected, helper.FindString("good", "good"));
        }

        [Test]
        public void FindByteArray_WithValidSetting_ReturnsValue()
        {
            var helper = new RuleSettingsHelper();
            var expected = new byte[] { 1, 2, 3 };
            var defaultValue = new byte[] { 4, 5, 6 };
            helper.Add("good", expected);
            CollectionAssert.AreEqual(expected, helper.FindByteArray("good", defaultValue));
        }

        [Test]
        public void FindByteArray_WithInvalidSetting_ReturnsDefault()
        {
            var helper = new RuleSettingsHelper();
            var expected = new byte[] { 1, 2, 3 };
            CollectionAssert.AreEqual(expected, helper.FindByteArray("good", expected));
        }

        [Test]
        public void FindBooelan_WithValidSetting_ReturnsValue()
        {
            var helper = new RuleSettingsHelper();
            var expected = true;
            var defaultValue = false;
            helper.Add("good", expected);
            Assert.AreEqual(expected, helper.FindBoolean("good", defaultValue));
        }

        [Test]
        public void FindBooelan_WithInvalidSetting_ReturnsDefault()
        {
            var helper = new RuleSettingsHelper();
            Assert.IsFalse(helper.FindBoolean("good", false));
            Assert.IsTrue(helper.FindBoolean("good", true));
        }

        [Test]
        public void FindInteger_WithValidSetting_ReturnsValue()
        {
            var helper = new RuleSettingsHelper();
            var expected = 15;
            var defaultValue = 20;
            helper.Add("good", expected);
            Assert.AreEqual(expected, helper.FindInteger("good", defaultValue));
        }

        [Test]
        public void FindInteger_WithInvalidSetting_ReturnsDefault()
        {
            var helper = new RuleSettingsHelper();
            var expected = 15;
            Assert.AreEqual(expected, helper.FindInteger("good", expected));
        }

        [Test]
        public void FindString_WithValidSetting_ReturnsValue()
        {
            var helper = new RuleSettingsHelper();
            var expected = "hello";
            var defaultValue = "goodbye";
            helper.Add("good", expected);
            Assert.AreEqual(expected, helper.FindString("good", defaultValue));
        }

        [Test]
        public void FindString_WithInvalidSetting_ReturnsDefault()
        {
            var helper = new RuleSettingsHelper();
            var expected = "goodbye";
            Assert.AreEqual(expected, helper.FindString("good", expected));
        }
    }
}
