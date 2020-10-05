namespace ConsoleApp
{
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;
    using System.Collections.Generic;
    using System.IO;
    using System.Threading.Tasks;
    using XmlRepository;
    using static System.Console;
    using static XmlRepository.Extension;

    internal class Program
    {
        private const string _Prefix = "ASPNETCORE_";
        private static readonly Dictionary<string, string> _map = new Dictionary<string, string>();

        public static IConfigurationRoot Configuration { get; private set; }

        public static void Main(string[] args)
        {
            Task.Run(() => MainAsync(args)).GetAwaiter().GetResult();
        }

        private static async Task MainAsync(string[] args)
            => await CreateHostBuilder(args).Build().RunAsync();

        private static IHostBuilder CreateHostBuilder(string[] args)
            => Host
                .CreateDefaultBuilder(args)
                .ConfigureHostConfiguration(configBuilder =>
                {
                    configBuilder.SetBasePath(Directory.GetCurrentDirectory());
                    configBuilder.AddJsonFile("appsettings.json", optional: true);
                    configBuilder.AddEnvironmentVariables("ASPNETCORE_");
                    configBuilder.AddCommandLine(args);
                })
                .ConfigureAppConfiguration((hostBuilderContext, configBuilder) =>
                {
                    configBuilder.SetBasePath(Directory.GetCurrentDirectory());
                    configBuilder.AddJsonFile("appsettings.json", optional: true);
                    configBuilder.AddJsonFile($"appsettings.{hostBuilderContext.HostingEnvironment.EnvironmentName}.json", optional: true);
                    configBuilder.AddCommandLine(args);
                })
                .ConfigureLogging((hostBuilderContext, loggingBuilder) =>
                {
                    loggingBuilder.AddConsole();
                    loggingBuilder.AddDebug();
                })
                .ConfigureServices((hostBuilderContext, services) =>
                {
                    services.AddHostedService<ConsoleHostedService>();
                    services.AddLogging();
                    services.AddXmlRepositoryService(hostBuilderContext.Configuration);
                });

        private static IConfigurationRoot GetConfigurationRoot(string environment)
        {
            WriteLine($"============ Bootstrapping the application to {environment} environment ============");

            var configurationBuilder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", true, true)
                .AddJsonFile($"appsettings.{environment}.json")
                .AddEnvironmentVariables();
            var configurationRoot = configurationBuilder.Build();

            WriteLine($"============ Bootstrapp to {environment} environment is successfully ============");

            return configurationRoot;
        }
    }
}
