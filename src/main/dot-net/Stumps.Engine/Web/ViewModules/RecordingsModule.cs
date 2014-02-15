namespace Stumps.Web.ViewModules
{

    using System.Collections;
    using System.Globalization;
    using Nancy;
    using Stumps.Proxy;

    public class RecordingsModule : NancyModule
    {

        public RecordingsModule(IProxyHost proxyHost)
        {

            Get["/proxy/{proxyId}/recordings"] = _ =>
            {
                var proxyId = (string)_.proxyId;
                var environment = proxyHost.FindProxy(proxyId);

                var recordingModelArray = new ArrayList();

                var lastIndex = -1;

                var recordingList = environment.Recordings.Find(-1);
                for (var i = 0; i < recordingList.Count; i++)
                {
                    var recordingModel = new
                    {
                        Index = i,
                        Method = recordingList[i].Request.HttpMethod,
                        RawUrl = recordingList[i].Request.RawUrl,
                        StatusCode = recordingList[i].Response.StatusCode
                    };

                    recordingModelArray.Add(recordingModel);
                    lastIndex = i;
                }

                var model = new
                {
                    ProxyId = environment.ProxyId,
                    ExternalHostName = environment.UseSsl ? environment.ExternalHostName + " (SSL)" : environment.ExternalHostName,
                    LocalWebsite = "http://localhost:" + environment.Port.ToString(CultureInfo.InvariantCulture) + "/",
                    IsRecording = environment.RecordTraffic,
                    LastIndex = lastIndex,
                    Recordings = recordingModelArray
                };

                return View["recordings", model];
            };

        }

    }

}