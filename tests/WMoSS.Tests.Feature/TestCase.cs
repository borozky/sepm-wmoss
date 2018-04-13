using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.TestHost;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Net.Http.Headers;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace WMoSS.Tests.Feature
{

    // In razor page integration testing, tests will return a 500 response error
    // without the following lines of code in .csproj file in the test project
    //
    //<Target Name="CopyDepsFiles" AfterTargets="Build" Condition="'$(TargetFramework)'!=''">
    //  <ItemGroup>
    //      <DepsFilePaths Include="$([System.IO.Path]::ChangeExtension('%(_ResolvedProjectReferencePaths.FullPath)', '.deps.json'))" />
    //  </ItemGroup>
    //  <Copy SourceFiles="%(DepsFilePaths.FullPath)" DestinationFolder="$(OutputPath)" Condition="Exists('%(DepsFilePaths.FullPath)')" />
    //</Target>


    /// <summary>
    /// Superclass for all feature tests
    /// </summary>
    public abstract class TestCase : IDisposable
    {
        protected readonly TestServer server;
        protected readonly HttpClient client;

        protected TestCase()
        {
            var webfolder = @"C:\DEV\sepm-wmoss\src\WMoSS.Web";
            var webhostBuilder = new WebHostBuilder()
                .UseEnvironment("Development")
                .UseContentRoot(webfolder)
                .UseStartup<WMoSS.Web.Startup>();

            server = new TestServer(webhostBuilder);
            client = server.CreateClient();
        }

        public void Dispose()
        {
            client.Dispose();
            server.Dispose();
        }
    }
}
