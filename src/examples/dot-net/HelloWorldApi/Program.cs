namespace Stumps.Examples.HelloWorldApi
{

    /* Stumps Hello World API Example
     * ==============================
     * 
     * This Example is used to demonstrate using the basic API provided by Stumps
     * to create a new instance of a Stumps HTTP server which will respond back to
     * the URL /HelloWorld.htm.
     */

    using System;
    using Stumps;
    using Stumps.Rules;

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

            ConsoleHelper.ApplicationBanner("Hello World API");

            // Create a new Stumps Server
            var server = new StumpsServer();

            // An open port will be chosen automatically unless specified
            // server.ListensOnPort = 9100;

            // Create a new Stump for the server
            var stump = new Stump("HelloWorldStump");
            
            // Add two rules that stumps out HTTP GET requests for the url /HeloWorld.htm
            stump.AddRule(new HttpMethodRule("GET"));
            stump.AddRule(new UrlRule("/HelloWorld.htm"));

            // Create a response for the rule
            var response = new BasicHttpResponse();
            response.Headers["Content-Type"] = "text/html;charset=UTF-8";
            response.AppendToBody(
                "<html><header><title>Stumps Hello World</title></header><body><p>Hello From Stumps</p></body></html>");

            // Add the response to the stump
            stump.Responses = new StumpResponseFactory();
            stump.Responses.Add(response);

            // Add the stump to the server
            server.AddStump(stump);

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
