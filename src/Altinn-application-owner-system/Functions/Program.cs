﻿using AltinnApplicationOwnerSystem.Functions.Config;
using AltinnApplicationOwnerSystem.Functions.Services.Implementation;
using AltinnApplicationOwnerSystem.Functions.Services.Interface;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace AltinnApplicationsOwnerSystem.Functions
{
    /// <summary>
    /// Host program for Azure Function
    /// </summary>
    public class Program
    {
        /// <summary>
        /// Main
        /// </summary>
        public static void Main()
        {
            var host = new HostBuilder()
                .ConfigureFunctionsWorkerDefaults()
                .ConfigureServices(s =>
                {
                    s.AddOptions<AltinnApplicationOwnerSystemSettings>()
                    .Configure<IConfiguration>((settings, configuration) =>
                    {
                        configuration.GetSection("AltinnApplicationOwnerSystemSettings").Bind(settings);
                    });
                    s.AddOptions<KeyVaultSettings>()
                    .Configure<IConfiguration>((settings, configuration) =>
                    {
                        configuration.GetSection("KeyVault").Bind(settings);
                    });

                    s.AddOptions<QueueStorageSettings>()
                    .Configure<IConfiguration>((settings, configuration) =>
                    {
                        configuration.GetSection("QueueStorageSettings").Bind(settings);
                    });
            
                    s.AddTransient<IKeyVaultService, KeyVaultService>();
                    s.AddSingleton<IAuthenticationService, AuthenticationService>();
                    s.AddSingleton<IQueueService, QueueService>();
                    s.AddSingleton<IStorage, StorageService>();
                    s.AddHttpClient<ISubscription, SubscriptionService>();
                    s.AddHttpClient<IAltinnApp, AltinnAppService>();
                    s.AddHttpClient<IPlatform, PlatformService>();
                    s.AddHttpClient<IAuthenticationClientWrapper, AuthenticationClientWrapper>();
                    s.AddHttpClient<IMaskinPortenClientWrapper, MaskinportenClientWrapper>();
                })
                .Build();

            host.Run();
        }
    }
}
