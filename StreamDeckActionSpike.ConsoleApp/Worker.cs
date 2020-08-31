using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace StreamDeckActionSpike.ConsoleApp
{
	public class Worker : BackgroundService
	{
		private readonly ILogger<Worker> _logger;
		private readonly Clients.IStreamDeckClient _client;

		public Worker(
			ILogger<Worker> logger,
			Clients.IStreamDeckClient client,
			IOptions<Models.ConfigObject> options)
		{
			_logger = logger;
			_client = client;

			var settings = options.Value;
			var uri = new Uri("ws://localhost:" + settings.Port, UriKind.Absolute);

			_client.ConnectAsync(uri);
			_client.RegisterAsync(settings.RegisterEvent!, settings.PluginUuid!);
		}

		protected override async Task ExecuteAsync(CancellationToken stoppingToken)
		{
			while (!stoppingToken.IsCancellationRequested)
			{
				var payload = await _client.ReceiveAsync<Models.PayloadWrapperObject>(stoppingToken);

				_logger.LogInformation(payload.@event.ToString("G"));

				if (payload.@event.HasFlag(Models.Events.keyDown))
				{
					await _client.SetTitleAsync(payload.context!, "hello world", stoppingToken);
				}

				await Task.Delay(millisecondsDelay: 1_000, stoppingToken);
			}
		}
	}
}
