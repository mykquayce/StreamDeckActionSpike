using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using StreamDeckActionSpike.ConsoleApp.Extensions;
using System.Diagnostics;
using System.Net.WebSockets;
using System.Threading.Tasks;

namespace StreamDeckActionSpike.ConsoleApp
{
	public static class Program
	{
		public static async Task Main(string[] args)
		{
#if DEBUG
			while (!Debugger.IsAttached) await Task.Delay(millisecondsDelay: 1_000);
			Debugger.Break();
#endif
			await CreateHostBuilder(args).RunConsoleAsync();
		}

		public static IHostBuilder CreateHostBuilder(string[] args)
		{
			var hostBuilder = new HostBuilder();

			hostBuilder
				.ConfigureAppConfiguration((context, builder) =>
				{
					var environment = context.HostingEnvironment.EnvironmentName ?? Environments.Production;

					builder
						.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
						.AddJsonFile($"appsettings.{environment}.json", optional: true, reloadOnChange: true)
						.AddStreamDeckCommandLine(args);
				});

			hostBuilder
				.ConfigureServices((hostContext, services) =>
				{
					var configuration = hostContext.Configuration;

					services
						.Configure<Models.ConfigObject>(configuration.GetSection("StreamDeck"));

					services
						.AddTransient<ClientWebSocket>()
						.AddTransient<Clients.IStreamDeckClient, Clients.Concrete.StreamDeckClient>()
						.AddTransient<Clients.IWebSocketClient, Clients.Concrete.WebSocketClient>();

					services
						.AddLogging(builder =>
						{
							builder.AddConsole();
						});

					services
						.AddHostedService<Worker>();
				});

			return hostBuilder;
		}
	}
}
