﻿using System;
using Microsoft.AspNetCore.Hosting;
using System.Threading.Tasks;
using Autofac.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NServiceBus;

static class Program
{
    public static async Task Main(string[] args)
    {
        var host = CreateHostBuilder(args).Build();
        await host.StartAsync();

        Console.WriteLine("Press any key to shutdown");
        Console.ReadKey();
        await host.StopAsync();
    }

    static IHostBuilder CreateHostBuilder(string[] args) =>
    #region ServiceProviderFactoryAutofac
        Host.CreateDefaultBuilder(args)
            .UseServiceProviderFactory(new AutofacServiceProviderFactory())
    #endregion
            .UseNServiceBus(c =>
            {
                var endpointConfiguration = new EndpointConfiguration("Sample.Core");
                endpointConfiguration.UseTransport<LearningTransport>();
                return endpointConfiguration;
            })
            .ConfigureWebHostDefaults(webBuilder => webBuilder.UseStartup<Startup>());
}