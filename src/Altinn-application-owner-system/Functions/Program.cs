using AltinnApplicationOwnerSystem.Functions.Config;
using AltinnApplicationOwnerSystem.Functions.Services.Implementation;
using AltinnApplicationOwnerSystem.Functions.Services.Interface;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using static AltinnApplicationsOwnerSystem.Functions.Function4.Function5;

namespace AltinnApplicationsOwnerSystem.Functions
{
    public class Program
    {
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
                    s.AddSingleton<IStorage, StorageSI>();
                    s.AddHttpClient<IAltinnApp, AltinnAppSI>();
                    s.AddHttpClient<IPlatform, PlatformSI>();
                    s.AddHttpClient<IAuthenticationClientWrapper, AuthenticationClientWrapper>();
                    s.AddHttpClient<IMaskinPortenClientWrapper, MaskinportenClientWrapper>();
                })
                .Build();

            host.Run();
        }
    }
}
