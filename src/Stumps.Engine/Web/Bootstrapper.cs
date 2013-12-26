namespace Stumps.Web {

    using System;
    using Nancy;
    using Stumps.Proxy;
    using Stumps.Utility;
    using Stumps.Web.Responses;

    public class Bootstrapper : DefaultNancyBootstrapper, IDisposable {

        private IProxyHost _host;
        private bool _disposed;
        private readonly byte[] _favIcon;

        public Bootstrapper(IProxyHost proxyHost) {

            if ( proxyHost == null ) {
                throw new ArgumentNullException("proxyHost");
            }

            using ( var resourceStream = this.GetType().Assembly.GetManifestResourceStream("Stumps.Resources.favicon.ico") ) {
                _favIcon = StreamUtility.ConvertStreamToByteArray(resourceStream);
            }

            _host = proxyHost;


        }

        protected override byte[] FavIcon {
            get {
                return _favIcon;
            }
        }

        protected override void ConfigureApplicationContainer(Nancy.TinyIoc.TinyIoCContainer container) {

            if ( container == null ) {
                throw new ArgumentNullException("container");
            }

            base.ConfigureApplicationContainer(container);

            container.Register<IProxyHost>(_host);

        }

        protected override void RequestStartup(Nancy.TinyIoc.TinyIoCContainer container, Nancy.Bootstrapper.IPipelines pipelines, NancyContext context) {

            if ( container == null ) {
                throw new ArgumentNullException("container");
            }

            if ( pipelines == null ) {
                throw new ArgumentNullException("pipelines");
            }

            if ( context == null ) {
                throw new ArgumentNullException("context");
            }

            pipelines.OnError.AddItemToEndOfPipeline((z, a) => ErrorJsonResponse.FromException(a));
            base.RequestStartup(container, pipelines, context);

        }

        #region IDisposable Members

        protected virtual void Dispose(bool disposing) {

            if ( disposing && !_disposed ) {

                _disposed = true;

                if ( _host != null ) {
                    _host.Dispose();
                    _host = null;
                }

                base.Dispose();

            }

        }

        public new void Dispose() {

            this.Dispose(true);
            GC.SuppressFinalize(this);

        }

        #endregion

    }

}
