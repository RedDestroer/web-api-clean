# Web Api Clean Architecture

This is the reference template of web API service based on .NET Core 3.1.
It takes a lot of time just to prepare fresh solution for new web api service. This template will save you some time at a start phase along with enforcing the principles of clean architecture inside new solution.

# How to use

There are three different ways how to use this template

1) Use prebuild nuget package
2) Fork or download & use `dotnet new` to generate and install template locally
3) Fork or download & rename

## Install template from nuget package

Download the latest 'WebApiClean.Template.Solution' nuget package.

Use this command to install a new template:
`dotnet new --install WebApiClean.Template.Solution.1.0.0.nupkg`

Use this command to generate fresh solution from the template:
`dotnet new webapiclean -o NewSolutionName`

It is reported that sometimes it doesn't work without any visible reasons. You can use second option if this is the case.

## Install template from sources

Download source code as zip and unzip it somewhere at your hard drive or fork & clone sources.

Use this command to install a new template:
`dotnet new --install [path_to_sources]`

Use this command to generate fresh solution from the template:
`dotnet new webapiclean -o NewSolutionName`

## Create new solution using rename

After all you can just download sources, unpack it somewhere and just rename all strings from *WebApiClean* to *NewSolutionName*. And rename the solution file as well. But I suggest using this approach only as a last resort.

# Useful links

https://www.youtube.com/watch?v=ekBWizEpyvo

https://www.youtube.com/watch?v=GDNcxU0_OuE

https://github.com/dotnet/templating/wiki/Reference-for-template.json

https://github.com/sayedihashimi/template-sample

# Denis & Andrei videos about clean architecture

https://www.youtube.com/watch?v=GRURPkkEjMk

https://www.youtube.com/watch?v=Bd83nPK_K3U

https://www.youtube.com/watch?v=GW1sN7OLTXo

https://www.youtube.com/watch?v=SazEYHv2IbI
