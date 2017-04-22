using System;
using System.Reflection;
using Microsoft.Extensions.Configuration;
using Tabi.Shared.Helpers;

namespace Tabi.Shared.Extensions
{
    public static class ConfigurationBuilderExtensions
    {
        public static IConfigurationBuilder AddEmbeddedJsonFile(this IConfigurationBuilder configurationBuilder,
            Assembly assembly, string name, bool optional = false)
        {
            // reload on change is not supported, always pass in false
            return configurationBuilder.AddJsonFile(new EmbeddedFileProvider(assembly), name, optional, false);
        }

        public static IConfigurationBuilder AddEmbeddedXmlFile(this IConfigurationBuilder configurationBuilder,
            Assembly assembly, string name, bool optional = false)
        {
            // reload on change is not supported, always pass in false
            return configurationBuilder.AddXmlFile(new EmbeddedFileProvider(assembly), name, optional, false);
        }
    }
}
