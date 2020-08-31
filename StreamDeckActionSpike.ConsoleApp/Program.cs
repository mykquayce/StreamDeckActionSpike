using Microsoft.Extensions.Configuration;
using StreamDeckActionSpike.ConsoleApp.Clients.Concrete;
using StreamDeckActionSpike.ConsoleApp.Extensions;
using System;
using System.Diagnostics;
using System.Net.WebSockets;
using System.Threading;
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

			var config = new ConfigurationBuilder()
				.AddStreamDeckCommandLine(args)
				.Build();

			var settings = config.GetSection("StreamDeck").Get<Models.ConfigObject>();

			using IStreamDeckClient client = new StreamDeckClient();

			var uri = new Uri("ws://localhost:" + settings.Port);

			// Cancellation token
			using var cancellationTokenSource = new CancellationTokenSource();

			// Connect
			await client.ConnectAsync(uri, cancellationTokenSource.Token);

			// Register
			await client.RegisterAsync(settings.RegisterEvent!, settings.PluginUuid!, cancellationTokenSource.Token);

			// Receive
			while (!cancellationTokenSource.IsCancellationRequested)
			{
				try
				{
					var payload = await client.ReceiveAsync<Models.PayloadWrapperObject>(cancellationTokenSource.Token);

					if (payload.@event.HasFlag(Models.Events.keyDown))
					{
						await client.SetTitleAsync(payload.context!, "hello world", cancellationTokenSource.Token);
					}
				}
				catch
				{
					cancellationTokenSource.Cancel();
				}

				await Task.Delay(millisecondsDelay: 1_000, cancellationTokenSource.Token);
			}

			// Close
			await client.CloseAsync(WebSocketCloseStatus.NormalClosure, statusDescription: default, cancellationTokenSource.Token);
		}
	}
}
