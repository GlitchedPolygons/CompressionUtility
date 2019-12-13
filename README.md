[![NuGet](https://img.shields.io/nuget/v/GlitchedPolygons.Services.CompressionUtility.svg)](https://www.nuget.org/packages/GlitchedPolygons.Services.CompressionUtility) [![CircleCI](https://circleci.com/gh/GlitchedPolygons/CompressionUtility/tree/master.svg?style=shield)](https://circleci.com/gh/GlitchedPolygons/CompressionUtility/tree/master) [![Build Status](https://travis-ci.org/GlitchedPolygons/CompressionUtility.svg?branch=master)](https://travis-ci.org/GlitchedPolygons/CompressionUtility)

# Compression Utility

Useful compression utility for quickly and easily (de)compressing strings and byte[] arrays.
Can be used in [ASP.NET Core](https://docs.microsoft.com/en-us/aspnet/core/?view=aspnetcore-2.1) apps using the included dependency injection container (under _Startup.cs_ call `services.AddTransient` and register the compression utility service into the DI container).

This library is built as a **netstandard2.0** class library and available through [NuGet](https://www.nuget.org/packages/GlitchedPolygons.Services.CompressionUtility).

### Implementations

* Currently, only a GZip implementation (making use of [`GZipStream`](https://docs.microsoft.com/en-us/dotnet/api/system.io.compression.gzipstream)) as well as a [Brotli](https://github.com/google/brotli) variant exist. But more can be added in the future by implementing the [`ICompressionUtility`](https://github.com/GlitchedPolygons/CompressionUtility/blob/master/src/ICompressionUtility.cs) interface.

## Dependencies

* [Brotli.NET](https://www.nuget.org/packages/Brotli.NET)
* xunit NuGet packages (for unit testing only).
