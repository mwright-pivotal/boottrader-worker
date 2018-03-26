using Microsoft.Extensions.Configuration;
using Steeltoe.Extensions.Configuration;
using Steeltoe.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace boottrader_worker.App_Start
{
    public class ServerConfig
    {
        public static IConfigurationRoot Configuration { get; set; }

        public static void RegisterConfig(string environment)
        {
            // Set up configuration sources.
            var builder = new ConfigurationBuilder()
                .SetBasePath(GetContentRoot())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
                .AddJsonFile($"appsettings.{environment}.json", optional: true)
                .AddCloudFoundry()
                .AddEnvironmentVariables();


                builder.ConfigureLogging((builderContext, loggingBuilder) =>
                {
                    loggingBuilder.AddConfiguration(builderContext.Configuration.GetSection("Logging"));

                    // Add Steeltoe Dynamic Logging provider
                    loggingBuilder.AddDynamicConsole();
                });

            Configuration = builder.Build();
        }
        public static string GetContentRoot()
        {
            var basePath = (string)AppDomain.CurrentDomain.GetData("APP_CONTEXT_BASE_DIRECTORY") ??
               AppDomain.CurrentDomain.BaseDirectory;
            return Path.GetFullPath(basePath);
        }
    }
}