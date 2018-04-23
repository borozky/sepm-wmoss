using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace WMoSS.Tests
{
    public static class Utilities
    {
        /// <summary>
        /// Encodes the data from the form. This method also includes the anti-forgery tokens. The path must have a cookie header.
        /// </summary>
        /// <param name="client"></param>
        /// <param name="path"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public static async Task<FormUrlEncodedContent> GetRequestContentAsync(
            HttpClient client, string path, IDictionary<string, string> data
            )
        {
            // make request for the resource
            var response = await client.GetAsync(path);

            client.DefaultRequestHeaders.Add("Cookie", response.Headers.GetValues("Set-Cookie"));

            var responseMarkup = await response.Content.ReadAsStringAsync();
            var regex_RequestVerificationToken = new Regex(
                "<input name=\"__RequestVerificationToken\" type=\"hidden\" value=\"(.*?)\" \\/>", RegexOptions.Compiled
            );
            var matches = regex_RequestVerificationToken.Matches(responseMarkup);
            var token = matches?.FirstOrDefault().Groups[1].Value;
            data.Add("__RequestVerificationToken", token);

            return new FormUrlEncodedContent(data);
        }
        
        public static DbContextOptions<T> TestingDbContextOptions<T>() where T : DbContext
        {
            // use in-memory database for testing
            // register in memory db as a service
            var serviceProvider = new ServiceCollection()
                .AddEntityFrameworkInMemoryDatabase()
                .BuildServiceProvider();

            var builder = new DbContextOptionsBuilder<T>()
                .UseInMemoryDatabase("InMemoryDb")
                .UseInternalServiceProvider(serviceProvider);

            return builder.Options;

        }
    }
}
