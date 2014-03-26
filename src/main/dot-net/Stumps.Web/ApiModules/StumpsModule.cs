namespace Stumps.Web.ApiModules
{

    using System;
    using System.Collections.Generic;
    using System.Text;
    using Nancy;
    using Nancy.ModelBinding;
    using Stumps.Server;
    using Stumps.Web.Models;

    /// <summary>
    ///     A class that provides support for managing the stumps of a proxy server through the REST API.
    /// </summary>
    public class StumpsModule : NancyModule
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Stumps.Web.ApiModules.StumpsModule"/> class.
        /// </summary>
        /// <param name="serverHost">The <see cref="T:Stumps.Server.IStumpsHost"/> used by the instance.</param>
        /// <exception cref="System.ArgumentNullException"><paramref name="serverHost"/> is <c>null</c>.</exception>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "Assumed to be handled by Nancy")]
        public StumpsModule(IStumpsHost serverHost)
        {

            if (serverHost == null)
            {
                throw new ArgumentNullException("serverHost");
            }

            Get["/api/proxy/{serverId}/stumps/{stumpId}"] = _ =>
            {

                var serverId = (string)_.serverId;
                var stumpId = (string)_.stumpId;
                var server = serverHost.FindServer(serverId);
                var stump = server.FindStump(stumpId);

                var model = CreateStumpModel(stump, serverId, stumpId);

                return Response.AsJson(model);

            };

            Get["/api/proxy/{serverId}/stumps/{stumpId}/request"] = _ =>
            {
                var serverId = (string)_.serverId;
                var stumpId = (string)_.stumpId;
                var server = serverHost.FindServer(serverId);
                var stump = server.FindStump(stumpId);

                var ms = new System.IO.MemoryStream(stump.MatchBody);

                return Response.FromStream(ms, stump.MatchBodyContentType);
            };

            Get["/api/proxy/{serverId}/stumps/{stumpId}/response"] = _ =>
            {
                var serverId = (string)_.serverId;
                var stumpId = (string)_.stumpId;
                var server = serverHost.FindServer(serverId);
                var stump = server.FindStump(stumpId);

                var ms = new System.IO.MemoryStream(stump.Response.GetBody());

                return Response.FromStream(ms, stump.Response.Headers["Content-Type"] ?? string.Empty);
            };

            Post["/api/proxy/{serverId}/stumps"] = _ =>
            {

                var serverId = (string)_.serverId;
                var server = serverHost.FindServer(serverId);

                var model = this.Bind<StumpModel>();
                var contract = CreateContractFromRecord(model, server);

                server.CreateStump(contract);

                return HttpStatusCode.OK;

            };

            Put["/api/proxy/{serverId}/stumps/{stumpId}"] = _ =>
            {

                var serverId = (string)_.serverId;

                var server = serverHost.FindServer(serverId);

                var model = this.Bind<StumpModel>();
                var contract = CreateContractFromStump(model, server);

                if (server.FindStump(contract.StumpId).Equals(null))
                {
                    throw new ArgumentException("Stump name cannot be null.");
                }

                if (server.StumpNameExists(contract.StumpName))
                {
                    var oldStump = server.FindStump(contract.StumpId);
                    if (!oldStump.StumpName.Equals(contract.StumpName, StringComparison.OrdinalIgnoreCase))
                    {
                        throw new ArgumentException("Attempting to create a stump with a name that already exists.");
                    }
                }

                server.DeleteStump(model.StumpId);
                server.CreateStump(contract);

                var stump = server.FindStump(model.StumpId);

                var returnModel = CreateStumpModel(stump, serverId, model.StumpId);

                return Response.AsJson(returnModel);

            };

            Delete["/api/proxy/{serverId}/stumps/{stumpId}/delete"] = _ =>
            {

                var serverId = (string)_.serverId;
                var stumpId = (string)_.stumpId;
                var server = serverHost.FindServer(serverId);
                server.DeleteStump(stumpId);

                return HttpStatusCode.OK;

            };

            Get["/api/proxy/{serverId}/stumps/isStumpNameAvailable/{stumpName}"] = _ =>
            {

                var serverId = (string)_.serverId;
                var stumpName = (string)_.stumpName;
                var server = serverHost.FindServer(serverId);

                var isStumpNameAvailable = !server.StumpNameExists(stumpName);

                var model = new
                {
                    StumpNameIsAvailable = isStumpNameAvailable
                };

                return Response.AsJson(model);

            };

        }

        /// <summary>
        /// Copies an array of header models to a header dictionary.
        /// </summary>
        /// <param name="models">The source header models.</param>
        /// <param name="dict">The target header dictionary.</param>
        private void CopyHeaderModelToDictionary(IEnumerable<HeaderModel> models, IHttpHeaders dict)
        {
            dict.Clear();

            foreach (var model in models)
            {
                dict[model.Name] = model.Value;
            }

        }

        /// <summary>
        ///     Creates a Stump contract from a recorded web request.
        /// </summary>
        /// <param name="model">The <see cref="T:Stumps.Web.Models.StumpModel"/> used to create the contract.</param>
        /// <param name="server">The <see cref="T:Stumps.Server.StumpsServerInstance" /> that contains the recorded web request.</param>
        /// <returns>A <see cref="T:Stumps.Server.StumpContract" /> created from a recorded web request.</returns>
        private StumpContract CreateContractFromRecord(StumpModel model, StumpsServerInstance server)
        {

            var record = server.Recordings.FindAt(model.RecordId);

            var contract = new StumpContract
            {
                HttpMethod = model.RequestHttpMethod,
                RawUrl = model.RequestUrl,
                StumpId = string.Empty,
                StumpName = model.Name,
                StumpCategory = "Uncategorized"
            };

            contract.MatchRawUrl = model.RequestUrlMatch;
            contract.MatchHttpMethod = model.RequestHttpMethodMatch;

            contract.MatchHeaders = CreateHeader(model.RequestHeaderMatch);

            contract.MatchBodyMaximumLength = -1;
            contract.MatchBodyMinimumLength = -1;

            switch (model.RequestBodyMatch)
            {
                case BodyMatch.ContainsText:
                    contract.MatchBodyText = model.RequestBodyMatchValues;
                    contract.MatchBody = record.Request.GetBody();
                    contract.MatchBodyContentType = record.Request.Headers["Content-Type"];
                    contract.MatchBodyIsImage = record.Request.BodyType == HttpBodyClassification.Image;
                    contract.MatchBodyIsText = record.Request.BodyType == HttpBodyClassification.Text;
                    break;

                case BodyMatch.ExactMatch:
                    contract.MatchBody = record.Request.GetBody();
                    contract.MatchBodyContentType = record.Request.Headers["Content-Type"];
                    contract.MatchBodyIsImage = record.Request.BodyType == HttpBodyClassification.Image;
                    contract.MatchBodyIsText = record.Request.BodyType == HttpBodyClassification.Text;
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

            switch (model.ResponseBodySource)
            {

                case BodySource.Modified:
                    contract.Response.ClearBody();
                    contract.Response.AppendToBody(Encoding.UTF8.GetBytes(model.ResponseBodyModification));
                    contract.Response.BodyIsImage = false;
                    contract.Response.BodyIsText = true;
                    break;

                case BodySource.NoBody:
                    contract.Response.ClearBody();
                    contract.Response.BodyIsImage = false;
                    contract.Response.BodyIsText = false;
                    break;

                case BodySource.Origin:
                    contract.Response.ClearBody();
                    contract.Response.AppendToBody(record.Response.GetBody());
                    contract.Response.BodyIsImage = record.Response.BodyType == HttpBodyClassification.Image;
                    contract.Response.BodyIsText = record.Response.BodyType == HttpBodyClassification.Text;
                    break;

            }

            CopyHeaderModelToDictionary(model.ResponseHeaders, contract.Response.Headers);
            contract.Response.StatusCode = model.ResponseStatusCode;
            contract.Response.StatusDescription = model.ResponseStatusDescription;

            return contract;

        }

        /// <summary>
        ///     Creates a Stump contract from an existing Stump.
        /// </summary>
        /// <param name="model">The <see cref="T:Stumps.Web.Models.StumpModel"/> used to create the contract.</param>
        /// <param name="server">The <see cref="T:Stumps.Server.StumpsServerInstance" /> that contains the Stump.</param>
        /// <returns>A <see cref="T:Stumps.Server.StumpContract" /> created from an existing Stump.</returns>
        private StumpContract CreateContractFromStump(StumpModel model, StumpsServerInstance server)
        {

            var originalContract = server.FindStump(model.StumpId);

            var contract = new StumpContract
            {
                HttpMethod = model.RequestHttpMethod,
                MatchBodyMaximumLength = -1,
                MatchBodyMinimumLength = -1,
                MatchHttpMethod = model.RequestHttpMethodMatch,
                MatchRawUrl = model.RequestUrlMatch,
                RawUrl = model.RequestUrl,
                StumpId = model.StumpId,
                StumpName = model.Name,
                StumpCategory = "Uncategorized"
            };

            contract.MatchHeaders = CreateHeader(model.RequestHeaderMatch);

            switch (model.RequestBodyMatch)
            {
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

            switch (model.ResponseBodySource)
            {

                case BodySource.Modified:
                    contract.Response.ClearBody();
                    contract.Response.AppendToBody(Encoding.UTF8.GetBytes(model.ResponseBodyModification));
                    contract.Response.BodyIsImage = false;
                    contract.Response.BodyIsText = true;
                    break;

                case BodySource.NoBody:
                    contract.Response.ClearBody();
                    contract.Response.BodyIsImage = false;
                    contract.Response.BodyIsText = false;
                    break;

                case BodySource.Origin:
                    contract.Response.ClearBody();
                    contract.Response.AppendToBody(originalContract.Response.GetBody());
                    contract.Response.BodyIsImage = originalContract.Response.BodyIsImage;
                    contract.Response.BodyIsText = originalContract.Response.BodyIsText;
                    break;

            }

            CopyHeaderModelToDictionary(model.ResponseHeaders, contract.Response.Headers);
            contract.Response.StatusCode = model.ResponseStatusCode;
            contract.Response.StatusDescription = model.ResponseStatusDescription;

            return contract;

        }

        /// <summary>
        ///     Converts an enumerable list of <see cref="T:Stumps.Web.Models.HeaderModel"/> objects into an 
        ///     array of <see cref="T:Stumps.Server.HttpHeader"/> objects.
        /// </summary>
        /// <param name="headers">The enumerable list of <see cref="T:Stumps.Web.Models.HeaderModel"/> objects.</param>
        /// <returns>An array of <see cref="T:Stumps.Server.HttpHeader"/> objects.</returns>
        private HttpHeader[] CreateHeader(IEnumerable<HeaderModel> headers)
        {

            var headerList = new List<HttpHeader>();

            if (headers != null)
            {
                foreach (var header in headers)
                {
                    var httpHeader = new HttpHeader
                    {
                        Name = header.Name,
                        Value = header.Value
                    };

                    headerList.Add(httpHeader);
                }
            }

            return headerList.ToArray();

        }

        /// <summary>
        ///     Converts an enumerable list of <see cref="T:Stumps.Server.HttpHeader"/> objects into an 
        ///     array of <see cref="T:Stumps.Web.Models.HeaderModel"/> objects.
        /// </summary>
        /// <param name="headers">The enumerable list of <see cref="T:Stumps.Server.HttpHeader"/> objects.</param>
        /// <returns>An array of <see cref="T:Stumps.Web.Models.HeaderModel"/> objects.</returns>
        private HeaderModel[] CreateHeaderModel(IEnumerable<HttpHeader> headers)
        {

            var headerList = new List<HeaderModel>();

            if (headers != null)
            {
                foreach (var header in headers)
                {
                    var headerModel = new HeaderModel
                    {
                        Name = header.Name,
                        Value = header.Value
                    };

                    headerList.Add(headerModel);
                }
            }

            return headerList.ToArray();

        }

        /// <summary>
        ///     Converts a header dictionary into an array of <see cref="T:Stumps.Web.Models.HeaderModel"/> objects.
        /// </summary>
        /// <param name="dictionary">The source <see cref="T:Stumps.IHeaderDictionary"/> dictionary.</param>
        /// <returns>An array of <see cref="T:Stumps.Web.Models.HeaderModel"/> objects.</returns>
        private HeaderModel[] CreateHeaderModel(IHttpHeaders dictionary)
        {
            var headerList = new List<HeaderModel>();

            foreach (var name in dictionary.HeaderNames)
            {
                var model = new HeaderModel()
                {
                    Name = name,
                    Value = dictionary[name]
                };

                headerList.Add(model);
            }

            return headerList.ToArray();
        }

        /// <summary>
        ///     Creates a <see cref="T:Stumps.Web.Models.StumpModel"/> from an existing Stump.
        /// </summary>
        /// <param name="stump">The <see cref="T:Stumps.Server.StumpContract"/> used to create the model.</param>
        /// <param name="serverId">The unique identifier for the proxy the Stump belongs to.</param>
        /// <param name="stumpId">The unique identifier of the Stump.</param>
        /// <returns>
        ///     A new <see cref="T:Stumps.Web.Models.StumpModel"/> object.
        /// </returns>
        private StumpModel CreateStumpModel(StumpContract stump, string serverId, string stumpId)
        {

            var bodyMatch = BodyMatch.IsAnything;

            if (stump.MatchBodyMaximumLength == 0 && stump.MatchBodyMinimumLength == 0)
            {
                bodyMatch = BodyMatch.IsBlank;
            }
            else if (stump.MatchBodyMaximumLength == int.MaxValue && stump.MatchBodyMinimumLength == 0)
            {
                bodyMatch = BodyMatch.IsNotBlank;
            }
            else if (stump.MatchBodyText != null && stump.MatchBodyText.Length > 0 &&
                     stump.MatchBody != null && stump.MatchBody.Length > 0)
            {
                bodyMatch = BodyMatch.ContainsText;
            }
            else if (stump.MatchBody != null && stump.MatchBody.Length > 0)
            {
                bodyMatch = BodyMatch.ExactMatch;
            }

            var model = new StumpModel
            {
                Name = stump.StumpName,
                Origin = StumpOrigin.ExistingStump,
                RecordId = -1,
                RequestBody =
                    stump.MatchBodyIsText ? Encoding.UTF8.GetString(stump.MatchBody) : string.Empty,
                RequestBodyIsImage = stump.MatchBodyIsImage,
                RequestBodyIsText = stump.MatchBodyIsText,
                RequestBodyLength = stump.MatchBody != null ? stump.MatchBody.Length : 0,
                RequestBodyMatch = bodyMatch,
                RequestBodyMatchValues = stump.MatchBodyText,
                RequestBodyUrl = "/api/proxy/" + serverId + "/stumps/" + stumpId + "/request",
                RequestHeaderMatch = CreateHeaderModel(stump.MatchHeaders),
                RequestHttpMethod = stump.HttpMethod,
                RequestHttpMethodMatch = stump.MatchHttpMethod,
                RequestUrl = stump.RawUrl,
                RequestUrlMatch = stump.MatchRawUrl,
                ResponseBody =
                    stump.Response.BodyIsText
                        ? Encoding.UTF8.GetString(stump.Response.GetBody())
                        : string.Empty,
                ResponseBodyIsImage = stump.Response.BodyIsImage,
                ResponseBodyIsText = stump.Response.BodyIsText,
                ResponseBodyLength = stump.Response.BodyLength,
                ResponseBodyModification = string.Empty,
                ResponseBodySource = BodySource.Origin,
                ResponseBodyUrl = "/api/proxy/" + serverId + "/stumps/" + stumpId + "/response",
                ResponseHeaders = CreateHeaderModel(stump.Response.Headers),
                ResponseStatusCode = stump.Response.StatusCode,
                ResponseStatusDescription = stump.Response.StatusDescription,
                StumpId = stump.StumpId
            };

            return model;

        }

    }

}