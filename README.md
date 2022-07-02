# Web API Clean Architecture Solution Template

[![WebApiClean.Template.Solution NuGet Package](https://img.shields.io/badge/nuget-1.0.7-blue)](https://www.nuget.org/packages/WebApiClean.Template.Solution)
[![NuGet](https://img.shields.io/nuget/dt/WebApiClean.Template.Solution.svg)](https://www.nuget.org/packages/WebApiClean.Template.Solution)

This is the reference template of web API service based on .NET Core 3.1.
It takes a lot of time just to prepare fresh solution for new web api service. This template will save you some time at a start phase along with
enforcing the principles of clean architecture inside new solution.

## Technologies

* ASP.NET Core 3.1
* [MediatR](https://github.com/jbogard/MediatR)
* [AutoMapper](https://automapper.org/)
* [FluentValidation](https://fluentvalidation.net/)
* [Polly](https://www.nuget.org/packages/Microsoft.Extensions.Http.Polly/)
* [NUnit](https://nunit.org/), [FluentAssertions](https://fluentassertions.com/), [Moq](https://github.com/moq)
* [Serilog](https://serilog.net/), [Destructurama.ByIgnoring](https://www.nuget.org/packages/Destructurama.ByIgnoring/)
* [Swagger(Swashbuckle)](https://docs.microsoft.com/en-us/aspnet/core/tutorials/web-api-help-pages-using-swagger?view=aspnetcore-3.1)

# How to use

There are three different ways how to use this template

1) Use prebuild nuget package
2) Fork or download & use `dotnet new` to generate and install template locally
3) Fork or download & rename

## Install template from nuget package

The easiest way to get started is to install the [WebApiClean.Template.Solution](https://www.nuget.org/packages/WebApiClean.Template.Solution) nuget package.

Use this command to install a new template on your local machine:
`dotnet new --install WebApiClean.Template.Solution --nuget-source "https://api.nuget.org/v3/index.json"`

Use this command to generate fresh solution from the template:
`dotnet new webapiclean -o NewSolutionName`

## Install template from sources

Download source code as zip and unzip it somewhere at your hard drive or fork & clone sources.

Use this command to install a new template on your local machine:
`dotnet new --install path_to_the_sources_root`

Use this command to generate fresh solution from the template:
`dotnet new webapiclean -o NewSolutionName`

## Create new solution using rename

After all you can just download sources, unpack it somewhere and just rename all strings from *WebApiClean* to *NewSolutionName*. And rename the solution file as well. But I suggest using this approach only as a last resort.

# Useful links

https://www.youtube.com/watch?v=ekBWizEpyvo

https://www.youtube.com/watch?v=GDNcxU0_OuE

https://github.com/dotnet/templating/wiki/Reference-for-template.json

https://github.com/sayedihashimi/template-sample

https://github.com/JasonTaylorDev/CleanArchitecture

# Denis & Andrei videos about clean architecture

https://www.youtube.com/watch?v=GRURPkkEjMk

https://www.youtube.com/watch?v=Bd83nPK_K3U

https://www.youtube.com/watch?v=GW1sN7OLTXo

https://www.youtube.com/watch?v=SazEYHv2IbI
