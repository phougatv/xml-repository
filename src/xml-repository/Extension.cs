namespace XmlRepository
{
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using System;
    using XmlRepository.Base.Data;
    using XmlRepository.Dto;

    public static class Extension
    {
        public static IServiceCollection AddXmlRepositoryService(this IServiceCollection services, IConfiguration configuration)
        {
            //logger.LogInformation("========== Adding Xml Repository Service... ==========");
            Console.WriteLine("========== Adding Xml Repository Service... ==========");

            services.AddApplicationDependencies(configuration);

            Console.WriteLine("========== Successfully added Xml Repository Service. ==========");
            //logger.LogInformation("========== Successfully added Xml Repository Service. ==========");
            return services;
        }

        private static IServiceCollection AddApplicationDependencies(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<IDataSource<Book>, XmlDataSource<Book>>();
            services.AddSingleton<IDataSource<Book>, XmlDataSource<Book>>();

            return services;
        }
    }
}
