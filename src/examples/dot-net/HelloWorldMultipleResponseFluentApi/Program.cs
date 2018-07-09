namespace Stumps.Examples.HelloWorldMultipleResponseFluentApi
{
    /* Stumps Hello World (Multiple Response) Fluent API Example
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
        /// <param name="args">The command-line ar
        public static void Main(string[] args)
        {

            ConsoleHelper.ApplicationBanner("Hello World (Multiple Response) Fluent API");

            // Create a new Stumps Server
            var server = new StumpsServer().RespondsWithHttp404();

            // Showing off the multi-response behavior. When the URL /HelloWorld.htm is requested, 
            // return back multiple HTML pages that are loaded from a file at random.
            var randomResponseStump = server
                .HandlesRequest("HelloWorld").MatchingMethod("GET")
                                             .MatchingUrl("/HelloWorld.htm")
                                             .ReturnsMultipleResponses(ResponseFactoryBehavior.Random);

            randomResponseStump.Responds().WithFile("HelloWorld1.htm");
            randomResponseStump.Responds().WithFile("HelloWorld2.htm");
            randomResponseStump.Responds().WithFile("HelloWorld3.htm");

            // Showing off the ability to drop a connection for an incomming URL
            server.HandlesRequest("HelloDrop").MatchingMethod("GET")
                                              .MatchingUrl("/HelloDrop.htm")
                                              .Responds().ByDroppingTheConnection();

            // Showing off the ability to mix and match the delay and terminate connection
            // features within a sequence using the default looping behavior.
            var mixedStump = server
                .HandlesRequest("HelloMixed").MatchingMethod("GET")
                                             .MatchingUrl("/HelloMixed.htm");

            mixedStump.Responds().WithFile("HelloWorld1.htm");
            mixedStump.Responds().WithFile("HelloWorld2.htm").DelayedBy(2000);
            mixedStump.Responds().WithFile("HelloWorld3.htm");
            mixedStump.Responds().ByDroppingTheConnection();

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
