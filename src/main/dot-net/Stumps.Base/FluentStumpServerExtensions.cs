namespace Stumps
{
    using System;

    /// <summary>
    ///     A class that provides a set of Fluent extension methods to objects inheriting from the <see cref="T:Stumps.IStumpsServer"/> interface.
    /// </summary>
    public static class FluentStumpServerExtensions
    {
        /// <summary>
        ///     Asserts that the <see cref="T:Stumps.IStumpsServer"/> will handle certain HTTP with a <see cref="T:Stumps.Stump"/>.
        /// </summary>
        /// <param name="server">The <see cref="T:Stumps.IStumpsServer"/> that is processing incoming HTTP requests.</param>
        /// <returns>A <see cref="T:Stumps.Stump"/> created for the <paramref name="server"/>.</returns>
        /// <exception cref="System.ArgumentNullException"><paramref name="server"/> is <c>null</c>.</exception>
        public static Stump HandlesRequest(this IStumpsServer server)
        {
            server = server ?? throw new ArgumentNullException(nameof(server));

            var stumpId = Guid.NewGuid().ToString();
            var stump = server.AddNewStump(stumpId);
            return stump;
        }

        /// <summary>
        ///     Asserts that the <see cref="T:Stumps.IStumpsServer" /> will handle certain HTTP with a <see cref="T:Stumps.Stump" />.
        /// </summary>
        /// <param name="server">The <see cref="T:Stumps.IStumpsServer" /> that is processing incoming HTTP requests.</param>
        /// <param name="stumpId">The unique identifier for the <see cref="T:Stumps.Stump"/> being created.</param>
        /// <returns>A <see cref="T:Stumps.Stump"/> created for the <paramref name="server"/>.</returns>
        /// <exception cref="System.ArgumentNullException"><paramref name="server"/> is <c>null</c>.</exception>
        public static Stump HandlesRequest(this IStumpsServer server, string stumpId)
        {
            server = server ?? throw new ArgumentNullException(nameof(server));

            var stump = server.AddNewStump(stumpId);
            return stump;
        }

        /// <summary>
        ///     Asserts that the <see cref="T:Stumps.IStumpsServer" /> should not redirect traffic to a remote HTTP server.
        /// </summary>
        /// <param name="server">The <see cref="T:Stumps.IStumpsServer" /> that is processing incoming HTTP requests.</param>
        /// <returns>The calling <see cref="T:Stumps.IStumpsServer"/>.</returns>
        /// <exception cref="System.ArgumentNullException"><paramref name="server"/> is <c>null</c>.</exception>
        public static IStumpsServer IsNotAProxy(this IStumpsServer server)
        {
            server = server ?? throw new ArgumentNullException(nameof(server));

            server.RemoteHttpServer = null;
            return server;
        }

        /// <summary>
        ///     Asserts that the <see cref="T:Stumps.IStumpsServer" /> should redirect traffic to a specified remote HTTP server.
        /// </summary>
        /// <param name="server">The <see cref="T:Stumps.IStumpsServer" /> that is processing incoming HTTP requests.</param>
        /// <param name="remoteServer">The <see cref="T:System.Uri"/> for the remote HTTP server.</param>
        /// <returns>The calling <see cref="T:Stumps.IStumpsServer" />.</returns>
        /// <exception cref="System.ArgumentNullException"><paramref name="server"/> is <c>null</c>.</exception>
        public static IStumpsServer IsProxyFor(this IStumpsServer server, Uri remoteServer)
        {
            server = server ?? throw new ArgumentNullException(nameof(server));

            server.RemoteHttpServer = remoteServer;
            return server;
        }

        /// <summary>
        ///     Asserts that the <see cref="T:Stumps.IStumpsServer" /> should redirect traffic to a specified remote HTTP server.
        /// </summary>
        /// <param name="server">The <see cref="T:Stumps.IStumpsServer" /> that is processing incoming HTTP requests.</param>
        /// <param name="remoteServerUrl">The URL for the remote HTTP server.</param>
        /// <returns>The calling <see cref="T:Stumps.IStumpsServer" />.</returns>
        /// <exception cref="System.ArgumentNullException"><paramref name="server"/> is <c>null</c>.</exception>
        public static IStumpsServer IsProxyFor(this IStumpsServer server, string remoteServerUrl)
        {
            server = server ?? throw new ArgumentNullException(nameof(server));

            var uri = new Uri(remoteServerUrl);
            server.IsProxyFor(uri);
            return server;
        }

        /// <summary>
        ///     Asserts that the <see cref="T:Stumps.IStumpsServer" /> should return an HTTP 404 error message for requests that cannot be handled.
        /// </summary>
        /// <param name="server">The <see cref="T:Stumps.IStumpsServer" /> that is processing incoming HTTP requests.</param>
        /// <returns>The calling <see cref="T:Stumps.IStumpsServer"/>.</returns>
        /// <exception cref="System.ArgumentNullException"><paramref name="server"/> is <c>null</c>.</exception>
        public static IStumpsServer RespondsWithHttp404(this IStumpsServer server)
        {
            server = server ?? throw new ArgumentNullException(nameof(server));

            server.DefaultResponse = FallbackResponse.Http404NotFound;
            return server;
        }

        /// <summary>
        ///     Asserts that the <see cref="T:Stumps.IStumpsServer" /> should return an HTTP 503 error message for requests that cannot be handled.
        /// </summary>
        /// <param name="server">The <see cref="T:Stumps.IStumpsServer" /> that is processing incoming HTTP requests.</param>
        /// <returns>The calling <see cref="T:Stumps.IStumpsServer"/>.</returns>
        /// <exception cref="System.ArgumentNullException"><paramref name="server"/> is <c>null</c>.</exception>
        public static IStumpsServer RespondsWithHttp503(this IStumpsServer server)
        {
            server = server ?? throw new ArgumentNullException(nameof(server));

            server.DefaultResponse = FallbackResponse.Http503ServiceUnavailable;
            return server;
        }
    }
}
