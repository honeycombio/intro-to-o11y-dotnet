# Introduction to Observability in .NET 5

This repo hosts a .NET5 version of a demo used by [Glitch OpenTelemetry demo for .NET]([intro-to-o11y-dotnet](https://glitch.com/~intro-to-o11y-dotnet)).

# Run

```sh
export HONEYCOMB__APIKEY=<your api key here>
export HONEYCOMB__DATASET=otel-dotnet # or whatever value you prefer

dotnet run
```

# Into to Observability: OpenTelemetry in .NET

This ASP.NET application is here for you to try out tracing.
It consists of a microservice that calls itself, so you can simulate
a whole microservice ecosystem with just one service!

## What to do

Clone this and run it in VSCode or Visual Studio.

If you use Docker and VSCode's Remote Container Extension, then you can start this repo in a container with the .NET runtime you need.

### 1. Autoinstrument!

This tracing happens with only one code change!

See the "Tracing!" comment in `Startup.cs`. That is where OpenTelemetry instrumentation is set up.

This sets up auto-instrumentation for ASP.NET Core, and configures it to send to Honeycomb
based on environment variables.

You'll see the web requests coming in. They'll even nest inside each other when the service calls itself. You will not yet
see information that is special to this app, like the query parameter on the request.

#### Configure the connection to Honeycomb

In `.env` in glitch or your run configuration in your IDE, add these
environment variables:

```
HONEYCOMB_API_KEY=replace-this-with-a-real-api-key
HONEYCOMB_DATASET=otel-dotnet
```

Get a Honeycomb API Key from your Team Settings in [Honeycomb](https://ui.honeycomb.io).
(find this by clicking on your profile in the lower-left corner.)

You can name the Honeycomb Dataset anything you want.

#### See the results

Run the app. Push "Go" to activate the sequence of numbers. (and then stop it!)
Go to [Honeycomb](https://ui.honeycomb.io) and choose the Dataset you configured.

How many traces are there?

How many spans are in the traces?

Why are there so many??

Which trace has the most, and why is it different?

## 2. Customize a span: custom attributes

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

