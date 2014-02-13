namespace Stumps.Web.ApiModules {

    using System.Collections.Generic;
    using System.Text;
    using System;
    using Nancy;
    using Nancy.ModelBinding;
    using Stumps.Proxy;
    using Stumps.Web.Models;

    public class StumpsModule : NancyModule {

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification="Assumed to be handled by Nancy")]
        public StumpsModule(IProxyHost proxyHost) {

            Get["/api/proxy/{proxyId}/stumps/{stumpId}"] = _ => {

                var proxyId = (string) _.proxyId;
                var stumpId = (string) _.stumpId;
                var environment = proxyHost.FindProxy(proxyId);
                var stump = environment.Stumps.FindStump(stumpId);

                var model = CreateStumpModel(stump, proxyId, stumpId);

                return Response.AsJson(model);

            };

            Get["/api/proxy/{proxyId}/stumps/{stumpId}/request"] = _ => {
                var proxyId = (string) _.proxyId;
                var stumpId = (string) _.stumpId;
                var environment = proxyHost.FindProxy(proxyId);
                var stump = environment.Stumps.FindStump(stumpId);

                var ms = new System.IO.MemoryStream(stump.Contract.MatchBody);

                return Response.FromStream(ms, stump.Contract.MatchBodyContentType);
            };

            Get["/api/proxy/{proxyId}/stumps/{stumpId}/response"] = _ => {
                var proxyId = (string) _.proxyId;
                var stumpId = (string) _.stumpId;
                var environment = proxyHost.FindProxy(proxyId);
                var stump = environment.Stumps.FindStump(stumpId);

                var ms = new System.IO.MemoryStream(stump.Contract.Response.Body);

                return Response.FromStream(ms, stump.Contract.Response.BodyContentType);
            };

            Post["/api/proxy/{proxyId}/stumps"] = _ => {

                var proxyId = (string) _.proxyId;
                var environment = proxyHost.FindProxy(proxyId);

                var model = this.Bind<StumpModel>();
                var contract = CreateContractFromRecord(model, environment);

                environment.Stumps.CreateStump(contract);

                return HttpStatusCode.OK;

            };

            Put["/api/proxy/{proxyId}/stumps/{stumpId}"] = _ => {

                var proxyId = (string) _.proxyId;

                var environment = proxyHost.FindProxy(proxyId);

                var model = this.Bind<StumpModel>();
                var contract = CreateContractFromStump(model, environment);

                environment.Stumps.DeleteStump(model.StumpId);
                environment.Stumps.CreateStump(contract);

                var stump = environment.Stumps.FindStump(model.StumpId);

                var returnModel = CreateStumpModel(stump, proxyId, model.StumpId);

                return Response.AsJson(returnModel);

            };

            Delete["/api/proxy/{proxyId}/stumps/{stumpId}/delete"] = _ => {

                var proxyId = (string) _.proxyId;
                var stumpId = (string) _.stumpId;
                var environment = proxyHost.FindProxy(proxyId);
                environment.Stumps.DeleteStump(stumpId);

                return HttpStatusCode.OK;

            };

            Get["/api/proxy/{proxyId}/stumps/isStumpNameAvailable/{stumpName}"] = _ =>
            {

                var proxyId = (string)_.proxyId;
                var stumpName = (string)_.stumpName;
                var environment = proxyHost.FindProxy(proxyId);

                var isStumpNameAvailable = !environment.Stumps.StumpNameExists(stumpName);

                var model = new
                {
                    StumpNameIsAvailable = isStumpNameAvailable
                };

                return Response.AsJson(model);

            };

        }

        private StumpContract CreateContractFromRecord(StumpModel model, ProxyEnvironment environment) {

            var record = environment.Recordings.FindAt(model.RecordId);

            var contract = new StumpContract {
                HttpMethod = model.RequestHttpMethod,
                RawUrl = model.RequestUrl,
                Response = new RecordedResponse(),
                StumpId = string.Empty,
                StumpName = model.Name,
                StumpCategory = "Uncategorized"
            };

            contract.MatchRawUrl = model.RequestUrlMatch;
            contract.MatchHttpMethod = model.RequestHttpMethodMatch;

            contract.MatchHeaders = CreateHeader(model.RequestHeaderMatch);

            contract.MatchBodyMaximumLength = -1;
            contract.MatchBodyMinimumLength = -1;

            switch ( model.RequestBodyMatch ) {
                case BodyMatch.ContainsText:
                    contract.MatchBodyText = model.RequestBodyMatchValues;
                    contract.MatchBody = record.Request.Body;
                    contract.MatchBodyContentType = record.Request.BodyContentType;
                    contract.MatchBodyIsImage = record.Request.BodyIsImage;
                    contract.MatchBodyIsText = record.Request.BodyIsText;
                    break;

                case BodyMatch.ExactMatch:
                    contract.MatchBody = record.Request.Body;
                    contract.MatchBodyContentType = record.Request.BodyContentType;
                    contract.MatchBodyIsImage = record.Request.BodyIsImage;
                    contract.MatchBodyIsText = record.Request.BodyIsText;
                    break;

                case BodyMatch.IsBlank:
                    contract.MatchBodyMaximumLength = 0;
                    contract.MatchBodyMinimumLength = 0;
                    break;

                case BodyMatch.IsNotBlank:
                    contract.MatchBodyMaximumLength = int.MaxValue;
                    contract.MatchBodyMinimumLength = 1;
                    break;

            }

            switch ( model.ResponseBodySource ) {

                case BodySource.Modified:
                    contract.Response.Body = System.Text.Encoding.UTF8.GetBytes(model.ResponseBodyModification);
                    contract.Response.BodyContentType = record.Response.BodyContentType;
                    contract.Response.BodyIsImage = false;
                    contract.Response.BodyIsText = true;
                    break;

                case BodySource.NoBody:
                    contract.Response.Body = new byte[] { };
                    contract.Response.BodyIsImage = false;
                    contract.Response.BodyIsText = false;
                    break;

                case BodySource.Origin:
                    contract.Response.Body = record.Response.Body;
                    contract.Response.BodyContentType = record.Response.BodyContentType;
                    contract.Response.BodyIsImage = record.Response.BodyIsImage;
                    contract.Response.BodyIsText = record.Response.BodyIsText;
                    break;

            }

            contract.Response.Headers = CreateHeader(model.ResponseHeaders);
            contract.Response.StatusCode = model.ResponseStatusCode;
            contract.Response.StatusDescription = model.ResponseStatusDescription;

            return contract;

        }

        private StumpContract CreateContractFromStump(StumpModel model, ProxyEnvironment environment) {

            var originalContract = environment.Stumps.FindStump(model.StumpId).Contract;

            var contract = new StumpContract {
                HttpMethod = model.RequestHttpMethod,
                MatchBodyMaximumLength = -1,
                MatchBodyMinimumLength = -1,
                MatchHttpMethod = model.RequestHttpMethodMatch,
                MatchRawUrl = model.RequestUrlMatch,
                RawUrl = model.RequestUrl,
                Response = new RecordedResponse(),
                StumpId = model.StumpId,
                StumpName = model.Name,
                StumpCategory = "Uncategorized"
            };

            contract.MatchHeaders = CreateHeader(model.RequestHeaderMatch);

            switch ( model.RequestBodyMatch ) {
                case BodyMatch.ContainsText:
                    contract.MatchBodyText = model.RequestBodyMatchValues;
                    contract.MatchBody = originalContract.MatchBody;
                    contract.MatchBodyContentType = originalContract.MatchBodyContentType;
                    contract.MatchBodyIsImage = originalContract.MatchBodyIsImage;
                    contract.MatchBodyIsText = originalContract.MatchBodyIsText;
                    break;

                case BodyMatch.ExactMatch:
                    contract.MatchBody = originalContract.MatchBody;
                    contract.MatchBodyContentType = originalContract.MatchBodyContentType;
                    contract.MatchBodyIsImage = originalContract.MatchBodyIsImage;
                    contract.MatchBodyIsText = originalContract.MatchBodyIsText;
                    break;

                case BodyMatch.IsBlank:
                    contract.MatchBodyMaximumLength = 0;
                    contract.MatchBodyMinimumLength = 0;
                    break;

                case BodyMatch.IsNotBlank:
                    contract.MatchBodyMaximumLength = int.MaxValue;
                    contract.MatchBodyMinimumLength = 1;
                    break;

            }

            switch ( model.ResponseBodySource ) {

                case BodySource.Modified:
                    contract.Response.Body = System.Text.Encoding.UTF8.GetBytes(model.ResponseBodyModification);
                    contract.Response.BodyContentType = originalContract.Response.BodyContentType;
                    contract.Response.BodyIsImage = false;
                    contract.Response.BodyIsText = true;
                    break;

                case BodySource.NoBody:
                    contract.Response.Body = new byte[] { };
                    contract.Response.BodyIsImage = false;
                    contract.Response.BodyIsText = false;
                    break;

                case BodySource.Origin:
                    contract.Response.Body = originalContract.Response.Body;
                    contract.Response.BodyContentType = originalContract.Response.BodyContentType;
                    contract.Response.BodyIsImage = originalContract.Response.BodyIsImage;
                    contract.Response.BodyIsText = originalContract.Response.BodyIsText;
                    break;

            }

            contract.Response.Headers = CreateHeader(model.ResponseHeaders);
            contract.Response.StatusCode = model.ResponseStatusCode;
            contract.Response.StatusDescription = model.ResponseStatusDescription;

            return contract;

        }

        private HttpHeader[] CreateHeader(IEnumerable<HeaderModel> headers) {

            var headerList = new List<HttpHeader>();

            if ( headers != null ) {
                foreach ( var header in headers ) {
                    var httpHeader = new HttpHeader {
                        Name = header.Name,
                        Value = header.Value
                    };

                    headerList.Add(httpHeader);
                }
            }

            return headerList.ToArray();

        }

        private HeaderModel[] CreateHeaderModel(IEnumerable<HttpHeader> headers) {

            var headerList = new List<HeaderModel>();

            if ( headers != null ) {
                foreach ( var header in headers ) {
                    var headerModel = new HeaderModel {
                        Name = header.Name,
                        Value = header.Value
                    };

                    headerList.Add(headerModel);
                }
            }

            return headerList.ToArray();

        }

        private StumpModel CreateStumpModel(Stump stump, string proxyId, string stumpId) {

            var bodyMatch = BodyMatch.IsAnything;

            if ( stump.Contract.MatchBodyMaximumLength == 0 && stump.Contract.MatchBodyMinimumLength == 0 ) {
                bodyMatch = BodyMatch.IsBlank;
            }
            else if ( stump.Contract.MatchBodyMaximumLength == int.MaxValue &&
                     stump.Contract.MatchBodyMinimumLength == 0 ) {
                bodyMatch = BodyMatch.IsNotBlank;
            }
            else if ( stump.Contract.MatchBodyText != null && stump.Contract.MatchBodyText.Length > 0 &&
                     stump.Contract.MatchBody != null && stump.Contract.MatchBody.Length > 0 ) {
                bodyMatch = BodyMatch.ContainsText;
            }
            else if ( stump.Contract.MatchBody != null && stump.Contract.MatchBody.Length > 0 ) {
                bodyMatch = BodyMatch.ExactMatch;
            }

            var model = new StumpModel {
                Name = stump.Contract.StumpName,
                Origin = StumpOrigin.ExistingStump,
                RecordId = -1,
                RequestBody =
                    stump.Contract.MatchBodyIsText ? Encoding.UTF8.GetString(stump.Contract.MatchBody) : string.Empty,
                RequestBodyIsImage = stump.Contract.MatchBodyIsImage,
                RequestBodyIsText = stump.Contract.MatchBodyIsText,
                RequestBodyLength = (stump.Contract.MatchBody != null ? stump.Contract.MatchBody.Length : 0),
                RequestBodyMatch = bodyMatch,
                RequestBodyMatchValues = stump.Contract.MatchBodyText,
                RequestBodyUrl = "/api/proxy/" + proxyId + "/stumps/" + stumpId + "/request",
                RequestHeaderMatch = CreateHeaderModel(stump.Contract.MatchHeaders),
                RequestHttpMethod = stump.Contract.HttpMethod,
                RequestHttpMethodMatch = stump.Contract.MatchHttpMethod,
                RequestUrl = stump.Contract.RawUrl,
                RequestUrlMatch = stump.Contract.MatchRawUrl,
                ResponseBody =
                    stump.Contract.Response.BodyIsText
                        ? Encoding.UTF8.GetString(stump.Contract.Response.Body)
                        : string.Empty,
                ResponseBodyIsImage = stump.Contract.Response.BodyIsImage,
                ResponseBodyIsText = stump.Contract.Response.BodyIsText,
                ResponseBodyLength = (stump.Contract.Response.Body != null ? stump.Contract.Response.Body.Length : 0),
                ResponseBodyModification = string.Empty,
                ResponseBodySource = BodySource.Origin,
                ResponseBodyUrl = "/api/proxy/" + proxyId + "/stumps/" + stumpId + "/response",
                ResponseHeaders = CreateHeaderModel(stump.Contract.Response.Headers),
                ResponseStatusCode = stump.Contract.Response.StatusCode,
                ResponseStatusDescription = stump.Contract.Response.StatusDescription,
                StumpId = stump.Contract.StumpId
            };

            return model;

        }

    }

}
