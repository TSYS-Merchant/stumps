namespace Stumps.Web
{

    using System;
    using Nancy;
    using Stumps.Proxy;
    using Stumps.Utility;
    using Stumps.Web.Responses;

    /// <summary>
    ///     A class that provides a new Nancy boot strapper.
    /// </summary>
    public class Bootstrapper : DefaultNancyBootstrapper, IDisposable
    {

        private readonly byte[] _favIcon;
        private bool _disposed;
        private IProxyHost _host;

        /// <summary>
        ///     Initializes a new instance of the <see cref="T:Stumps.Web.Bootstrapper"/> class.
        /// </summary>
        /// <param name="proxyHost">The <see cref="T:Stumps.Proxy.IProxyHost"/> used by the instance.</param>
        /// <exception cref="System.ArgumentNullException"><paramref name="proxyHost"/> is <c>null</c>.</exception>
        public Bootstrapper(IProxyHost proxyHost)
        {

            if (proxyHost == null)
            {
                throw new ArgumentNullException("proxyHost");
            }

            using (
                var resourceStream = this.GetType().Assembly.GetManifestResourceStream("Stumps.Resources.favicon.ico"))
            {
                _favIcon = StreamUtility.ConvertStreamToByteArray(resourceStream);
            }

            _host = proxyHost;

        }

        /// <summary>
        ///     Gets the bytes for the Favorite icon.
        /// </summary>
        /// <value>
        /// The bytes for the Favorite icon.
        /// </value>
        protected override byte[] FavIcon
        {
            get { return _favIcon; }
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        public new void Dispose()
        {

            this.Dispose(true);
            GC.SuppressFinalize(this);

        }

        /// <summary>
        ///     Configures the container using AutoRegister followed by registration of default INancyModuleCatalog and IRouteResolver.
        /// </summary>
        /// <param name="container">Container instance</param>
        /// <exception cref="System.ArgumentNullException"><paramref name="container"/> is <c>null</c>.</exception>
        protected override void ConfigureApplicationContainer(Nancy.TinyIoc.TinyIoCContainer container)
        {

            if (container == null)
            {
                throw new ArgumentNullException("container");
            }

            base.ConfigureApplicationContainer(container);

            container.Register(_host);

        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {

            if (disposing && !_disposed)
            {

                _disposed = true;

                if (_host != null)
                {
                    _host.Dispose();
                    _host = null;
                }

                base.Dispose();

            }

        }

        /// <summary>
        ///     Requests the startup.
        /// </summary>
        /// <param name="container">The container.</param>
        /// <param name="pipelines">The pipelines.</param>
        /// <param name="context">The context.</param>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="container"/> is <c>null</c>.
        /// or
        /// <paramref name="pipelines"/> is <c>null</c>.
        /// or
        /// <paramref name="context"/> is <c>null</c>.
        /// </exception>
        protected override void RequestStartup(Nancy.TinyIoc.TinyIoCContainer container, Nancy.Bootstrapper.IPipelines pipelines, NancyContext context)
        {

            if (container == null)
            {
                throw new ArgumentNullException("container");
            }

            if (pipelines == null)
            {
                throw new ArgumentNullException("pipelines");
            }

            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            pipelines.OnError.AddItemToEndOfPipeline((z, a) => ErrorJsonResponse.FromException(a));
            base.RequestStartup(container, pipelines, context);

        }

    }

}