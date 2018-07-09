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
        /// <param name="entity">The <see cref="StumpEntity"/> used to create the contract.</param>
        /// <param name="dataAccess">The data access provider used by the instance.</param>
        /// <returns>
        ///     A <see cref="StumpContract"/> created from the specified <paramref name="entity"/>.
        /// </returns>
        public static StumpContract CreateContractFromEntity(string serverId, StumpEntity entity, IDataAccess dataAccess)
        {
            var contract = new StumpContract
            {
                OriginalRequest = new RecordedRequest(new HttpRequestEntityReader(serverId, entity.OriginalRequest, dataAccess), ContentDecoderHandling.DecodeNotRequired),
                OriginalResponse = new RecordedResponse(new HttpResponseEntityReader(serverId, entity.OriginalResponse, dataAccess), ContentDecoderHandling.DecodeNotRequired),
                Response = new RecordedResponse(new HttpResponseEntityReader(serverId, entity.Response, dataAccess), ContentDecoderHandling.DecodeNotRequired),
                Rules = new RuleContractCollection(),
                StumpCategory = entity.StumpName,
                StumpId = entity.StumpId,
                StumpName = entity.StumpName,
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
        /// <param name="contract">The <see cref="StumpContract"/> used to create the entity.</param>
        /// <returns>
        ///     A <see cref="StumpEntity"/> created from the specified <paramref name="contract"/>.
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
                ResponseDelay = contract.OriginalResponse.ResponseDelay,
                StatusCode = contract.OriginalResponse.StatusCode,
                StatusDescription = contract.OriginalResponse.StatusDescription,
                TerminateConnection = contract.OriginalResponse.TerminateConnection
            };

            var response = new HttpResponseEntity
            {
                BodyResourceName = string.Empty,
                Headers = CreateNameValuePairFromHeaders(contract.Response.Headers),
                RedirectAddress = contract.Response.RedirectAddress,
                ResponseDelay = contract.Response.ResponseDelay,
                StatusCode = contract.Response.StatusCode,
                StatusDescription = contract.Response.StatusDescription,
                TerminateConnection = contract.Response.TerminateConnection
            };

            var entity = new StumpEntity
            {
                OriginalRequest = originalRequest,
                OriginalResponse = originalResponse,
                Response = response,
                Rules = new List<RuleEntity>(),
                StumpCategory = contract.StumpCategory,
                StumpId = contract.StumpId,
                StumpName = contract.StumpName,
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
        ///     Creates a list of <see cref="NameValuePairEntity"/> objects a <see cref="IHttpHeaders"/> object.
        /// </summary>
        /// <param name="headers">The headers.</param>
        /// <returns>A list of <see cref="NameValuePairEntity"/> objects.</returns>
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
