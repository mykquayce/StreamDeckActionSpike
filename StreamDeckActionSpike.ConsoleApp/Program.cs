using Microsoft.Extensions.Configuration;
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

			using var clientWebSocket = new ClientWebSocket();

			var uri = new Uri("ws://localhost:" + config["StreamDeck:Port"]);

			// Cancellation token
			using var cancellationTokenSource = new CancellationTokenSource();

			// Connect
			await clientWebSocket.ConnectAsync(uri, cancellationTokenSource.Token);

			// Register
			await clientWebSocket.RegisterAsync(config["StreamDeck:RegisterEvent"], config["StreamDeck:PluginUUID"], cancellationTokenSource.Token);

			// Receive
			while (!cancellationTokenSource.IsCancellationRequested)
			{
				try
				{
					var payload = await clientWebSocket.ReceiveAsync<Models.PayloadWrapperObject>(cancellationTokenSource.Token);

					if (payload.@event.HasFlag(Models.Events.keyDown))
					{
						await clientWebSocket.SetTitleAsync(payload.context!, "hello world", cancellationTokenSource.Token);
					}

					await Task.Delay(millisecondsDelay: 1_000, cancellationTokenSource.Token);
				}
				catch
				{
					cancellationTokenSource.Cancel();
				}
			}

			// Close
			await clientWebSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, statusDescription: default, cancellationTokenSource.Token);
		}
	}
}
