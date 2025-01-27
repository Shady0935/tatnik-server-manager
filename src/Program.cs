using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using pterodactyl.DataProviders;
using pterodactyl.Services;
using System;
using discord.Services;
using pterodactyl.Utility;

Console.WriteLine("----------------------------------------------------------------------------");
Console.WriteLine(" _____  _  _____ _   _ ___ _  __      ____  _____ ______     _______ ____   ");
Console.WriteLine("|_   _|/ \|_   _| \ | |_ _| |/ /     / ___|| ____|  _ \ \   / / ____|  _ \  ");
Console.WriteLine("  | | / _ \ | | |  \| || || ' /      \___ \|  _| | |_) \ \ / /|  _| | |_) | ");
Console.WriteLine("  | |/ ___ \| | | |\  || || . \       ___) | |___|  _ < \ V / | |___|  _ <  ");
Console.WriteLine("  |_/_/   \_\_| |_| \_|___|_|\_\     |____/|_____|_| \_\ \_/  |_____|_| \_\ ");
Console.WriteLine("                                                                            ");
Console.WriteLine("              __  __    _    _   _    _    ____ _____ ____                  ");
Console.WriteLine("             |  \/  |  / \  | \ | |  / \  / ___| ____|  _ \                 ");
Console.WriteLine("             | |\/| | / _ \ |  \| | / _ \| |  _|  _| | |_) |                ");
Console.WriteLine("             | |  | |/ ___ \| |\  |/ ___ \ |_| | |___|  _ <                 ");
Console.WriteLine("             |_|  |_/_/   \_\_| \_/_/   \_\____|_____|_| \_\                ");
Console.WriteLine("----------------------------------------------------------------------------");

var discordToken = Settings.DiscordToken;
var url = Settings.PterodactylUrl;
var authGroup = Settings.DiscordAuthGroup;
var globalKey = Settings.GlobalPterodactylKey;

if (string.IsNullOrEmpty(authGroup))
   Console.WriteLine("Actualmente no se agregan usuarios a un grupo de Discord especial sobre autenticación.");
else
   Console.WriteLine("Currently adding users to a special discord group on authentication. (Group ID "+ authGroup + ")");

if (string.IsNullOrEmpty(globalKey))
   Console.WriteLine("Actualmente no se utiliza una clave API global que todos puedan usar.");

using IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
       var discordConfig = new DiscordSocketConfig()
       {
          GatewayIntents = Discord.GatewayIntents.Guilds
       };

       services.AddSingleton(new DiscordSocketClient(discordConfig));
       services.AddSingleton<InteractionService>();
       services.AddScoped<IPterodactylModuleDataProvider, PterodactylModuleDataProvider>();
       services.AddHostedService<InteractionHandlingService>();
       services.AddHostedService<DiscordStartupService>();
       services.AddHttpClient<IPterodactylHttpService, PterodactylHttpService>(client =>
       {
          var address = Settings.PterodactylUrl;
          client.BaseAddress = new Uri(address);
       });

    })
    .Build();

await host.RunAsync();
