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
				var (result, payload) = await clientWebSocket.ReceiveAsync<Models.PayloadWrapperObject>(cancellationTokenSource.Token);

				Debug.WriteLine(payload.@event);

				if ((payload.@event & Models.Events.keyDown) != 0)
				{
					await clientWebSocket.SetTitleAsync(payload.context!, "hello world", cancellationTokenSource.Token);
				}

				await Task.Delay(millisecondsDelay: 1_000, cancellationTokenSource.Token);
			}

			// Close
			await clientWebSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, statusDescription: default, cancellationTokenSource.Token);
		}
	}
}
