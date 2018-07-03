namespace Stumps.Server
{
    using System.Collections.Generic;
    using Stumps.Server.Data;

    /// <summary>
    ///     A helper class that provides a translation between contracts and entities.
    /// </summary>
    internal static class ContractEntityBinding
    {
        /// <summary>
        ///     Creates a Stump contract from a Stump data entity.
        /// </summary>
        /// <param name="serverId">The unique identifier for the server.</param>
        /// <param name="entity">The <see cref="T:Stumps.Server.Data.StumpEntity"/> used to create the contract.</param>
        /// <param name="dataAccess">The data access provider used by the instance.</param>
        /// <returns>
        ///     A <see cref="T:Stumps.Server.StumpContract"/> created from the specified <paramref name="entity"/>.
        /// </returns>
        public static StumpContract CreateContractFromEntity(string serverId, StumpEntity entity, IDataAccess dataAccess)
        {
            var contract = new StumpContract
            {
                OriginalRequest = new RecordedRequest(new HttpRequestEntityReader(serverId, entity.OriginalRequest, dataAccess), ContentDecoderHandling.DecodeNotRequired),
                OriginalResponse = new RecordedResponse(new HttpResponseEntityReader(serverId, entity.OriginalResponse, dataAccess), ContentDecoderHandling.DecodeNotRequired),
                Response = new RecordedResponse(new HttpResponseEntityReader(serverId, entity.Response, dataAccess), ContentDecoderHandling.DecodeNotRequired),
                ResponseDelay = entity.ResponseDelay,
                Rules = new RuleContractCollection(),
                StumpCategory = entity.StumpName,
                StumpId = entity.StumpId,
                StumpName = entity.StumpName,
                TerminateConnection = entity.TerminateConnection
            };

            foreach (var ruleEntity in entity.Rules)
            {
                var rule = new RuleContract
                {
                    RuleName = ruleEntity.RuleName
                };

                foreach (var value in ruleEntity.Settings)
                {
                    var setting = new RuleSetting
                    {
                        Name = value.Name,
                        Value = value.Value
                    };
                    rule.AppendRuleSetting(setting);
                }

                contract.Rules.Add(rule);
            }

            return contract;
        }

        /// <summary>
        ///     Creates a Stump data entity from a Stump contract.
        /// </summary>
        /// <param name="contract">The <see cref="T:Stumps.Server.StumpContract"/> used to create the entity.</param>
        /// <returns>
        ///     A <see cref="T:Stumps.Server.Data.StumpEntity"/> created from the specified <paramref name="contract"/>.
        /// </returns>
        public static StumpEntity CreateEntityFromContract(StumpContract contract)
        {
            var originalRequest = new HttpRequestEntity
            {
                BodyResourceName = string.Empty,
                Headers = CreateNameValuePairFromHeaders(contract.OriginalRequest.Headers),
                HttpMethod = contract.OriginalRequest.HttpMethod,
                LocalEndPoint = contract.OriginalRequest.LocalEndPoint.ToString(),
                ProtocolVersion = contract.OriginalRequest.ProtocolVersion,
                RawUrl = contract.OriginalRequest.RawUrl,
                RemoteEndPoint = contract.OriginalRequest.RemoteEndPoint.ToString()
            };

            var originalResponse = new HttpResponseEntity
            {
                BodyResourceName = string.Empty,
                Headers = CreateNameValuePairFromHeaders(contract.OriginalResponse.Headers),
                RedirectAddress = contract.OriginalResponse.RedirectAddress,
                StatusCode = contract.OriginalResponse.StatusCode,
                StatusDescription = contract.OriginalResponse.StatusDescription
            };

            var response = new HttpResponseEntity
            {
                BodyResourceName = string.Empty,
                Headers = CreateNameValuePairFromHeaders(contract.Response.Headers),
                RedirectAddress = contract.Response.RedirectAddress,
                StatusCode = contract.Response.StatusCode,
                StatusDescription = contract.Response.StatusDescription
            };

            var entity = new StumpEntity
            {
                OriginalRequest = originalRequest,
                OriginalResponse = originalResponse,
                Response = response,
                ResponseDelay = contract.ResponseDelay,
                Rules = new List<RuleEntity>(),
                StumpCategory = contract.StumpCategory,
                StumpId = contract.StumpId,
                StumpName = contract.StumpName,
                TerminateConnection = contract.TerminateConnection
            };

            foreach (var rule in contract.Rules)
            {
                var ruleEntity = new RuleEntity
                {
                    RuleName = rule.RuleName,
                    Settings = new List<NameValuePairEntity>()
                };

                var settings = rule.GetRuleSettings();
                foreach (var setting in settings)
                {
                    ruleEntity.Settings.Add(
                        new NameValuePairEntity
                        {
                            Name = setting.Name,
                            Value = setting.Value
                        });
                }

                entity.Rules.Add(ruleEntity);
            }

            return entity;
        }

        /// <summary>
        ///     Creates a list of <see cref="T:Stumps.Server.Data.NameValuePairEntity"/> objects a <see cref="T:Stumps.IHttpHeaders"/> object.
        /// </summary>
        /// <param name="headers">The headers.</param>
        /// <returns>A list of <see cref="T:Stumps.Server.Data.NameValuePairEntity"/> objects.</returns>
        private static List<NameValuePairEntity> CreateNameValuePairFromHeaders(IHttpHeaders headers)
        {
            var pairs = new List<NameValuePairEntity>();

            foreach (var headerName in headers.HeaderNames)
            {
                var pair = new NameValuePairEntity
                {
                    Name = headerName,
                    Value = headers[headerName]
                };
                pairs.Add(pair);
            }

            return pairs;
        }
    }
}
