namespace Stumps.Web.ViewModules
{

    using System;
    using System.Collections;
    using System.Globalization;
    using Nancy;
    using Stumps.Proxy;

    /// <summary>
    ///     A class that provides support the recordings overview webpage of the Stumps website.
    /// </summary>
    public class RecordingsModule : NancyModule
    {

        /// <summary>
        ///     Initializes a new instance of the <see cref="T:Stumps.Web.ViewModules.RecordingsModule"/> class.
        /// </summary>
        /// <param name="proxyHost">The <see cref="T:Stumps.Proxy.IProxyHost"/> used by the instance.</param>
        /// <exception cref="System.ArgumentNullException"><paramref name="proxyHost"/> is <c>null</c>.</exception>
        public RecordingsModule(IProxyHost proxyHost)
        {

            if (proxyHost == null)
            {
                throw new ArgumentNullException("proxyHost");
            }

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