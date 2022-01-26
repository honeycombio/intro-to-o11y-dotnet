# Intro to Observability: OpenTelemetry in .NET

This ASP.NET application is here for you to try out tracing.
It consists of a microservice that calls itself, so you can simulate a whole microservice ecosystem with just one service!

## What to Do

Recommended:

[![Open in Gitpod](https://gitpod.io/button/open-in-gitpod.svg)](https://gitpod.io/#https://github.com/honeycombio/intro-to-o11y-dotnet)

Or run locally:

Clone this repository and run it in VSCode or Visual Studio.

### Starting the App

In your terminal, enter:

`dotnet run`

### Accessing the App

In GitPod: While the app is running, select "Remote Explorer" on the left sidebar.
Then expand **Ports**.
Next to **3000**, choose the middle **Open Preview** icon for the app to appear in a new tab.

Locally: Go to [http://localhost:5000](http://localhost:5000)

### Stopping the app

Push `Ctrl-C` in the terminal where it's running.

## Connect to Honeycomb

WARNING: You can configure these values in `appsetting.json` instead, but DO NOT commit that file to git with your API key in it as it is a security risk.

In your terminal window, run the following command: 

```sh
export HONEYCOMB__APIKEY=<your api key here>
export HONEYCOMB__DATASET=hello-observability # optional; this variable will default to this value. You can set your own dataset name here.

dotnet run
```

Get a Honeycomb API Key from your Team Settings in [Honeycomb](https://ui.honeycomb.io).
(Find your Honeycomb API Key by selecting your profile in the lower-left corner, and select **Team Settings**.)

You can name the Honeycomb Dataset anything you want.
Honeycomb will create your dataset once it knows your Honeycomb Dataset name.

The app runs at http://localhost:3000.

### See the Results

Run the app.
Push "Go" to activate the sequence of numbers, then push "Stop".
Repeat several times.

Go to [Honeycomb](https://ui.honeycomb.io) and choose the Dataset you configured.

In Part 2 of the workshop, explore the data you find there.

## Customize a Span: Custom Attributes

This is for Part 3 of the workshop.

Let's make it easier to see what the "index" query parameter is.

In `Controllers/FibonacciController.cs`, find the "CUSTOM ATTRIBUTE" comment.
Uncomment these lines:

```csharp
var currentSpan = Tracer.CurrentSpan;
currentSpan.SetAttribute("parameter.index", index);
```

This will add that parameter information to the current span.

Wait for the app to restart... and then try out the app again (push Go, then Stop).
Can you find the field in honeycomb?

### FYI, Tracing is also a .NET Activity

You don't even have to access OpenTelemetry directly to do this.
You can use the built-in .NET concept of Activity.

In `FibonacciController.cs`, add the index parameter as a custom attribute like this:

```dotnet
System.Diagnostics.Activity.Current.AddTag("parameter.index", iv.ToString());
```

Restart the app, make the go-stop sequence happen, and find that field on the new spans.

Can you make the trace waterfall view show the index?
What pattern does it show?

## 3. Create a Custom Span

Make the fibonacci calculation into its own span.
This will let you see how much of the time spent on this service is the meat: adding the fibonacci numbers.

Find the "CUSTOM SPAN" comment, and uncomment the sections below it.

After a restart, do your traces show this extra span?
Do you see the name of your method?
What percentage of the service time is spend in it?

## How Does This Work?

Auto-instrumentation!

This tracing happens with only one code change.

See the "Tracing!" comment in `Startup.cs`. That is where OpenTelemetry instrumentation is set up.

This sets up auto-instrumentation for ASP.NET Core, and configures it to send to Honeycomb based on environment variables.

You'll see the web requests coming in.
They'll even nest inside each other when the service calls itself.
You will not yet see information that is special to this app, like the query parameter on the request.

## Updating Libraries

For maintaining this project, this section reminds us how to check that we're on the latest Honeycomb OpenTelemetry Distribution for .NET.

Check [the .csproj file](https://github.com/honeycombio/intro-to-o11y-dotnet/blob/main/intro-to-observability-dotnet.csproj) against [the latest release](https://github.com/honeycombio/honeycomb-opentelemetry-dotnet/releases).

Update the version in the `.csproj` file if necessary, and then the `dotnet run` command will get the new version.

## Troubleshooting

Our documentation: [https://docs.honeycomb.io/getting-data-in/dotnet/opentelemetry-distro/](https://docs.honeycomb.io/getting-data-in/dotnet/opentelemetry-distro/).

If you see this error: `Unhandled exception. System.NullReferenceException: Object reference not set to an instance of an object.`
... then you probably need to define the `HONEYCOMB__APIKEY` environment variable before running the app.

If you see the warnings `Unable to bind to https://localhost:5001 on the IPv6 loopback interface: 'Cannot assign requested address'.` or `Unable to bind to http://localhost:5000 on the IPv6 loopback interface: 'Cannot assign requested address'.`
... then ignore it. We aren't using IPv6 here.
