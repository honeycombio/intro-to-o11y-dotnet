# OpenTelemetry .NET Instructor Project

ASP.NET Core MVC 2.1 sample application for workshops.

Calculates Fibonacci Sequence at /api/Fibonacci/?iv=x, where x is the desired position in the sequence.

## Notes
- The fibonacci logic is all in ./Controllers/FibonacciController.cs
- Adding additional exporters, etc. can be done by adding a new PackageReference to `Website.csproj` then modifying the OTel config in `Startup.cs`
- Creating additional attributes or other spans should be done through `ActivitySource` and `System.Diagnostics`