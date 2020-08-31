using StreamDeckActionSpike.ConsoleApp.Extensions;
using System;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;

namespace StreamDeckActionSpike.ConsoleApp.Clients.Concrete
{
	public class WebSocketClient : IWebSocketClient
	{
		private readonly ClientWebSocket _clientWebSocket;

		public WebSocketClient(ClientWebSocket clientWebSocket)
		{
			_clientWebSocket = clientWebSocket;
		}

		public void Dispose() => _clientWebSocket?.Dispose();

		public Task CloseAsync(WebSocketCloseStatus closeStatus, string? statusDescription = default, CancellationToken? cancellationToken = default)
			=> _clientWebSocket.CloseAsync(closeStatus, statusDescription, cancellationToken ?? CancellationToken.None);

		public Task ConnectAsync(Uri uri, CancellationToken? cancellationToken = default)
			=> _clientWebSocket.ConnectAsync(uri, cancellationToken ?? CancellationToken.None);

		public async Task<T> ReceiveAsync<T>(CancellationToken? cancellationToken = default)
		{
			var buffer = new byte[65_536];

			var result = await _clientWebSocket.ReceiveAsync(buffer, cancellationToken ?? CancellationToken.None);

			switch (result.CloseStatus)
			{
				case null:
				case WebSocketCloseStatus.Empty:
				case WebSocketCloseStatus.NormalClosure:
					var bytes = buffer[..result.Count];
					var value = await bytes.JsonDeserializeAsync<T>(cancellationToken ?? CancellationToken.None);
					return value;
				default:
					throw new ArgumentOutOfRangeException(nameof(WebSocketCloseStatus), result.CloseStatus, $"Unexpected {nameof(WebSocketCloseStatus)}: {result.CloseStatus}")
					{
						Data = { [nameof(WebSocketCloseStatus)] = result.CloseStatus, },
					};
			}
		}

		public async Task SendAsync(object o, CancellationToken? cancellationToken = default)
		{
			var buffer = await o.JsonSerializeAsync(cancellationToken ?? CancellationToken.None);

			await _clientWebSocket.SendAsync(buffer, WebSocketMessageType.Text, endOfMessage: true, cancellationToken ?? CancellationToken.None);
		}
	}
}
