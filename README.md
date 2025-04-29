# Intro to Observability: OpenTelemetry in .NET - DEPRECATED

DEPRECATED - Please prefer the Meminator application as an example: https://github.com/honeycombio/meminator-workshop/

## Former Description

This ASP.NET application is here for you to try out tracing with Honeycomb.
It consists of a microservice that calls itself, so you can simulate
a whole microservice ecosystem with just one service!

## What to do

Recommended: 

[![Open in Gitpod](https://gitpod.io/button/open-in-gitpod.svg)](https://gitpod.io/#https://github.com/honeycombio/intro-to-o11y-dotnet)


Gitpod is a free cloud environment where you can run the example without needing to clone the code to your machine.

You can also clone this repo and run the app locally in VSCode or Visual Studio.

### Starting the app

`dotnet run`

### Accessing the app

If you are running the app in Gitpod, navigate to the "Ports" tab and click the address for port 5000 to open the app in a new tab:

![Gitpod open address](img/gitpod-ports-dotnet.png "Gitpod open address")

If you are running locally, access the app at [http://localhost:5000](http://localhost:5000).

Activate the sequence of numbers by selecting the **Go** button.
After the app displays numbers, select **Stop**.
Try this a few times.

Once that works, stop the app and configure it to send traces.

### Stopping the app

Push Ctrl-C in the terminal where it's running.

# Connect to Honeycomb

Our goal is to configure the connection to send traces to Honeycomb.

Open `appsettings.json`

Find the "Honeycomb" section.

Update the "ApiKey" property. Try not to commit this file with your key in it.

Get a Honeycomb API Key from your Team Settings in [Honeycomb](https://ui.honeycomb.io).
(find this by clicking on your profile in the lower-left corner.)

Alternative: you can set these on the command line.

```sh
export HONEYCOMB__APIKEY=<your api key here>
export HONEYCOMB__SERVICENAME=fib-microsvc

dotnet run
```

You can use whatever service name you like.

If you are running the app in Gitpod, navigate to the "Ports" tab and click the address for port 5000 to open the app in a new tab:

![Gitpod open address](img/gitpod-ports-dotnet.png "Gitpod open address")

If you are running locally, access the app at [http://localhost:5000](http://localhost:5000).

#### See the results

Run the app. Push "Go" to activate the sequence of numbers, the "Stop".
Do that several times.

Go to [Honeycomb](https://ui.honeycomb.io) and choose the Dataset that matches the service name you configured (default is `fib-microsvc`).

In Part 2 of the workshop, explore the data you find there.

## Customize a span: custom attributes

This is for Part 3 of the workshop.

Let's make it easier to see what the "index" query parameter is.

In Controllers/FibonacciController.cs, find the "CUSTOM ATTRIBUTE" comment.
Uncomment these lines:

```csharp
var currentSpan = Tracer.CurrentSpan;
currentSpan.SetAttribute("parameter.index", index);
```

This will add that parameter information to the current span.

Wait for the app to restart... and then try out the app again (push Go, then Stop). 
Can you find the field in honeycomb?

### FYI, tracing is also a .NET Activity

You don't even have to access OpenTelemetry directly to do this.
You can use the built-in .NET concept of Activity.

In `FibonacciController.cs`, add the index parameter as a custom attribute like this:

`System.Diagnostics.Activity.Current.AddTag("parameter.index", iv.ToString());`

Restart the app, make the sequence go, and find that field on the new spans.

Can you make the trace waterfall view show the index? What pattern does it show?

## 3. Create a custom span

Make the calculation into its own span, to see how much of the time spent on
this service is the meat: adding the fibonacci numbers.

Find the "CUSTOM SPAN" comment, and uncomment the sections below it.

After a restart, do your traces show this extra span? Do you see the name of your method?
What percentage of the service time is spend in it?

## How does this work?

Autoinstrumentation!

This tracing happens with only one code change.

See the "Tracing!" comment in `Startup.cs`. That is where OpenTelemetry instrumentation is set up.

This sets up auto-instrumentation for ASP.NET Core, and configures it to send to Honeycomb
based on environment variables.

You'll see the web requests coming in. They'll even nest inside each other when the service calls itself. You will not yet
see information that is special to this app, like the query parameter on the request.

## Updating libraries

For maintaining this project, this section reminds us how to check that we're on the latest Honeycomb OpenTelemetry Distribution for .NET.

Check [the .csproj file](https://github.com/honeycombio/intro-to-o11y-dotnet/blob/main/intro-to-observability-dotnet.csproj) against [the latest release](https://github.com/honeycombio/honeycomb-opentelemetry-dotnet/releases).

Update the version in the .csproj file if necessary, and then 'dotnet run' will get the new version.

## Troubleshooting

Our documentation: [https://docs.honeycomb.io/getting-data-in/dotnet/opentelemetry-distro/]()

If you see this error: `Unhandled exception. System.NullReferenceException: Object reference not set to an instance of an object.`
... then you probably need to define the `HONEYCOMB__APIKEY` environment variable before running the app.

If you see the warnings `Unable to bind to https://localhost:5001 on the IPv6 loopback interface: 'Cannot assign requested address'.` or `Unable to bind to http://localhost:5000 on the IPv6 loopback interface: 'Cannot assign requested address'.`
... then ignore it. We aren't using IPv6 here.
