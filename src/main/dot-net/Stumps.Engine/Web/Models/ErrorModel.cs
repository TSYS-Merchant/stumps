namespace Stumps.Web.Models {

    public class ErrorModel {

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays", Justification = "Models used for serialization.")]
        public string[] Errors { get; set; }

        public string ErrorMessage { get; set; }

        public string FullException { get; set; }

    }

}
