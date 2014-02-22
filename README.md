#stumps

Stumps is a HTTP/s proxy server that's useful when testing web services (eg: REST or SOAP services) as well as traditional browser-based applications. Stumps allows you to quickly create mocks for all your HTTP/s based services using an intuitive record & replay mechanism. This allows you to write meaningful acceptance and performance tests that aren't tightly coupled to your partners' services.

#Background

Some examples of where Merchant Warehouse makes a SOA call to a third party include (but are not limited to):

* Sending encrypted card data to Magtek or Idtech to decrypt
* Running a sale/void/refund through Google Wallet, ISIS, Level Up, or any of our other platform partners
* Running a sale/void/refund through FirstData Nashville, or many of our other payment processors
* Integrating with the STS gift card solution, or any of our other gift solutions
* Ditto for [ACH](http://en.wikipedia.org/wiki/Automated_Clearing_House) (check clearing houses)

Stumps allows us to clearly demarkate what the System Under Test is, and allows us to test our products without also testing our partners' services. We'll cover that in separate integration tests. 

Stumps also plays a big role in helping Merchant Warehouse test our internal services. Our products are built using a Service Oriented Architecture design, all layered atop our Core Payment Gateway. Stumps allows us to introduce mocks in between these SOA hops, so that we can clearly isolate our system under test.

#Motivation

In many circumstances, it is advantageous to be able to test your core offerings without actually hitting third party partners' services (or sometimes, even one of your internal services). This is the case for many reasons:

* Not all of your partners may have test systems or even test accounts
* Some services won't allow you to reuse data
* You want to promote and enforce a [loose coupling](http://en.wikipedia.org/wiki/Loose_coupling) between components
* You want to clearly delineate what the [system under test](http://en.wikipedia.org/wiki/System_under_test) is
 * Are you testing your code? Are you testing your partner's service? Are you testing them both (integration test)?
 * In the first case, you would prefer to use Stumps to emulate a real request/response, rather than actually reaching out to the third party
* You want to run a performance test
 * You'll seldom want to peg a partner's servers with a stress test. But you still need to know how well your components perform under load.

Importantly, none of this obviates the need for full integration tests with your partners' systems.

# Use cases

At Merchant Warehouse, we use Stumps create Mocks/Sims/Fakes for our acceptance testing initiatives, such as:

* Card readers: Magensa/Magtek, IDTech, ...
* Platform Partners: LevelUp, Google Wallet, ISIS, ...
* Many of our payment processors (Nashville, Paymentech, Vantiv, ....)

Stumps also plays a role in MW's performance testing initiatives, allowing us to run meaningful performance tests without having crushing our partners' platforms.

# Requirements

* Must be standalone (VCR, node-replay, Betamax, and others fail on one or more of the following points)
 * Cannot be tightly integrated with any testing framework 
 * Cannot be tightly integrated with the system under test
 * Language neutral
* Be a pass-through HTTP/HTTPS proxy server
* Be able to record & replay HTTP/HTTPS trafficBe able to return a specific, canned response if certain criteria are met
 * (Certain) HTTP headers must match
 * HTTP body must match
* The matching criteria should be fairly flexible
 * You can choose which fields must match (eg: we care about the "Accept" header, but not "Date" header)
 * You can use regular expression matches (eg: the "Accept" header must contain "text/html", but we don't care what else is in the accept header)
 * You have some simple boolean logic (eg: "The 'Accept' header must not contain 'text/html'")
* Be able to edit or manually create responses
* Be able to return specific HTTP headers
* Must support gzip/deflate compression
* Must support SOAP
* Must support REST
* Must support HTML/browser-based use cases (eg: selenium)

#Usage

Please see the Stumps Usage Guide page for instructions on how to use Stumps, including a video demo of Stumps in action.

# Source code, issues, and feature enhancements

Stumps' source code is hosted on GitHub @ https://github.com/merchantwarehouse/stumps. All issues and feature enhancements are being tracked in the Stumps GitHub Issue Tracker.

# Competitors

Below are a number of tools playing in a similar space. A lot of them look interesting, but for various reasons, we felt that each weren't fit for purpose.

* [Web Page Replay](https://code.google.com/p/web-page-replay/)
* [stubby4j](https://github.com/azagniotov/stubby4j) and its clones
* [Betamax](http://freeside.co/betamax/)
* [Node-replay](https://github.com/assaf/node-replay)
* [Vcr](https://www.relishapp.com/vcr/vcr/docs)
* [http-impersonator](https://code.google.com/p/http-impersonator/)

# Downloading / Installation
Currently, Merchant Warehouse’s Stumps is only available as a source download. If you’d like to provide a MSI package, that'd be very welcome.

#Contributing
We love contributions! Please send [pull requests](https://help.github.com/articles/using-pull-requests) our way. All that we ask is that you please include unit tests with all of your pull requests.

#Getting help
We also love bug reports & feature requests. You can file bugs and feature requests in our Github Issue Tracker. Please consider including the following information when you file a ticket:
* What version you're using
* What command or code you ran
* What output you saw
* How the problem can be reproduced. A small Visual Studio project zipped up or code snippet that demonstrates or reproduces the issue is always appreciated.

You can also always find help on the Stumps Google Group.
