using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ConsoleSecrets
{
    class BootstrapSecrets
    {
        public static IConfigurationRoot Configuration { get; set; }

        static BootstrapSecrets()
        {

        }

        public static T GetSecrets<T>(string sectionName) where T : class
        {
            var devEnvironmentVariable = Environment.GetEnvironmentVariable("NETCORE_ENVIRONMENT");
            var isDevelopment = string.IsNullOrEmpty(devEnvironmentVariable) || devEnvironmentVariable.ToLower() == "development";

            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables();

            if (isDevelopment) //only add secrets in development
            {
                builder.AddUserSecrets<T>();
            }

            Configuration = builder.Build();

            return Configuration.GetSection(sectionName).Get<T>();
        }
    }
}
