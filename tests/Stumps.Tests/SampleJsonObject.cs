namespace Stumps {

    using System;

    public class SampleJsonObject {

        public enum SampleValues {
            Value1,
            Value2,
            Value3
        }

        public string Color {
            get;
            set;
        }

        public int Number {
            get;
            set;
        }

        public SampleValues EnumerationValue {
            get;
            set;
        }

    }

}
