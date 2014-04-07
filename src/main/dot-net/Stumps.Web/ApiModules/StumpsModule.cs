namespace Stumps.Web.ApiModules
{

    using System;
    using System.Collections.Generic;
    using System.Text;
    using Nancy;
    using Nancy.ModelBinding;
    using Stumps.Rules;
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

                var ms = new System.IO.MemoryStream(stump.Request.GetBody());

                return Response.FromStream(ms, stump.Request.Headers["Content-Type"]);
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
                Request = record.Request,
                Response = record.Response,
                StumpId = string.Empty,
                StumpName = model.Name,
                StumpCategory = "Uncategorized"
            };

            if (model.RequestUrlMatch)
            {
                contract.Rules.Add(new RuleContract(new UrlRule(model.RequestUrl)));
            }

            if (model.RequestHttpMethodMatch)
            {
                contract.Rules.Add(new RuleContract(new HttpMethodRule(model.RequestHttpMethod)));
            }

            foreach (var h in model.RequestHeaderMatch)
            {
                contract.Rules.Add(new RuleContract(new HeaderRule(h.Name, h.Value)));
            }

            switch (model.RequestBodyMatch)
            {
                case BodyMatch.ContainsText:
                    contract.Rules.Add(new RuleContract(new BodyContentRule(model.RequestBodyMatchValues)));
                    break;

                case BodyMatch.ExactMatch:
                    contract.Rules.Add(
                        new RuleContract(new BodyMatchRule(record.Request.BodyLength, record.Request.BodyMd5Hash)));
                    break;

                case BodyMatch.IsBlank:
                    contract.Rules.Add(new RuleContract(new BodyLengthRule(0, 0)));
                    break;

                case BodyMatch.IsNotBlank:
                    contract.Rules.Add(new RuleContract(new BodyLengthRule(1, int.MaxValue)));
                    break;

            }

            switch (model.ResponseBodySource)
            {

                case BodySource.Modified:
                    contract.Response.ClearBody();
                    contract.Response.AppendToBody(Encoding.UTF8.GetBytes(model.ResponseBodyModification));
                    break;

                case BodySource.NoBody:
                    contract.Response.ClearBody();
                    break;

                case BodySource.Origin:
                    contract.Response.ClearBody();
                    contract.Response.AppendToBody(record.Response.GetBody());
                    break;

            }

            CopyHeaderModelToDictionary(model.ResponseHeaders, contract.Response.Headers);
            contract.Response.StatusCode = model.ResponseStatusCode;
            contract.Response.StatusDescription = model.ResponseStatusDescription;

            contract.Response.ExamineBody();

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
                Request = originalContract.Request,
                Response = originalContract.Response,
                StumpId = model.StumpId,
                StumpName = model.Name,
                StumpCategory = "Uncategorized"
            };

            if (model.RequestUrlMatch)
            {
                contract.Rules.Add(new RuleContract(new UrlRule(model.RequestUrl)));
            }

            if (model.RequestHttpMethodMatch)
            {
                contract.Rules.Add(new RuleContract(new HttpMethodRule(model.RequestHttpMethod)));
            }

            foreach (var h in model.RequestHeaderMatch)
            {
                contract.Rules.Add(new RuleContract(new HeaderRule(h.Name, h.Value)));
            }

            switch (model.RequestBodyMatch)
            {
                case BodyMatch.ContainsText:
                    contract.Rules.Add(new RuleContract(new BodyContentRule(model.RequestBodyMatchValues)));
                    break;

                case BodyMatch.ExactMatch:
                    contract.Rules.Add(
                        new RuleContract(new BodyMatchRule(contract.Request.BodyLength, contract.Request.BodyMd5Hash)));
                    break;

                case BodyMatch.IsBlank:
                    contract.Rules.Add(new RuleContract(new BodyLengthRule(0, 0)));
                    break;

                case BodyMatch.IsNotBlank:
                    contract.Rules.Add(new RuleContract(new BodyLengthRule(1, int.MaxValue)));
                    break;

            }

            switch (model.ResponseBodySource)
            {

                case BodySource.Modified:
                    contract.Response.ClearBody();
                    contract.Response.AppendToBody(Encoding.UTF8.GetBytes(model.ResponseBodyModification));
                    break;

                case BodySource.NoBody:
                    contract.Response.ClearBody();
                    break;

                case BodySource.Origin:
                    contract.Response.ClearBody();
                    contract.Response.AppendToBody(originalContract.Response.GetBody());
                    break;

            }

            CopyHeaderModelToDictionary(model.ResponseHeaders, contract.Response.Headers);
            contract.Response.StatusCode = model.ResponseStatusCode;
            contract.Response.StatusDescription = model.ResponseStatusDescription;

            contract.Response.ExamineBody();

            return contract;

        }
        
        /// <summary>
        ///     Converts an enumerable list of <see cref="T:Stumps.Server.HttpHeader"/> objects into an 
        ///     array of <see cref="T:Stumps.Web.Models.HeaderModel"/> objects.
        /// </summary>
        /// <param name="headers">The enumerable list of <see cref="T:Stumps.Server.HttpHeader"/> objects.</param>
        /// <returns>An array of <see cref="T:Stumps.Web.Models.HeaderModel"/> objects.</returns>
        private HeaderModel[] CreateHeaderModel(IHttpHeaders headers)
        {

            var headerList = new List<HeaderModel>();

            foreach (var header in headers.HeaderNames)
            {
                var headerModel = new HeaderModel
                {
                    Name = header,
                    Value = headers[header]
                };

                headerList.Add(headerModel);
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

            var bodyMatch = DetrmineBodyMatch(stump);

            var model = new StumpModel
            {
                Name = stump.StumpName,
                Origin = StumpOrigin.ExistingStump,
                RecordId = -1,
                RequestBody =
                    stump.Request.BodyType == HttpBodyClassification.Text ? stump.Request.GetBodyAsString() : string.Empty,
                RequestBodyIsImage = stump.Request.BodyType == HttpBodyClassification.Image,
                RequestBodyIsText = stump.Request.BodyType == HttpBodyClassification.Text,
                RequestBodyLength = stump.Request.BodyLength,
                RequestBodyMatch = bodyMatch,
                RequestBodyMatchValues = 
                    stump.Rules.FindRuleContractByName(typeof(BodyContentRule).Name).Count > 0 ?
                        ContractBindings.CreateRuleFromContract<BodyContentRule>(stump.Rules.FindRuleContractByName(typeof(BodyContentRule).Name)[0]).TextEvaluators :
                        new string[0],
                RequestBodyUrl = "/api/proxy/" + serverId + "/stumps/" + stumpId + "/request",
                RequestHeaderMatch = CreateHeadersFromRules(stump),
                RequestHttpMethod = stump.Request.HttpMethod,
                RequestHttpMethodMatch = stump.Rules.FindRuleContractByName(typeof(HttpMethodRule).Name).Count > 0,
                RequestUrl = stump.Request.RawUrl,
                RequestUrlMatch = stump.Rules.FindRuleContractByName(typeof(UrlRule).Name).Count > 0,
                ResponseBody =
                    stump.Response.BodyType == HttpBodyClassification.Text
                        ? stump.Response.GetBodyAsString()
                        : string.Empty,
                ResponseBodyIsImage = stump.Response.BodyType == HttpBodyClassification.Image,
                ResponseBodyIsText = stump.Response.BodyType == HttpBodyClassification.Text,
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

        private HeaderModel[] CreateHeadersFromRules(StumpContract contract)
        {

            var models = new List<HeaderModel>();

            var rules = contract.Rules.FindRuleContractByName(typeof(HeaderRule).Name);

            foreach (var rule in rules)
            {
                var headerRule = ContractBindings.CreateRuleFromContract<HeaderRule>(rule);

                var model = new HeaderModel
                {
                    Name = headerRule.HeaderNameTextMatch,
                    Value = headerRule.HeaderValueTextMatch
                };

                models.Add(model);
            }

            return models.ToArray();

        }

        /// <summary>
        ///     Detrmines the body match based on the rules in the contract.
        /// </summary>
        /// <param name="contract">The contract.</param>
        /// <returns>A member of the <see cref="T:Stumps.Web.Models.BodyMatch"/> enumeration.</returns>
        private BodyMatch DetrmineBodyMatch(StumpContract contract)
        {

            var rules = contract.Rules.FindRuleContractByName(typeof(BodyLengthRule).Name);

            if (rules.Count > 0)
            {
                var blr = ContractBindings.CreateRuleFromContract<BodyLengthRule>(rules[0]);

                if (blr.MinimumBodyLength == 0 && blr.MaximumBodyLength == 0)
                {
                    return BodyMatch.IsBlank;
                }
                
                if (blr.MinimumBodyLength == 1 && blr.MaximumBodyLength == int.MaxValue)
                {
                    return BodyMatch.IsNotBlank;
                }

            }

            if (contract.Rules.FindRuleContractByName(typeof(BodyContentRule).Name).Count > 0)
            {
                return BodyMatch.ContainsText;
            }

            if (contract.Rules.FindRuleContractByName(typeof(BodyMatch).Name).Count > 0)
            {
                return BodyMatch.ExactMatch;
            }

            return BodyMatch.IsAnything;

        }

    }

}