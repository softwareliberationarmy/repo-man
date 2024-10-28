﻿using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using repo_man.domain.DependencyInjection;
using repo_man.infrastructure.DependencyInjection;

namespace repo_man.console;

public static class Bootstrapper
{
    public static IHost BuildHost(string[] args)
    {
        if (args.Length == 0)
        {
            throw new ArgumentException("No action specified.");
        }

        // Extract the action and remaining arguments
        var action = args[0];
        var remainingArgs = args.Skip(1).ToArray();

        var host = Host.CreateDefaultBuilder(remainingArgs)
            .ConfigureAppConfiguration((context, config) =>
            {
                // Add the action to the configuration
                var actionConfig = new Dictionary<string, string?>
                {
                    { "action", action }
                };
                config.AddInMemoryCollection(actionConfig.AsEnumerable());
            })
            .ConfigureServices((_, services) =>
            {
                services.AddDomainServices();
                services.AddInfrastructureServices();
            })
            .Build();
        return host;
    }
}