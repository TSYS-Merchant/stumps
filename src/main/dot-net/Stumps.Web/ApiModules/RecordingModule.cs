namespace Stumps.Web.ApiModules
{

    using System;
    using System.Collections.Generic;
    using Nancy;
    using Nancy.ModelBinding;
    using Stumps.Server;
    using Stumps.Web.Models;

    /// <summary>
    ///     A class that provides support for managing the recordings of a proxy server through the REST API.
    /// </summary>
    public class RecordingModule : NancyModule
    {

        /// <summary>
        ///     Initializes a new instance of the <see cref="T:Stumps.Web.ApiModules.RecordingModule"/> class.
        /// </summary>
        /// <param name="stumpsHost">The <see cref="T:Stumps.Server.IStumpsHost"/> used by the instance.</param>
        /// <exception cref="System.ArgumentNullException"><paramref name="stumpsHost"/> is <c>null</c>.</exception>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "Assumed to be handled by Nancy")]
        public RecordingModule(IStumpsHost stumpsHost)
        {

            if (stumpsHost == null)
            {
                throw new ArgumentNullException("stumpsHost");
            }

            Get["/api/proxy/{serverId}/recording"] = _ =>
            {
                var serverId = (string)_.serverId;
                var environment = stumpsHost.FindServer(serverId);
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
                        Date = recording.ReceivedDate,
                        Method = recording.Request.HttpMethod,
                        RawUrl = recording.Request.RawUrl,
                        RequestSize = recording.Request.BodyLength,
                        ResponseSize = recording.Response.BodyLength,
                        StatusCode = recording.Response.StatusCode,
                        StatusDescription = recording.Response.StatusDescription
                    };

                    modelList.Add(model);
                }

                return Response.AsJson(modelList);
            };

            Get["/api/proxy/{serverId}/recording/{recordIndex}"] = _ =>
            {
                var serverId = (string)_.serverId;
                var recordIndex = (int)_.recordIndex;
                var server = stumpsHost.FindServer(serverId);

                var record = server.Recordings.FindAt(recordIndex);

                var model = new RecordingDetailsModel
                {
                    Index = recordIndex,
                    RequestBody = string.Empty,
                    RequestBodyIsImage = record.Request.BodyType == HttpBodyClassification.Image,
                    RequestBodyIsText = record.Request.BodyType == HttpBodyClassification.Text,
                    RequestBodyLength = record.Request.BodyLength,
                    RequestBodyUrl = "/api/proxy/" + serverId + "/recording/" + recordIndex + "/request",
                    RequestHttpMethod = record.Request.HttpMethod,
                    RequestRawUrl = record.Request.RawUrl,
                    RequestDate = record.ReceivedDate,
                    ResponseBody = string.Empty,
                    ResponseBodyIsImage = record.Response.BodyType == HttpBodyClassification.Image,
                    ResponseBodyIsText = record.Response.BodyType == HttpBodyClassification.Text,
                    ResponseBodyLength = record.Response.BodyLength,
                    ResponseBodyUrl = "/api/proxy/" + serverId + "/recording/" + recordIndex + "/response",
                    ResponseStatusCode = record.Response.StatusCode,
                    ResponseStatusDescription = record.Response.StatusDescription
                };

                model.RequestBody = record.Request.BodyType == HttpBodyClassification.Text
                                         ? record.Request.GetBodyAsString()
                                         : string.Empty;

                model.ResponseBody = record.Response.BodyType == HttpBodyClassification.Text
                                          ? record.Response.GetBodyAsString()
                                          : string.Empty;

                model.RequestHeaders = GenerateHeaderModels(record.Request);
                model.ResponseHeaders = GenerateHeaderModels(record.Response);

                return Response.AsJson(model);
            };

            Get["/api/proxy/{serverId}/recording/{recordIndex}/request"] = _ =>
            {
                var serverId = (string)_.serverId;
                var recordIndex = (int)_.recordIndex;
                var environment = stumpsHost.FindServer(serverId);

                var record = environment.Recordings.FindAt(recordIndex);

                var ms = new System.IO.MemoryStream(record.Request.GetBody());

                return Response.FromStream(ms, record.Request.Headers["Content-Type"]);
            };

            Get["/api/proxy/{serverId}/recording/{recordIndex}/response"] = _ =>
            {
                var serverId = (string)_.serverId;
                var recordIndex = (int)_.recordIndex;
                var server = stumpsHost.FindServer(serverId);

                var record = server.Recordings.FindAt(recordIndex);

                var ms = new System.IO.MemoryStream(record.Response.GetBody());

                return Response.FromStream(ms, record.Response.Headers["Content-Type"]);
            };

            Get["/api/proxy/{serverId}/recording/status"] = _ =>
            {
                var serverId = (string)_.serverId;
                var server = stumpsHost.FindServer(serverId);

                var model = new RecordStatusModel
                {
                    RecordTraffic = server.RecordTraffic
                };

                return Response.AsJson(model);
            };

            Put["/api/proxy/{serverId}/recording/status"] = _ =>
            {
                var serverId = (string)_.serverId;
                var server = stumpsHost.FindServer(serverId);

                var model = this.Bind<RecordStatusModel>();

                if (model.RecordTraffic)
                {
                    server.Recordings.Clear();
                }

                server.RecordTraffic = model.RecordTraffic;

                return Response.AsJson(model);
            };

        }

        /// <summary>
        ///     Generates the HTTP headers used by a <see cref="T:Stumps.IStumpsHttpContextPart"/>.
        /// </summary>
        /// <param name="part">The <see cref="T:Stumps.IStumpsHttpContextPart"/> used to generate headers.</param>
        /// <returns>An array of <see cref="Stumps.Web.Models.HeaderModel"/> objects.</returns>
        private HeaderModel[] GenerateHeaderModels(IStumpsHttpContextPart part)
        {

            var modelList = new List<HeaderModel>();

            foreach (var headerName in part.Headers.HeaderNames)
            {
                var modelHeader = new HeaderModel
                {
                    Name = headerName,
                    Value = part.Headers[headerName]
                };

                modelList.Add(modelHeader);
            }

            return modelList.ToArray();

        }

    }

}