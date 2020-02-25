[![NuGet](https://img.shields.io/nuget/v/GlitchedPolygons.Services.CompressionUtility.svg)](https://www.nuget.org/packages/GlitchedPolygons.Services.CompressionUtility) 
[![CircleCI](https://circleci.com/gh/GlitchedPolygons/CompressionUtility/tree/master.svg?style=shield)](https://circleci.com/gh/GlitchedPolygons/CompressionUtility/tree/master) 
[![Travis](https://travis-ci.org/GlitchedPolygons/CompressionUtility.svg?branch=master)](https://travis-ci.org/GlitchedPolygons/CompressionUtility)
[![Build status](https://ci.appveyor.com/api/projects/status/kf50jywtff4kcpwd?svg=true)](https://ci.appveyor.com/project/GlitchedPolygons/compressionutility)
[![Codecov](https://codecov.io/gh/GlitchedPolygons/CompressionUtility/branch/master/graph/badge.svg)](https://codecov.io/gh/GlitchedPolygons/CompressionUtility)
[![Codacy Badge](https://api.codacy.com/project/badge/Grade/99512152352640b49ee9c7707636ef8a)](https://app.codacy.com/manual/GlitchedPolygons/CompressionUtility?utm_source=github.com&utm_medium=referral&utm_content=GlitchedPolygons/CompressionUtility&utm_campaign=Badge_Grade_Dashboard)
[![API](https://img.shields.io/badge/api-docs-informational)](https://glitchedpolygons.github.io/CompressionUtility/api/GlitchedPolygons.Services.CompressionUtility.html) 

# Compression Utility

Useful compression utility for quickly and easily (de)compressing strings and byte[] arrays.
Can be used in [ASP.NET Core](https://docs.microsoft.com/en-us/aspnet/core/?view=aspnetcore-2.1) apps using the included dependency injection container (under _Startup.cs_ call `services.AddTransient` and register the compression utility service into the DI container).

This library is built as a **netstandard2.0** class library and available through [NuGet](https://www.nuget.org/packages/GlitchedPolygons.Services.CompressionUtility).

### Implementations

* Currently, there are a few compression implementations available such as [GZip](http://gzip.org/) (making use of [`GZipStream`](https://docs.microsoft.com/en-us/dotnet/api/system.io.compression.gzipstream)), a [Brotli](https://github.com/google/brotli) implementation as well as [LZMA](https://www.7-zip.org/sdk.html) (uses the managed C# API). But more can be added in the future by implementing the [`ICompressionUtility`](https://github.com/GlitchedPolygons/CompressionUtility/blob/master/src/ICompressionUtility.cs) interface.

## Dependencies

* [Brotli.NET](https://www.nuget.org/packages/Brotli.NET)
* xunit NuGet packages (for unit testing only).
