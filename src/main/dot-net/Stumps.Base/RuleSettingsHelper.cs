namespace Stumps
{

    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;

    /// <summary>
    ///     A class that provides a friendly interface for managing the settings of a <see cref="T:Stumps.IStumpRule"/> object.
    /// </summary>
    public sealed class RuleSettingsHelper
    {

        private readonly Dictionary<string, string> _dict;

        /// <summary>
        ///     Initializes a new instance of the <see cref="T:Stumps.RuleSettingsHelper"/> class.
        /// </summary>
        public RuleSettingsHelper()
        {
            _dict = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="T:Stumps.RuleSettingsHelper"/> class.
        /// </summary>
        /// <param name="settings">An enumerable collection of <see cref="T:Stumps.RuleSetting"/> objects.</param>
        public RuleSettingsHelper(IEnumerable<RuleSetting> settings) : this()
        {

            if (settings == null)
            {
                return;
            }

            foreach (var setting in settings)
            {

                if (setting == null || string.IsNullOrWhiteSpace(setting.Name) || setting.Value == null)
                {
                    continue;
                }

                if (_dict.ContainsKey(setting.Name))
                {
                    _dict[setting.Name] = setting.Value;
                }
                else
                {
                    _dict.Add(setting.Name, setting.Value);
                }

            }

        }

        /// <summary>
        /// Gets the number of <see cref="T:Stumps.RuleSetting"/> objects contained by the instance.
        /// </summary>
        /// <value>
        /// The number of <see cref="T:Stumps.RuleSetting"/> objects contained by the instance.
        /// </value>
        public int Count
        {
            get { return _dict.Count; }
        }

        /// <summary>
        ///     Adds a specified byte array to the settings list.
        /// </summary>
        /// <param name="settingName">The name of the setting.</param>
        /// <param name="value">The value of the setting.</param>
        /// <exception cref="System.ArgumentNullException"><paramref name="settingName" /> is <c>null</c>.
        /// -or-
        /// <paramref name="value" /> is <c>null</c>.</exception>
        /// <exception cref="System.ArgumentOutOfRangeException"><paramref name="value"/> cannot have a length of '0'.</exception>
        public void Add(string settingName, byte[] value)
        {
        
            if (settingName == null)
            {
                throw new ArgumentNullException("settingName");
            }

            if (value == null)
            {
                throw new ArgumentNullException("value");
            }

            if (value.Length == 0)
            {
                throw new ArgumentOutOfRangeException("value");
            }

            var s = Convert.ToBase64String(value, Base64FormattingOptions.None);
            AddOrUpdateSetting(settingName, s);

        }

        /// <summary>
        ///     Adds a specified boolean to the settings list.
        /// </summary>
        /// <param name="settingName">The name of the setting.</param>
        /// <param name="value">The value of the setting.</param>
        /// <exception cref="System.ArgumentNullException"><paramref name="settingName"/> is <c>null</c>.</exception>
        public void Add(string settingName, bool value)
        {

            if (settingName == null)
            {
                throw new ArgumentNullException("settingName");
            }

            var s = value.ToString(CultureInfo.InvariantCulture);
            AddOrUpdateSetting(settingName, s);

        }

        /// <summary>
        ///     Adds a specified integer to the settings list.
        /// </summary>
        /// <param name="settingName">The name of the setting.</param>
        /// <param name="value">The value of the setting.</param>
        /// <exception cref="System.ArgumentNullException"><paramref name="settingName"/> is <c>null</c>.</exception>
        public void Add(string settingName, int value)
        {

            if (settingName == null)
            {
                throw new ArgumentNullException("settingName");
            }

            var s = value.ToString(CultureInfo.InvariantCulture);
            AddOrUpdateSetting(settingName, s);

        }

        /// <summary>
        ///     Adds a specified string to the settings list.
        /// </summary>
        /// <param name="settingName">The name of the setting.</param>
        /// <param name="value">The value of the setting.</param>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="settingName"/> is <c>null</c>.
        /// -or-
        /// <paramref name="value"/> is <c>null</c>.
        /// </exception>
        public void Add(string settingName, string value)
        {

            if (settingName == null)
            {
                throw new ArgumentNullException("settingName");
            }

            if (value == null)
            {
                throw new ArgumentNullException("value");
            }

            AddOrUpdateSetting(settingName, value);

        }

        /// <summary>
        ///     Finds the array of <see cref="T:System.Byte"/> values for the specified setting.
        /// </summary>
        /// <param name="settingName">The name of the setting.</param>
        /// <param name="defaultValue">The default value of the setting if it is not found.</param>
        /// <returns>An array of <see cref="T:System.Byte"/> values for the specified <paramref name="settingName"/>.</returns>
        public byte[] FindByteArray(string settingName, byte[] defaultValue)
        {

            var value = FindSettingValue(settingName);

            if (value == null)
            {
                return defaultValue;
            }

            try
            {
                var bytes = Convert.FromBase64String(value);
                return bytes;
            }
            catch (FormatException ex)
            {
                Console.WriteLine("Type - " + ex.GetType() + " \nSource - " + ex.Source + " \nException - " + ex.ToString() + " \n\n");
                return defaultValue;
            }

        }

        /// <summary>
        ///     Finds the <see cref="T:System.Boolean"/> value for the specified setting.
        /// </summary>
        /// <param name="settingName">The name of the setting.</param>
        /// <param name="defaultValue">The default value of the setting if it is not found.</param>
        /// <returns>A <see cref="T:System.Boolean"/> containing the value for the specified <paramref name="settingName"/>.</returns>
        public bool FindBoolean(string settingName, bool defaultValue)
        {

            var value = FindSettingValue(settingName);

            bool result;
            
            var parseResult = bool.TryParse(value, out result);
            
            result = parseResult ? result : defaultValue;

            return result;

        }

        /// <summary>
        ///     Finds the <see cref="T:System.Int32"/> value for the specified setting.
        /// </summary>
        /// <param name="settingName">The name of the setting.</param>
        /// <param name="defaultValue">The default value of the setting if it is not found.</param>
        /// <returns>A <see cref="T:System.Int32"/> containing the value for the specified <paramref name="settingName"/>.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId = "integer", Justification = "The identifier is appropriate in this context.")]
        public int FindInteger(string settingName, int defaultValue)
        {

            var value = FindSettingValue(settingName);

            int result;

            var parseResult = int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out result);

            result = parseResult ? result : defaultValue;

            return result;
            
        }

        /// <summary>
        ///     Finds the <see cref="T:System.String"/> value for the specified setting.
        /// </summary>
        /// <param name="settingName">The name of the setting.</param>
        /// <param name="defaultValue">The default value of the setting if it is not found.</param>
        /// <returns>A <see cref="T:System.String"/> containing the value for the specified <paramref name="settingName"/>.</returns>
        public string FindString(string settingName, string defaultValue)
        {

            var value = FindSettingValue(settingName);

            value = value ?? defaultValue;

            return value;
            
        }

        /// <summary>
        ///     Gets an enumearable list of the <see cref="T:Stumps.RuleSetting"/> objects.
        /// </summary>
        /// <returns>An enumerable list of <see cref="T:Stumps.RuleSetting"/> objects.</returns>
        public IEnumerable<RuleSetting> ToEnumerableList()
        {

            return this._dict.Keys.Select(key => new RuleSetting
            {
                Name = key,
                Value = this._dict[key]
            }).ToList();

        }

        /// <summary>
        ///     Adds or udpates the value of a setting.
        /// </summary>
        /// <param name="settingName">The name of the setting.</param>
        /// <param name="value">The value of the setting.</param>
        private void AddOrUpdateSetting(string settingName, string value)
        {
        
            if (_dict.ContainsKey(settingName))
            {
                _dict[settingName] = value;
            }
            else
            {
                _dict.Add(settingName, value);
            }

        }

        /// <summary>
        ///     Finds a specified value from the dictionary.
        /// </summary>
        /// <param name="settingName">The name of the setting.</param>
        /// <returns>The value for <paramref name="settingName"/>.</returns>
        /// <remarks>A <c>null</c> value is returned if the setting is not found.</remarks>
        private string FindSettingValue(string settingName)
        {

            var value = _dict.ContainsKey(settingName) ? _dict[settingName] : null;
            return value;

        }

    }

}
