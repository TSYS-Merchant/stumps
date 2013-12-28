namespace Stumps.Proxy {

    using System.Collections.Generic;
    using System.Text;
    using Stumps.Http;
    using Stumps.Utility;

    public class BodyContentRule : IStumpRule {

        private readonly List<TextContainsMatch> _textMatchList;

        public BodyContentRule(string[] contentRules) {

            if ( contentRules == null ) {
                return;
            }

            _textMatchList = new List<TextContainsMatch>(contentRules.Length);

            foreach ( var rule in contentRules ) {
                _textMatchList.Add(new TextContainsMatch(rule, false));
            }

        }

        #region IStumpRule Members

        public bool IsMatch(IStumpsHttpRequest request) {

            if ( request == null || request.InputStream.Length == 0 ) {
                return false;
            }

            var buffer = StreamUtility.ConvertStreamToByteArray(request.InputStream);

            if ( !StringUtility.IsText(buffer) ) {
                return false;
            }

            var body = Encoding.UTF8.GetString(buffer);

            var match = true;

            foreach ( var textMatch in _textMatchList ) {
                match &= textMatch.IsMatch(body);

                if ( !match ) {
                    break;
                }
            }

            return match;

        }

        #endregion

    }

}
