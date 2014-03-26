namespace Stumps.Examples
{

    using System;
    using System.Collections.Generic;
    using Stumps;

    /// <summary>
    ///     A class that provides common console display functionality used across multiple examples. 
    /// </summary>
    public static class ConsoleHelper
    {

        private static Dictionary<HttpResponseOrigin, string> FriendlyOrigin = new Dictionary
            <HttpResponseOrigin, string>()
        {
            { HttpResponseOrigin.NotFoundResponse, "404" },
            { HttpResponseOrigin.RemoteServer, "REMOTE: " },
            { HttpResponseOrigin.ServiceUnavailable, "503" },
            { HttpResponseOrigin.Stump, "STUMP: " },
            { HttpResponseOrigin.Unprocessed, "INCOMMING" }
        };

        /// <summary>
        ///     Shows the banner displaying the application name.
        /// </summary>
        /// <param name="applicationName">The name of the application.</param>
        public static void ApplicationBanner(string applicationName)
        {
            Console.ForegroundColor = ConsoleColor.White;
            applicationName = "Stumps Example: " + applicationName;
            Console.WriteLine(applicationName);
            Console.WriteLine(new string('=', applicationName.Length));
            Console.WriteLine();
            Console.ResetColor();
        }

        /// <summary>
        /// Shows the response to an HTTP request on the screen.
        /// </summary>
        /// <param name="server">The <see cref="T:Stumps.StumpsServer"/> that processed the request.</param>
        /// <param name="e">The <see cref="StumpsContextEventArgs"/> instance containing the event data.</param>
        public static void ShowHttpResponse(StumpsServer server, StumpsContextEventArgs e)
        {

            var message = FriendlyOrigin[e.ResponseOrigin];
            
            if (e.ResponseOrigin == HttpResponseOrigin.Stump)
            {
                message += e.StumpId;
            }
            else if (e.ResponseOrigin == HttpResponseOrigin.RemoteServer)
            {
                message += server.RemoteHttpServer.DnsSafeHost;
            }

            Console.WriteLine("[{0}] => {1} {2}", message, e.Context.Request.HttpMethod, e.Context.Request.RawUrl);

        }

        /// <summary>
        ///     Prompts the user to press the escape key to exit.
        /// </summary>
        public static void WaitForExit()
        {

            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Press [ESC] to Exit.");
            Console.WriteLine();
            Console.ResetColor();

            while (Console.ReadKey(true).Key != ConsoleKey.Escape)
            {
            }

            Console.WriteLine("Goodbye!");

        }

    }

}
