namespace Stumps.Data {

    using System;

    public class ConfigurationEntity {

        public int DataCompatibilityVersion { get; set; }

        public string StoragePath { get; set; }

        public int WebApiPort { get; set; }

    }

}
