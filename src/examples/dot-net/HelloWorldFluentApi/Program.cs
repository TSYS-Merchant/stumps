namespace Stumps.Examples.HelloWorldFluentApi
{

    /* Stumps Hello World Fluent API Example
     * =====================================
     * 
     * This Example is used to demonstrate using the Fluent API extensions provided 
     * by Stumps to create a new instance of a Stumps HTTP server which will respond 
     * back to the URL /HelloWorld.htm.
     */

    using System;
    using Stumps;

    /// <summary>
    ///     A class that provides the primary entry point for the application.
    /// </summary>
    public class Program
    {

        /// <summary>
        ///     Defines the entry point of the application.
        /// </summary>
        /// <param name="args">The command-line arguments.</param>
        public static void Main(string[] args)
        {

            ConsoleHelper.ApplicationBanner("Hello World Fluent API");

            // Create a new Stumps Server
            var server = new StumpsServer();

            // Showing off some chaining: Make the server return an HTTP 404 for all unknown
            // and then when the URL /HelloWorld.htm is requested, return back an HTML page
            // that was loaded from a file, but the response is delayed by 2000 milliseconds.
            server.RespondsWithHttp404()
                  .HandlesRequest("HelloWorld").MatchingMethod("GET")
                                               .MatchingUrl("/HelloWorld.htm")
                                               .Responds().WithFile("HelloWorld.htm")
                                                          .DelayedBy(2000);

            // Showing off the ability to drop a connection for an incomming URL
            server.HandlesRequest("HelloDrop").MatchingMethod("GET")
                                              .MatchingUrl("/HelloDrop.htm")
                                              .Responds().ByDroppingTheConnection();

            // Showing off a stump
            // Show the requests that are incomming
            server.RequestProcessed += (o, e) => ConsoleHelper.ShowHttpResponse(server, e);

            // Start the server and wait!
            server.Start();

            // Show the URL to the user
            Console.WriteLine("Browse to http://localhost:{0}/HelloWorld.htm", server.ListeningPort);
            Console.WriteLine();

            // Wait to exit
            ConsoleHelper.WaitForExit();

            server.Shutdown();
            server.Dispose();

        }

    }

}
