namespace Stumps.Web.ApiModules
{

    using System;
    using System.Collections.Generic;
    using Nancy;
    using Nancy.ModelBinding;
    using Stumps.Proxy;
    using Stumps.Web.Models;

    /// <summary>
    ///     A class that provides support for managing the recordings of a proxy server through the RESET API.
    /// </summary>
    public class RecordingModule : NancyModule
    {

        /// <summary>
        ///     Initializes a new instance of the <see cref="T:Stumps.Web.ApiModules.RecordingModule"/> class.
        /// </summary>
        /// <param name="proxyHost">The <see cref="T:Stumps.Proxy.IProxyHost"/> used by the instance.</param>
        /// <exception cref="System.ArgumentNullException"><paramref name="proxyHost"/> is <c>null</c>.</exception>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "Assumed to be handled by Nancy")]
        public RecordingModule(IProxyHost proxyHost)
        {

            if (proxyHost == null)
            {
                throw new ArgumentNullException("proxyHost");
            }

            Get["/api/proxy/{proxyId}/recording"] = _ =>
            {
                var proxyId = (string)_.proxyId;
                var environment = proxyHost.FindProxy(proxyId);
                var afterIndex = -1;

                if (Request.Query.after != null)
                {
                    var afterIndexString = (string)Request.Query.after;
                    afterIndex = int.TryParse(afterIndexString, out afterIndex) ? afterIndex : -1;
                }

                var recordingList = environment.Recordings.Find(afterIndex);
                var modelList = new List<RecordingModel>();

                foreach (var recording in recordingList)
                {
                    afterIndex++;

                    var model = new RecordingModel
                    {
                        Index = afterIndex,
                        Date = recording.RequestDate,
                        Method = recording.Request.HttpMethod,
                        RawUrl = recording.Request.RawUrl,
                        RequestSize = recording.Request.Body == null ? 0 : recording.Request.Body.Length,
                        ResponseSize = recording.Response.Body == null ? 0 : recording.Response.Body.Length,
                        StatusCode = recording.Response.StatusCode,
                        StatusDescription = recording.Response.StatusDescription
                    };

                    modelList.Add(model);
                }

                return Response.AsJson(modelList);
            };

            Get["/api/proxy/{proxyId}/recording/{recordIndex}"] = _ =>
            {
                var proxyId = (string)_.proxyId;
                var recordIndex = (int)_.recordIndex;
                var environment = proxyHost.FindProxy(proxyId);

                var record = environment.Recordings.FindAt(recordIndex);

                var model = new RecordingDetailsModel
                {
                    Index = recordIndex,
                    RequestBody = string.Empty,
                    RequestBodyIsImage = record.Request.BodyIsImage,
                    RequestBodyIsText = record.Request.BodyIsText,
                    RequestBodyLength = record.Request.Body != null ? record.Request.Body.Length : 0,
                    RequestBodyUrl = "/api/proxy/" + proxyId + "/recording/" + recordIndex + "/request",
                    RequestHttpMethod = record.Request.HttpMethod,
                    RequestRawUrl = record.Request.RawUrl,
                    RequestDate = record.RequestDate,
                    ResponseBody = string.Empty,
                    ResponseBodyIsImage = record.Response.BodyIsImage,
                    ResponseBodyIsText = record.Response.BodyIsText,
                    ResponseBodyLength = record.Response.Body != null ? record.Response.Body.Length : 0,
                    ResponseBodyUrl = "/api/proxy/" + proxyId + "/recording/" + recordIndex + "/response",
                    ResponseStatusCode = record.Response.StatusCode,
                    ResponseStatusDescription = record.Response.StatusDescription
                };

                model.RequestBody = record.Request.BodyIsText
                                         ? NancyModuleHelper.GenerateBodyText(record.Request)
                                         : string.Empty;
                model.ResponseBody = record.Response.BodyIsText
                                          ? NancyModuleHelper.GenerateBodyText(record.Response)
                                          : string.Empty;

                model.RequestHeaders = GenerateHeaders(record.Request);
                model.ResponseHeaders = GenerateHeaders(record.Response);

                return Response.AsJson(model);
            };

            Get["/api/proxy/{proxyId}/recording/{recordIndex}/request"] = _ =>
            {
                var proxyId = (string)_.proxyId;
                var recordIndex = (int)_.recordIndex;
                var environment = proxyHost.FindProxy(proxyId);

                var record = environment.Recordings.FindAt(recordIndex);

                var ms = new System.IO.MemoryStream(record.Request.Body);

                return Response.FromStream(ms, record.Request.BodyContentType);
            };

            Get["/api/proxy/{proxyId}/recording/{recordIndex}/response"] = _ =>
            {
                var proxyId = (string)_.proxyId;
                var recordIndex = (int)_.recordIndex;
                var environment = proxyHost.FindProxy(proxyId);

                var record = environment.Recordings.FindAt(recordIndex);

                var ms = new System.IO.MemoryStream(record.Response.Body);

                return Response.FromStream(ms, record.Response.BodyContentType);
            };

            Get["/api/proxy/{proxyId}/recording/status"] = _ =>
            {
                var proxyId = (string)_.proxyId;
                var environment = proxyHost.FindProxy(proxyId);

                var model = new RecordStatusModel
                {
                    RecordTraffic = environment.RecordTraffic
                };

                return Response.AsJson(model);
            };

            Put["/api/proxy/{proxyId}/recording/status"] = _ =>
            {
                var proxyId = (string)_.proxyId;
                var environment = proxyHost.FindProxy(proxyId);

                var model = this.Bind<RecordStatusModel>();

                if (model.RecordTraffic)
                {
                    environment.Recordings.Clear();
                }

                environment.RecordTraffic = model.RecordTraffic;

                return Response.AsJson(model);
            };

        }

        /// <summary>
        ///     Generates the HTTP headers used by a <see cref="T:Stumps.Proxy.IRecordedContextPart"/>.
        /// </summary>
        /// <param name="part">The <see cref="T:Stumps.Proxy.IRecordedContextPart"/> used to generate headers.</param>
        /// <returns>An array of <see cref="Stumps.Web.Models.HeaderModel"/> objects.</returns>
        private HeaderModel[] GenerateHeaders(IRecordedContextPart part)
        {

            var modelList = new List<HeaderModel>();

            foreach (var header in part.Headers)
            {
                var modelHeader = new HeaderModel
                {
                    Name = header.Name,
                    Value = header.Value
                };

                modelList.Add(modelHeader);
            }

            return modelList.ToArray();

        }

    }

}